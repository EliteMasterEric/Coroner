using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Coroner
{
    public static class LanguageManager
    {
        private static string currentCulture = "en";
        private static Dictionary<string, string> translations;

        public static string CurrentCulture => currentCulture;

        static LanguageManager()
        {
            LoadTranslations();
        }

        public static void SetLanguage(string languageCode)
        {
            try
            {
                currentCulture = languageCode;
                LoadTranslations();
            }
            catch (Exception ex)
            {
                Plugin.Instance.PluginLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} LanguageManagerException: {ex.Message}");
                currentCulture = "en";
                LoadTranslations();
            }
        }

        public static void LoadTranslations()
        {
            try
            {
                var xmlFilePath = Path.Combine("BepInEx", "Lang", "Coroner", $"Strings_{currentCulture}.xml");
                translations = XmlParser.ParseTranslations(xmlFilePath);
            }
            catch (Exception ex)
            {
                HandleTranslationLoadException(ex);
            }
        }

        private static void HandleTranslationLoadException(Exception ex)
        {
            Plugin.Instance.PluginLogger.LogError($"{PluginInfo.PLUGIN_NAME} LanguageManagerException: {ex.Message}");
            translations = LoadDefaultTranslations();
        }

        private static Dictionary<string, string> LoadDefaultTranslations()
        {
            var defaultTranslations = new Dictionary<string, string>();
            try
            {
                var xmlFilePath = Path.Combine("BepInEx", "Lang", "Coroner", "Strings_en.xml");
                defaultTranslations = XmlParser.ParseTranslations(xmlFilePath);
            }
            catch (Exception ex)
            {
                Plugin.Instance.PluginLogger.LogError($"{PluginInfo.PLUGIN_NAME} LanguageManagerException: {ex.Message}");
            }

            return defaultTranslations;
        }

        //public static string GetTranslation(string id)
        //{
        //    try
        //    {
        //        return translations.TryGetValue(id, out var translation) ? translation : id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Plugin.Instance.PluginLogger.LogError($"{PluginInfo.PLUGIN_NAME} Error getting translation for {id}: {ex.Message}");
        //        return id;
        //    }
        //}
        public static string GetTranslation(string id)
        {
            try
            {
                if (translations.TryGetValue(id, out var translation))
                {
                    return translation;
                }
                else
                {
                    var fallbackTranslations = LoadDefaultTranslations();
                    return fallbackTranslations.TryGetValue(id, out var fallbackTranslation) ? fallbackTranslation : id;
                }
            }
            catch (Exception ex)
            {
                Plugin.Instance.PluginLogger.LogError($"{PluginInfo.PLUGIN_NAME} Error getting translation for {id}: {ex.Message}");
                return id;
            }
        }
    }

    public static class XmlParser
    {
        public static Dictionary<string, string> ParseTranslations(string xmlFilePath)
        {
            var translations = new Dictionary<string, string>();
            try
            {
                var doc = new XmlDocument();
                doc.Load(xmlFilePath);

                var nodes = doc.SelectNodes("/base/strings/string");
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        var id = node.Attributes?["id"]?.Value;
                        var text = node.Attributes?["text"]?.Value;
                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(text))
                        {
                            translations[id] = text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing translations from XML file '{xmlFilePath}': {ex.Message}", ex);
            }

            return translations;
        }
    }

    public static class Strings
    {
        static Strings()
        {
            LanguageManager.LoadTranslations();
        }


        public static string FunnyNote1 => LanguageManager.GetTranslation("FunnyNote1");
        public static string FunnyNote2 => LanguageManager.GetTranslation("FunnyNote2");
        public static string FunnyNote3 => LanguageManager.GetTranslation("FunnyNote3");
        public static string FunnyNote4 => LanguageManager.GetTranslation("FunnyNote4");
        public static string FunnyNote5 => LanguageManager.GetTranslation("FunnyNote5");
        public static string FunnyNote6 => LanguageManager.GetTranslation("FunnyNote6");
        public static string FunnyNote7 => LanguageManager.GetTranslation("FunnyNote7");
        public static string FunnyNote8 => LanguageManager.GetTranslation("FunnyNote8");
        public static string FunnyNote9 => LanguageManager.GetTranslation("FunnyNote9");
        public static string FunnyNote10 => LanguageManager.GetTranslation("FunnyNote10");
        public static string FunnyNote11 => LanguageManager.GetTranslation("FunnyNote11");
        public static string FunnyNote12 => LanguageManager.GetTranslation("FunnyNote12");
        public static string FunnyNote13 => LanguageManager.GetTranslation("FunnyNote13");
        public static string FunnyNote14 => LanguageManager.GetTranslation("FunnyNote14");
        public static string FunnyNote15 => LanguageManager.GetTranslation("FunnyNote15");
        public static string FunnyNote16 => LanguageManager.GetTranslation("FunnyNote16");
        public static string FunnyNote17 => LanguageManager.GetTranslation("FunnyNote17");
        public static string FunnyNote18 => LanguageManager.GetTranslation("FunnyNote18");
        public static string FunnyNote19 => LanguageManager.GetTranslation("FunnyNote19");
        public static string FunnyNote20 => LanguageManager.GetTranslation("FunnyNote20");

        public static string BludgeoningDeath1 => LanguageManager.GetTranslation("BludgeoningDeath");

        public static string GravityDeath1 => LanguageManager.GetTranslation("GravityDeath1");
        public static string GravityDeath2 => LanguageManager.GetTranslation("GravityDeath2");

        public static string BlastDeath1 => LanguageManager.GetTranslation("BlastDeath1");
        public static string BlastDeath2 => LanguageManager.GetTranslation("BlastDeath2");
        public static string BlastDeath3 => LanguageManager.GetTranslation("BlastDeath3");

        public static string StrangulationDeath1 => LanguageManager.GetTranslation("StrangulationDeath1");

        public static string SuffocationDeath1 => LanguageManager.GetTranslation("SuffocationDeath1");

        public static string MaulingDeath1 => LanguageManager.GetTranslation("MaulingDeath1");

        public static string GunshotsDeath1 => LanguageManager.GetTranslation("GunshotsDeath1");
        public static string GunshotsDeath2 => LanguageManager.GetTranslation("GunshotsDeath2");

        public static string CrushingDeath1 => LanguageManager.GetTranslation("CrushingDeath1");

        public static string DrowningDeath1 => LanguageManager.GetTranslation("DrowningDeath1");

        public static string AbandonedDeath1 => LanguageManager.GetTranslation("AbandonedDeath1");

        public static string ElectrocutionDeath1 => LanguageManager.GetTranslation("ElectrocutionDeath1");

        public static string KickingDeath1 => LanguageManager.GetTranslation("KickingDeath1");

        public static string Enemy_BrackenDeath1 => LanguageManager.GetTranslation("Enemy_BrackenDeath1");
        public static string Enemy_BrackenDeath2 => LanguageManager.GetTranslation("Enemy_BrackenDeath2");

        public static string Enemy_EyelessDogDeath1 => LanguageManager.GetTranslation("Enemy_EyelessDogDeath1");
        public static string Enemy_EyelessDogDeath2 => LanguageManager.GetTranslation("Enemy_EyelessDogDeath2");
        public static string Enemy_EyelessDogDeath3 => LanguageManager.GetTranslation("Enemy_EyelessDogDeath3");

        public static string Enemy_ForestGiantDeath1 => LanguageManager.GetTranslation("Enemy_ForestGiantDeath1");

        public static string Enemy_CircuitBeesDeath1 => LanguageManager.GetTranslation("Enemy_CircuitBeesDeath1");

        public static string Enemy_GhostGirlDeath1 => LanguageManager.GetTranslation("Enemy_GhostGirlDeath1");
        public static string Enemy_GhostGirlDeath2 => LanguageManager.GetTranslation("Enemy_GhostGirlDeath2");
        public static string Enemy_GhostGirlDeath3 => LanguageManager.GetTranslation("Enemy_GhostGirlDeath3");
        public static string Enemy_GhostGirlDeath4 => LanguageManager.GetTranslation("Enemy_GhostGirlDeath4");

        public static string Enemy_EarthLeviathanDeath1 => LanguageManager.GetTranslation("Enemy_EarthLeviathanDeath1");

        public static string Enemy_BaboonHawkDeath1 => LanguageManager.GetTranslation("Enemy_BaboonHawkDeath1");
        public static string Enemy_BaboonHawkDeath2 => LanguageManager.GetTranslation("Enemy_BaboonHawkDeath2");

        public static string Enemy_JesterDeath1 => LanguageManager.GetTranslation("Enemy_JesterDeath1");
        public static string Enemy_JesterDeath2 => LanguageManager.GetTranslation("Enemy_JesterDeath2");
        public static string Enemy_JesterDeath3 => LanguageManager.GetTranslation("Enemy_JesterDeath3");
        public static string Enemy_JesterDeath4 => LanguageManager.GetTranslation("Enemy_JesterDeath4");

        public static string Enemy_CoilHeadDeath1 => LanguageManager.GetTranslation("Enemy_CoilHeadDeath1");
        public static string Enemy_CoilHeadDeath2 => LanguageManager.GetTranslation("Enemy_CoilHeadDeath2");
        public static string Enemy_CoilHeadDeath3 => LanguageManager.GetTranslation("Enemy_CoilHeadDeath3");

        public static string Enemy_SnareFleaDeath1 => LanguageManager.GetTranslation("Enemy_SnareFleaDeath1");

        public static string Enemy_HygrodereDeath1 => LanguageManager.GetTranslation("Enemy_HygrodereDeath1");
        public static string Enemy_HygrodereDeath2 => LanguageManager.GetTranslation("Enemy_HygrodereDeath2");
        public static string Enemy_HygrodereDeath3 => LanguageManager.GetTranslation("Enemy_HygrodereDeath3");

        public static string Enemy_HoarderBugDeath1 => LanguageManager.GetTranslation("Enemy_HoarderBugDeath1");
        public static string Enemy_HoarderBugDeath2 => LanguageManager.GetTranslation("Enemy_HoarderBugDeath2");
        public static string Enemy_HoarderBugDeath3 => LanguageManager.GetTranslation("Enemy_HoarderBugDeath3");
        public static string Enemy_HoarderBugDeath4 => LanguageManager.GetTranslation("Enemy_HoarderBugDeath4");

        public static string Enemy_SporeLizardDeath1 => LanguageManager.GetTranslation("Enemy_SporeLizardDeath1");
        public static string Enemy_SporeLizardDeath2 => LanguageManager.GetTranslation("Enemy_SporeLizardDeath2");

        public static string Enemy_BunkerSpiderDeath1 => LanguageManager.GetTranslation("Enemy_BunkerSpiderDeath1");

        public static string Enemy_ThumperDeath1 => LanguageManager.GetTranslation("Enemy_ThumperDeath1");
        public static string Enemy_ThumperDeath2 => LanguageManager.GetTranslation("Enemy_ThumperDeath2");

        public static string Enemy_MaskedPlayer_WearDeath1 => LanguageManager.GetTranslation("Enemy_MaskedPlayer_WearDeath1");
        public static string Enemy_MaskedPlayer_WearDeath2 => LanguageManager.GetTranslation("Enemy_MaskedPlayer_WearDeath2");

        public static string Enemy_MaskedPlayer_VictimDeath1 => LanguageManager.GetTranslation("Enemy_MaskedPlayer_VictimDeath1");
        public static string Enemy_MaskedPlayer_VictimDeath2 => LanguageManager.GetTranslation("Enemy_MaskedPlayer_VictimDeath2");

        public static string Enemy_Nutcracker_KickedDeath1 => LanguageManager.GetTranslation("Enemy_Nutcracker_KickedDeath1");
        public static string Enemy_Nutcracker_KickedDeath2 => LanguageManager.GetTranslation("Enemy_Nutcracker_KickedDeath2");

        public static string Enemy_Nutcracker_ShotDeath1 => LanguageManager.GetTranslation("Enemy_Nutcracker_ShotDeath1");
        public static string Enemy_Nutcracker_ShotDeath2 => LanguageManager.GetTranslation("Enemy_Nutcracker_ShotDeath2");

        public static string Player_Jetpack_GravityDeath1 => LanguageManager.GetTranslation("Player_Jetpack_GravityDeath1");
        public static string Player_Jetpack_GravityDeath2 => LanguageManager.GetTranslation("Player_Jetpack_GravityDeath2");
        public static string Player_Jetpack_GravityDeath3 => LanguageManager.GetTranslation("Player_Jetpack_GravityDeath3");

        public static string Player_Jetpack_BlastDeath1 => LanguageManager.GetTranslation("Player_Jetpack_BlastDeath1");
        public static string Player_Jetpack_BlastDeath2 => LanguageManager.GetTranslation("Player_Jetpack_BlastDeath2");

        public static string Player_Murder_MeleeDeath1 => LanguageManager.GetTranslation("Player_Murder_MeleeDeath1");
        public static string Player_Murder_MeleeDeath2 => LanguageManager.GetTranslation("Player_Murder_MeleeDeath2");
        public static string Player_Murder_MeleeDeath3 => LanguageManager.GetTranslation("Player_Murder_MeleeDeath3");
        public static string Player_Murder_MeleeDeath4 => LanguageManager.GetTranslation("Player_Murder_MeleeDeath4");

        public static string Player_Murder_ShotgunDeath1 => LanguageManager.GetTranslation("Player_Murder_ShotgunDeath1");
        public static string Player_Murder_ShotgunDeath2 => LanguageManager.GetTranslation("Player_Murder_ShotgunDeath2");
        public static string Player_Murder_ShotgunDeath3 => LanguageManager.GetTranslation("Player_Murder_ShotgunDeath3");
        public static string Player_Murder_ShotgunDeath4 => LanguageManager.GetTranslation("Player_Murder_ShotgunDeath4");
        public static string Player_Murder_ShotgunDeath5 => LanguageManager.GetTranslation("Player_Murder_ShotgunDeath5");

        public static string Player_QuicksandDeath1 => LanguageManager.GetTranslation("Player_QuicksandDeath1");
        public static string Player_QuicksandDeath2 => LanguageManager.GetTranslation("Player_QuicksandDeath2");

        public static string Player_StunGrenadeDeath1 => LanguageManager.GetTranslation("Player_StunGrenadeDeath1");
        public static string Player_StunGrenadeDeath2 => LanguageManager.GetTranslation("Player_StunGrenadeDeath2");

        public static string Other_DepositItemsDeskDeath1 => LanguageManager.GetTranslation("Other_DepositItemsDeskDeath1");
        public static string Other_DepositItemsDeskDeath2 => LanguageManager.GetTranslation("Other_DepositItemsDeskDeath2");

        public static string Other_DropshipDeath1 => LanguageManager.GetTranslation("Other_DropshipDeath1");
        public static string Other_DropshipDeath2 => LanguageManager.GetTranslation("Other_DropshipDeath2");
        public static string Other_DropshipDeath3 => LanguageManager.GetTranslation("Other_DropshipDeath3");

        public static string Other_LandmineDeath1 => LanguageManager.GetTranslation("Other_LandmineDeath1");

        public static string Other_TurretDeath1 => LanguageManager.GetTranslation("Other_TurretDeath1");

        public static string Other_LightningDeath1 => LanguageManager.GetTranslation("Other_LightningDeath1");

        public static string UnknownDeath1 => LanguageManager.GetTranslation("UnknownDeath1");
        public static string UnknownDeath2 => LanguageManager.GetTranslation("UnknownDeath2");
        public static string UnknownDeath3 => LanguageManager.GetTranslation("UnknownDeath3");

        public static string TextMesh => LanguageManager.GetTranslation("TextMesh");

    }
}
