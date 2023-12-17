namespace Coroner {
    class DeathBroadcaster {
        const string SIGNATURE_DEATH = PluginInfo.PLUGIN_GUID + ".death";

        public static void Initialize() {
            if (Plugin.Instance.IsLCAPIPresent) {
                Coroner.LCAPI.DeathBroadcasterLCAPI.Initialize();
            } else {
                Plugin.Instance.PluginLogger.LogInfo("LC_API is not present! Skipping registration...");
            }
        }

        public static void BroadcastCauseOfDeath(int playerId, AdvancedCauseOfDeath causeOfDeath) {
            AttemptBroadcast(BuildDataCauseOfDeath(playerId, causeOfDeath), SIGNATURE_DEATH);
        }

        static string BuildDataCauseOfDeath(int playerId, AdvancedCauseOfDeath causeOfDeath) {
            return playerId + "|" + ((int) causeOfDeath);
        }

        static void AttemptBroadcast(string data, string signature) {
            if (Plugin.Instance.IsLCAPIPresent) {
                Coroner.LCAPI.DeathBroadcasterLCAPI.AttemptBroadcast(data, signature);
            } else {
                Plugin.Instance.PluginLogger.LogInfo("LC_API is not present! Skipping broadcast...");
            }
        }
    }
}