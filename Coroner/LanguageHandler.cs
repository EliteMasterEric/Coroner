using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Coroner {
    public class LanguageHandler {
        public const string DEFAULT_LANGUAGE = "en";

        public static readonly string[] AVAILABLE_LANGUAGES = [
            "en", // English
            "de", // German
            "es", // Spanish
            "fr", // French
            "hu", // Hungarian
            "it", // Italian
            "ko", // Korean
            "nl", // Dutch/Netherlands
            "pt-br", // Portuguese (Brazil)
            "ru", // Russian
            "zh-cn" // Chinese (Simplified)
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
        public const string TAG_DEATH_PLAYER_LADDER = "DeathPlayerLadder";
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

        public string languageCode;
        XDocument languageData;

        public LanguageHandler(string languageCode) {
            Plugin.Instance.PluginLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} loading language support: {languageCode}");
            this.languageCode = languageCode;

            ValidateLanguage(languageCode);

            LoadLanguageData(languageCode);
        }

        void ValidateLanguage(string languageCode) {
            if (!AVAILABLE_LANGUAGES.Contains(languageCode)) {
                // Just throw a warning.
                Plugin.Instance.PluginLogger.LogWarning($"{PluginInfo.PLUGIN_NAME} Unknown language code: {languageCode}");
                Plugin.Instance.PluginLogger.LogWarning($"{PluginInfo.PLUGIN_NAME} There may be issues loading language data.");
            }
        }

        void LoadLanguageData(string languageCode) {
            try
            {
                Plugin.Instance.PluginLogger.LogInfo($"Loading language data from config folder: {Plugin.Instance.GetConfigPath()}");
                // R2Modman is a weirdo.
                // languageData = XDocument.Load($"./BepInEx/Lang/Coroner/Strings_{languageCode}.xml");
                // languageData = XDocument.Load($"./BepInEx/plugins/{PluginInfo.PLUGIN_AUTHOR}-{PluginInfo.PLUGIN_NAME}/Strings_{languageCode}.xml");
                // languageData = XDocument.Load($"{Plugin.AssemblyDirectory}/Strings_{languageCode}.xml");

                if (!Directory.Exists(Plugin.Instance.GetConfigPath()))
                {
                    Plugin.Instance.PluginLogger.LogError($"Config folder not found at: {Plugin.Instance.GetConfigPath()}");
                    var wrongConfigPath = $"{Plugin.AssemblyDirectory}/BepInEx/config/{PluginInfo.PLUGIN_AUTHOR}-{PluginInfo.PLUGIN_NAME}/Strings_{languageCode}.xml";
                    if (File.Exists(wrongConfigPath)) {
                        Plugin.Instance.PluginLogger.LogError($"IMPORTANT: You didn't install the mod correctly! Move the BepInEx/config folder to the right spot!");
                    } else {
                        Plugin.Instance.PluginLogger.LogError($"Try reinstalling the mod from scratch.");
                    }
                }
                else if (!File.Exists($"{Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml")) {
                    Plugin.Instance.PluginLogger.LogError($"Localization File not found at: {Plugin.Instance.GetConfigPath()}");
                }
                else
                {
                    languageData = XDocument.Load($"{Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml");
                }
            }
            catch(Exception ex)
            {
                Plugin.Instance.PluginLogger.LogError($"Error loading language data: {ex.Message}");
                Plugin.Instance.PluginLogger.LogError(ex.StackTrace);
            }
        }

        public static string GetLanguageList() {
            return "(" + string.Join(", ", AVAILABLE_LANGUAGES) + ")";
        }

        public string GetValueByTag(string tag) {
            if (languageData == null && languageCode != DEFAULT_LANGUAGE) {
                return Plugin.Instance.FallbackLanguageHandler.GetValueByTag(tag);
            }

            var tagElement = languageData.Descendants(tag).FirstOrDefault();
            var value = tagElement?.Attribute("text")?.Value;

            if (value == null || value.Length == 0) {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying error text...");
                return $"{{'{tag}'}}";
            }

            return value;
        }

        public string[] GetValuesByTag(string tag) {
            if (languageData == null && languageCode != DEFAULT_LANGUAGE) {
                return Plugin.Instance.FallbackLanguageHandler.GetValuesByTag(tag);
            }

            var tags = languageData.Descendants(tag);
            var values = tags.Select(item => item.Attribute("text")?.Value);
            var filteredValues = values.Where(item => item != null);
            var valuesArray = filteredValues.ToArray();

            if (valuesArray.Length == 0 && languageCode != DEFAULT_LANGUAGE) {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', using fallback...");
                return Plugin.Instance.FallbackLanguageHandler.GetValuesByTag(tag);
            } else if (valuesArray.Length == 0) {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying error text...");
                return [$"{{'{tag}'}}"];
            }

            return valuesArray;
        }
    }
}