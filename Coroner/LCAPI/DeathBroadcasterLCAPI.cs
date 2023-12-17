// DO NOT LOAD THIS CLASS UNLESS LC_API IS PRESENT!

namespace Coroner.LCAPI {
    class DeathBroadcasterLCAPI {
        const string SIGNATURE_DEATH = PluginInfo.PLUGIN_GUID + ".death";

        public static void Initialize() {
            Plugin.Instance.PluginLogger.LogDebug("Initializing DeathBroadcaster...");
            if (Plugin.Instance.IsLCAPIPresent) {
                Plugin.Instance.PluginLogger.LogDebug("LC_API is present! Registering signature...");
                LC_API.ServerAPI.Networking.GetString += OnBroadcastString;
            } else {
                Plugin.Instance.PluginLogger.LogError("LC_API is not present! Why did you try to register the DeathBroadcaster?");
            }
        }

        static void OnBroadcastString(string data, string signature) {
            if (signature == SIGNATURE_DEATH) {
                Plugin.Instance.PluginLogger.LogDebug("Broadcast has been received from LC_API!");
                string[] split = data.Split('|');
                int playerId = int.Parse(split[0]);
                int causeOfDeathInt = int.Parse(split[1]);
                AdvancedCauseOfDeath causeOfDeath = (AdvancedCauseOfDeath) causeOfDeathInt;
                Plugin.Instance.PluginLogger.LogDebug("Player " + playerId + " died of " + AdvancedDeathTracker.StringifyCauseOfDeath(causeOfDeath));
                AdvancedDeathTracker.SetCauseOfDeath(playerId, causeOfDeath, false);
            }
        }

        public static void AttemptBroadcast(string data, string signature) {
            if (Plugin.Instance.IsLCAPIPresent) {
                Plugin.Instance.PluginLogger.LogDebug("LC_API is present! Broadcasting...");
                LC_API.ServerAPI.Networking.Broadcast(data, signature);
            } else {
                Plugin.Instance.PluginLogger.LogDebug("LC_API is not present! Skipping broadcast...");
            }
        }
    }
}