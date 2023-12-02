using System;
using GameNetcodeStuff;
using TMPro;

namespace Coroner.Patch {
    [HarmonyLib.HarmonyPatch(typeof(HUDManager))]
    [HarmonyLib.HarmonyPatch("FillEndGameStats")]
    class HUDManagerFillEndGameStatsPatch {
        /*
         * A list of possible funny strings.
         * Displayed when the player has no notes (and hasn't died).
         */
        public static readonly string[] FUNNY_NOTES = {
            "* The goofiest goober.\n",
            "* The cutest employee.\n",
            "* Had the most fun.\n",
            "* Had the least fun.\n",
            "* The bravest employee.\n",
            "* Did a sick flip.\n",
            "* Stubbed their toe.\n",
            "* The most likely to die next time.\n",
            "* The least likely to die next time.\n",
            "* Dislikes smoke.\n",
            "* A team player.\n",
            "* A real go-getter.\n",
            "* Ate the most snacks.\n",
            "* Passed GO and collected $200.\n",
            "* Got freaky on a Friday night.\n",
            "* I think this one's a serial killer.\n",
            "* Perfectly unremarkable.\n",
            "* Hasn't called their mother in a while.\n",
            "* Has IP address 127.0.0.1.\n",
            "* Secretly a lizard.\n"
        };
        
        static readonly Random RANDOM = new Random();

        public static void Postfix(HUDManager __instance) {
            try {
                OverridePerformanceReport(__instance);
            } catch (Exception e) {
                Plugin.Instance.PluginLogger.LogError("Coroner threw an exception while Caught an exception overriding performance report: ");
                // Display the error and the stack trace.
                Plugin.Instance.PluginLogger.LogError(e.ToString());
                Plugin.Instance.PluginLogger.LogError(e.StackTrace.ToString());
            }
        }

        static void OverridePerformanceReport(HUDManager __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Applying Coroner patches to player notes...");

            for (int playerIndex = 0; playerIndex < __instance.statsUIElements.playerNotesText.Length; playerIndex++) {
                PlayerControllerB playerController = __instance.playersManager.allPlayerScripts[playerIndex];
                if (!playerController.disconnectedMidGame && !playerController.isPlayerDead && !playerController.isPlayerControlled) {
                    Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is not controlled by a player. Skipping...");
                    continue;
                }

                TextMeshProUGUI textMesh = __instance.statsUIElements.playerNotesText[playerIndex];
                if (playerController.isPlayerDead) {
                    if (Plugin.Instance.PluginConfig.ShouldDisplayCauseOfDeath()) {
                        var causeOfDeath = AdvancedDeathTracker.GetCauseOfDeath(playerController);
                        var causeOfDeathStr = AdvancedDeathTracker.StringifyCauseOfDeath(causeOfDeath);

                        if (Plugin.Instance.PluginConfig.ShouldDeathReplaceNotes()) {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead! Replacing notes with Cause of Death...");
                            // Reset the notes.
                            textMesh.text = "Notes: \n";
                        } else {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead! Appending notes with Cause of Death...");
                        }
                        textMesh.text += "* " + causeOfDeathStr + "\n";
                    } else {
                        Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead, but Config says leave it be...");
                    }
                } else {
                    Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is not dead!");
                    if (textMesh.text == "Notes: \n") {
                        if (Plugin.Instance.PluginConfig.ShouldDisplayFunnyNotes()) {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " has no notes! Injecting something funny...");
                            // Reset the notes.
                            textMesh.text = "Notes: \n";
                            textMesh.text += ChooseFunnyNote();
                        } else {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " has no notes, but Config says leave it be...");
                        }
                    } else {
                        Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " has notes! Let's leave it be...");
                    }
                }
            }

            // We are done with the death tracker, so clear it.
            AdvancedDeathTracker.ClearDeathTracker();
        }

        static string ChooseFunnyNote() {
            // Choose a random entry from the list.
            return FUNNY_NOTES[RANDOM.Next(FUNNY_NOTES.Length)];
        }
    }
}