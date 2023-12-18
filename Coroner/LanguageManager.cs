using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace Coroner
{
    public static class LanguageManager
    {
        private static CultureInfo currentCulture = CultureInfo.CurrentCulture;

        public static CultureInfo CurrentCulture
        {
            get { return currentCulture; }
        }

        public static void SetLanguage(string languageCode)
        {
            try
            {
                currentCulture = new CultureInfo(languageCode);
            }
            catch (CultureNotFoundException)
            {
                currentCulture = CultureInfo.CurrentCulture;
            }
        }
    }
    public static class LangStrings
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager(typeof(LangStrings));
    
        public static string FunnyNote1 => ResourceManager.GetString("FunnyNote1", LanguageManager.CurrentCulture);
        public static string FunnyNote2 => ResourceManager.GetString("FunnyNote2", LanguageManager.CurrentCulture);
        public static string FunnyNote3 => ResourceManager.GetString("FunnyNote3", LanguageManager.CurrentCulture);
        public static string FunnyNote4 => ResourceManager.GetString("FunnyNote4", LanguageManager.CurrentCulture);
        public static string FunnyNote5 => ResourceManager.GetString("FunnyNote5", LanguageManager.CurrentCulture);
        public static string FunnyNote6 => ResourceManager.GetString("FunnyNote6", LanguageManager.CurrentCulture);
        public static string FunnyNote7 => ResourceManager.GetString("FunnyNote7", LanguageManager.CurrentCulture);
        public static string FunnyNote8 => ResourceManager.GetString("FunnyNote8", LanguageManager.CurrentCulture);
        public static string FunnyNote9 => ResourceManager.GetString("FunnyNote9", LanguageManager.CurrentCulture);
        public static string FunnyNote10 => ResourceManager.GetString("FunnyNote10", LanguageManager.CurrentCulture);
        public static string FunnyNote11 => ResourceManager.GetString("FunnyNote11", LanguageManager.CurrentCulture);
        public static string FunnyNote12 => ResourceManager.GetString("FunnyNote12", LanguageManager.CurrentCulture);
        public static string FunnyNote13 => ResourceManager.GetString("FunnyNote13", LanguageManager.CurrentCulture);
        public static string FunnyNote14 => ResourceManager.GetString("FunnyNote14", LanguageManager.CurrentCulture);
        public static string FunnyNote15 => ResourceManager.GetString("FunnyNote15", LanguageManager.CurrentCulture);
        public static string FunnyNote16 => ResourceManager.GetString("FunnyNote16", LanguageManager.CurrentCulture);
        public static string FunnyNote17 => ResourceManager.GetString("FunnyNote17", LanguageManager.CurrentCulture);
        public static string FunnyNote18 => ResourceManager.GetString("FunnyNote18", LanguageManager.CurrentCulture);
        public static string FunnyNote19 => ResourceManager.GetString("FunnyNote19", LanguageManager.CurrentCulture);
        public static string FunnyNote20 => ResourceManager.GetString("FunnyNote20", LanguageManager.CurrentCulture);
    
    
    
        public static string BludgeoningDeath1 => ResourceManager.GetString("BludgeoningDeath", LanguageManager.CurrentCulture);
    
        public static string GravityDeath1 => ResourceManager.GetString("GravityDeath1", LanguageManager.CurrentCulture);
        public static string GravityDeath2 => ResourceManager.GetString("GravityDeath2", LanguageManager.CurrentCulture);
    
        public static string BlastDeath1 => ResourceManager.GetString("BlastDeath1", LanguageManager.CurrentCulture);
        public static string BlastDeath2 => ResourceManager.GetString("BlastDeath2", LanguageManager.CurrentCulture);
        public static string BlastDeath3 => ResourceManager.GetString("BlastDeath3", LanguageManager.CurrentCulture);
        
        public static string StrangulationDeath1 => ResourceManager.GetString("StrangulationDeath1", LanguageManager.CurrentCulture);
        
        public static string SuffocationDeath1 => ResourceManager.GetString("SuffocationDeath1", LanguageManager.CurrentCulture);
        
        public static string MaulingDeath1 => ResourceManager.GetString("MaulingDeath1", LanguageManager.CurrentCulture);
        
        public static string GunshotsDeath1 => ResourceManager.GetString("GunshotsDeath1", LanguageManager.CurrentCulture);
        public static string GunshotsDeath2 => ResourceManager.GetString("GunshotsDeath2", LanguageManager.CurrentCulture);
        
        public static string CrushingDeath1 => ResourceManager.GetString("CrushingDeath1", LanguageManager.CurrentCulture);
        
        public static string DrowningDeath1 => ResourceManager.GetString("DrowningDeath1", LanguageManager.CurrentCulture);
        
        public static string AbandonedDeath1 => ResourceManager.GetString("AbandonedDeath1", LanguageManager.CurrentCulture);
        
        public static string ElectrocutionDeath1 => ResourceManager.GetString("ElectrocutionDeath1", LanguageManager.CurrentCulture);
        
        public static string KickingDeath1 => ResourceManager.GetString("KickingDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_BrackenDeath1 => ResourceManager.GetString("Enemy_BrackenDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_BrackenDeath2 => ResourceManager.GetString("Enemy_BrackenDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_EyelessDogDeath1 => ResourceManager.GetString("Enemy_EyelessDogDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_EyelessDogDeath2 => ResourceManager.GetString("Enemy_EyelessDogDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_EyelessDogDeath3 => ResourceManager.GetString("Enemy_EyelessDogDeath3", LanguageManager.CurrentCulture);
        
        public static string Enemy_ForestGiantDeath1 => ResourceManager.GetString("Enemy_ForestGiantDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_CircuitBeesDeath1 => ResourceManager.GetString("Enemy_CircuitBeesDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_GhostGirlDeath1 => ResourceManager.GetString("Enemy_GhostGirlDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_GhostGirlDeath2 => ResourceManager.GetString("Enemy_GhostGirlDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_GhostGirlDeath3 => ResourceManager.GetString("Enemy_GhostGirlDeath3", LanguageManager.CurrentCulture);
        public static string Enemy_GhostGirlDeath4 => ResourceManager.GetString("Enemy_GhostGirlDeath4", LanguageManager.CurrentCulture);
        
        public static string Enemy_EarthLeviathanDeath1 => ResourceManager.GetString("Enemy_EarthLeviathanDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_BaboonHawkDeath1 => ResourceManager.GetString("Enemy_BaboonHawkDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_BaboonHawkDeath2 => ResourceManager.GetString("Enemy_BaboonHawkDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_JesterDeath1 => ResourceManager.GetString("Enemy_JesterDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_JesterDeath2 => ResourceManager.GetString("Enemy_JesterDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_JesterDeath3 => ResourceManager.GetString("Enemy_JesterDeath3", LanguageManager.CurrentCulture);
        public static string Enemy_JesterDeath4 => ResourceManager.GetString("Enemy_JesterDeath4", LanguageManager.CurrentCulture);
        
        public static string Enemy_CoilHeadDeath1 => ResourceManager.GetString("Enemy_CoilHeadDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_CoilHeadDeath2 => ResourceManager.GetString("Enemy_CoilHeadDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_CoilHeadDeath3 => ResourceManager.GetString("Enemy_CoilHeadDeath3", LanguageManager.CurrentCulture);
        
        public static string Enemy_SnareFleaDeath1 => ResourceManager.GetString("Enemy_SnareFleaDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_HygrodereDeath1 => ResourceManager.GetString("Enemy_HygrodereDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_HygrodereDeath2 => ResourceManager.GetString("Enemy_HygrodereDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_HygrodereDeath3 => ResourceManager.GetString("Enemy_HygrodereDeath3", LanguageManager.CurrentCulture);
        
        public static string Enemy_HoarderBugDeath1 => ResourceManager.GetString("Enemy_HoarderBugDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_HoarderBugDeath2 => ResourceManager.GetString("Enemy_HoarderBugDeath2", LanguageManager.CurrentCulture);
        public static string Enemy_HoarderBugDeath3 => ResourceManager.GetString("Enemy_HoarderBugDeath3", LanguageManager.CurrentCulture);
        public static string Enemy_HoarderBugDeath4 => ResourceManager.GetString("Enemy_HoarderBugDeath4", LanguageManager.CurrentCulture);
        
        public static string Enemy_SporeLizardDeath1 => ResourceManager.GetString("Enemy_SporeLizardDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_SporeLizardDeath2 => ResourceManager.GetString("Enemy_SporeLizardDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_BunkerSpiderDeath1 => ResourceManager.GetString("Enemy_BunkerSpiderDeath1", LanguageManager.CurrentCulture);
        
        public static string Enemy_ThumperDeath1 => ResourceManager.GetString("Enemy_ThumperDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_ThumperDeath2 => ResourceManager.GetString("Enemy_ThumperDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_MaskedPlayer_WearDeath1 => ResourceManager.GetString("Enemy_MaskedPlayer_WearDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_MaskedPlayer_WearDeath2 => ResourceManager.GetString("Enemy_MaskedPlayer_WearDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_MaskedPlayer_VictimDeath1 => ResourceManager.GetString("Enemy_MaskedPlayer_VictimDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_MaskedPlayer_VictimDeath2 => ResourceManager.GetString("Enemy_MaskedPlayer_VictimDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_Nutcracker_KickedDeath1 => ResourceManager.GetString("Enemy_Nutcracker_KickedDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_Nutcracker_KickedDeath2 => ResourceManager.GetString("Enemy_Nutcracker_KickedDeath2", LanguageManager.CurrentCulture);
        
        public static string Enemy_Nutcracker_ShotDeath1 => ResourceManager.GetString("Enemy_Nutcracker_ShotDeath1", LanguageManager.CurrentCulture);
        public static string Enemy_Nutcracker_ShotDeath2 => ResourceManager.GetString("Enemy_Nutcracker_ShotDeath2", LanguageManager.CurrentCulture);
        
        public static string Player_Jetpack_GravityDeath1 => ResourceManager.GetString("Player_Jetpack_GravityDeath1", LanguageManager.CurrentCulture);
        public static string Player_Jetpack_GravityDeath2 => ResourceManager.GetString("Player_Jetpack_GravityDeath2", LanguageManager.CurrentCulture);
        public static string Player_Jetpack_GravityDeath3 => ResourceManager.GetString("Player_Jetpack_GravityDeath3", LanguageManager.CurrentCulture);
        
        public static string Player_Jetpack_BlastDeath1 => ResourceManager.GetString("Player_Jetpack_BlastDeath1", LanguageManager.CurrentCulture);
        public static string Player_Jetpack_BlastDeath2 => ResourceManager.GetString("Player_Jetpack_BlastDeath2", LanguageManager.CurrentCulture);
        
        public static string Player_Murder_MeleeDeath1 => ResourceManager.GetString("Player_Murder_MeleeDeath1", LanguageManager.CurrentCulture);
        public static string Player_Murder_MeleeDeath2 => ResourceManager.GetString("Player_Murder_MeleeDeath2", LanguageManager.CurrentCulture);
        public static string Player_Murder_MeleeDeath3 => ResourceManager.GetString("Player_Murder_MeleeDeath3", LanguageManager.CurrentCulture);
        public static string Player_Murder_MeleeDeath4 => ResourceManager.GetString("Player_Murder_MeleeDeath4", LanguageManager.CurrentCulture);
        
        public static string Player_Murder_ShotgunDeath1 => ResourceManager.GetString("Player_Murder_ShotgunDeath1", LanguageManager.CurrentCulture);
        public static string Player_Murder_ShotgunDeath2 => ResourceManager.GetString("Player_Murder_ShotgunDeath2", LanguageManager.CurrentCulture);
        public static string Player_Murder_ShotgunDeath3 => ResourceManager.GetString("Player_Murder_ShotgunDeath3", LanguageManager.CurrentCulture);
        public static string Player_Murder_ShotgunDeath4 => ResourceManager.GetString("Player_Murder_ShotgunDeath4", LanguageManager.CurrentCulture);
        public static string Player_Murder_ShotgunDeath5 => ResourceManager.GetString("Player_Murder_ShotgunDeath5", LanguageManager.CurrentCulture);
        
        public static string Player_QuicksandDeath1 => ResourceManager.GetString("Player_QuicksandDeath1", LanguageManager.CurrentCulture);
        public static string Player_QuicksandDeath2 => ResourceManager.GetString("Player_QuicksandDeath2", LanguageManager.CurrentCulture);
        
        public static string Player_StunGrenadeDeath1 => ResourceManager.GetString("Player_StunGrenadeDeath1", LanguageManager.CurrentCulture);
        public static string Player_StunGrenadeDeath2 => ResourceManager.GetString("Player_StunGrenadeDeath2", LanguageManager.CurrentCulture);
        
        public static string Other_DepositItemsDeskDeath1 => ResourceManager.GetString("Other_DepositItemsDeskDeath1", LanguageManager.CurrentCulture);
        public static string Other_DepositItemsDeskDeath2 => ResourceManager.GetString("Other_DepositItemsDeskDeath2", LanguageManager.CurrentCulture);
        
        public static string Other_DropshipDeath1 => ResourceManager.GetString("Other_DropshipDeath1", LanguageManager.CurrentCulture);
        public static string Other_DropshipDeath2 => ResourceManager.GetString("Other_DropshipDeath2", LanguageManager.CurrentCulture);
        public static string Other_DropshipDeath3 => ResourceManager.GetString("Other_DropshipDeath3", LanguageManager.CurrentCulture);
        
        public static string Other_LandmineDeath1 => ResourceManager.GetString("Other_LandmineDeath1", LanguageManager.CurrentCulture);
        
        public static string Other_TurretDeath1 => ResourceManager.GetString("Other_TurretDeath1", LanguageManager.CurrentCulture);
        
        public static string Other_LightningDeath1 => ResourceManager.GetString("Other_LightningDeath1", LanguageManager.CurrentCulture);
        
        public static string UnknownDeath1 => ResourceManager.GetString("UnknownDeath1Death1", LanguageManager.CurrentCulture);
        public static string UnknownDeath2 => ResourceManager.GetString("UnknownDeath2Death2", LanguageManager.CurrentCulture);
        public static string UnknownDeath3 => ResourceManager.GetString("UnknownDeath3Death3", LanguageManager.CurrentCulture);
        
        public static string TextMesh => ResourceManager.GetString("TextMesh", LanguageManager.CurrentCulture);
    
    }
}
