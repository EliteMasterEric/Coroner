using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Coroner.Patch
{
    /*
     * A set of patches dedicated to tracking when a player dies in a specific manner,
     * and storing it in the AdvancedDeathTracker, because `causeOfDeath` is not precise enough.
     */

    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("KillPlayer")]
    class PlayerControllerBKillPlayerPatch
    {
        public static void Prefix(PlayerControllerB __instance, ref CauseOfDeath causeOfDeath)
        {
            try
            {
                // NOTE: Only called on the client of the player who died.

                if ((int)causeOfDeath == AdvancedDeathTracker.PLAYER_CAUSE_OF_DEATH_DROPSHIP)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player died from item dropship! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Dropship);
                    // Now to fix the jank by adding a normal value!
                    causeOfDeath = CauseOfDeath.Crushing;
                    return;
                }

                if (__instance.isSinking && causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player died of suffociation while sinking in quicksand! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Quicksand);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is dying! No cause of death registered in hook...");
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in PlayerControllerBKillPlayerPatch.Prefix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyPatch("AnimationGrabPlayer")]
    class DepositItemsDeskAnimationGrabPlayerPatch
    {
        public static void Postfix(int playerID)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after tentacle devouring...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerID];
                Plugin.Instance.PluginLogger.LogInfo("Player is dying! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Player_DepositItemsDesk);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in DepositItemsDeskAnimationGrabPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(JesterAI))]
    [HarmonyPatch("killPlayerAnimation")]
    class JesterAIKillPlayerAnimationPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after Jester mauling...");

                PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Jester);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in JesterAIKillPlayerAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(SandWormAI))]
    [HarmonyPatch("EatPlayer")]
    class SandWormAIEatPlayerPatch
    {
        public static void Postfix(PlayerControllerB playerScript)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after Sand Worm devouring...");

                if (playerScript == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerScript, AdvancedCauseOfDeath.Enemy_EarthLeviathan);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in SandWormAIEatPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyPatch("BeeKillPlayerOnLocalClient")]
    class RedLocustBeesBeeKillPlayerOnLocalClientPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after Circuit Bee electrocution...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerDying.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_CircuitBees);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(DressGirlAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class DressGirlAIOnCollideWithPlayerPatch
    {
        public static void Postfix(DressGirlAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Processing Ghost Girl player collision...");

                if (__instance.hauntingPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after collision!");
                    return;
                }
                if (__instance.hauntingPlayer.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
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

    [HarmonyPatch(typeof(FlowermanAI))]
    [HarmonyPatch("killAnimation")]
    class FlowermanAIKillAnimationPatch
    {
        public static void Postfix(FlowermanAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after Bracken snapping neck...");

                if (__instance.inSpecialAnimationWithPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after snapping neck!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Bracken);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in FlowermanAIKillAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("EatPlayerAnimation")]
    class ForestGiantAIEatPlayerAnimationPatch
    {
        public static void Postfix(PlayerControllerB playerBeingEaten)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after Forest Giant devouring...");

                if (playerBeingEaten == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerBeingEaten, AdvancedCauseOfDeath.Enemy_ForestGiant);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ForestGiantAIEatPlayerAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(MouthDogAI))]
    [HarmonyPatch("KillPlayer")]
    class MouthDogAIKillPlayerPatch
    {
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Accessing state after dog devouring...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_EyelessDog);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in MouthDogAIKillPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(CentipedeAI))]
    [HarmonyPatch("DamagePlayerOnIntervals")]
    class CentipedeAIDamagePlayerOnIntervalsPatch
    {
        public static void Postfix(CentipedeAI __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Snare Flea damage...");
                if (__instance.clingingToPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player being clung to!");
                    return;
                }

                if (__instance.clingingToPlayer.isPlayerDead && __instance.clingingToPlayer.causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance.clingingToPlayer, AdvancedCauseOfDeath.Enemy_SnareFlea);
                }
                else if (__instance.clingingToPlayer.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player somehow died while attacked by Snare Flea! Skipping...");
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

    [HarmonyPatch(typeof(BaboonBirdAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BaboonBirdAIOnCollideWithPlayerPatch
    {
        public static void Postfix(BaboonBirdAI __instance, Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Baboon Hawk damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BaboonHawk);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("DamagePlayerFromOtherClientClientRpc")]
    class PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch
    {
        public static void Postfix(PlayerControllerB __instance, int playerWhoHit)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling friendly fire damage...");
                if (__instance == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access victim after death!");
                    return;
                }
                // PlayerControllerB playerControllerWhoHit = StartOfRound.Instance.allPlayerScripts[playerWhoHit];

                if (__instance.isPlayerDead)
                {
                    if (__instance.causeOfDeath == CauseOfDeath.Bludgeoning)
                    {
                        Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Melee);
                    }
                    else if (__instance.causeOfDeath == CauseOfDeath.Mauling)
                    {
                        Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Melee);
                    }
                    else if (__instance.causeOfDeath == CauseOfDeath.Gunshots)
                    {
                        Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Shotgun);
                    }
                    else
                    {
                        Plugin.Instance.PluginLogger.LogWarning("Player was killed by someone else but we don't know how! " + __instance.causeOfDeath);
                    }
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(PufferAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class PufferAIOnCollideWithPlayerPatch
    {
        public static void Postfix(PufferAI __instance, Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Spore Lizard damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_SporeLizard);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(SpringManAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SpringManAIOnCollideWithPlayerPatch
    {
        public static void Postfix(PufferAI __instance, Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Coil Head damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_CoilHead);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(BlobAI))]
    [HarmonyPatch("SlimeKillPlayerEffectServerRpc")]
    class BlobAISlimeKillPlayerEffectServerRpcPatch
    {
        public static void Postfix(int playerKilled)
        {
            try
            {
                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerKilled];

                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerDying.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_Hygrodere);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in BlobAISlimeKillPlayerEffectServerRpcPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(HoarderBugAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class HoarderBugAIOnCollideWithPlayerPatch
    {
        public static void Postfix(Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Hoarder Bug damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_HoarderBug);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(CrawlerAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class CrawlerAIOnCollideWithPlayerPatch
    {
        public static void Postfix(CrawlerAI __instance, Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Thumper damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Thumper);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(SandSpiderAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class SandSpiderAIOnCollideWithPlayerPatch
    {
        public static void Postfix(SandSpiderAI __instance, Collider other)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling Bunker Spider damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BunkerSpider);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    [HarmonyPatch("LegKickPlayer")]
    class NutcrackerEnemyAILegKickPlayerPatch
    {
        public static void Postfix(NutcrackerEnemyAI __instance, int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Nutcracker kicked a player to death!");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                Plugin.Instance.PluginLogger.LogInfo("Player is dying! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in NutcrackerEnemyAILegKickPlayerPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(ShotgunItem))]
    [HarmonyPatch("ShootGun")]
    class ShotgunItemShootGunPatch
    {
        public static void Postfix(ShotgunItem __instance, Vector3 shotgunPosition, Vector3 shotgunForward)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Handling shotgun shot...");

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
                        Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(localPlayerController, AdvancedCauseOfDeath.Enemy_Nutcracker_Shot);
                    }
                    else
                    {
                        // Player fired the shotgun.
                        Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
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

    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    [HarmonyPatch("killAnimation")]
    class MaskedPlayerEnemykillAnimationPatch
    {
        public static void Postfix(MaskedPlayerEnemy __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Masked Player killed someone...");

                PlayerControllerB playerControllerB = __instance.inSpecialAnimationWithPlayer;
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                // playerControllerB.isPlayerDead is false here but we just assume they will die here.
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim);
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in MaskedPlayerEnemykillAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(HauntedMaskItem))]
    [HarmonyPatch("FinishAttaching")]
    class HauntedMaskItemFinishAttachingPatch
    {
        public static void Postfix(HauntedMaskItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Masked Player killed someone...");

                PlayerControllerB previousPlayerHeldBy = Traverse.Create(__instance).Field("previousPlayerHeldBy").GetValue<PlayerControllerB>();
                if (previousPlayerHeldBy == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (previousPlayerHeldBy.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(previousPlayerHeldBy, AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
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

    [HarmonyPatch(typeof(ExtensionLadderItem))]
    [HarmonyPatch("StartLadderAnimation")]
    class ExtensionLadderItemStartLadderAnimationPatch
    {
        public static void Postfix(ExtensionLadderItem __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Extension ladder started animation! Modifying kill trigger...");

                GameObject extensionLadderGameObject = __instance.gameObject;
                if (extensionLadderGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ExtensionLadderItem.");
                }
                Transform killTriggerTransform = extensionLadderGameObject.transform.Find("AnimContainer/MeshContainer/LadderMeshContainer/BaseLadder/LadderSecondPart/KillTrigger");

                if (killTriggerTransform == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ExtensionLadderItem.");
                }

                GameObject killTriggerGameObject = killTriggerTransform.gameObject;

                if (killTriggerGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ExtensionLadderItem.");
                }

                KillLocalPlayer killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

                if (killLocalPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
                }

                // Correct the cause of death.
                killLocalPlayer.causeOfDeath = CauseOfDeath.Crushing;
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ExtensionLadderItemStartLadderAnimationPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(ItemDropship))]
    [HarmonyPatch("Start")]
    class ItemDropshipStartPatch
    {
        public static void Postfix(ItemDropship __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo("Item dropship spawned! Modifying kill trigger...");

                GameObject itemDropshipGameObject = __instance.gameObject;
                if (itemDropshipGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ItemDropship.");
                }
                Transform killTriggerTransform = itemDropshipGameObject.transform.Find("ItemShip/KillTrigger");

                if (killTriggerTransform == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ItemDropship.");
                }

                GameObject killTriggerGameObject = killTriggerTransform.gameObject;

                if (killTriggerGameObject == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ItemDropship.");
                }

                KillLocalPlayer killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

                if (killLocalPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
                }

                // Modify the cause of death in a janky way.
                killLocalPlayer.causeOfDeath = (CauseOfDeath)AdvancedDeathTracker.PLAYER_CAUSE_OF_DEATH_DROPSHIP;
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in ItemDropshipStartPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }
}