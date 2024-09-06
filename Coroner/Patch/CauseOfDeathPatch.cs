using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

#nullable enable

namespace Coroner.Patch
{
    /*
     * A set of patches dedicated to tracking when a player dies in a specific manner,
     * and storing it in the AdvancedDeathTracker, because `causeOfDeath` is not precise enough.
     */

    // =========
    // Enemies
    // =========

    // Enemy_BaboonHawk
    [HarmonyPatch(typeof(BaboonBirdAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BaboonBirdAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Baboon Hawk damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BaboonHawk);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in BaboonBirdAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Bracken
    [HarmonyPatch(typeof(FlowermanAI))]
    [HarmonyPatch("killAnimation")]
    class FlowermanAIKillAnimationPatch
    {
        public static void Postfix(FlowermanAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Bracken snapping neck...");

                if (__instance.inSpecialAnimationWithPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after snapping neck!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player killed by Bracken! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Bracken);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in FlowermanAIKillAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_BunkerSpider
    [HarmonyPatch(typeof(SandSpiderAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SandSpiderAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Bunker Spider damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player killed by Bunker Spider! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BunkerSpider);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after hit by Forest Spider! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in SandSpiderAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_CircuitBees
    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyPatch("BeeKillPlayerOnLocalClient")]
    class RedLocustBeesBeeKillPlayerOnLocalClientPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Circuit Bee electrocution...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerDying.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player killed by Circuit Bees! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_CircuitBees);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in RedLocustBeesBeeKillPlayerOnLocalClientPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_CoilHead
    [HarmonyPatch(typeof(SpringManAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SpringManAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Coil Head damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player killed by Coil Head! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_CoilHead);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after hit by Coil Head! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in SpringManAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_EarthLeviathan
    [HarmonyPatch(typeof(SandWormAI))]
    [HarmonyPatch("EatPlayer")]
    class SandWormAIEatPlayerPatch
    {
        public static void Postfix(PlayerControllerB playerScript)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Sand Worm devouring...");

                if (playerScript == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player was eaten by Earth Leviathan! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerScript, AdvancedCauseOfDeath.Enemy_EarthLeviathan);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in SandWormAIEatPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_EyelessDog
    [HarmonyPatch(typeof(MouthDogAI))]
    [HarmonyPatch("KillPlayer")]
    class MouthDogAIKillPlayerPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after dog devouring...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player was killed by Eyeless Dog! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_EyelessDog);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in MouthDogAIKillPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_ForestGiant_Eaten
    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("EatPlayerAnimation")]
    class ForestGiantAIEatPlayerAnimationPatch
    {
        public static void Postfix(PlayerControllerB playerBeingEaten)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Forest Giant devouring...");

                if (playerBeingEaten == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                // These aren't true yet because this is a coroutine, UGH
                // if (playerBeingEaten.isPlayerDead && playerBeingEaten.causeOfDeath == CauseOfDeath.Crushing) {
                Plugin.Instance.PluginLogger.LogDebug("Player was eaten by Forest Giant! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerBeingEaten, AdvancedCauseOfDeath.Enemy_ForestGiant_Eaten, false);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ForestGiantAIEatPlayerAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_ForestGiant_Crushed
    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("AnimationEventA")]
    class ForestGiantAIAnimationEventAPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in ForestGiantAIAnimationEventAPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in ForestGiantAIAnimationEventAPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into ForestGiantAI.AnimationEventA...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            IList<LocalVariableInfo> localVars = method.GetMethodBody().LocalVariables;
            LocalVariableInfo? localVar_component = null;

            for (int i = 0; i < localVars.Count; i++)
            {
                var currentLocalVar = localVars[i];

                if (currentLocalVar.LocalType == typeof(PlayerControllerB))
                {
                    if (localVar_component != null)
                    {
                        Plugin.Instance.PluginLogger.LogError("Found multiple PlayerControllerB local variables in ForestGiantAIAnimationEventAPatch!");
                        return null;
                    }
                    localVar_component = currentLocalVar;
                    break;
                }
            }

            if (localVar_component == null) {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB local variable in ForestGiantAIAnimationEventAPatch!");
                return null;
            }

            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_0180: call      void [Coroner]Coroner.Patch.ForestGiantAIAnimationEventAPatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(ForestGiantAIAnimationEventAPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer)
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player was crushed by dying Forest Giant! Setting special cause of death...");
            // For whatever reason, the game tries to set Eaten here instead of Crushed.
            AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Enemy_ForestGiant_Death, true);
        }
    }

    // Enemy_GhostGirl
    [HarmonyPatch(typeof(DressGirlAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class DressGirlAIOnCollideWithPlayerPatch
    {
        public static void Postfix(DressGirlAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Processing Ghost Girl player collision...");

                if (__instance.hauntingPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after collision!");
                    return;
                }
                if (__instance.hauntingPlayer.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Ghost Girl! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance.hauntingPlayer, AdvancedCauseOfDeath.Enemy_GhostGirl);
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in DressGirlAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_HoarderBug
    [HarmonyPatch(typeof(HoarderBugAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class HoarderBugAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Hoarder Bug damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Hoarder Bug! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_HoarderBug);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after hit by Hoarder Bug! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in HoarderBugAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Hygrodere
    [HarmonyPatch(typeof(BlobAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BlobAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Hygrodere damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Hygrodere! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Hygrodere);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after hit by Hygrodere! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in BlobAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Jester
    [HarmonyPatch(typeof(JesterAI))]
    [HarmonyPatch("killPlayerAnimation")]
    class JesterAIKillPlayerAnimationPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Jester mauling...");

                PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player was killed by Jester! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Jester);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in JesterAIKillPlayerAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Maneater
    [HarmonyPatch(typeof(CaveDwellerAI))]
    [HarmonyPatch("killAnimation")]
    class CaveDwellerAIKillAnimationPatch
    {
        public static void Postfix(CaveDwellerAI __instance, PlayerControllerB killingPlayer)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Maneater mauling...");

                if (killingPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player was killed by Maneater! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(killingPlayer, AdvancedCauseOfDeath.Enemy_Maneater);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in CaveDwellerAIKillAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_SnareFlea
    [HarmonyPatch(typeof(CentipedeAI))]
    [HarmonyPatch("DamagePlayerOnIntervals")]
    class CentipedeAIDamagePlayerOnIntervalsPatch
    {
        public static void Postfix(CentipedeAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Snare Flea damage...");
                if (__instance.clingingToPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player being clung to!");
                    return;
                }

                if (__instance.clingingToPlayer.isPlayerDead && __instance.clingingToPlayer.causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Snare Flea! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance.clingingToPlayer, AdvancedCauseOfDeath.Enemy_SnareFlea);
                }
                else if (__instance.clingingToPlayer.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player somehow died while attacked by Snare Flea! Skipping...");
                    return;
                }
                else
                {
                    // Player still alive.
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in CentipedeAIDamagePlayerOnIntervalsPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_SporeLizard
    [HarmonyPatch(typeof(PufferAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class PufferAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Spore Lizard damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Spore Lizard! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_SporeLizard);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is alive after being hit by Spore Lizard! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in PufferAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Thumper
    [HarmonyPatch(typeof(CrawlerAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class CrawlerAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Thumper damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Thumper! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Thumper);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after being hit by Thumper! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in CrawlerAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_MaskedPlayer_Wear
    [HarmonyPatch(typeof(HauntedMaskItem))]
    [HarmonyPatch("FinishAttaching")]
    class HauntedMaskItemFinishAttachingPatch
    {
        public static void Postfix(HauntedMaskItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Mask attached to someone...");

                PlayerControllerB previousPlayerHeldBy = Traverse.Create(__instance).Field("previousPlayerHeldBy").GetValue<PlayerControllerB>();
                if (previousPlayerHeldBy == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (previousPlayerHeldBy.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by putting on a Mask! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(previousPlayerHeldBy, AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive after putting on a Mask! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in HauntedMaskItemFinishAttachingPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_MaskedPlayer_Victim
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    [HarmonyPatch("killAnimation")]
    class MaskedPlayerEnemykillAnimationPatch
    {
        public static void Postfix(MaskedPlayerEnemy __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Masked Player killed someone...");

                PlayerControllerB playerControllerB = __instance.inSpecialAnimationWithPlayer;
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                // playerControllerB.isPlayerDead is false here but we just assume they will die here.
                Plugin.Instance.PluginLogger.LogDebug("Player was killed by Masked Player! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in MaskedPlayerEnemykillAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Nutcracker_Kicked
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    [HarmonyPatch("LegKickPlayer")]
    class NutcrackerEnemyAILegKickPlayerPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Nutcracker kicked a player to death!");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                Plugin.Instance.PluginLogger.LogDebug("Player was kicked by Nutcracker! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in NutcrackerEnemyAILegKickPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Nutcracker_Shot
    // Player_Murder_Shotgun
    [HarmonyPatch(typeof(ShotgunItem))]
    [HarmonyPatch("ShootGun")]
    class ShotgunItemShootGunPatch
    {
        public static void Postfix(ShotgunItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling shotgun shot...");

                PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
                if (localPlayerController == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access local player after shotgun shot!");
                    return;
                }
                if (localPlayerController.isPlayerDead && localPlayerController.causeOfDeath == CauseOfDeath.Gunshots)
                {
                    if (__instance.isHeldByEnemy)
                    {
                        // Enemy Nutcracker fired the shotgun.
                        Plugin.Instance.PluginLogger.LogDebug("Player was killed by Shotgun (Nutcracker)! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(localPlayerController, AdvancedCauseOfDeath.Enemy_Nutcracker_Shot);
                    }
                    else
                    {
                        // Player fired the shotgun.
                        Plugin.Instance.PluginLogger.LogDebug("Player was killed by Shotgun (Player)! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(localPlayerController, AdvancedCauseOfDeath.Player_Murder_Shotgun);
                    }
                }
                else if (localPlayerController.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player died while attacked by shotgun? Skipping... " + localPlayerController.causeOfDeath);
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ShotgunItemShootGunPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Butler_Stab
    [HarmonyPatch(typeof(ButlerEnemyAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class ButlerEnemyAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Butler stab damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Butler (Stabbing)! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Butler_Stab);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ButlerEnemyAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Mask_Hornets
    [HarmonyPatch(typeof(ButlerBeesEnemyAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class ButlerBeesEnemyAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling Mask Hornet stab damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player was killed by Mask Hornets! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_MaskHornets);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ButlerBeesEnemyAIOnCollideWithPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Enemy_Tulip_Snake_Drop
    [HarmonyPatch(typeof(FlowerSnakeEnemy))]
    [HarmonyPatch("StopClingingOnLocalClient")]
    class FlowerSnakeEnemyStopClingingOnLocalClientPatch {
        public static void Prefix(FlowerSnakeEnemy __instance) {
            if (__instance.clingingToPlayer != null) {
                Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player...");

                if (__instance.clingingToPlayer.isPlayerDead) {
                    Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player because they died...");

                    if (__instance.clingingToPlayer.causeOfDeath == AdvancedCauseOfDeath.Gravity) {
                        Plugin.Instance.PluginLogger.LogDebug("Tulip Snake let go of player because they died of gravity! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance.clingingToPlayer, AdvancedCauseOfDeath.Enemy_TulipSnake_Drop);
                    }
                }
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
            if (__instance.inSpecialAnimationWithPlayer != null)
            {
                Plugin.Instance.PluginLogger.LogDebug("Player was torched to death by Old Bird, setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Old_Bird_Torch);
            }
        }
    }

    // Enemy_Old_Bird_Stomp
    [HarmonyPatch(typeof(RadMechAI))]
    [HarmonyPatch("Stomp")]
    class RadMechAIStompPatch
    {
        public static void Postfix(RadMechAI __instance)
        {
            PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;

            if (localPlayerController.isPlayerDead && !AdvancedDeathTracker.HasCauseOfDeath(localPlayerController)
                && localPlayerController.causeOfDeath == AdvancedCauseOfDeath.Crushing) {
                Plugin.Instance.PluginLogger.LogDebug("Player was crushed to death by Old Bird, setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(localPlayerController, AdvancedCauseOfDeath.Enemy_Old_Bird_Stomp);
            }
        }
    }

    // Enemy_Kidnapper_Wolf
    [HarmonyPatch(typeof(BushWolfEnemy))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BushWolfOnCollideWithPlayerPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in BushWolfOnCollideWithPlayerPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in BushWolfOnCollideWithPlayerPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into BushWolf.OnCollideWithPlayer...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // No local variables needed! We just need to make the static call.

            // IL_0180: call      void [Coroner]Coroner.Patch.BushWolfOnCollideWithPlayerPatch::RewriteCauseOfDeath()
            result.Add(new CodeInstruction(OpCodes.Call, typeof(BushWolfOnCollideWithPlayerPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath()
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player died to Kidnapper Wolf! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Enemy_KidnapperFox);
        }
    }

    // =========
    // Players
    // =========

    // Player_Ladder
    // Player_Quicksand
    // Player_StunGrenade
    // Other_Dropship
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("KillPlayer")]
    class PlayerControllerBKillPlayerPatch
    {
        public static void Prefix(PlayerControllerB __instance, ref CauseOfDeath causeOfDeath)
        {
            try
            {
                if (AdvancedDeathTracker.HasCauseOfDeath(__instance))
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player already has a known specific cause of death! Skipping advanced processing...");
                    return;
                }

                // NOTE: Only called on the client of the player who died.
                if (causeOfDeath == AdvancedCauseOfDeath.Other_Dropship)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died from item dropship! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Other_Dropship);
                    // Now to fix the jank by adding a normal value!
                    causeOfDeath = CauseOfDeath.Crushing;
                    return;
                }
                else if (causeOfDeath == AdvancedCauseOfDeath.Player_Ladder)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died from ladder! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Ladder);
                    // Now to fix the jank by adding a normal value!
                    causeOfDeath = CauseOfDeath.Crushing;
                    return;
                }

                if (__instance.isSinking && causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died of suffociation while sinking in quicksand! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Quicksand);
                }
                else if (causeOfDeath == CauseOfDeath.Blast)
                {
                    // Make a guess at what caused the blast.
                    var heldObjectServer = __instance.currentlyHeldObjectServer;
                    if (heldObjectServer != null)
                    {
                        var heldObjectGameObject = heldObjectServer.gameObject;
                        if (heldObjectGameObject != null)
                        {
                            var heldObject = heldObjectGameObject.GetComponent<GrabbableObject>();
                            if (heldObject != null)
                            {
                                if (heldObject is StunGrenadeItem)
                                {
                                    // Players take 20 damage when holding a stun grenade while it explodes.
                                    Plugin.Instance.PluginLogger.LogDebug("Player died from stun grenade explosion! Setting special cause of death...");
                                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_StunGrenade);
                                }
                            }
                        }

                    }
                }
                else if (causeOfDeath == CauseOfDeath.Snipped)
                {
                    // There's only one method of triggering this.
                    Plugin.Instance.PluginLogger.LogDebug($"Player died to Snipped! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Enemy_Barber);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is dying! No cause of death registered in hook...");
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in PlayerControllerBKillPlayerPatch.Prefix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
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

        public static void MaybeRewriteCauseOfDeath(PlayerControllerB targetPlayer, int damageAmount, Vector3 hitDirection, int playerWhoHit, int newHealthAmount) {
            Plugin.Instance.PluginLogger.LogDebug($"Player damaged another player ${targetPlayer}({damageAmount}, {hitDirection}, {playerWhoHit}, {newHealthAmount})");
            // Called when the player is DAMAGED by an explosion, but not necessarily killed.
            if (targetPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from friendly fire damage");
                RewriteCauseOfDeath(targetPlayer, playerWhoHit);
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from friendly fire (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer, int playerWhoHitIndex)
        {
            PlayerControllerB playerWhoHit = StartOfRound.Instance.allPlayerScripts[playerWhoHitIndex];

            if (targetPlayer == null) {
                Plugin.Instance.PluginLogger.LogError("Damage from other client: victim is null!");
            } else if (playerWhoHit == null) {
                Plugin.Instance.PluginLogger.LogError("Damage from other client: attacker is null!");
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from murder ({targetPlayer.causeOfDeath}), determining special cause of death...");

                if (AdvancedDeathTracker.IsHoldingShotgun(playerWhoHit)) {
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


    // Player_Ladder
    [HarmonyPatch(typeof(ExtensionLadderItem))]
    [HarmonyPatch("StartLadderAnimation")]
    class ExtensionLadderItemStartLadderAnimationPatch
    {
        public static void Postfix(ExtensionLadderItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Extension ladder started animation! Modifying kill trigger...");

                GameObject? extensionLadderGameObject = __instance.gameObject;
                if (extensionLadderGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ExtensionLadderItem.");
                    return;
                }
                Transform? killTriggerTransform = extensionLadderGameObject.transform.Find("AnimContainer/MeshContainer/LadderMeshContainer/BaseLadder/LadderSecondPart/KillTrigger");

                if (killTriggerTransform == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ExtensionLadderItem.");
                    return;
                }

                GameObject? killTriggerGameObject = killTriggerTransform.gameObject;

                if (killTriggerGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ExtensionLadderItem.");
                    return;
                }

                KillLocalPlayer? killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

                if (killLocalPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
                    return;
                }

                // Modify the trigger to specify cause of death in a janky way.
                killLocalPlayer.causeOfDeath = AdvancedCauseOfDeath.Player_Ladder;
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ExtensionLadderItemStartLadderAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Player_Cruiser_Driver
    // Player_Cruiser_Passenger
    // Player_Cruiser_Explode_Bystander
    [HarmonyPatch(typeof(VehicleController))]
    [HarmonyPatch("DestroyCar")]
    class VehicleControllerDestroyCarPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in VehicleControllerDestroyCarPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in VehicleControllerDestroyCarPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into VehicleController.DestroyCar...");
                // Inject BEFORE the KillPlayer rather than after, so they're still in the vehicle.
                code.InsertRange(insertionIndex, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // IL_024B: ldarg.0
            result.Add(new CodeInstruction(OpCodes.Ldarg_0));

            // IL_0180: call      void [Coroner]Coroner.Patch.VehicleControllerDestroyCarPatch::RewriteCauseOfDeath()
            result.Add(new CodeInstruction(OpCodes.Call, typeof(VehicleControllerDestroyCarPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath(VehicleController vehicle)
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player died to Cruiser Explosion! Setting special cause of death...");

            if (vehicle == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not get reference to vehicle...");
                AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Explode_Bystander);
            } else {

                Plugin.Instance.PluginLogger.LogDebug($"Got vehicle controller. ({vehicle.localPlayerInControl}, {vehicle.localPlayerInPassengerSeat})");

                if (vehicle.localPlayerInControl) {
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Driver);
                } else if (vehicle.localPlayerInPassengerSeat) {
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Passenger);
                } else {
                    Plugin.Instance.PluginLogger.LogWarning("Could not get reference to local player in control or passenger seat...");
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Explode_Bystander);
                }
            }
        }
    }

    [HarmonyPatch(typeof(VehicleController))]
    [HarmonyPatch("RemovePlayerControlOfVehicleClientRpc")]
    class VehicleControllerRemovePlayerControlOfVehicleClientRpcPatch {

        static void Postfix(VehicleController __instance, int playerId, bool setIgnitionStarted) {
            Plugin.Instance.PluginLogger.LogDebug($"Removed player control of vehicle: {playerId} ({setIgnitionStarted})");
        }
    }

    // Player_Cruiser_Driver
    // Player_Cruiser_Passenger
    [HarmonyPatch(typeof(VehicleController))]
    [HarmonyPatch("DamagePlayerInVehicle")]
    class VehicleControllerDamagePlayerInVehiclePatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";
        // IL_021c: callvirt instance void GameNetcodeStuff.PlayerControllerB::DamagePlayer(int32, bool, bool, valuetype CauseOfDeath, int32, bool, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string DAMAGE_PLAYER_SIGNATURE = "Void DamagePlayer(Int32, Boolean, Boolean, CauseOfDeath, Int32, Boolean, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInjectKill = BuildInstructionsToInsertKill(method);
            if (codeToInjectKill == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (kill) to insert in VehicleControllerDamagePlayerInVehiclePatch! Safely aborting...");
                return instructions;
            }

            var codeToInjectDamage = BuildInstructionsToInsertDamage(method);
            if (codeToInjectDamage == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (damage) to insert in VehicleControllerDamagePlayerInVehiclePatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndexKill = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndexKill = i;
                }
            }

            if (insertionIndexKill == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in VehicleControllerDamagePlayerInVehiclePatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting kill patches into VehicleController.DamagePlayer...");
                code.InsertRange(insertionIndexKill + 1, codeToInjectKill);
                Plugin.Instance.PluginLogger.LogDebug("Done.");
            }

            // Search for where PlayerControllerB.DamagePlayer is called.
            // Do this after the first insection is done.
            int insertionIndexDamage = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == DAMAGE_PLAYER_SIGNATURE)
                {
                    insertionIndexDamage = i;
                }
            }

            if (insertionIndexDamage == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.DamagePlayer call in VehicleControllerDamagePlayerInVehiclePatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting damage patches into VehicleController.DamagePlayerInVehicle...");
                code.InsertRange(insertionIndexDamage + 1, codeToInjectDamage);
                Plugin.Instance.PluginLogger.LogDebug("Done.");
            }

            Plugin.Instance.PluginLogger.LogDebug("VehicleController.DamagePlayerInVehicle patch done.");
            return code;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertKill(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // IL_024B: ldarg.0
            result.Add(new CodeInstruction(OpCodes.Ldarg_0));

            // IL_0180: call      void [Coroner]Coroner.Patch.VehicleControllerDamagePlayerInVehiclePatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(VehicleControllerDamagePlayerInVehiclePatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertDamage(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // IL_024B: ldarg.0
            result.Add(new CodeInstruction(OpCodes.Ldarg_0));

            // IL_0180: call      void [Coroner]Coroner.Patch.VehicleControllerDamagePlayerInVehiclePatch::MaybeRewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(VehicleControllerDamagePlayerInVehiclePatch).GetMethod(nameof(MaybeRewriteCauseOfDeath))));

            return result;
        }

        public static void MaybeRewriteCauseOfDeath(VehicleController vehicle) {
            // Called when the player is DAMAGED by a crash, but not necessarily killed.
            var targetPlayer = GameNetworkManager.Instance.localPlayerController;
            if (targetPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from car accident damage");
                RewriteCauseOfDeath(vehicle);
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from car accident (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath(VehicleController vehicle)
        {
            // This function sets the cause of death when the player is killed in a crash.
            Plugin.Instance.PluginLogger.LogDebug("Player died to Cruiser Explosion! Setting special cause of death...");

            if (vehicle == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not get reference to vehicle...");
                AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Ran_Over);
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Got vehicle controller. ({vehicle.localPlayerInControl}, {vehicle.localPlayerInPassengerSeat})");

                if (vehicle.localPlayerInControl) {
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Driver);
                } else if (vehicle.localPlayerInPassengerSeat) {
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Passenger);
                } else {
                    Plugin.Instance.PluginLogger.LogWarning("Could not get reference to local player in control or passenger seat...");
                    AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Ran_Over);
                }
            }
        }
    }

    // Player_Cruiser_Ran_Over
    [HarmonyPatch(typeof(VehicleCollisionTrigger))]
    [HarmonyPatch("OnTriggerEnter")]
    class VehicleCollisionTriggerOnTriggerEnterPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";
        // IL_021c: callvirt instance void GameNetcodeStuff.PlayerControllerB::DamagePlayer(int32, bool, bool, valuetype CauseOfDeath, int32, bool, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string DAMAGE_PLAYER_SIGNATURE = "Void DamagePlayer(Int32, Boolean, Boolean, CauseOfDeath, Int32, Boolean, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInjectKill = BuildInstructionsToInsertKill(method);
            if (codeToInjectKill == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (kill) to insert in VehicleCollisionTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }

            var codeToInjectDamage = BuildInstructionsToInsertDamage(method);
            if (codeToInjectDamage == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (damage) to insert in VehicleCollisionTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndexKill = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndexKill = i;
                }
            }

            if (insertionIndexKill == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in VehicleCollisionTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #1 into VehicleCollisionTrigger.OnTriggerEnter...");
                code.InsertRange(insertionIndexKill + 1, codeToInjectKill);
                Plugin.Instance.PluginLogger.LogDebug("Done.");
            }

            // Search for where PlayerControllerB.DamagePlayer is called.
            // Do this after the first insection is done.
            int insertionIndexDamage = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == DAMAGE_PLAYER_SIGNATURE)
                {
                    insertionIndexDamage = i;
                }
            }

            if (insertionIndexDamage == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.DamagePlayer call in VehicleCollisionTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #2 into VehicleCollisionTrigger.OnTriggerEnter...");
                code.InsertRange(insertionIndexDamage + 1, codeToInjectDamage);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

            }

            Plugin.Instance.PluginLogger.LogDebug("Done with all VehicleCollisionTriggerOnTriggerEnterPatch patches.");
            return code;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertKill(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // IL_024B: ldarg.0
            result.Add(new CodeInstruction(OpCodes.Ldarg_0));

            // IL_0180: call      void [Coroner]Coroner.Patch.VehicleControllerDamagePlayerInVehiclePatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(VehicleControllerDamagePlayerInVehiclePatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertDamage(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // IL_024B: ldarg.0
            result.Add(new CodeInstruction(OpCodes.Ldarg_0));

            // IL_0180: call      void [Coroner]Coroner.Patch.VehicleControllerDamagePlayerInVehiclePatch::MaybeRewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(VehicleControllerDamagePlayerInVehiclePatch).GetMethod(nameof(MaybeRewriteCauseOfDeath))));

            return result;
        }

        public static void MaybeRewriteCauseOfDeath() {
            // Called when the player is DAMAGED by being run over, but not necessarily killed.
            var targetPlayer = GameNetworkManager.Instance.localPlayerController;
            if (targetPlayer.isPlayerDead) {
                if (targetPlayer.causeOfDeath == CauseOfDeath.Crushing) {
                    Plugin.Instance.PluginLogger.LogDebug($"Player died from car accident damage");
                    RewriteCauseOfDeath();
                } else {
                    Plugin.Instance.PluginLogger.LogWarning($"Player was hit by a car but died of something else? {targetPlayer.causeOfDeath}");
                }
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from car accident (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath()
        {
            // This function sets the cause of death when the player is run over.
            Plugin.Instance.PluginLogger.LogDebug("Player died to run over by Cruiser! Setting special cause of death...");

            AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Player_Cruiser_Ran_Over);
        }
    }

    // =========
    // Other
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
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";
        // IL_021c: callvirt instance void GameNetcodeStuff.PlayerControllerB::DamagePlayer(int32, bool, bool, valuetype CauseOfDeath, int32, bool, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string DAMAGE_PLAYER_SIGNATURE = "Void DamagePlayer(Int32, Boolean, Boolean, CauseOfDeath, Int32, Boolean, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInjectKill = BuildInstructionsToInsertKill(method);
            if (codeToInjectKill == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (kill) to insert in LandmineSpawnExplosionPatch! Safely aborting...");
                return instructions;
            }

            var codeToInjectDamage = BuildInstructionsToInsertDamage(method);
            if (codeToInjectDamage == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions (damage) to insert in LandmineSpawnExplosionPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndexKill = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndexKill = i;
                }
            }

            if (insertionIndexKill == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in LandmineSpawnExplosionPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #1 into Landmine.SpawnExplosion...");
                code.InsertRange(insertionIndexKill + 1, codeToInjectKill);
                Plugin.Instance.PluginLogger.LogDebug("Done.");
            }

            // Search for where PlayerControllerB.DamagePlayer is called.
            // Do this after the first insection is done.
            int insertionIndexDamage = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == DAMAGE_PLAYER_SIGNATURE)
                {
                    insertionIndexDamage = i;
                }
            }

            if (insertionIndexDamage == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.DamagePlayer call in LandmineSpawnExplosionPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch #2 into Landmine.SpawnExplosion...");
                code.InsertRange(insertionIndexDamage + 1, codeToInjectDamage);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

            }

            Plugin.Instance.PluginLogger.LogDebug("Done with all LandmineSpawnExplosionPatch patches.");
            return code;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertKill(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // var argumentIndex_explosionPosition = 0;
            // var argumentIndex_spawnExplosionEffect = 1;
            var argumentIndex_killRange = 2;
            // var argumentIndex_damageRange = 3;

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

            if (localVar_component == null) {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB local variable in LandmineSpawnExplosionPatch!");
                return null;
            }

            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_017F: ldarg.2
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_killRange));

            // IL_0180: call      void [Coroner]Coroner.Patch.LandmineSpawnExplosionPatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(LandmineSpawnExplosionPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        static List<CodeInstruction>? BuildInstructionsToInsertDamage(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // var argumentIndex_explosionPosition = 0;
            // var argumentIndex_spawnExplosionEffect = 1;
            var argumentIndex_killRange = 2;
            // var argumentIndex_damageRange = 3;
            // var argumentIndex_nonLethalDamage = 4;
            var argumentIndex_physicsForce = 5;
            // var argumentIndex_overridePrefab = 6;
            // var argumentIndex_goThroughCards = 7;

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

            if (localVar_component == null) {
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

        public static void MaybeRewriteCauseOfDeath(PlayerControllerB targetPlayer, float killRange, float physicsForce) {
            // Called when the player is DAMAGED by an explosion, but not necessarily killed.
            if (targetPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogDebug($"Player died from landmine damage");
                RewriteCauseOfDeath(targetPlayer, killRange, physicsForce);
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player did not die from landmine (left at ${targetPlayer.health} health)");
            }
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer, float killRange, float physicsForce)
        {
            // This function sets different causes of death for the player based on the kill range of the explosion.
            // This works because different explosion types have different kill ranges.
            // EDIT: It's a miracle this workaround can still work in v51.
            // It will become a problem if two different sources can create identically-sized explosions.

            AdvancedCauseOfDeath causeOfDeath = AdvancedCauseOfDeath.Blast;
            if ( /* killRange == ??.?f && */ physicsForce == 50.0f) {
                // Check if it's a meteor (which has a random kill range)
                causeOfDeath = AdvancedCauseOfDeath.Other_Meteor;
            }
            else if (killRange == 0.0f && physicsForce == 80.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Enemy_Butler_Explode;
            }
            else if (killRange == 6.0f && physicsForce == 200.0f)
            {
                // Assume that the player is outside the vehicle.
                // We can update this later.
                causeOfDeath = AdvancedCauseOfDeath.Player_Cruiser_Explode_Bystander;
            }
            else if (killRange == 5.0f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Player_Jetpack_Blast;
            }
            else if (killRange == 5.7f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Other_Landmine;
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
            else if (killRange == 2.4f && physicsForce == 0.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Other_Lightning;
            }
            else if (killRange == 0.5f && physicsForce == 45.0f)
            {
                causeOfDeath = AdvancedCauseOfDeath.Player_EasterEgg;
            } else {
                Plugin.Instance.PluginLogger.LogWarning($"Could not identify explosion type! Using generic cause of death for Blasts...");
            }

            Plugin.Instance.PluginLogger.LogDebug($"Player died from Explosion({causeOfDeath})! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, causeOfDeath);
        }
    }

    // Other_DepositItemsDesk
    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyPatch("AnimationGrabPlayer")]
    class DepositItemsDeskAnimationGrabPlayerPatch
    {
        public static void Postfix(int playerID)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after tentacle devouring...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerID];
                Plugin.Instance.PluginLogger.LogDebug("Player was killed by the Deposit Desk! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Other_DepositItemsDesk);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in DepositItemsDeskAnimationGrabPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Other_Turret
    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("Update")]
    class TurretUpdatePatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in TurretUpdatePatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in TurretUpdatePatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into Turret.Update...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // No local variables needed! We just need to make the static call.

            // IL_0180: call      void [Coroner]Coroner.Patch.TurretUpdatePatch::RewriteCauseOfDeath()
            result.Add(new CodeInstruction(OpCodes.Call, typeof(TurretUpdatePatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath()
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player died to Turret! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Other_Turret);
        }
    }
 
    // Unknown, possibly a trigger on the player ship 
    [HarmonyPatch(typeof(AnimatedObjectFloatSetter))]
    [HarmonyPatch("KillPlayerAtPoint")]
    class AnimatedObjectFloatSetterKillPlayerAtPointPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in AnimatedObjectFloatSetterKillPlayerAtPointPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in AnimatedObjectFloatSetterKillPlayerAtPointPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into AnimatedObjectFloatSetter.KillPlayerAtPoint...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // No local variables needed! We just need to make the static call.

            // IL_0180: call      void [Coroner]Coroner.Patch.AnimatedObjectFloatSetterKillPlayerAtPointPatch::RewriteCauseOfDeath()
            result.Add(new CodeInstruction(OpCodes.Call, typeof(AnimatedObjectFloatSetterKillPlayerAtPointPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath()
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player died to AnimatedObjectFloatSetter! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Unknown);
        }
    }

    // Ensure causes of death get reset when relogging
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyPatch("StartGame")]
    class StartOfRoundStartGamePatch
    {
        public static void Postfix()
        {
            Plugin.Instance.PluginLogger.LogDebug("New round started! Resetting causes of death...");
            AdvancedDeathTracker.ClearDeathTracker();
        }
    }

    // Other_Spike_Trap
    [HarmonyPatch(typeof(SpikeRoofTrap))]
    [HarmonyPatch("OnTriggerStay")]
    class SpikeRoofTrapOnTriggerStayPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in SpikeRoofTrapOnTriggerStayPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in SpikeRoofTrapOnTriggerStayPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into SpikeRoofTrap.OnTriggerStay...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            // No local variables needed! We just need to make the static call.

            // IL_0180: call      void [Coroner]Coroner.Patch.SpikeRoofTrapOnTriggerStayPatch::RewriteCauseOfDeath()
            result.Add(new CodeInstruction(OpCodes.Call, typeof(SpikeRoofTrapOnTriggerStayPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath()
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player died to Spike Trap! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(GameNetworkManager.Instance.localPlayerController, AdvancedCauseOfDeath.Other_Spike_Trap);
        }
    }

    // Other_OutOfBounds
    [HarmonyPatch(typeof(OutOfBoundsTrigger))]
    [HarmonyPatch("OnTriggerEnter")]
    class OutOfBoundsTriggerOnTriggerEnterPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32, UnityEngine.Vector3)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
        {
            var code = new List<CodeInstruction>(instructions);

            // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in OutOfBoundsTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }

            // Search for where PlayerControllerB.KillPlayer is called.
            int insertionIndex = -1;
            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == KILL_PLAYER_SIGNATURE)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex == -1)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in OutOfBoundsTriggerOnTriggerEnterPatch! Safely aborting...");
                return instructions;
            }
            else
            {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogDebug("Injecting patch into OutOfBoundsTrigger.OnTriggerEnter...");
                code.InsertRange(insertionIndex + 1, codeToInject);
                Plugin.Instance.PluginLogger.LogDebug("Done.");

                return code;
            }
        }

        static List<CodeInstruction>? BuildInstructionsToInsert(MethodBase method)
        {
            var result = new List<CodeInstruction>();

            IList<LocalVariableInfo> localVars = method.GetMethodBody().LocalVariables;
            LocalVariableInfo? localVar_component = null;

            for (int i = 0; i < localVars.Count; i++)
            {
                var currentLocalVar = localVars[i];

                if (currentLocalVar.LocalType == typeof(PlayerControllerB))
                {
                    if (localVar_component != null)
                    {
                        Plugin.Instance.PluginLogger.LogError("Found multiple PlayerControllerB local variables in OutOfBoundsTriggerOnTriggerEnterPatch!");
                        return null;
                    }
                    localVar_component = currentLocalVar;
                    break;
                }
            }

            if (localVar_component == null)
            {
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB local variable in OutOfBoundsTriggerOnTriggerEnterPatch!");
                return null;
            }

            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_0180: call      void [Coroner]Coroner.Patch.OutOfBoundsTriggerOnTriggerEnterPatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(OutOfBoundsTriggerOnTriggerEnterPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer)
        {
            // This function sets the cause of death when the player is crushed by the giant's body.
            Plugin.Instance.PluginLogger.LogDebug("Player fell out of the facility! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Other_OutOfBounds);
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
                if (itemDropshipGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ItemDropship.");
                    return;
                }
                Transform? killTriggerTransform = itemDropshipGameObject.transform.Find("ItemShip/KillTrigger");

                if (killTriggerTransform == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ItemDropship.");
                    return;
                }

                GameObject? killTriggerGameObject = killTriggerTransform.gameObject;

                if (killTriggerGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ItemDropship.");
                    return;
                }

                KillLocalPlayer? killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

                if (killLocalPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
                    return;
                }

                // Modify the trigger to specify cause of death in a janky way.
                killLocalPlayer.causeOfDeath = AdvancedCauseOfDeath.Other_Dropship;
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ItemDropshipStartPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    // Other_Facility_Pit
    // Generic_Fan
    [HarmonyPatch(typeof(KillLocalPlayer))]
    [HarmonyPatch("KillPlayer")]
    class KillLocalPlayerKillPlayerPatch
    {
        // Sadly, we can't patch "Start" because the class doesn't reimplement it.

        public static void Postfix(KillLocalPlayer __instance, PlayerControllerB playerWhoTriggered)
        {
            try
            {
                // Modify the cause of death in a janky way.
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
                else
                {
                    // Display an unknown cause of death.
                    Plugin.Instance.PluginLogger.LogDebug($"Player died to UNKNOWN KillPlayerTrigger({__instance.causeOfDeath})! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerWhoTriggered, __instance.causeOfDeath);
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in KillLocalPlayerStartPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }

        public static AdvancedCauseOfDeath DistinguishGravityKillTrigger(KillLocalPlayer __instance) {
            var instanceGameObject = __instance.gameObject;
            var instanceParent = __instance.gameObject.transform.parent;

            if (instanceGameObject == null || instanceParent == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject or parent from KillLocalPlayer.");
                return AdvancedCauseOfDeath.Pit_Generic;
            }

            var name = instanceParent.name.Replace("(Clone)", "");
            switch (name) {
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
}