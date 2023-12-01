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
            "* The most likely to succeed.\n",
            "* The least likely to succeed.\n",
            "* The most likely to die.\n",
            "* The least likely to die.\n",
            "* A team player.\n",
            "* A real go-getter.\n",
            "* Passed GO and collected $200.\n",
            "* Getting freaky on a Friday night.\n",
            "* I think this one's a serial killer.\n",
        };
        
        static readonly Random RANDOM = new Random();

        public static void Postfix(HUDManager __instance) {
            Plugin.Instance.PluginLogger.LogInfo("Applying Coroner patches to player notes...");

            for (int playerIndex = 0; playerIndex < __instance.statsUIElements.playerNotesText.Length; playerIndex++) {
                PlayerControllerB playerController = __instance.playersManager.allPlayerScripts[playerIndex];
                if (!playerController.disconnectedMidGame && !playerController.isPlayerDead && !playerController.isPlayerControlled) {
                    Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is not controlled by a player. Skipping...");
                    continue;
                }

                TextMeshProUGUI textMesh = __instance.statsUIElements.playerNotesText[playerIndex];
                if (__instance.playersManager.allPlayerScripts[playerIndex].isPlayerDead) {
                    if (Plugin.Instance.PluginConfig.ShouldDisplayCauseOfDeath()) {
                        var causeOfDeath = AdvancedDeathTracker.GetCauseOfDeath(playerController);
                        var causeOfDeathStr = AdvancedDeathTracker.StringifyCauseOfDeath(causeOfDeath);

                        if (Plugin.Instance.PluginConfig.ShouldDeathReplaceNotes()) {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead! Replacing notes with Cause of Death...");
                            textMesh.text = "* " + causeOfDeathStr + "\n";
                        } else {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead! Appending notes with Cause of Death...");
                            textMesh.text += "* " + causeOfDeathStr + "\n";
                        }
                    } else {
                        Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is dead, but Config says leave it be...");
                    }
                } else {
                    Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " is not dead!");
                    if (textMesh.text == "") {
                        if (Plugin.Instance.PluginConfig.ShouldDisplayFunnyNotes()) {
                            Plugin.Instance.PluginLogger.LogInfo("Player " + playerIndex + " has no notes! Injecting something funny...");
                            textMesh.text = ChooseFunnyNote();
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