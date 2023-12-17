using System;
using GameNetcodeStuff;
using TMPro;

namespace Coroner.Patch {
    [HarmonyLib.HarmonyPatch(typeof(HUDManager))]
    [HarmonyLib.HarmonyPatch("FillEndGameStats")]
    class HUDManagerFillEndGameStatsPatch {
        public static void Postfix(HUDManager __instance) {
            try {
                OverridePerformanceReport(__instance);
            } catch (Exception e) {
                Plugin.Instance.PluginLogger.LogError("Error in HUDManagerFillEndGameStatsPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }

        static Random BuildSyncedRandom(HUDManager __instance) {
            var seed = StartOfRound.Instance.randomMapSeed;
            Plugin.Instance.PluginLogger.LogInfo("Syncing randomization to map seed: '" + seed + "'");
            return new Random(seed);
        }

        /*
         * Override the performance report to display our notes.
         */
        static void OverridePerformanceReport(HUDManager __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Applying Coroner patches to player notes...");

            var syncedRandom = BuildSyncedRandom(__instance);

            for (int playerIndex = 0; playerIndex < __instance.statsUIElements.playerNotesText.Length; playerIndex++) {
                PlayerControllerB playerController = __instance.playersManager.allPlayerScripts[playerIndex];
                if (!playerController.disconnectedMidGame && !playerController.isPlayerDead && !playerController.isPlayerControlled) {
                    Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is not controlled by a player. Skipping...");
                    continue;
                }

                TextMeshProUGUI textMesh = __instance.statsUIElements.playerNotesText[playerIndex];
                if (playerController.isPlayerDead) {
                    if (Plugin.Instance.PluginConfig.ShouldDisplayCauseOfDeath()) {
                        if (Plugin.Instance.PluginConfig.ShouldDeathReplaceNotes()) {
                            Plugin.Instance.PluginLogger.LogInfo("[REPORT] Player " + playerIndex + " is dead! Replacing notes with Cause of Death...");
                            // Reset the notes.
                            textMesh.text = "Notes: \n";
                        } else {
                            Plugin.Instance.PluginLogger.LogInfo("[REPORT] Player " + playerIndex + " is dead! Appending notes with Cause of Death...");
                        }
                        
                        var causeOfDeath = AdvancedDeathTracker.GetCauseOfDeath(playerController);
                        var notes = "* " + AdvancedDeathTracker.StringifyCauseOfDeath(causeOfDeath, syncedRandom) + "\n";

                        textMesh.text += notes;
                    } else {
                        Plugin.Instance.PluginLogger.LogInfo("[REPORT] Player " + playerIndex + " is dead, but Config says leave it be...");
                    }
                } else {
                    if (Plugin.Instance.PluginConfig.ShouldDisplayFunnyNotes()) {
                        Plugin.Instance.PluginLogger.LogInfo("[REPORT] Player " + playerIndex + " has no notes! Injecting something funny...");

                        var notes = "* " + AdvancedDeathTracker.StringifyCauseOfDeath(null, syncedRandom) + "\n";

                        textMesh.text += notes;

                        textMesh.text = "Notes: \n";
                        textMesh.text += notes;
                    } else {
                        Plugin.Instance.PluginLogger.LogInfo("[REPORT] Player " + playerIndex + " has no notes, but Config says leave it be...");
                    }
                }
            }

            // We are done with the death tracker, so clear it.
            AdvancedDeathTracker.ClearDeathTracker();
        }
    }
}