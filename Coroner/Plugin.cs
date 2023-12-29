using BepInEx;

using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System;
using System.Reflection;
using System.IO;

namespace Coroner
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "Coroner";
        public const string PLUGIN_NAME = "Coroner";
        public const string PLUGIN_AUTHOR = "EliteMasterEric";
        public const string PLUGIN_VERSION = "1.5.1";
        public const string PLUGIN_GUID = "com.elitemastereric.coroner";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("LC_API", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public static readonly Random RANDOM = new Random();

        public PluginLogger PluginLogger;

        public PluginConfig PluginConfig;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public bool IsLCAPIPresent = false;
        
        private void Awake()
        {
            Instance = this;

            PluginLogger = new PluginLogger(Logger);

            // Apply Harmony patches (if any exist)
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            PluginLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");  

            LoadConfig();
            LanguageHandler.Initialize();
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

    public class PluginLogger
    {
        ManualLogSource manualLogSource;
        LogLevel logLevel;

        public PluginLogger(ManualLogSource manualLogSource, LogLevel logLevel = LogLevel.Info) {
            this.manualLogSource = manualLogSource;
            this.logLevel = logLevel;
        }

        public void LogFatal(object data)
        {
            if (logLevel >= LogLevel.Fatal) manualLogSource.LogFatal(data);
        }

        public void LogError(object data)
        {
            if (logLevel >= LogLevel.Error) manualLogSource.LogError(data);
        }

        public void LogWarning(object data)
        {
            if (logLevel >= LogLevel.Warning) manualLogSource.LogWarning(data);
        }

        public void LogMessage(object data)
        {
            if (logLevel >= LogLevel.Message) manualLogSource.LogMessage(data);
        }

        public void LogInfo(object data)
        {
            if (logLevel >= LogLevel.Info) manualLogSource.LogInfo(data);
        }

        public void LogDebug(object data)
        {
            if (logLevel >= LogLevel.Debug) manualLogSource.LogDebug(data);
        }
    }
}
