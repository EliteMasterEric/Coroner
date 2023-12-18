using BepInEx;

using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System;

namespace Coroner
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "Coroner";
        public const string PLUGIN_NAME = "Coroner";
        public const string PLUGIN_VERSION = "1.4.1";
        public const string PLUGIN_GUID = "com.elitemastereric.coroner";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("LC_API", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public static readonly Random RANDOM = new Random();

        public ManualLogSource PluginLogger;

        public PluginConfig PluginConfig;

        public bool IsLCAPIPresent = false;
        
        private void Awake()
        {
            Instance = this;

            PluginLogger = Logger;

            // Apply Harmony patches (if any exist)
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            PluginLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");

            LoadConfig();
            QueryLCAPI();
            DeathBroadcaster.Initialize();
        }

        private void QueryLCAPI()
        {
            PluginLogger.LogInfo("Checking for LC_API...");
            if (Chainloader.PluginInfos.ContainsKey("LC_API"))
            {
                BepInEx.PluginInfo pluginInfo;
                Chainloader.PluginInfos.TryGetValue("LC_API", out pluginInfo);

                if (pluginInfo == null)
                {
                    PluginLogger.LogError("Detected LC_API, but could not get plugin info!");
                    IsLCAPIPresent = false;
                    return;
                }
                
                PluginLogger.LogInfo("LCAPI is present! " + pluginInfo.Metadata.GUID + ":" + pluginInfo.Metadata.Version);
                IsLCAPIPresent = true;
            }
            else
            {
                PluginLogger.LogInfo("LCAPI is not present.");
                IsLCAPIPresent = false;
            }
        }

        private void LoadConfig()
        {
            PluginConfig = new PluginConfig();
            PluginConfig.BindConfig(Config);
        }
    }
}
