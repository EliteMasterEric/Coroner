using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Xml.Linq;

#nullable enable

namespace Coroner
{
    class LanguageHandler
    {
        public const string DEFAULT_LANGUAGE = "en-us";

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
        public const string TAG_DEATH_GENERIC_BURNING = "DeathBurning"; // New in v50
        public const string TAG_DEATH_GENERIC_STABBING = "DeathStabbing"; // New in v50
        public const string TAG_DEATH_GENERIC_FAN = "DeathFan"; // New in v50
        public const string TAG_DEATH_GENERIC_INERTIA = "DeathInertia"; // New in v55
        public const string TAG_DEATH_GENERIC_SNIPPED = "DeathSnipped"; // New in v55


        // Custom causes (enemies)
        public const string TAG_DEATH_ENEMY_BABOON_HAWK = "DeathEnemyBaboonHawk";
        public const string TAG_DEATH_ENEMY_BRACKEN = "DeathEnemyBracken";
        public const string TAG_DEATH_ENEMY_BUNKER_SPIDER = "DeathEnemyBunkerSpider";
        public const string TAG_DEATH_ENEMY_CIRCUIT_BEES = "DeathEnemyCircuitBees";
        public const string TAG_DEATH_ENEMY_COILHEAD = "DeathEnemyCoilHead";
        public const string TAG_DEATH_ENEMY_EARTH_LEVIATHAN = "DeathEnemyEarthLeviathan";
        public const string TAG_DEATH_ENEMY_EYELESS_DOG = "DeathEnemyEyelessDog";
        public const string TAG_DEATH_ENEMY_GHOST_GIRL = "DeathEnemyGhostGirl";
        public const string TAG_DEATH_ENEMY_HOARDER_BUG = "DeathEnemyHoarderBug";
        public const string TAG_DEATH_ENEMY_HYGRODERE = "DeathEnemyHygrodere";
        public const string TAG_DEATH_ENEMY_JESTER = "DeathEnemyJester";
        public const string TAG_DEATH_ENEMY_LASSO_MAN = "DeathEnemyLassoMan";
        public const string TAG_DEATH_ENEMY_SNARE_FLEA = "DeathEnemySnareFlea";
        public const string TAG_DEATH_ENEMY_SPORE_LIZARD = "DeathEnemySporeLizard";
        public const string TAG_DEATH_ENEMY_THUMPER = "DeathEnemyThumper";

        public const string TAG_DEATH_ENEMY_FOREST_GIANT_EATEN = "DeathEnemyForestGiantEaten";
        public const string TAG_DEATH_ENEMY_FOREST_GIANT_DEATH = "DeathEnemyForestGiantDeath";

        // Enemies from v45
        public const string TAG_DEATH_ENEMY_MASKED_PLAYER_WEAR = "DeathEnemyMaskedPlayerWear";
        public const string TAG_DEATH_ENEMY_MASKED_PLAYER_VICTIM = "DeathEnemyMaskedPlayerVictim";
        public const string TAG_DEATH_ENEMY_NUTCRACKER_KICKED = "DeathEnemyNutcrackerKicked";
        public const string TAG_DEATH_ENEMY_NUTCRACKER_SHOT = "DeathEnemyNutcrackerShot";

        // Enemies from v50
        public const string TAG_DEATH_ENEMY_BUTLER_STAB = "DeathEnemyButlerStab";
        public const string TAG_DEATH_ENEMY_BUTLER_EXPLODE = "DeathEnemyButlerExplode";
        public const string TAG_DEATH_ENEMY_MASK_HORNETS = "DeathEnemyMaskHornets";
        public const string TAG_DEATH_ENEMY_TULIP_SNAKE_DROP = "DeathEnemyTulipSnakeDrop";
        public const string TAG_DEATH_ENEMY_OLD_BIRD_ROCKET = "DeathEnemyOldBirdRocket";
        public const string TAG_DEATH_ENEMY_OLD_BIRD_STOMP = "DeathEnemyOldBirdStomp";
        public const string TAG_DEATH_ENEMY_OLD_BIRD_CHARGE = "DeathEnemyOldBirdCharge";
        public const string TAG_DEATH_ENEMY_OLD_BIRD_TORCH = "DeathEnemyOldBirdTorch";

        // Enemies from v55
        public const string TAG_DEATH_ENEMY_KIDNAPPER_FOX = "DeathEnemyKidnapperFox";
        public const string TAG_DEATH_ENEMY_BARBER = "DeathEnemyBarber";

        // Enemies from v60
        public const string TAG_DEATH_ENEMY_MANEATER = "DeathEnemyManeater";

        // Custom causes (player)
        public const string TAG_DEATH_PLAYER_JETPACK_GRAVITY = "DeathPlayerJetpackGravity";
        public const string TAG_DEATH_PLAYER_JETPACK_BLAST = "DeathPlayerJetpackBlast";
        public const string TAG_DEATH_PLAYER_LADDER = "DeathPlayerLadder";
        public const string TAG_DEATH_PLAYER_MURDER_SHOVEL = "DeathPlayerMurderShovel";
        public const string TAG_DEATH_PLAYER_MURDER_STOP_SIGN = "DeathPlayerMurderStopSign";
        public const string TAG_DEATH_PLAYER_MURDER_YIELD_SIGN = "DeathPlayerMurderYieldSign";
        public const string TAG_DEATH_PLAYER_MURDER_KNIFE = "DeathPlayerMurderKnife";
        public const string TAG_DEATH_PLAYER_EASTER_EGG = "DeathPlayerEasterEgg";
        public const string TAG_DEATH_PLAYER_MURDER_SHOTGUN = "DeathPlayerMurderShotgun";
        public const string TAG_DEATH_PLAYER_QUICKSAND = "DeathPlayerQuicksand";
        public const string TAG_DEATH_PLAYER_STUN_GRENADE = "DeathPlayerStunGrenade";

        // Custom causes (player vehicles)
        public const string TAG_DEATH_PLAYER_CRUISER_DRIVER = "DeathPlayerCruiserDriver";
        public const string TAG_DEATH_PLAYER_CRUISER_PASSENGER = "DeathPlayerCruiserPassenger";
        public const string TAG_DEATH_PLAYER_CRUISER_EXPLODE_BYSTANDER = "DeathPlayerCruiserExplodeBystander";
        public const string TAG_DEATH_PLAYER_CRUISER_RAN_OVER = "DeathPlayerCruiserRanOver";

        // Custom causes (pits)
        public const string TAG_DEATH_PIT_GENERIC = "DeathPitGeneric";
        public const string TAG_DEATH_PIT_FACILITY_PIT = "DeathPitFacilityPit";
        public const string TAG_DEATH_PIT_FACILITY_CATWALK_JUMP = "DeathPitFacilityCatwalkJump";
        public const string TAG_DEATH_PIT_MINE_PIT = "DeathPitMinePit";
        public const string TAG_DEATH_PIT_MINE_CAVE = "DeathPitMineCave";
        public const string TAG_DEATH_PIT_MINE_ELEVATOR = "DeathPitMineElevator";

        // Custom causes (other)
        public const string TAG_DEATH_OTHER_DEPOSIT_ITEMS_DESK = "DeathOtherDepositItemsDesk";
        public const string TAG_DEATH_OTHER_ITEM_DROPSHIP = "DeathOtherItemDropship";
        public const string TAG_DEATH_OTHER_LANDMINE = "DeathOtherLandmine";
        public const string TAG_DEATH_OTHER_TURRET = "DeathOtherTurret";
        public const string TAG_DEATH_OTHER_LIGHTNING = "DeathOtherLightning";
        public const string TAG_DEATH_OTHER_METEOR = "DeathOtherMeteor";
        public const string TAG_DEATH_OTHER_SPIKE_TRAP = "DeathOtherSpikeTrap";
        public const string TAG_DEATH_OTHER_OUT_OF_BOUNDS = "DeathOtherOutOfBounds";

        public const string TAG_DEATH_UNKNOWN = "DeathUnknown";

        public string languageCode;
        Dictionary<string, List<string>> languageData;
        bool fallback;

        public LanguageHandler(string languageCode, bool fallback = false)
        {
            Plugin.Instance.PluginLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} loading {(fallback ? "fallback " : "")}language support: {languageCode}");
            this.languageCode = languageCode;
            this.fallback = fallback;
            this.languageData = new Dictionary<string, List<string>>();

            LoadLanguageData(languageCode);
        }

        void LoadLanguageData(string languageCode)
        {
            if (!Directory.Exists(Plugin.Instance.GetConfigPath()))
            {
                Plugin.Instance.PluginLogger.LogError($"Config folder not found at: {Plugin.Instance.GetConfigPath()}");
                var wrongConfigPath = $"{Plugin.AssemblyDirectory}/BepInEx/config/{PluginInfo.PLUGIN_AUTHOR}-{PluginInfo.PLUGIN_NAME}/Strings_{languageCode}.xml";
                if (File.Exists(wrongConfigPath))
                {
                    Plugin.Instance.PluginLogger.LogError($"IMPORTANT: You didn't install the mod correctly! Move the BepInEx/config folder to the right spot!");
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogError($"Try reinstalling the mod from scratch.");
                }
            }
            else if (!File.Exists($"{Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml"))
            {
                Plugin.Instance.PluginLogger.LogError($"Localization File not found at: {Plugin.Instance.GetConfigPath()}");
            }
            else
            {
                Plugin.Instance.PluginLogger.LogInfo($"Loading language data from config folder: {Plugin.Instance.GetConfigPath()}");
                // Load the main language document.
                var languageDoc = XDocument.Load($"{Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml");

                if (languageDoc == null)
                {
                    Plugin.Instance.PluginLogger.LogError($"Localization could not be parsed at: {Plugin.Instance.GetConfigPath()}/Strings_{languageCode}.xml");
                    return;
                }

                PopulateLanguageData(languageDoc);

                // Look for additional language documents.
                var additionalLanguageFiles = Directory.GetFiles(Plugin.Instance.GetConfigPath(), $"Strings_{languageCode}_*.xml").ToArray();
                Plugin.Instance.PluginLogger.LogInfo($"Loading {additionalLanguageFiles.Length} additional language data files from config folder");
                for (int index = 0; index < additionalLanguageFiles.Length; index++)
                {
                    Plugin.Instance.PluginLogger.LogInfo($"Loading additional language data file: {additionalLanguageFiles[index]}");
                    var additionalLanguageDoc = XDocument.Load(additionalLanguageFiles[index]);
                    PopulateLanguageData(additionalLanguageDoc, true);
                }

                Plugin.Instance.PluginLogger.LogInfo($"Success, loaded {languageData.Count()} types of language tags with {CountLanguageEntries()} total entries.");
            }
        }

        /**
         * Populate the Language Data dictionary with keys from the XML data.
         * If `append` is false, the tag will be cleared before populating.
         */
        public void PopulateLanguageData(XDocument languageDoc, bool append = false)
        {
            if (!append)
            {
                languageData.Clear();
            }

            var stringsTag = languageDoc.Descendants("strings").First();
            foreach (var tag in stringsTag.Descendants())
            {
                var key = tag.Name.ToString();
                var value = tag.Attribute("text").Value;

                // Create a new array if the key doesn't exist
                if (!languageData.ContainsKey(key))
                {
                    languageData[key] = [];
                }

                // Add the value to the array
                var currentValues = languageData[key];
                if (currentValues != null)
                {
                    currentValues.Add(value);
                    // Plugin.Instance.PluginLogger.LogDebug($"Current values for key '{key}': [{string.Join(", ", currentValues)}] + {value}");
                    languageData[key] = currentValues;
                }
                else
                {
                    Plugin.Instance.PluginLogger.LogError($"Current values for key '{key}' is null!");
                }
            }
        }

        public int CountLanguageEntries()
        {
            return languageData.Sum(x => x.Value.Count);
        }

        public string GetFirstValueByTag(string tag)
        {
            return GetValuesByTag(tag)[0];
        }

        public string[] GetValuesByTag(string tag)
        {
            if (languageData.Count == 0 && languageCode != DEFAULT_LANGUAGE)
            {
                Plugin.Instance.PluginLogger.LogWarning($"No language data loaded for '{languageCode}', displaying fallback language {DEFAULT_LANGUAGE}...");
                return Plugin.Instance.FallbackLanguageHandler.GetValuesByTag(tag);
            }
            else if (languageData.Count == 0)
            {
                Plugin.Instance.PluginLogger.LogWarning("No language data loaded for default language, displaying error text...");
                return [$"{{'{tag}'}}"];
            }

            if (!languageData.ContainsKey(tag) && languageCode != DEFAULT_LANGUAGE)
            {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying fallback language {DEFAULT_LANGUAGE}...");
                return Plugin.Instance.FallbackLanguageHandler.GetValuesByTag(tag);
            }
            else if (!languageData.ContainsKey(tag))
            {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying error text...");
                return [$"{{'{tag}'}}"];
            }

            var values = languageData[tag];

            if ((values == null || values.Count == 0) && languageCode != DEFAULT_LANGUAGE)
            {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying fallback language {DEFAULT_LANGUAGE}...");
                return Plugin.Instance.FallbackLanguageHandler.GetValuesByTag(tag);
            }
            else if (values == null || values.Count == 0)
            {
                Plugin.Instance.PluginLogger.LogWarning($"No values found for tag '{tag}' in language '{languageCode}', displaying error text...");
                return [$"{{'{tag}'}}"];
            }

            return values.ToArray();
        }
    }
}