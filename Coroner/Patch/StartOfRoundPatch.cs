using HarmonyLib;

#nullable enable

namespace Coroner.Patch
{
    // Ensure causes of death get reset when leaving a lobby.
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

}