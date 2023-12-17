using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
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
                    Plugin.Instance.PluginLogger.LogDebug("Player died from item dropship! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Other_Dropship);
                    // Now to fix the jank by adding a normal value!
                    causeOfDeath = CauseOfDeath.Crushing;
                    return;
                }

                if (__instance.isSinking && causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player died of suffociation while sinking in quicksand! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Quicksand);
                } else if (causeOfDeath == CauseOfDeath.Blast) {
                    // Determine what caused the blast.
                    // WARNING: This code is very janky and may cause mental damage to anyone who reads it.

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
                Plugin.Instance.PluginLogger.LogDebug("Player is dying! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Other_DepositItemsDesk);
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Jester mauling...");

                PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Sand Worm devouring...");

                if (playerScript == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Circuit Bee electrocution...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerDying.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Bracken snapping neck...");

                if (__instance.inSpecialAnimationWithPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after snapping neck!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after Forest Giant devouring...");

                if (playerBeingEaten == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Accessing state after dog devouring...");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                if (playerDying == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Handling Snare Flea damage...");
                if (__instance.clingingToPlayer == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player being clung to!");
                    return;
                }

                if (__instance.clingingToPlayer.isPlayerDead && __instance.clingingToPlayer.causeOfDeath == CauseOfDeath.Suffocation)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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

    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("DamagePlayerFromOtherClientClientRpc")]
    class PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch
    {
        public static void Postfix(PlayerControllerB __instance)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Handling friendly fire damage...");
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
                        Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Melee);
                    }
                    else if (__instance.causeOfDeath == CauseOfDeath.Mauling)
                    {
                        Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Melee);
                    }
                    else if (__instance.causeOfDeath == CauseOfDeath.Gunshots)
                    {
                        Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder_Shotgun);
                    }
                    else
                    {
                        Plugin.Instance.PluginLogger.LogWarning("Player was killed by someone else but we don't know how! " + __instance.causeOfDeath);
                    }
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_SporeLizard);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_CoilHead);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_Hygrodere);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                Plugin.Instance.PluginLogger.LogDebug("Handling Hoarder Bug damage...");

                PlayerControllerB playerControllerB = other.gameObject.GetComponent<PlayerControllerB>();
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (playerControllerB.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_HoarderBug);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Thumper);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BunkerSpider);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
        public static void Postfix(int playerId)
        {
            try
            {
                Plugin.Instance.PluginLogger.LogDebug("Nutcracker kicked a player to death!");

                PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
                Plugin.Instance.PluginLogger.LogDebug("Player is dying! Setting special cause of death...");
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
                        Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                        AdvancedDeathTracker.SetCauseOfDeath(localPlayerController, AdvancedCauseOfDeath.Enemy_Nutcracker_Shot);
                    }
                    else
                    {
                        // Player fired the shotgun.
                        Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Masked Player killed someone...");

                PlayerControllerB playerControllerB = __instance.inSpecialAnimationWithPlayer;
                if (playerControllerB == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                // playerControllerB.isPlayerDead is false here but we just assume they will die here.
                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
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
                Plugin.Instance.PluginLogger.LogDebug("Masked Player killed someone...");

                PlayerControllerB previousPlayerHeldBy = Traverse.Create(__instance).Field("previousPlayerHeldBy").GetValue<PlayerControllerB>();
                if (previousPlayerHeldBy == null)
                {
                    Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                    return;
                }

                if (previousPlayerHeldBy.isPlayerDead)
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                    AdvancedDeathTracker.SetCauseOfDeath(previousPlayerHeldBy, AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear);
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogDebug("Player is somehow still alive! Skipping...");
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
                Plugin.Instance.PluginLogger.LogDebug("Extension ladder started animation! Modifying kill trigger...");

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
                Plugin.Instance.PluginLogger.LogDebug("Item dropship spawned! Modifying kill trigger...");

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

    [HarmonyPatch(typeof(Turret))]
    [HarmonyPatch("Update")]
    public class TurretUpdatePatch
    {
        public static void Postfix(Turret __instance) {
            if (__instance.turretMode != TurretMode.Firing) return;

            // TODO: This might conflict with other Gunshots causes of death?

            var targetPlayer = GameNetworkManager.Instance.localPlayerController;
            if (targetPlayer.isPlayerDead && targetPlayer.causeOfDeath == CauseOfDeath.Gunshots) {
                Plugin.Instance.PluginLogger.LogDebug("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, AdvancedCauseOfDeath.Other_Turret);
            }
        }
    }

    [HarmonyPatch(typeof(Landmine))]
    [HarmonyPatch("SpawnExplosion")]
    public class LandmineSpawnExplosionPatch
    {
        // IL_0177: callvirt  instance void GameNetcodeStuff.PlayerControllerB::KillPlayer(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, bool, valuetype CauseOfDeath, int32)
        const string KILL_PLAYER_SIGNATURE = "Void KillPlayer(UnityEngine.Vector3, Boolean, CauseOfDeath, Int32)";

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
	    {
		    var code = new List<CodeInstruction>(instructions);

		    // We'll need to modify code here.
            var codeToInject = BuildInstructionsToInsert(method);
            if (codeToInject == null) {
                Plugin.Instance.PluginLogger.LogError("Could not build instructions to insert in LandmineSpawnExplosionPatch! Safely aborting...");
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
                Plugin.Instance.PluginLogger.LogError("Could not find PlayerControllerB.KillPlayer call in LandmineSpawnExplosionPatch! Safely aborting...");
                return instructions;
            } else {
                // Moment of truth.
                Plugin.Instance.PluginLogger.LogInfo("Injecting patch into Landmine.SpawnExplosion...");
                code.InsertRange(insertionIndex, codeToInject);
                Plugin.Instance.PluginLogger.LogInfo("Done.");

		        return code;
            }
        }

        static List<CodeInstruction> BuildInstructionsToInsert(MethodBase method) {
            var result = new List<CodeInstruction>();

            // var argumentIndex_explosionPosition = 0;
            // var argumentIndex_spawnExplosionEffect = 1;
            var argumentIndex_killRange = 2;
            // var argumentIndex_damageRange = 3;

            IList<LocalVariableInfo> localVars = method.GetMethodBody().LocalVariables;
            LocalVariableInfo localVar_component = null;

            for (int i = 0; i < localVars.Count; i++) {
                var currentLocalVar = localVars[i];

                if (currentLocalVar.LocalType == typeof(PlayerControllerB)) {
                    if (localVar_component != null) {
                        Plugin.Instance.PluginLogger.LogError("Found multiple PlayerControllerB local variables in LandmineSpawnExplosionPatch!");
                        return null;
                    }
                    localVar_component = currentLocalVar;
                    break;
                }
            }


            // IL_017D: ldloc.s   component
            result.Add(new CodeInstruction(OpCodes.Ldloc_S, localVar_component.LocalIndex));

            // IL_017F: ldarg.2
            result.Add(new CodeInstruction(OpCodes.Ldarg, argumentIndex_killRange));

            // IL_0180: call      void [Coroner]Coroner.Patch.LandmineSpawnExplosionPatch::RewriteCauseOfDeath(class GameNetcodeStuff.PlayerControllerB, float32)
            result.Add(new CodeInstruction(OpCodes.Call, typeof(LandmineSpawnExplosionPatch).GetMethod(nameof(RewriteCauseOfDeath))));

            return result;
        }

        public static void RewriteCauseOfDeath(PlayerControllerB targetPlayer, float killRange) {
            // This function sets different causes of death for the player based on the kill range of the explosion.
            // This works because different explosion types have different kill ranges.

            AdvancedCauseOfDeath causeOfDeath = AdvancedCauseOfDeath.Blast;
            if (killRange == 5.0f) {
                causeOfDeath = AdvancedCauseOfDeath.Player_Jetpack_Blast;
            } else if (killRange == 5.7f) {
                causeOfDeath = AdvancedCauseOfDeath.Other_Landmine;
            } else if (killRange == 2.4f) {
                causeOfDeath = AdvancedCauseOfDeath.Other_Lightning;
            }

            AdvancedDeathTracker.SetCauseOfDeath(targetPlayer, causeOfDeath);
        }
    }
}