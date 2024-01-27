using System;
using System.Linq;
using System.Xml.Linq;

namespace Coroner {
    class LanguageHandler {
        public const string DEFAULT_LANGUAGE = "en";

        public static readonly string[] AVAILABLE_LANGUAGES = [
            "en", // English
            "ru", // Russian
            "nl", // Dutch
            "fr", // French
            // "ptbr", // Portuguese (Brazil)
            // "de", // German
            // "hu", // Hungarian
            // "zh-cn", // Chinese (Simplified)
        ];

        public const string TAG_FUNNY_NOTES = "FunnyNote";

        public const string TAG_UI_NOTES = "UINotes";
        public const string TAG_UI_DEATH = "UICauseOfDeath";

        // Basic causes of death
        public const string TAG_DEATH_GENERIC_BLUDGEONING = "DeathBludgeoning";
        public const string TAG_DEATH_GENERIC_GRAVITY = "DeathGravity";
        public const string TAG_DEATH_GENERIC_BLAST = "DeathBlast";
        public const string TAG_DEATH_GENERIC_STRANGULATION = "DeathStrangulation";
        public const string TAG_DEATH_GENERIC_SUFFOCATION = "DeathSuffocation";
        public const string TAG_DEATH_GENERIC_MAULING = "DeathMauling";
        public const string TAG_DEATH_GENERIC_GUNSHOTS = "DeathGunshots";
        public const string TAG_DEATH_GENERIC_CRUSHING = "DeathCrushing";
        public const string TAG_DEATH_GENERIC_DROWNING = "DeathDrowning";
        public const string TAG_DEATH_GENERIC_ABANDONED = "DeathAbandoned";
        public const string TAG_DEATH_GENERIC_ELECTROCUTION = "DeathElectrocution";
        public const string TAG_DEATH_GENERIC_KICKING = "DeathKicking"; // New in v45

        // Custom causes (enemies)
        public const string TAG_DEATH_ENEMY_BRACKEN = "DeathEnemyBracken";
        public const string TAG_DEATH_ENEMY_EYELESS_DOG = "DeathEnemyEyelessDog";
        public const string TAG_DEATH_ENEMY_FOREST_GIANT = "DeathEnemyForestGiant";
        public const string TAG_DEATH_ENEMY_CIRCUIT_BEES = "DeathEnemyCircuitBees";
        public const string TAG_DEATH_ENEMY_GHOST_GIRL = "DeathEnemyGhostGirl";
        public const string TAG_DEATH_ENEMY_EARTH_LEVIATHAN = "DeathEnemyEarthLeviathan";
        public const string TAG_DEATH_ENEMY_BABOON_HAWK = "DeathEnemyBaboonHawk";
        public const string TAG_DEATH_ENEMY_JESTER = "DeathEnemyJester";
        public const string TAG_DEATH_ENEMY_COILHEAD = "DeathEnemyCoilHead";
        public const string TAG_DEATH_ENEMY_SNARE_FLEA = "DeathEnemySnareFlea";
        public const string TAG_DEATH_ENEMY_HYGRODERE = "DeathEnemyHygrodere";
        public const string TAG_DEATH_ENEMY_HOARDER_BUG = "DeathEnemyHoarderBug";
        public const string TAG_DEATH_ENEMY_SPORE_LIZARD = "DeathEnemySporeLizard";
        public const string TAG_DEATH_ENEMY_BUNKER_SPIDER = "DeathEnemyBunkerSpider";
        public const string TAG_DEATH_ENEMY_THUMPER = "DeathEnemyThumper";

        // Enemies from v45
        public const string TAG_DEATH_ENEMY_MASKED_PLAYER_WEAR = "DeathEnemyMaskedPlayerWear";
        public const string TAG_DEATH_ENEMY_MASKED_PLAYER_VICTIM = "DeathEnemyMaskedPlayerVictim";
        public const string TAG_DEATH_ENEMY_NUTCRACKER_KICKED = "DeathEnemyNutcrackerKicked";
        public const string TAG_DEATH_ENEMY_NUTCRACKER_SHOT = "DeathEnemyNutcrackerShot";

        // Custom causes (player)
        public const string TAG_DEATH_PLAYER_JETPACK_GRAVITY = "DeathPlayerJetpackGravity";
        public const string TAG_DEATH_PLAYER_JETPACK_BLAST = "DeathPlayerJetpackBlast";
        public const string TAG_DEATH_PLAYER_MURDER_MELEE = "DeathPlayerMurderMelee";
        public const string TAG_DEATH_PLAYER_MURDER_SHOTGUN = "DeathPlayerMurderShotgun";
        public const string TAG_DEATH_PLAYER_QUICKSAND = "DeathPlayerQuicksand";
        public const string TAG_DEATH_PLAYER_STUN_GRENADE = "DeathPlayerStunGrenade";

        // Custom causes (other)
        public const string TAG_DEATH_OTHER_DEPOSIT_ITEMS_DESK = "DeathOtherDepositItemsDesk";
        public const string TAG_DEATH_OTHER_ITEM_DROPSHIP = "DeathOtherItemDropship";
        public const string TAG_DEATH_OTHER_LANDMINE = "DeathOtherLandmine";
        public const string TAG_DEATH_OTHER_TURRET = "DeathOtherTurret";
        public const string TAG_DEATH_OTHER_LIGHTNING = "DeathOtherLightning";

        public const string TAG_DEATH_UNKNOWN = "DeathUnknown";

        static XDocument languageData;

        public static void Initialize() {
            Plugin.Instance.PluginLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} Language Support: {Plugin.Instance.PluginConfig.GetSelectedLanguage()}");

            ValidateLanguage(Plugin.Instance.PluginConfig.GetSelectedLanguage());

            LoadLanguageData(Plugin.Instance.PluginConfig.GetSelectedLanguage());
        }

        static void ValidateLanguage(string languageCode) {
            if (!AVAILABLE_LANGUAGES.Contains(languageCode)) {
                // Just throw a warning.
                Plugin.Instance.PluginLogger.LogWarning($"{PluginInfo.PLUGIN_NAME} Unknown language code: {languageCode}");
                Plugin.Instance.PluginLogger.LogWarning($"{PluginInfo.PLUGIN_NAME} There may be issues loading language data.");
            }
        }

        static void LoadLanguageData(string languageCode) {
            try
            {
                // R2Modman is a weirdo.
                // languageData = XDocument.Load($"./BepInEx/Lang/Coroner/Strings_{languageCode}.xml");
                // languageData = XDocument.Load($"./BepInEx/plugins/{PluginInfo.PLUGIN_AUTHOR}-{PluginInfo.PLUGIN_NAME}/Strings_{languageCode}.xml");
                // languageData = XDocument.Load($"{Plugin.AssemblyDirectory}/Strings_{languageCode}.xml");
                Plugin.Instance.PluginLogger.LogInfo($"Loading Coroner language data from {Plugin.Instance.GetConfigPath()}");
                languageData = XDocument.Load($"{Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml");
            }
            catch(Exception ex)
            {
                Plugin.Instance.PluginLogger.LogError($"{PluginInfo.PLUGIN_NAME} Error loading language data: {ex.Message}");
                Plugin.Instance.PluginLogger.LogError(ex.StackTrace);
                if (languageCode != DEFAULT_LANGUAGE) LoadLanguageData(DEFAULT_LANGUAGE);
            }
        }

        public static string GetLanguageList() {
            return "(" + string.Join(", ", AVAILABLE_LANGUAGES) + ")";
        }

        public static string GetValueByTag(string tag) {
            var tagElement = languageData.Descendants(tag).FirstOrDefault();
            var value = tagElement?.Attribute("text")?.Value;

            return value;
        }

        public static string[] GetValuesByTag(string tag) {
            var tags = languageData.Descendants(tag);
            var values = tags.Select(item => item.Attribute("text")?.Value);
            var filteredValues = values.Where(item => item != null);
            var valuesArray = filteredValues.ToArray();

            return valuesArray;
        }
    }
}