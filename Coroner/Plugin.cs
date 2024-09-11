using BepInEx;

using HarmonyLib;
using BepInEx.Logging;
using System;
using System.Reflection;
using System.IO;
using static BepInEx.BepInDependency;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Features;

#nullable enable

namespace Coroner
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "Coroner";
        public const string PLUGIN_NAME = "Coroner";
        public const string PLUGIN_AUTHOR = "EliteMasterEric";
        public const string PLUGIN_VERSION = "2.2.0";
        public const string PLUGIN_GUID = "com.elitemastereric.coroner";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    // [BepInDependency("LethalNetworkAPI")]
    [BepInDependency(StaticNetcodeLib.MyPluginInfo.PLUGIN_GUID, DependencyFlags.HardDependency)]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {   
        // Variables instantiated in Awake() should never be null.
        #nullable disable
        public static Plugin Instance { get; private set; }

        public static readonly Random RANDOM = new Random();
        
        public PluginLogger PluginLogger;

        internal PluginConfig PluginConfig;

        internal LanguageHandler LanguageHandler; // Uses player selected language, default to English (American)
        internal LanguageHandler FallbackLanguageHandler; // Always uses English (American)
        #nullable enable

        public static string AssemblyDirectory
        {
            get
            {
                // TODO: Replace with Plugin.Info.Location
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void Awake()
        {
            Instance = this;

            PluginLogger = new PluginLogger(Logger);

            // Apply Harmony patches (if any exist)
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            PluginLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");

            // Lobby Compatibility
            Version ver = new Version("2.1.0");
            PluginHelper.RegisterPlugin(PluginInfo.PLUGIN_GUID, ver, CompatibilityLevel.Everyone, VersionStrictness.Patch);

            LoadConfig();
            LoadLanguageHandlers();
        }

        public string GetConfigPath()
        {
            return $"{Paths.ConfigPath}/{PluginInfo.PLUGIN_AUTHOR}-{PluginInfo.PLUGIN_NAME}";
        }

        public void LoadLanguageHandlers()
        {
            LanguageHandler = new LanguageHandler(PluginConfig.GetSelectedLanguage());
            FallbackLanguageHandler = new LanguageHandler(LanguageHandler.DEFAULT_LANGUAGE, true);
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

        public PluginLogger(ManualLogSource manualLogSource)
        {
            this.manualLogSource = manualLogSource;
        }

        public void LogFatal(object data)
        {
            manualLogSource.LogFatal(data);
        }

        public void LogError(object data)
        {
            manualLogSource.LogError(data);
        }

        public void LogWarning(object data)
        {
            manualLogSource.LogWarning(data);
        }

        public void LogMessage(object data)
        {
            manualLogSource.LogMessage(data);
        }

        public void LogInfo(object data)
        {
            manualLogSource.LogInfo(data);
        }

        public void LogDebug(object data)
        {
            manualLogSource.LogDebug(data);
        }
    }
}
