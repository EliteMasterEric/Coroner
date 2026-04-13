using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using Mono.Cecil;
using UnityEngine;

#nullable enable

/*
 * A set of patches dedicated to tracking when a player dies in a specific manner,
 * and storing it in the AdvancedDeathTracker, because `causeOfDeath` is not precise enough.
 */
namespace Coroner.Patch
{
    /*
     * Common utilities for patches which inject code to detect cause of death.
     */
    class CauseOfDeathPatch {
        // IL_0177: callvirt instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool)
        public const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3, Boolean)";
        // IL_021c: callvirt instance void GameNetcodeStuff.PlayerControllerB::DamagePlayer(int32, bool, bool, valuetype CauseOfDeath, int32, bool, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        public const string DAMAGE_PLAYER_SIGNATURE = "Void DamagePlayer(Int32, Boolean, Boolean, CauseOfDeath, Int32, Boolean, UnityEngine.Vector3)";

        public static void LogException(System.Exception e, string location) {
            Plugin.Instance.PluginLogger.LogError($"Caught exception in {location}: {e.Message}");
            Plugin.Instance.PluginLogger.LogError(e.StackTrace);
        }

        public static int LocateKillPlayerCall(List<CodeInstruction> instructions) {
            for (int i = 0; i < instructions.Count; i++) {
                CodeInstruction instruction = instructions[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE) {
                    return i;
                }
            }
            return -1;
        }

        public static int LocateDamagePlayerCall(List<CodeInstruction> instructions) {
            for (int i = 0; i < instructions.Count; i++) {
                CodeInstruction instruction = instructions[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == DAMAGE_PLAYER_SIGNATURE) {
                    return i;
                }
            }
            return -1;
        }
    
        public static void OverrideKillLocalPlayer(KillLocalPlayer target, AdvancedCauseOfDeath causeOfDeath)
        {
            if (target == null) return;
            // Directly modify the kill trigger so that `KillPlayer(causeOfDeath)` uses our custom cause of death.
            // Hopefully this doesn't break anything.
            target.causeOfDeath = causeOfDeath;
        }

        public static void OverrideKillLocalPlayer(GameObject target, AdvancedCauseOfDeath causeOfDeath)
        {
            if (target == null) return;
            OverrideKillLocalPlayer(target.GetComponent<KillLocalPlayer>(), causeOfDeath);
        }

        public static void OverrideKillLocalPlayer(Transform target, AdvancedCauseOfDeath causeOfDeath)
        {
            if (target == null) return;
            OverrideKillLocalPlayer(target.gameObject, causeOfDeath);
        }
    }

    /*
     * A structure value used to store the state between a patch's Prefix and Postfix.
     */
    struct CauseOfDeathPatchState
    {
        private int targetPlayerIndex;

        private bool wasPlayerQueried;

        // Set of values set when the player is queried
        public bool previousIsPlayerDead;
        public CauseOfDeath? previousCauseOfDeath;
        public bool wasHoldingJetpack;

        public CauseOfDeathPatchState()
        {
            // Default values
            targetPlayerIndex = -1;
        }

        // ============
        // Player Assign
        // ============

        public void TrySetPlayer(int targetPlayerIndex) {
            // Set the target player based on the index.
            this.targetPlayerIndex = targetPlayerIndex;
        }

        public void TrySetPlayer(PlayerControllerB playerControllerB) {
            // Set the target player based on the index of the PlayerController.
            if (playerControllerB == null)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: PlayerController was null");
                return;
            }

            TrySetPlayer((int) playerControllerB.playerClientId);
        }

        public void TrySetPlayer(Collider collider)
        {
            // Set the target player based on the Collider attached to the PlayerController.
            if (collider == null)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Collider was null!");
                return;
            }

            PlayerControllerB playerControllerB = collider.gameObject.GetComponent<PlayerControllerB>();
            TrySetPlayer(playerControllerB);
        }

        // ============
        // Player Query
        // ============

        public void QueryPlayerState()
        {
            // Obtain information about the player before the hooked function executes.
            // This lets us check if the player was alive and became dead.

            if (wasPlayerQueried) return;

            PlayerControllerB? targetPlayer = FetchPlayer();
            if (targetPlayer == null) return;

            wasPlayerQueried = true;
            previousIsPlayerDead = targetPlayer.isPlayerDead;
            previousCauseOfDeath = targetPlayer.causeOfDeath;
            wasHoldingJetpack = AdvancedDeathTracker.IsHoldingJetpack(targetPlayer);
        }

        public int FetchPlayerIndex()
        {
            if (targetPlayerIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Index not assigned!");
                return -1;
            }
            return targetPlayerIndex;
        }

        public PlayerControllerB? FetchPlayer()
        {
            // Obtain the target player. Return null if it failed to be assigned.
            if (targetPlayerIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Index not assigned!");
                return null;
            }

            return StartOfRound.Instance.allPlayerScripts[targetPlayerIndex];
        }

        public PlayerControllerB GetPlayer()
        {
            // Obtain the target player. Throws an error if it failed to be assigned.

            PlayerControllerB? targetPlayer = FetchPlayer();
            if (targetPlayer == null) throw new System.Exception("Could not retrieve player from state!");
            return targetPlayer;
        }
    
        // ============
        // Validation
        // ============

        public bool ValidateWasntAlreadyDead()
        {
            // Returns false if the player couldn't be queried, or if the player is was dead before this check

            if (!wasPlayerQueried) return false;

            PlayerControllerB? targetPlayer = FetchPlayer();
            if (targetPlayer == null) return false;

            if (previousIsPlayerDead)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Player was already dead!");
                return false;
            }

            return true;
        }

        public bool ValidateIsPlayerDead()
        {
            // Returns false if the player couldn't be queried, or if the player is still alive

            if (!wasPlayerQueried) return false;

            PlayerControllerB? targetPlayer = FetchPlayer();
            if (targetPlayer == null) return false;

            if (!targetPlayer.isPlayerDead)
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Player is still alive!");
                return false;
            }
            
            return true;
        }
    
        public bool ValidateHasNoCauseOfDeath()
        {
            // Returns false if the player couldn't be queried, or if the player had a cause of death

            if (!wasPlayerQueried) return false;

            if (AdvancedDeathTracker.HasCauseOfDeath(FetchPlayerIndex()))
            {
                Plugin.Instance.PluginLogger.LogWarning("Could not access dying player: Player already had a cause of death!");
                return false;
            }

            return true;
        }
    }

    // =========
    // DamagePlayer()
    // =========

    // Enemy_BaboonHawk
    [HarmonyPatch(typeof(BaboonBirdAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BaboonBirdAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BaboonBirdAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Baboon Hawk damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Baboon Hawk killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_BaboonHawk);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BaboonBirdAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Mask_Hornets
    [HarmonyPatch(typeof(ButlerBeesEnemyAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class ButlerBeesEnemyAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ButlerBeesEnemyAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Mask Hornet damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Mask Hornet killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_MaskHornets);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ButlerBeesEnemyAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Butler_Stab
    [HarmonyPatch(typeof(ButlerEnemyAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class ButlerEnemyAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ButlerEnemyAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Butler damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Butler killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Butler_Stab);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ButlerEnemyAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Cadaver_Bloom
    [HarmonyPatch(typeof(CadaverBloomAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class CadaverBloomAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CadaverBloomAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Cadaver Bloom damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Cadaver Bloom killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Cadaver_Bloom);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CadaverBloomAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Giant_Sapsucker
    [HarmonyPatch(typeof(GiantKiwiAI))]
    [HarmonyPatch("AnimationEventB")]
    class GiantKiwiAIAnimationEventBPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "GiantKiwiAI.AnimationEventB:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Giant Sapsucker damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Giant Sapsucker killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Giant_Sapsucker);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "GiantKiwiAI.AnimationEventB:Postfix");
            }
        }
    }

    // Enemy_Lasso_Man
    [HarmonyPatch(typeof(LassoManAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class LassoManAIOnCollideWithPlayer
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "LassoManAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Lasso Man damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Lasso Man killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_LassoMan);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "LassoManAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Player_Electric_Chair
    [HarmonyPatch(typeof(MoveToExitSpecialAnimation))]
    [HarmonyPatch("shockChair")]
    class MoveToExitSpecialAnimationShockChairPatch
    {
        public static void Prefix(MoveToExitSpecialAnimation __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance.interactTrigger.lockedPlayer.GetComponent<PlayerControllerB>());
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MoveToExitSpecialAnimation.shockChair:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Lasso Man damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Electric Chair killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Player_Electric_Chair);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MoveToExitSpecialAnimation.shockChair:Postfix");
            }
        }
    }

    // Enemy_SporeLizard
    [HarmonyPatch(typeof(PufferAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class PufferAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PufferAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Spore Lizard damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Spore Lizard killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_SporeLizard);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PufferAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Feiopar
    [HarmonyPatch(typeof(PumaAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class PumaAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PumaAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Feiopar damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Feiopar killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Feiopar);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PumaAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Old_Bird_Stomp
    [HarmonyPatch(typeof(RadMechAI))]
    [HarmonyPatch("Stomp")]
    class RadMechAIStompPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "RadMechAI.Stomp:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Old Bird (Stomp) damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Old Bird killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Old_Bird_Stomp);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "RadMechAI.Stomp:Postfix");
            }
        }
    }

    // Enemy_Old_Bird_Torch
    [HarmonyPatch(typeof(RadMechAI))]
    [HarmonyPatch("CancelTorchPlayerAnimation")]
    class RadMechAICancelTorchPlayerAnimationPatch
    {
        public static void Prefix(RadMechAI __instance)
        {
            Plugin.Instance.PluginLogger.LogDebug("Handling Old Bird (Torch) damage...");

            // TorchPlayerAnimation() is what actually kills the player, but that's an Enumerator, and this gets called immediately after.

            if (__instance.inSpecialAnimationWithPlayer != null)
            {
                Plugin.Instance.PluginLogger.LogDebug("Old Bird killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Old_Bird_Torch);
            }
        }
    }

    // Enemy_BunkerSpider
    [HarmonyPatch(typeof(SandSpiderAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SandSpiderAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SandSpiderAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Bunker Spider damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Bunker Spider killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_BunkerSpider);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SandSpiderAI.OnCollideWithPlayer:Postfix");
            }
        }
    }
    
    // Other_Turret
    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("Update")]
    class TurretUpdatePatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                // It only kills if we are the local player, so we can just query that.
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "Turret.Update:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Turret damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Turret killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Other_Turret);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "Turret.Update:Postfix");
            }
        }
    }

    // Enemy_Hygrodere
    [HarmonyPatch(typeof(BlobAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BlobAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BlobAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Bunker Spider damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Bunker Spider killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Hygrodere);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BlobAI.OnCollideWithPlayer:Postfix");
            }
        }
    }
    
    // Enemy_SnareFlea
    [HarmonyPatch(typeof(CentipedeAI))]
    [HarmonyPatch("DamagePlayerOnIntervals")]
    class CentipedeAIDamagePlayerOnIntervalsPatch
    {
        public static void Prefix(CentipedeAI __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance.clingingToPlayer);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CentipedeAI.DamagePlayerOnIntervals:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Centipede damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Centipede killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_SnareFlea);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CentipedeAI.DamagePlayerOnIntervals:Postfix");
            }
        }
    }

    // Enemy_Thumper
    [HarmonyPatch(typeof(CrawlerAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class CrawlerAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CrawlerAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Thumper damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Thumper killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Thumper);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CrawlerAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_CircuitBees
    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyPatch("BeeKillPlayerOnLocalClient")]
    class RedLocustBeesBeeKillPlayerOnLocalClientPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "RedLocustBees.BeeKillPlayerOnLocalClient:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Circuit Bees damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Circuit Bees killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_CircuitBees);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "RedLocustBees.BeeKillPlayerOnLocalClient:Postfix");
            }
        }
    }

    // Enemy_HoarderBug
    [HarmonyPatch(typeof(HoarderBugAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class HoarderBugAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "HoarderBugAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Hoarder Bug damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Hoarder Bug killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_HoarderBug);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "HoarderBugAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_CoilHead
    [HarmonyPatch(typeof(SpringManAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SpringManAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SpringManAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Coil-Head damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Coil-Head killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_CoilHead);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SpringManAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Nutcracker_Shot
    // Player_Murder_Shotgun
    [HarmonyPatch(typeof(ShotgunItem))]
    [HarmonyPatch("ShootGun")]
    class ShotgunItemShootGunPatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ShotgunItem.ShootGun:Prefix");
            }
        }

        public static void Postfix(ShotgunItem __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Shotgun damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Shotgun killed Player! Checking wielder...");

                if (__instance.isHeldByEnemy)
                {
                    // Enemy Nutcracker fired the shotgun.
                    Plugin.Instance.PluginLogger.LogDebug("Shotgun (Nutcracker) killed player! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Nutcracker_Shot);
                }
                else
                {
                    // Player fired the shotgun.
                    Plugin.Instance.PluginLogger.LogDebug("Shotgun (Player) killed player! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Player_Murder_Shotgun);
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ShotgunItem.ShootGun:Postfix");
            }
        }
    }

    // Other_Vomiting
    [HarmonyPatch(typeof(SprayPaintItem))]
    [HarmonyPatch("HealPlayerInfection")]
    class SprayPaintItemHealPlayerInfectionPatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SprayPaintItem.HealPlayerInfection:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling vomiting damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                // This will handle checking if the Cadaver Bloom burst out.
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Vomiting killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Other_Vomiting);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SprayPaintItem.HealPlayerInfection:Postfix");
            }
        }
    }
    
    // Player_StunGrenade
    [HarmonyPatch(typeof(StunGrenadeItem))]
    [HarmonyPatch("StunExplosion")]
    class StunGrenadeItemStunExplosionPatch
    {
        public static void Prefix(StunGrenadeItem __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance.playerHeldBy);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "StunGrenadeItem.StunExplosion:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Stun Grenade damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Stun Grenade killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Player_StunGrenade);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "StunGrenadeItem.StunExplosion:Postfix");
            }
        }
    }

    // Player_Cruiser_Ran_Over
    [HarmonyPatch(typeof(VehicleCollisionTrigger))]
    [HarmonyPatch("OnTriggerEnter")]
    class VehicleCollisionTriggerOnTriggerEnterPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                
                if (!other.gameObject.CompareTag("Player")) {
                    Plugin.Instance.PluginLogger.LogDebug("Vehicle collision with non-player...");
                    return;
                }

                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "VehicleCollisionTrigger.OnTriggerEnter:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Vehicle damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Vehicle (Collision) killed Player (Pedestrian)! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Player_Cruiser_Ran_Over);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "VehicleCollisionTrigger.OnTriggerEnter:Postfix");
            }
        }
    }

    // Player_Cruiser_Driver
    // Player_Cruiser_Passenger
    [HarmonyPatch(typeof(VehicleController))]
    [HarmonyPatch("DamagePlayerInVehicle")]
    class VehicleControllerDamagePlayerInVehiclePatch
    {
        public static void Prefix(VehicleController __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                
                if (__instance.localPlayerInControl || __instance.localPlayerInPassengerSeat)
                {
                    __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                    __state.QueryPlayerState();
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Couldn't determine player in vehicle...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "VehicleController.DamagePlayerInVehicle:Prefix");
            }
        }

        public static void Postfix(VehicleController __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Vehicle damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                if (__instance.localPlayerInControl)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Vehicle (Collision) killed Player (Driver)! Setting cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Driver);
                }
                else if (__instance.localPlayerInPassengerSeat)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Vehicle (Collision) killed Player (Passenger)! Setting cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Passenger);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Couldn't determine player in vehicle...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "VehicleController.DamagePlayerInVehicle:Postfix");
            }
        }
    }

    // Gravity
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("PlayerHitGroundEffects")]
    class PlayerControllerBPlayerHitGroundEffectsPatch
    {
        public static void Prefix(PlayerControllerB __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PlayerControllerB.PlayerHitGroundEffects:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling generic fall damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                PlayerControllerB player = __state.GetPlayer();
                FootstepSurface? targetSurface = StartOfRound.Instance.footstepSurfaces[player.currentFootstepSurfaceIndex];

                if (targetSurface != null)
                {
                    Plugin.Instance.PluginLogger.LogDebug($"Fall damage (surface tag: ${targetSurface.surfaceTag}) killed Player! Setting cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(player, AdvancedCauseOfDeath.Gravity);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug($"Fall damage (unknown surface) killed Player! Setting cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(player, AdvancedCauseOfDeath.Gravity);
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PlayerControllerB.PlayerHitGroundEffects:Postfix");
            }
        }
    }

    // Player_Murder_Knife
    // Player_Murder_Shovel
    // Player_Murder_Shotgun
    // Player_Murder_Sign
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("DamagePlayerFromOtherClientClientRpc")]
    class PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch
    {
        public static void Prefix(PlayerControllerB targetPlayer, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(targetPlayer);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PlayerControllerB.DamagePlayerFromOtherClientClientRpc:Prefix");
            }
        }

        public static void Postfix(int playerWhoHit, ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling friendly fire damage...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                PlayerControllerB target = __state.GetPlayer();
                PlayerControllerB? suspect = StartOfRound.Instance.allPlayerScripts[playerWhoHit];

                if (suspect == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Damage from other player: attacker is null!");
                    return;
                }

                if (AdvancedDeathTracker.IsHoldingShotgun(suspect))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Shotgun! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(target, AdvancedCauseOfDeath.Player_Murder_Shotgun);
                }
                else if (AdvancedDeathTracker.IsHoldingKnife(suspect))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Knife! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(target, AdvancedCauseOfDeath.Player_Murder_Knife);
                }
                else if (AdvancedDeathTracker.IsHoldingShovel(suspect))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Shovel! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(target, AdvancedCauseOfDeath.Player_Murder_Shovel);
                }
                else if (AdvancedDeathTracker.IsHoldingStopSign(suspect))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Stop Sign! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(target, AdvancedCauseOfDeath.Player_Murder_Stop_Sign);
                }
                else if (AdvancedDeathTracker.IsHoldingYieldSign(suspect))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Yield Sign! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(target, AdvancedCauseOfDeath.Player_Murder_Yield_Sign);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning($"Player was murdered, by an unknown item {AdvancedDeathTracker.GetHeldObject(suspect)}! Setting cause of death to: " + target.causeOfDeath);
                    AdvancedDeathTracker.SetCauseOfDeath(target, target.causeOfDeath);
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch:Postfix");
            }
        }




        // IL_021c: callvirt instance void GameNetcodeStuff.PlayerControllerB::DamagePlayer(int32, bool, bool, valuetype CauseOfDeath, int32, bool, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string DAMAGE_PLAYER_SIGNATURE = "Void DamagePlayer(Int32, Boolean, Boolean, CauseOfDeath, Int32, Boolean, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            var codeToInjectDamage = BuildInstructionsToInsert(method);
            if (codeToInjectDamage == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.DamagePlayer is called.
            // Do this after the first insection is done.
            int insertionIndexDamage = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Call && instruction.operand.ToString() == DAMAGE_PLAYER_SIGNATURE)
                {
                    insertionIndexDamage = i;
                }
            }

            if (insertionIndexDamage == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.DamagePlayer call in PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into PlayerControllerB.DamagePlayerFromOtherClientClientRpc...");
                code.InsertRange(insertionIndexDamage + 1, codeToInjectDamage);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

            }

            Plugin.Instance.PluginLogger.LogDebug("Done with all PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch patches.");
            return code;
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            var argumentIndex_self = 0; // Instance functions are just static functions where the first argument is `self`
            var argumentIndex_damageAmount = 1;
            var argumentIndex_hitDirection = 2;
            var argumentIndex_playerWhoHit = 3;
            var argumentIndex_newHealthAmount = 4;

            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_self));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_damageAmount));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_hitDirection));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_playerWhoHit));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_newHealthAmount));

            // IL_0180: call      void [Coroner]Coroner.Patch.PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch::MaybeRewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch).GetMethod(nameof(MaybeRewriteCauseOfDeath))));

            return result;
        }

        public static void MaybeRewriteCauseOfDeath(PlayerControllerB targetPlayer, int damageAmount, Vector3 hitDirection, int playerWhoHit, int newHealthAmount)
        {
            Plugin.Instance.PluginLogger.LogDebug($"Player damaged another player ${targetPlayer}({damageAmount}, {hitDirection}, {playerWhoHit}, {newHealthAmount})");
            // Called when the player is DAMAGED by an explosion, but not necessarily killed.
            if (targetPlayer.isPlayerDead)
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from friendly fire damage");
                RewriteCauseOfDeath(targetPlayer, playerWhoHit);
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from friendly fire (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer, int playerWhoHitIndex)
        {
            PlayerControllerB playerWhoHit = StartOfRound.Instance.allPlayerScripts[playerWhoHitIndex];

            if (targetPlayer == null)
            {
                Plugin.Instance.PluginLogger.LogError("Damage from other client: victim is null!");
            }
            else if (playerWhoHit == null)
            {
                Plugin.Instance.PluginLogger.LogError("Damage from other client: attacker is null!");
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from murder ({targetPlayer.causeOfDeath}), determining special cause of death...");

                if (AdvancedDeathTracker.IsHoldingShotgun(playerWhoHit))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Shotgun! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Player_Murder_Shotgun);
                }
                else if (AdvancedDeathTracker.IsHoldingKnife(playerWhoHit))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Knife! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Player_Murder_Knife);
                }
                else if (AdvancedDeathTracker.IsHoldingShovel(playerWhoHit))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Shovel! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Player_Murder_Shovel);
                }
                else if (AdvancedDeathTracker.IsHoldingStopSign(playerWhoHit))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Stop Sign! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Player_Murder_Stop_Sign);
                }
                else if (AdvancedDeathTracker.IsHoldingYieldSign(playerWhoHit))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was murdered by Yield Sign! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Player_Murder_Yield_Sign);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning($"Player was killed by someone else, holding an unknown item {AdvancedDeathTracker.GetHeldObject(playerWhoHit)}! " + targetPlayer.causeOfDeath);
                    AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, targetPlayer.causeOfDeath);
                }
            }
        }
    }

    // =========
    // KillPlayer()
    // =========

    // Unknown, possibly a trigger on the player ship 
    [HarmonyPatch(typeof(AnimatedObjectFloatSetter))]
    [HarmonyPatch("KillPlayerAtPoint")]
    class AnimatedObjectFloatSetterKillPlayerAtPointPatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                // It only kills if we are the local player, so we can just query that.
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "AnimatedObjectFloatSetter.KillPlayerAtPoint:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling AnimatedObjectFloatSetter death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogWarning("AnimatedObjectFloatSetter killed Player! Unknown cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Unknown);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "AnimatedObjectFloatSetter.KillPlayerAtPoint:Postfix");
            }
        }
    }

    // Enemy_Kidnapper_Wolf
    [HarmonyPatch(typeof(BushWolfEnemy))]
    [HarmonyPatch("DoKillPlayerAnimationServerRpc")]
    class BushWolfDoKillPlayerAnimationServerRpcPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BushWolf.DoKillPlayerAnimationServerRpc:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Kidnapper Fox death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogWarning("Kidnapper Fox killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_KidnapperFox);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "BushWolf.DoKillPlayerAnimationServerRpc:Postfix");
            }
        }
    }

    // Enemy_Cadaver_Growth
    [HarmonyPatch(typeof(CadaverBloomAI))]
    [HarmonyPatch("BurstForth")]
    class CadaverBloomAIBurstForthPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CadaverBloomAI.BurstForth:Prefix");
            }
        }

        public static void Postfix(bool kill, ref CauseOfDeathPatchState __state)
        {
            try
            {
                // If false, Cadaver Bloom is merely instantiating.
                if (!kill) return;

                Plugin.Instance.PluginLogger.LogDebug("Handling Cadaver Growth death...");

                // The player should already be dead if the Bursting Forth happened because the player died some other way.
                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogWarning("Cadaver Growth killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Cadaver_Growth);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CadaverBloomAI.BurstForth:Postfix");
            }
        }
    }

    // Enemy_Maneater
    [HarmonyPatch(typeof(CaveDwellerAI))]
    [HarmonyPatch("KillPlayerAnimationClientRpc")]
    class CaveDwellerAIKillPlayerAnimationClientRpcPatch
    {

        public static void Prefix(int playerObjectId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerObjectId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CaveDwellerAI.KillPlayerAnimationClientRpc:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Maneater death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogWarning("Maneater killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Maneater);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "CaveDwellerAI.KillPlayerAnimationClientRpc:Postfix");
            }
        }
    }

    // Enemy_Barber
    [HarmonyPatch(typeof(ClaySurgeonAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class ClaySurgeonAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ClaySurgeonAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Barber death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Barber killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Barber);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ClaySurgeonAI.OnCollideWithPlayer:Postfix");
            }
        }
    }
    
    // Other_DepositItemsDesk
    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyPatch("AnimationGrabPlayer")]
    class DepositItemsDeskAnimationGrabPlayerPatch
    {
        public static void Prefix(int playerID, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerID);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "DepositItemsDesk.AnimationGrabPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Deposit Desk death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Deposit Desk killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Other_DepositItemsDesk);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "DepositItemsDesk.AnimationGrabPlayer:Postfix");
            }
        }
    }

    // Enemy_MaskedPlayer_Wear
    [HarmonyPatch(typeof(HauntedMaskItem))]
    [HarmonyPatch("FinishAttaching")]
    class HauntedMaskItemFinishAttachingPatch
    {
        public static void Prefix(HauntedMaskItem __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                PlayerControllerB previousPlayerHeldBy = Traverse.Create(__instance).Field("previousPlayerHeldBy").GetValue<PlayerControllerB>();
                __state.TrySetPlayer(previousPlayerHeldBy);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "HauntedMaskItem.FinishAttaching:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Haunted Mask death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Haunted Mask killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "HauntedMaskItem.FinishAttaching:Postfix");
            }
        }
    }

    // Enemy_Jester
    [HarmonyPatch(typeof(JesterAI))]
    [HarmonyPatch("killPlayerAnimation")]
    class JesterAIKillPlayerAnimationPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "JesterAI.killPlayerAnimation:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Jester death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Jester killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Jester);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "JesterAI.killPlayerAnimation:Postfix");
            }
        }
    }

    // Player_Jetpack_Gravity
    [HarmonyPatch(typeof(JetpackItem))]
    [HarmonyPatch("Update")]
    class JetpackItemUpdatePatch
    {
        public static void Prefix(JetpackItem __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance.playerHeldBy);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "JetpackItem.Update:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                if (!__state.ValidateIsPlayerDead()) return;

                Plugin.Instance.PluginLogger.LogDebug("Handling Jetpack collision death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Jetpack (Falling/Collision) killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Player_Jetpack_Gravity);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "JetpackItem.Update:Postfix");
            }
        }
    }

    // Enemy_MaskedPlayer_Victim
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    [HarmonyPatch("FinishKillAnimation")]
    class MaskedPlayerEnemyFinishKillAnimationPatch
    {
        public static void Prefix(MaskedPlayerEnemy __instance, bool killedPlayer, ref CauseOfDeathPatchState __state)
        {
            try
            {
                __state = new CauseOfDeathPatchState();

                if (!killedPlayer) return;

                __state.TrySetPlayer(__instance.inSpecialAnimationWithPlayer);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MaskedPlayerEnemyFinishKillAnimationPatch.Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Masked Player death...");

                // Player will already be dead before this function is called, but it's only called if it successfully killed the player.
                // if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Masked Player killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MaskedPlayerEnemyFinishKillAnimationPatch.Postfix");
            }
        }
    }

    // Enemy_Nutcracker_Kicked
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    [HarmonyPatch("LegKickPlayer")]
    class NutcrackerEnemyAILegKickPlayerPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "NutcrackerEnemyAILegKickPlayerPatch.Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Nutcracker death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Nutcracker (Kicking) killed player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "NutcrackerEnemyAILegKickPlayerPatch.Postfix");
            }
        }
    }
    
    // Other_OutOfBounds
    [HarmonyPatch(typeof(OutOfBoundsTrigger))]
    [HarmonyPatch("OnTriggerEnter")]
    class OutOfBoundsTriggerOnTriggerEnterPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "OutOfBoundsTrigger.OnTriggerEnter:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Out of Bounds death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Out of Bounds trigger killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Barber);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "OutOfBoundsTrigger.OnTriggerEnter:Postfix");
            }
        }
    }

    // Enemy_EarthLeviathan
    [HarmonyPatch(typeof(SandWormAI))]
    [HarmonyPatch("EatPlayer")]
    class SandWormAIEatPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SandWormAIEatPlayerPatch.Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Earth Leviathan death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Earth Leviathan trigger killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_EarthLeviathan);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SandWormAIEatPlayerPatch.Postfix");
            }
        }
    }

    // Unknown
    [HarmonyPatch(typeof(QuickMenuManager))]
    [HarmonyPatch("Debug_KillLocalPlayer")]
    class QuickMenuManagerDebugKillLocalPlayerPatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "QuickMenuManager.Debug_KillLocalPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling debug death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Debug manager killed Player! Unknown cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Unknown);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "QuickMenuManager.Debug_KillLocalPlayer:Postfix");
            }
        }
    }

    // StartOfRound.LateUpdate???

    // StartOfRound.bloomPlayerOnDelay???

    // Enemy_GhostGirl
    [HarmonyPatch(typeof(DressGirlAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class DressGirlAIOnCollideWithPlayerPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "DressGirlAI.OnCollideWithPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Ghost Girl death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Ghost Girl killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_GhostGirl);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "DressGirlAI.OnCollideWithPlayer:Postfix");
            }
        }
    }

    // Enemy_Bracken
    [HarmonyPatch(typeof(FlowermanAI))]
    [HarmonyPatch("killAnimation")]
    class FlowermanAIKillAnimationPatch
    {
        public static void Prefix(FlowermanAI __instance, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(__instance.inSpecialAnimationWithPlayer);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "FlowermanAI.killAnimation:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Bracken death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Bracken killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_Bracken);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "FlowermanAI.killAnimation:Postfix");
            }
        }
    }

    // Enemy_ForestGiant_Death
    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("AnimationEventA")]
    class ForestGiantAnimationEventAPatch
    {
        public static void Prefix(ref CauseOfDeathPatchState __state)
        {
            try
            {   
                // It only kills if we are the local player, so we can just query that.
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(GameNetworkManager.Instance.localPlayerController);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ForestGiantAI.AnimationEventA:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Dying Forest Giant death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Dying Forest Giant killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_ForestGiant_Death);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ForestGiantAI.AnimationEventA:Postfix");
            }
        }
    }

    // Enemy_ForestGiant_Eaten
    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("EatPlayerAnimation")]
    class ForestGiantAIEatPlayerAnimationPatch
    {
        public static void Prefix(PlayerControllerB playerBeingEaten, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                // It only kills if we are the local player, so we can just query that.
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerBeingEaten);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ForestGiantAI.EatPlayerAnimation:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Forest Giant death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Forest Giant killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_ForestGiant_Eaten);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ForestGiantAI.EatPlayerAnimation:Postfix");
            }
        }
    }

    // Enemy_EyelessDog
    [HarmonyPatch(typeof(MouthDogAI))]
    [HarmonyPatch("KillPlayer")]
    class MouthDogAIKillPlayerPatch
    {
        public static void Prefix(int playerId, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                // It only kills if we are the local player, so we can just query that.
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(playerId);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MouthDogAI.killPlayer:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Eyeless Dog death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Eyeless Dog killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Enemy_EyelessDog);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "MouthDogAI.killPlayer:Postfix");
            }
        }
    }

    // Other_Spike_Trap
    [HarmonyPatch(typeof(SpikeRoofTrap))]
    [HarmonyPatch("OnTriggerStay")]
    class SpikeRoofTrapOnTriggerStayPatch
    {
        public static void Prefix(Collider other, ref CauseOfDeathPatchState __state)
        {
            try
            {   
                __state = new CauseOfDeathPatchState();
                __state.TrySetPlayer(other);
                __state.QueryPlayerState();
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SpikeRoofTrap.OnTriggerStay:Prefix");
            }
        }

        public static void Postfix(ref CauseOfDeathPatchState __state)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Spike Trap death...");

                if (!__state.ValidateWasntAlreadyDead()) return;
                if (!__state.ValidateIsPlayerDead()) return;
                if (!__state.ValidateHasNoCauseOfDeath()) return;

                Plugin.Instance.PluginLogger.LogDebug("Spike Trap killed Player! Setting cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__state.GetPlayer(), AdvancedCauseOfDeath.Other_Spike_Trap);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "SpikeRoofTrap.OnTriggerStay:Postfix");
            }
        }
    }


    // ElevatorAnimationEvents.ElevatorFullyRunning uses CauseOfDeath.Abandoned, so easily skipped

    // =========
    // Advanced Hooks
    // =========

    // Enemy_Butler_Explode
    // Enemy_Old_Bird_Charge
    // Enemy_Old_Bird_Rocket
    // Player_EasterEgg
    // Player_Jetpack_Blast
    // Other_Landmine
    // Other_Lightning
    // Generic_Blast
    [HarmonyPatch(typeof(Landmine))]
    [HarmonyPatch("SpawnExplosion")]
    class LandmineSpawnExplosionPatch
    {
        // Landmind.SpawnExplosion() calls DamagePlayer or KillPlayer on all players in range.
        // Since we can't move up the stack and see what called KillPlayer, 
        // and we don't know what players will be affected before the function is called,
        // we have to inject code in the middle after each KillPlayer and DamagePlayer invocation.

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInjectKill = BuildInstructionsToInsertKill(method);
            if (codeToInjectKill == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build injected patch (kill) to insert in LandmineSpawnExplosionPatch!");
                return instructions;
            }

            var codeToInjectDamage = BuildInstructionsToInsertDamage(method);
            if (codeToInjectDamage == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build injected patch (damage) to insert in LandmineSpawnExplosionPatch!");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndexKill = CauseOfDeathPatch.LocateKillPlayerCall(code);

            if (insertionIndexKill == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not locate KillPlayer() call in LandmineSpawnExplosionPatch!");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #1 into Landmine.SpawnExplosion...");
                code.InsertRange(insertionIndexKill + 1, codeToInjectKill);
                Plugin.Instance.PluginLogger.LogDebug("  Success.");
            }

            // Search for where PlayerControllerB.DamagePlayer is called.
            // Do this after the first insection is done.
            int insertionIndexDamage = CauseOfDeathPatch.LocateDamagePlayerCall(code);

            if (insertionIndexDamage == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not locate DamagePlayer() call in LandmineSpawnExplosionPatch!");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #2 into Landmine.SpawnExplosion...");
                code.InsertRange(insertionIndexDamage + 1, codeToInjectDamage);
                Plugin.Instance.PluginLogger.LogDebug("  Success.");

            }

            Plugin.Instance.PluginLogger.LogDebug("Landmine.SpawnExplosion() injected patches successfully.");
            return code;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertKill(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // var argumentIndex_explosionPosition = 0; // UnityEngine.Vector3
            // var argumentIndex_spawnExplosionEffect = 1; // bool
            var argumentIndex_killRange = 2; // float (single)
            // var argumentIndex_damageRange = 3; // float (single)
            // var argumentIndex_nonLethalDamage = 4; // int
            var argumentIndex_physicsForce = 5; // float (single)
            // var argumentIndex_overridePrefab = 6; // UnityEngine.GameObject
            // var argumentIndex_goThroughCar = 7; // bool

            IList<LocalVariableInfo> localVars = method.GetMethodBody().LocalVariables;
            LocalVariableInfo? localVar_component = null;

            for (int i = 0; i < localVars.Count; i++)
            {
                var currentLocalVar = localVars[i];

                if (currentLocalVar.LocalType == typeof(PlayerControllerB))
                {
                    if (localVar_component != null)
                    {
                        Plugin.Instance.PluginLogger.LogError("Found multiple PlayerControllerB local variables in LandmineSpawnExplosionPatch!");
                        return null;
                    }
                    localVar_component = currentLocalVar;
                    break;
                }
            }

            if (localVar_component == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB local variable in LandmineSpawnExplosionPatch!");
                return null;
            }

            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_017F: ldarg.2
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_killRange));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_physicsForce));

            // IL_0180: call      void [Coroner]Coroner.Patch.LandmineSpawnExplosionPatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(LandmineSpawnExplosionPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertDamage(MethodBase method)
        {
            var result = new List<CodeInstruction>();
            // var argumentIndex_explosionPosition = 0; // UnityEngine.Vector3
            // var argumentIndex_spawnExplosionEffect = 1; // bool
            var argumentIndex_killRange = 2; // single
            // var argumentIndex_damageRange = 3; // single
            // var argumentIndex_nonLethalDamage = 4; // int
            var argumentIndex_physicsForce = 5; // single
            // var argumentIndex_overridePrefab = 6; // UnityEngine.GameObject
            // var argumentIndex_goThroughCars = 7; // bool

            IList<LocalVariableInfo> localVars = method.GetMethodBody().LocalVariables;
            LocalVariableInfo? localVar_component = null;

            for (int i = 0; i < localVars.Count; i++)
            {
                var currentLocalVar = localVars[i];

                if (currentLocalVar.LocalType == typeof(PlayerControllerB))
                {
                    if (localVar_component != null)
                    {
                        Plugin.Instance.PluginLogger.LogError("Found multiple PlayerControllerB local variables in LandmineSpawnExplosionPatch!");
                        return null;
                    }
                    localVar_component = currentLocalVar;
                    break;
                }
            }

            if (localVar_component == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB local variable in LandmineSpawnExplosionPatch!");
                return null;
            }

            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_017F: ldarg.2
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_killRange));
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_physicsForce));

            // IL_0180: call      void [Coroner]Coroner.Patch.LandmineSpawnExplosionPatch::MaybeRewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(LandmineSpawnExplosionPatch).GetMethod(nameof(MaybeRewriteCauseOfDeath))));

            return result;
        }

        public static void MaybeRewriteCauseOfDeath(PlayerControllerB targetPlayer, float killRange, float physicsForce)
        {
            // This function gets called AFTER the call to DamagePlayer() or KillPlayer()
            // We have to check that the target player is actually dead.

            if (targetPlayer.isPlayerDead)
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from landmine damage");
                RewriteCauseOfDeath(targetPlayer, killRange, physicsForce);
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from landmine (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer, float killRange, float physicsForce)
        {
            // This function sets different causes of death for the player based on the kill range of the explosion.
            // This works because different explosion types have different kill range and physics force.
            // EDIT: It's a miracle this workaround can still work in v81.
            // It will become a problem if two different sources can create identically-sized explosions.

            AdvancedCauseOfDeath causeOfDeath = AdvancedCauseOfDeath.Blast;

            if (killRange == 0.0f && physicsForce == 80.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Enemy_Butler_Explode;
            }
            else if (killRange == 5.0f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Player_Jetpack_Blast;
            }
            else if ( /* killRange == ??.?f && */ physicsForce == 50.0f)
            {
                // Check if it's a meteor (which has a random kill range based on size)
                causeOfDeath = AdvancedCauseOfDeath.Other_Meteor;
            }
            else if (killRange == 1.0f && physicsForce == 65.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Enemy_Old_Bird_Rocket;
            }
            else if (killRange == 1.0f && physicsForce == 35.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Enemy_Old_Bird_Rocket;
            }
            else if (killRange == 2.0f && physicsForce == 45.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Enemy_Old_Bird_Charge;
            }
            else if (killRange == 5.7f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Other_Landmine;
            }
            else if (killRange == 2.4f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Other_Lightning;
            }
            else if (killRange == 0.5f && physicsForce == 45.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Player_EasterEgg;
            }
            else if (killRange == 6.0f && physicsForce == 200.0f)
            {
                // Assume that the player is outside the vehicle.
                // We can update this later.
                causeOfDeath = AdvancedCauseOfDeath.Player_Cruiser_Explode_Bystander;
            }
            else
            {
                Plugin.Instance.PluginLogger.LogWarning($"Could not identify explosion (killRange: {killRange}, physicsForce: {physicsForce})! Using generic cause of death for Blasts...");
            }

            Plugin.Instance.PluginLogger.LogDebug($"Player died from Explosion({causeOfDeath})! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, causeOfDeath);
        }
    }

    // Player_Quicksand
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("KillPlayer")]
    class PlayerControllerBKillPlayerPatch
    {
        public static void Prefix(PlayerControllerB __instance, ref CauseOfDeath causeOfDeath)
        {
            try
            {
                // The easiest way to track this one is to check `isSinking` when KillPlayer() gets called.

                if (AdvancedDeathTracker.HasCauseOfDeath(__instance)) return;

                if (causeOfDeath == CauseOfDeath.Suffocation && __instance.isSinking)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died of suffociation while sinking in quicksand! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Quicksand);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is dying! Unknown cause of death in final hook...");
                }
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "PlayerControllerB.KillPlayer:Prefix");
            }
        }
    }

    // Other_ItemDropship
    [HarmonyPatch(typeof(ItemDropship))]
    [HarmonyPatch("Start")]
    class ItemDropshipStartPatch
    {
        public static void Postfix(ItemDropship __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Item dropship spawned! Modifying kill trigger...");

                GameObject? itemDropshipGameObject = __instance.gameObject;
                Transform? killTriggerTransform = itemDropshipGameObject.transform.Find("ItemShip/KillTrigger");

                CauseOfDeathPatch.OverrideKillLocalPlayer(killTriggerTransform, AdvancedCauseOfDeath.Other_Dropship);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ItemDropship.Start:Postfix");
            }
        }
    }

    // Player_Ladder
    [HarmonyPatch(typeof(ExtensionLadderItem))]
    [HarmonyPatch("StartLadderAnimation")]
    class ExtensionLadderItemStartLadderAnimationPatch
    {
        // We inject into StartLadderAnimation instead of Start because the latter wasn't implemented by the Ladder, ehe.

        public static void Postfix(ExtensionLadderItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Extension ladder started animation! Modifying kill trigger...");

                GameObject? extensionLadderGameObject = __instance.gameObject;
                Transform? killTriggerTransform = extensionLadderGameObject.transform.Find("AnimContainer/MeshContainer/LadderMeshContainer/BaseLadder/LadderSecondPart/KillTrigger");

                CauseOfDeathPatch.OverrideKillLocalPlayer(killTriggerTransform, AdvancedCauseOfDeath.Player_Ladder);
            }
            catch (System.Exception e)
            {
                CauseOfDeathPatch.LogException(e, "ExtensionLadderItem.StartLadderAnimation:Postfix");
            }
        }
    }

    // Other_Facility_Pit
    // Generic_Fan
    [HarmonyPatch(typeof(KillLocalPlayer))]
    [HarmonyPatch("KillPlayer")]
    class KillLocalPlayerKillPlayerPatch
    {
        // We inject into KillPlayer instead of Start because the latter wasn't implemented by the Ladder, ehe.

        public static void Postfix(KillLocalPlayer __instance, PlayerControllerB playerWhoTriggered)
        {
            try
            {
                // Modify the cause of death by comparing the kill trigger.
                if (__instance.causeOfDeath == AdvancedCauseOfDeath.Gravity)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died to KillPlayerTrigger(Gravity)! Setting special cause of death...");
                    // I believe the only KillTriggers which use this are pits in the facility.
                    AdvancedDeathTracker.SetCauseOfDeath(playerWhoTriggered, DistinguishGravityKillTrigger(__instance));
                }
                else if (__instance.causeOfDeath == AdvancedCauseOfDeath.Fan)
                {
                    // There's only one method of triggering this.
                    Plugin.Instance.PluginLogger.LogDebug($"Player died to KillPlayerTrigger(Fan)! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerWhoTriggered, AdvancedCauseOfDeath.Fan);
                }
                // Else, don't override existing cause of death.
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in KillLocalPlayerStartPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }

        public static AdvancedCauseOfDeath DistinguishGravityKillTrigger(KillLocalPlayer __instance)
        {
            var instanceGameObject = __instance.gameObject;
            var instanceParent = __instance.gameObject.transform.parent;

            if (instanceGameObject == null || instanceParent == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject or parent from KillLocalPlayer.");
                return AdvancedCauseOfDeath.Pit_Generic;
            }

            // Distinguish what pitfall killed the player by the name of the prefab.
            // Jank and prone to breaking but it works for now.

            var name = instanceParent.name.Replace("(Clone)", "");
            switch (name)
            {
                case "4x4BigStairTile": // Facility stairway with protected pit
                    return AdvancedCauseOfDeath.Pit_Facility_Pit;
                case "CatwalkTile2x1": // Facility protected catwalk
                    return AdvancedCauseOfDeath.Pit_Facility_Pit;
                case "CatwalkTile2x1Split": // Facility, the infamous "jump"
                    return AdvancedCauseOfDeath.Pit_Facility_Catwalk_Jump;
                case "LargeForkTileB": // U-shaped room in the facility
                    return AdvancedCauseOfDeath.Pit_Facility_Pit;
                case "SmallStairTile": // Spiral stairs in the facility
                    return AdvancedCauseOfDeath.Pit_Facility_Pit;

                case "CaveForkStairTile": // Cave in the Mines
                    return AdvancedCauseOfDeath.Pit_Mine_Cave;
                case "CaveLongRampTile": // Cave in the Mines
                    return AdvancedCauseOfDeath.Pit_Mine_Cave;
                case "DeepShaftTile": // Mines with protected pit
                    return AdvancedCauseOfDeath.Pit_Mine_Pit;
                case "MineshaftStartTile": // The elevator in the mines
                    return AdvancedCauseOfDeath.Pit_Mine_Elevator;

                default:
                    Plugin.Instance.PluginLogger.LogError($"Could not identify KillTrigger parent by name (got {name}).");
                    return AdvancedCauseOfDeath.Pit_Generic;
            }
        }
    }

    // Enemy_Tulip_Snake_Drop
    [HarmonyPatch(typeof(FlowerSnakeEnemy))]
    [HarmonyPatch("StopClingingOnLocalClient")]
    class FlowerSnakeEnemyStopClingingOnLocalClientPatch
    {
        public static void Prefix(FlowerSnakeEnemy __instance)
        {
            if (__instance.clingingToPlayer != null)
            {
                Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player...");

                if (__instance.clingingToPlayer.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player because they died...");

                    if (__instance.clingingToPlayer.causeOfDeath == AdvancedCauseOfDeath.Gravity)
                    {
                        Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player because they died of gravity! Will assume they were involved...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance.clingingToPlayer, AdvancedCauseOfDeath.Enemy_TulipSnake_Drop);
                    }
                }
            }
        }
    }
}
