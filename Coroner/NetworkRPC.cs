using StaticNetcodeLib;
using Unity.Netcode;

#nullable enable

namespace Coroner
{
    /**
     * Uses StaticNetcodeLib to perform RPCs
     */
    [StaticNetcode]
    class NetworkRPC
    {
        /**
         * The client died, and wishes to report the cause to the server.
         * Call this function on any client, and it will be invoked on the server.
         */
        [ServerRpc(RequireOwnership = false)]
        public static void ReportCauseOfDeathServerRpc(int playerClientId, string? codLanguageTag, bool forceOverride) {
            // Send the new cause of death to all clients.
            Plugin.Instance.PluginLogger.LogDebug($"Server received cause of death via RPC: ({playerClientId}, {(codLanguageTag == null ? "null" : codLanguageTag)})");
            BroadcastCauseOfDeathClientRpc(playerClientId, codLanguageTag, forceOverride);
        }

        /**
         * The server has received a cause of death from a client (possibly itself),
         * and wishes to report it back to all clients.
         * Call this function on the server, and it will be invoked on all clients.
         */
        [ClientRpc]
        public static void BroadcastCauseOfDeathClientRpc(int playerClientId, string? codLanguageTag, bool forceOverride) { 
            Plugin.Instance.PluginLogger.LogDebug($"Client received cause of death via RPC: ({playerClientId}, {(codLanguageTag == null ? "null" : codLanguageTag)})");
            
            if (codLanguageTag == null || !AdvancedCauseOfDeath.IsTagRegistered(codLanguageTag)) { 
                Plugin.Instance.PluginLogger.LogError($"Could not deserialize cause of death ({codLanguageTag})");
                return;
            }

            AdvancedCauseOfDeath? causeOfDeath = AdvancedCauseOfDeath.Fetch(codLanguageTag);

            Plugin.Instance.PluginLogger.LogDebug($"Deserialized cause of death to {causeOfDeath}");
            AdvancedDeathTracker.StoreLocalCauseOfDeath(playerClientId, causeOfDeath, forceOverride);
        }
    }
}