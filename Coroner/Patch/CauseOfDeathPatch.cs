using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Coroner.Patch {
    /*
     * A set of patches dedicated to tracking when a player dies in a specific manner,
     * and storing it in the AdvancedDeathTracker, because `causeOfDeath` is not precise enough.
     */

    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("KillPlayer")]
    class PlayerControllerBKillPlayerPatch {
        public static void Prefix(PlayerControllerB __instance, ref CauseOfDeath causeOfDeath) {
            // NOTE: Only called on the client of the player who died.

            if ((int) causeOfDeath == AdvancedDeathTracker.PLAYER_CAUSE_OF_DEATH_DROPSHIP) {
                Plugin.Instance.PluginLogger.LogInfo("Player died from item dropship! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Dropship);
                // Now to fix the jank by adding a normal value!
                causeOfDeath = CauseOfDeath.Crushing;
                return;
            }

            if (__instance.isSinking && causeOfDeath == CauseOfDeath.Suffocation) {
                Plugin.Instance.PluginLogger.LogInfo("Player died of suffociation while sinking in quicksand! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Quicksand);
            } else {
                Plugin.Instance.PluginLogger.LogInfo("Player is dying! No cause of death...");
            }
        }
    }

    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyPatch("AnimationGrabPlayer")]
    class DepositItemsDeskAnimationGrabPlayerPatch {
        public static void Postfix(int playerID) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after tentacle devouring...");

            PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerID];
            Plugin.Instance.PluginLogger.LogInfo("Player is dying! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Player_DepositItemsDesk);
        }
    }

    [HarmonyPatch(typeof(JesterAI))]
    [HarmonyPatch("killPlayerAnimation")]
    class JesterAIKillPlayerAnimationPatch {
        public static void Postfix(JesterAI __instance, int playerId) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Jester mauling...");

            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
            if (playerControllerB == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Jester);
        }
    }

    [HarmonyPatch(typeof(SandWormAI))]
    [HarmonyPatch("EatPlayer")]
    class SandWormAIEatPlayerPatch {
        public static void Postfix(PlayerControllerB playerScript) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Sand Worm devouring...");

            if (playerScript == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }
            
            Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(playerScript, AdvancedCauseOfDeath.Enemy_EarthLeviathan);
        }
    }

    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyPatch("BeeKillPlayerOnLocalClient")]
    class RedLocustBeesBeeKillPlayerOnLocalClientPatch {
        public static void Postfix(int playerId) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Circuit Bee electrocution...");

            PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
            if (playerDying == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            if (playerDying.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_CircuitBees);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(DressGirlAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class DressGirlAIOnCollideWithPlayerPatch {
        public static void Postfix(DressGirlAI __instance, Collider other) {
            Plugin.Instance.PluginLogger.LogInfo("Processing Ghost Girl player collision...");

            if (__instance.hauntingPlayer == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after collision!");
                return;
            }
            if (__instance.hauntingPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.hauntingPlayer, AdvancedCauseOfDeath.Enemy_GhostGirl);
            }
        }
    }

    [HarmonyPatch(typeof(FlowermanAI))]
    [HarmonyPatch("killAnimation")]
    class FlowermanAIKillAnimationPatch {
        public static void Postfix(FlowermanAI __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Bracken snapping neck...");

            if (__instance.inSpecialAnimationWithPlayer == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after snapping neck!");
                return;
            }

            Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Bracken);
        }
    }

    [HarmonyPatch(typeof(ForestGiantAI))]
    [HarmonyPatch("EatPlayerAnimation")]
    class ForestGiantAIEatPlayerAnimationPatch {
        public static void Postfix(PlayerControllerB playerBeingEaten) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Forest Giant devouring...");

            if (playerBeingEaten == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(playerBeingEaten, AdvancedCauseOfDeath.Enemy_ForestGiant);
        }
    }

    [HarmonyPatch(typeof(MouthDogAI))]
    [HarmonyPatch("KillPlayer")]
    class MouthDogAIKillPlayerPatch {
        public static void Postfix(int playerId) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after dog devouring...");

            PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerId];
            if (playerDying == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
            AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_EyelessDog);
        }
    }

    [HarmonyPatch(typeof(CentipedeAI))]
    [HarmonyPatch("DamagePlayerOnIntervals")]
    class CentipedeAIDamagePlayerOnIntervalsPatch {
        public static void Postfix(CentipedeAI __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Handling Snare Flea damage...");
            if (__instance.clingingToPlayer == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player being clung to!");
                return;
            }

            if (__instance.clingingToPlayer.isPlayerDead && __instance.clingingToPlayer.causeOfDeath == CauseOfDeath.Suffocation) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.clingingToPlayer, AdvancedCauseOfDeath.Enemy_SnareFlea);
            } else if (__instance.clingingToPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogWarning("Player died while attacked by Snare Flea! Skipping...");
                return;
            } else {
                // Player still alive.
            }
        }
    }

    [HarmonyPatch(typeof(BaboonBirdAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BaboonBirdAIOnCollideWithPlayerPatch {
        public static void Postfix(BaboonBirdAI __instance, Collider other) {
            Plugin.Instance.PluginLogger.LogInfo("Handling Baboon Hawk damage...");

            var doingKillAnimation = Traverse.Create(__instance).Field("doingKillAnimation").GetValue<bool>();
            PlayerControllerB playerControllerB = __instance.MeetsStandardPlayerCollisionConditions(other, __instance.inSpecialAnimation || doingKillAnimation);
            if (playerControllerB == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            if (playerControllerB.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_BaboonHawk);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyPatch("DamagePlayerFromOtherClientClientRpc")]
    class PlayerControllerBDamagePlayerFromOtherClientClientRpcPatch {
        public static void Postfix(PlayerControllerB __instance, int playerWhoHit) {
            Plugin.Instance.PluginLogger.LogInfo("Handling friendly fire damage...");
            if (__instance == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access victim after death!");
                return;
            }
            // PlayerControllerB playerControllerWhoHit = StartOfRound.Instance.allPlayerScripts[playerWhoHit];

            if (__instance.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance, AdvancedCauseOfDeath.Player_Murder);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(ExtensionLadderItem))]
    [HarmonyPatch("StartLadderAnimation")]
    class ExtensionLadderItemStartLadderAnimationPatch {
        public static void Postfix(ExtensionLadderItem __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Extension ladder started animation! Modifying kill trigger...");

            GameObject extensionLadderGameObject = __instance.gameObject;
            if (extensionLadderGameObject == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ExtensionLadderItem.");
            }
            Transform killTriggerTransform = extensionLadderGameObject.transform.Find("AnimContainer/MeshContainer/LadderMeshContainer/BaseLadder/LadderSecondPart/KillTrigger");

            if (killTriggerTransform == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ExtensionLadderItem.");
            }

            GameObject killTriggerGameObject = killTriggerTransform.gameObject;

            if (killTriggerGameObject == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ExtensionLadderItem.");
            }

            KillLocalPlayer killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

            if (killLocalPlayer == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
            }

            // Correct the cause of death.
            killLocalPlayer.causeOfDeath = CauseOfDeath.Crushing;
        }
    }
        
    [HarmonyPatch(typeof(ItemDropship))]
    [HarmonyPatch("Start")]
    class ItemDropshipStartPatch {
        public static void Postfix(ItemDropship __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Item dropship spawned! Modifying kill trigger...");

            GameObject itemDropshipGameObject = __instance.gameObject;
            if (itemDropshipGameObject == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch GameObject from ItemDropship.");
            }
            Transform killTriggerTransform = itemDropshipGameObject.transform.Find("ItemShip/KillTrigger");

            if (killTriggerTransform == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger Transform from ItemDropship.");
            }

            GameObject killTriggerGameObject = killTriggerTransform.gameObject;

            if (killTriggerGameObject == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillTrigger GameObject from ItemDropship.");
            }

            KillLocalPlayer killLocalPlayer = killTriggerGameObject.GetComponent<KillLocalPlayer>();

            if (killLocalPlayer == null) {
                Plugin.Instance.PluginLogger.LogError("Could not fetch KillLocalPlayer from KillTrigger GameObject.");
            }

            // Modify the cause of death in a janky way.
            killLocalPlayer.causeOfDeath = (CauseOfDeath) AdvancedDeathTracker.PLAYER_CAUSE_OF_DEATH_DROPSHIP;
        }
    }  
}