using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Coroner.Patch {
    /*
     * A set of patches dedicated to tracking when a player dies in a specific manner,
     * and storing it in the AdvancedDeathTracker, because `causeOfDeath` is not precise enough.
     */

    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyPatch("AnimationGrabPlayer")]
    class DepositItemsDeskAnimationGrabPlayerPatch {
        public static void Postfix(int playerID) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after tentacle devouring...");

            PlayerControllerB playerDying = StartOfRound.Instance.allPlayerScripts[playerID];
            if (playerDying.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_DepositItemsDesk);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
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

            if (__instance.inSpecialAnimationWithPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.inSpecialAnimationWithPlayer, AdvancedCauseOfDeath.Enemy_Bracken);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
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

            if (playerDying.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerDying, AdvancedCauseOfDeath.Enemy_EyelessDog);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
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

            if (playerBeingEaten.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerBeingEaten, AdvancedCauseOfDeath.Enemy_ForestGiant);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyPatch("BeeKillPlayerOnLocalClient")]
    class RedLocustBeesBeeKillPlayerOnLocalClientPatch {
        public static void Postfix(int playerId) {
            Plugin.Instance.PluginLogger.LogInfo("Accessing state after Red Locust devouring...");

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
            if (__instance.hauntingPlayer == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }
            if (__instance.hauntingPlayer.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(__instance.hauntingPlayer, AdvancedCauseOfDeath.Enemy_GhostGirl);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(SandWormAI))]
    [HarmonyPatch("EatPlayer")]
    class SandWormAIEatPlayerPatch {
        public static void Postfix(PlayerControllerB playerScript) {
            if (playerScript == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }
            if (playerScript.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerScript, AdvancedCauseOfDeath.Enemy_EarthLeviathan);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(BaboonBirdAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BaboonBirdAIOnCollideWithPlayerPatch {
        public static void Postfix(BaboonBirdAI __instance, Collider other) {
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

    [HarmonyPatch(typeof(JesterAI))]
    [HarmonyPatch("killPlayerAnimation")]
    class JesterAIKillPlayerAnimationPatch {
        public static void Postfix(JesterAI __instance, int playerId) {
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
            if (playerControllerB == null) {
                Plugin.Instance.PluginLogger.LogWarning("Could not access player after death!");
                return;
            }

            if (playerControllerB.isPlayerDead) {
                Plugin.Instance.PluginLogger.LogInfo("Player is now dead! Setting special cause of death...");
                AdvancedDeathTracker.SetCauseOfDeath(playerControllerB, AdvancedCauseOfDeath.Enemy_Jester);
            } else {
                Plugin.Instance.PluginLogger.LogWarning("Player is somehow still alive! Skipping...");
                return;
            }
        }
    }
}