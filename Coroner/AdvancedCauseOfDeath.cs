using System;
using System.Collections.Generic;
using GameNetcodeStuff;

#nullable enable

namespace Coroner
{
    class AdvancedDeathTracker
    {
        public static Dictionary<int, AdvancedCauseOfDeath?> causeOfDeathDictionary = new Dictionary<int, AdvancedCauseOfDeath?>();

        public static void ClearDeathTracker()
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                // Set the cause of death to null for each player.
                // true = force override
                StoreLocalCauseOfDeath((int) player.playerClientId, null, true);
            }
        }

        public static void SetCauseOfDeath(int playerIndex, AdvancedCauseOfDeath? causeOfDeath, bool forceOverride = false) {
            if (causeOfDeath == null) {
                NetworkRPC.ReportCauseOfDeathServerRpc(playerIndex, null, forceOverride);    
            } else {
                AdvancedCauseOfDeath result = (AdvancedCauseOfDeath) causeOfDeath;
                Plugin.Instance.PluginLogger.LogDebug($"Serializing {result} to {result.GetLanguageTag()}{(forceOverride ? " (FORCED)" : "")}");
                NetworkRPC.ReportCauseOfDeathServerRpc(playerIndex, result.GetLanguageTag(), forceOverride);
            }
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, AdvancedCauseOfDeath? causeOfDeath, bool forceOverride = false)
        {
            SetCauseOfDeath((int) playerController.playerClientId, causeOfDeath, forceOverride);
        }

        public static void StoreLocalCauseOfDeath(int playerId, AdvancedCauseOfDeath? causeOfDeath, bool overrideExisting)
        {
            // If overrideExisting is false, don't override an existing cause of death unless we are clearing it.
            if (!overrideExisting && (causeOfDeath == null || HasCauseOfDeath(playerId))) {
                if (causeOfDeath == null) {
                    Plugin.Instance.PluginLogger.LogDebug($"Ignoring null cause of death for player {playerId}");
                    return;
                } else {
                    var newCauseOfDeathTag = ((AdvancedCauseOfDeath) causeOfDeath).GetLanguageTag();
                    AdvancedCauseOfDeath? existingCauseOfDeath = GetCauseOfDeath(playerId, false);
                    string existingCauseOfDeathTag = existingCauseOfDeath == null ? "null" : ((AdvancedCauseOfDeath) existingCauseOfDeath).GetLanguageTag();
                    Plugin.Instance.PluginLogger.LogWarning($"Player {playerId} already has a cause of death set ({existingCauseOfDeathTag}), not overwriting it with {newCauseOfDeathTag}.");
                    return;
                }
            }

            if (causeOfDeath == null)
            {
                Plugin.Instance.PluginLogger.LogDebug($"Clearing cause of death for player {playerId}");
                causeOfDeathDictionary[playerId] = null;
            }
            else
            {
                AdvancedCauseOfDeath? existingCauseOfDeath = GetCauseOfDeath(playerId, false);
                string existingCauseOfDeathTag = existingCauseOfDeath == null ? "null" : ((AdvancedCauseOfDeath) existingCauseOfDeath).GetLanguageTag();
                string causeOfDeathTag = ((AdvancedCauseOfDeath)causeOfDeath).GetLanguageTag();
                Plugin.Instance.PluginLogger.LogDebug($"Storing cause of death {causeOfDeathTag} (overriding {existingCauseOfDeathTag}) for player {playerId}!");
                causeOfDeathDictionary[playerId] = causeOfDeath;
            }
        }

        /**
         * Retrieve the network variable storing the cause of death for the given player.
         * If it doesn't exist, instantiate it and store it statically.
         *
         * @param playerController
         * @return the cause of death network variable
         */
        static AdvancedCauseOfDeath? FetchCauseOfDeathVariable(PlayerControllerB playerController) {
            if (!HasCauseOfDeath(playerController)) {
                return null;
            }

            return causeOfDeathDictionary[(int) playerController.playerClientId];
        }

        public static AdvancedCauseOfDeath? GetCauseOfDeath(int playerIndex, bool shouldGuess = true)
        {
            PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[playerIndex];

            return GetCauseOfDeath(playerController, shouldGuess);
        }

        public static bool HasCauseOfDeath(int playerIndex)
        {
            return HasCauseOfDeath(StartOfRound.Instance.allPlayerScripts[playerIndex]);
        }

        public static bool HasCauseOfDeath(PlayerControllerB playerController)
        {
            return causeOfDeathDictionary.ContainsKey((int) playerController.playerClientId) && (causeOfDeathDictionary[(int) playerController.playerClientId] != null);
        }

        public static AdvancedCauseOfDeath? GetCauseOfDeath(PlayerControllerB playerController, bool shouldGuess = true)
        {
            AdvancedCauseOfDeath? causeOfDeath = FetchCauseOfDeathVariable(playerController);

            if (causeOfDeath != null)
            {
                AdvancedCauseOfDeath result = (AdvancedCauseOfDeath) causeOfDeath;
                Plugin.Instance.PluginLogger.LogDebug($"Player {playerController.playerClientId} has custom cause of death stored! {result.GetLanguageTag()}");
                return result;
            } else if (!shouldGuess) {
                Plugin.Instance.PluginLogger.LogDebug($"Player {playerController.playerClientId} has no custom cause of death stored! Returning null...");    
                return null;
            } else {
                Plugin.Instance.PluginLogger.LogDebug($"Player {playerController.playerClientId} has no custom cause of death stored! Using fallback...");
                return GuessCauseOfDeath(playerController);
            }
        }

        public static AdvancedCauseOfDeath GuessCauseOfDeath(PlayerControllerB playerController)
        {
            if (playerController.isPlayerDead)
            {
                if (IsHoldingJetpack(playerController))
                {
                    if (playerController.causeOfDeath == CauseOfDeath.Gravity)
                    {
                        return AdvancedCauseOfDeath.Player_Jetpack_Gravity;
                    }
                    else if (playerController.causeOfDeath == CauseOfDeath.Blast)
                    {
                        return AdvancedCauseOfDeath.Player_Jetpack_Blast;
                    }
                }

                // Implicit cast
                return playerController.causeOfDeath;
            }
            else
            {
                return AdvancedCauseOfDeath.Unknown;
            }
        }

        public static GrabbableObject? GetHeldObject(PlayerControllerB playerController)
        {
            var heldObjectServer = playerController.currentlyHeldObjectServer;
            if (heldObjectServer == null) return null;
            var heldObjectGameObject = heldObjectServer.gameObject;
            if (heldObjectGameObject == null) return null;
            var heldObject = heldObjectGameObject.GetComponent<GrabbableObject>();

            return heldObject;
        }

        public static bool IsHoldingJetpack(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is JetpackItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsHoldingShovel(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is Shovel)
            {
                if (heldObject.gameObject.name.StartsWith("Shovel"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsHoldingStopSign(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is Shovel)
            {
                if (heldObject.gameObject.name.StartsWith("StopSign"))
                {
                    return true;
                }
            }
            return false;

        }

        public static bool IsHoldingYieldSign(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is Shovel)
            {
                if (heldObject.gameObject.name.StartsWith("YieldSign"))
                {
                    return true;
                }
            }
            return false;

        }

        public static bool IsHoldingShotgun(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is ShotgunItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsHoldingKnife(PlayerControllerB playerController)
        {
            var heldObject = GetHeldObject(playerController);
            if (heldObject == null) return false;

            if (heldObject is KnifeItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string StringifyCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath)
        {
            return StringifyCauseOfDeath(causeOfDeath, Plugin.RANDOM);
        }

        public static string StringifyCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath, Random random)
        {
            var result = SelectCauseOfDeath(causeOfDeath);

            var shouldRandomize = result.Length > 1 && (causeOfDeath == null || !Plugin.Instance.PluginConfig.ShouldUseSeriousDeathMessages());

            if (shouldRandomize)
            {
                var index = random.Next(result.Length);
                return result[index];
            }
            else
            {
                return result[0];
            }
        }

        public static string[] SelectCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath)
        {
            if (causeOfDeath == null) return Plugin.Instance.LanguageHandler.GetValuesByTag(LanguageHandler.TAG_FUNNY_NOTES);

            // NOTE: First cause of death in the list should be the "serious" entry.

            if (AdvancedCauseOfDeath.IsCauseOfDeathRegistered(causeOfDeath))
            {
                return ((AdvancedCauseOfDeath)causeOfDeath).GetLanguageValues();
            }
            else
            {
                return Plugin.Instance.LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_UNKNOWN);
            }
        }
    }

    public struct AdvancedCauseOfDeath
    {
        public static readonly Dictionary<string, AdvancedCauseOfDeath> Registry = new Dictionary<string, AdvancedCauseOfDeath>();
        private static int NextId = 100;

        // Basic causes of death
        public static AdvancedCauseOfDeath Unknown = BuildFromExisting(LanguageHandler.TAG_DEATH_UNKNOWN, CauseOfDeath.Unknown);
        public static AdvancedCauseOfDeath Bludgeoning = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_BLUDGEONING, CauseOfDeath.Bludgeoning);
        public static AdvancedCauseOfDeath Gravity = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_GRAVITY, CauseOfDeath.Gravity);
        public static AdvancedCauseOfDeath Blast = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_BLAST, CauseOfDeath.Blast);
        public static AdvancedCauseOfDeath Strangulation = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_STRANGULATION, CauseOfDeath.Strangulation);
        public static AdvancedCauseOfDeath Suffocation = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_SUFFOCATION, CauseOfDeath.Suffocation);
        public static AdvancedCauseOfDeath Mauling = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_MAULING, CauseOfDeath.Mauling);
        public static AdvancedCauseOfDeath Gunshots = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_GUNSHOTS, CauseOfDeath.Gunshots);
        public static AdvancedCauseOfDeath Crushing = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_CRUSHING, CauseOfDeath.Crushing);
        public static AdvancedCauseOfDeath Drowning = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_DROWNING, CauseOfDeath.Drowning);
        public static AdvancedCauseOfDeath Abandoned = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_ABANDONED, CauseOfDeath.Abandoned);
        public static AdvancedCauseOfDeath Electrocution = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_ELECTROCUTION, CauseOfDeath.Electrocution);
        // New in v45
        public static AdvancedCauseOfDeath Kicking = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_KICKING, CauseOfDeath.Kicking); // This gets redirected to Enemy_Nutcracker_Kicked
        // New in v50
        public static AdvancedCauseOfDeath Burning = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_BURNING, CauseOfDeath.Burning);
        public static AdvancedCauseOfDeath Stabbing = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_STABBING, CauseOfDeath.Stabbing); // This gets redirected based on who stabbed
        public static AdvancedCauseOfDeath Fan = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_FAN, CauseOfDeath.Fan); // This one gets actually used
        public static AdvancedCauseOfDeath Inertia = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_INERTIA, CauseOfDeath.Inertia); // This gets redirected to one of the car ones
        public static AdvancedCauseOfDeath Snipped = BuildFromExisting(LanguageHandler.TAG_DEATH_GENERIC_SNIPPED, CauseOfDeath.Snipped); // This gets redirected to Enemy_Barber

        // Custom causes (enemies)
        public static AdvancedCauseOfDeath Enemy_BaboonHawk = Build(LanguageHandler.TAG_DEATH_ENEMY_BABOON_HAWK); // Also known as BaboonBird
        public static AdvancedCauseOfDeath Enemy_Bracken = Build(LanguageHandler.TAG_DEATH_ENEMY_BRACKEN); // Also known as Flowerman
        public static AdvancedCauseOfDeath Enemy_BunkerSpider = Build(LanguageHandler.TAG_DEATH_ENEMY_BUNKER_SPIDER);
        public static AdvancedCauseOfDeath Enemy_CircuitBees = Build(LanguageHandler.TAG_DEATH_ENEMY_CIRCUIT_BEES); // Also known as RedLocustBees
        public static AdvancedCauseOfDeath Enemy_CoilHead = Build(LanguageHandler.TAG_DEATH_ENEMY_COILHEAD); // Also known as SpringMan
        public static AdvancedCauseOfDeath Enemy_EarthLeviathan = Build(LanguageHandler.TAG_DEATH_ENEMY_EARTH_LEVIATHAN); // Also known as SandWorm
        public static AdvancedCauseOfDeath Enemy_EyelessDog = Build(LanguageHandler.TAG_DEATH_ENEMY_EYELESS_DOG); // Also known as MouthDog
        public static AdvancedCauseOfDeath Enemy_GhostGirl = Build(LanguageHandler.TAG_DEATH_ENEMY_GHOST_GIRL); // Also known as DressGirl
        public static AdvancedCauseOfDeath Enemy_HoarderBug = Build(LanguageHandler.TAG_DEATH_ENEMY_HOARDER_BUG);
        public static AdvancedCauseOfDeath Enemy_Hygrodere = Build(LanguageHandler.TAG_DEATH_ENEMY_HYGRODERE); // Also known as Blob
        public static AdvancedCauseOfDeath Enemy_Jester = Build(LanguageHandler.TAG_DEATH_ENEMY_JESTER);
        public static AdvancedCauseOfDeath Enemy_LassoMan = Build(LanguageHandler.TAG_DEATH_ENEMY_LASSO_MAN);
        public static AdvancedCauseOfDeath Enemy_SnareFlea = Build(LanguageHandler.TAG_DEATH_ENEMY_SNARE_FLEA); // Also known as Centipede
        public static AdvancedCauseOfDeath Enemy_SporeLizard = Build(LanguageHandler.TAG_DEATH_ENEMY_SPORE_LIZARD); // Also known as Puffer
        public static AdvancedCauseOfDeath Enemy_Thumper = Build(LanguageHandler.TAG_DEATH_ENEMY_THUMPER);
        
        public static AdvancedCauseOfDeath Enemy_ForestGiant_Eaten = Build(LanguageHandler.TAG_DEATH_ENEMY_FOREST_GIANT_EATEN);
        public static AdvancedCauseOfDeath Enemy_ForestGiant_Death = Build(LanguageHandler.TAG_DEATH_ENEMY_FOREST_GIANT_DEATH); // Crushed under the Forest Giant's body

        // Enemies from v45
        public static AdvancedCauseOfDeath Enemy_MaskedPlayer_Wear = Build(LanguageHandler.TAG_DEATH_ENEMY_MASKED_PLAYER_WEAR); // Comedy mask
        public static AdvancedCauseOfDeath Enemy_MaskedPlayer_Victim = Build(LanguageHandler.TAG_DEATH_ENEMY_MASKED_PLAYER_VICTIM); // Comedy mask
        public static AdvancedCauseOfDeath Enemy_Nutcracker_Kicked = Build(LanguageHandler.TAG_DEATH_ENEMY_NUTCRACKER_KICKED);
        public static AdvancedCauseOfDeath Enemy_Nutcracker_Shot = Build(LanguageHandler.TAG_DEATH_ENEMY_NUTCRACKER_SHOT);

        // Enemies from v50
        public static AdvancedCauseOfDeath Enemy_Butler_Stab = Build(LanguageHandler.TAG_DEATH_ENEMY_BUTLER_STAB);
        public static AdvancedCauseOfDeath Enemy_Butler_Explode = Build(LanguageHandler.TAG_DEATH_ENEMY_BUTLER_EXPLODE);
        public static AdvancedCauseOfDeath Enemy_MaskHornets = Build(LanguageHandler.TAG_DEATH_ENEMY_MASK_HORNETS); // Spawned by the Butler.
        public static AdvancedCauseOfDeath Enemy_TulipSnake_Drop = Build(LanguageHandler.TAG_DEATH_ENEMY_TULIP_SNAKE_DROP); // Upon dying from gravity. Also known as FlowerSnake
        public static AdvancedCauseOfDeath Enemy_Old_Bird_Rocket = Build(LanguageHandler.TAG_DEATH_ENEMY_OLD_BIRD_ROCKET); // Also known as RadMech
        public static AdvancedCauseOfDeath Enemy_Old_Bird_Charge = Build(LanguageHandler.TAG_DEATH_ENEMY_OLD_BIRD_CHARGE);
        public static AdvancedCauseOfDeath Enemy_Old_Bird_Stomp = Build(LanguageHandler.TAG_DEATH_ENEMY_OLD_BIRD_STOMP);
        public static AdvancedCauseOfDeath Enemy_Old_Bird_Torch = Build(LanguageHandler.TAG_DEATH_ENEMY_OLD_BIRD_TORCH);

        // Enemies from v55
        public static AdvancedCauseOfDeath Enemy_KidnapperFox = Build(LanguageHandler.TAG_DEATH_ENEMY_KIDNAPPER_FOX);
        public static AdvancedCauseOfDeath Enemy_Barber = Build(LanguageHandler.TAG_DEATH_ENEMY_BARBER);

        // Enemies from v60
        public static AdvancedCauseOfDeath Enemy_Maneater = Build(LanguageHandler.TAG_DEATH_ENEMY_MANEATER);

        // Custom causes (player)
        public static AdvancedCauseOfDeath Player_Jetpack_Gravity = Build(LanguageHandler.TAG_DEATH_PLAYER_JETPACK_GRAVITY); // I think this one just never triggers.
        public static AdvancedCauseOfDeath Player_Jetpack_Blast = Build(LanguageHandler.TAG_DEATH_PLAYER_JETPACK_BLAST);
        public static AdvancedCauseOfDeath Player_Quicksand = Build(LanguageHandler.TAG_DEATH_PLAYER_QUICKSAND);
        public static AdvancedCauseOfDeath Player_Ladder = Build(LanguageHandler.TAG_DEATH_PLAYER_LADDER);
        public static AdvancedCauseOfDeath Player_Murder_Shovel = Build(LanguageHandler.TAG_DEATH_PLAYER_MURDER_SHOVEL);
        public static AdvancedCauseOfDeath Player_Murder_Stop_Sign = Build(LanguageHandler.TAG_DEATH_PLAYER_MURDER_STOP_SIGN);
        public static AdvancedCauseOfDeath Player_Murder_Yield_Sign = Build(LanguageHandler.TAG_DEATH_PLAYER_MURDER_YIELD_SIGN);
        public static AdvancedCauseOfDeath Player_Murder_Shotgun = Build(LanguageHandler.TAG_DEATH_PLAYER_MURDER_SHOTGUN);
        public static AdvancedCauseOfDeath Player_Murder_Knife = Build(LanguageHandler.TAG_DEATH_PLAYER_MURDER_KNIFE);
        public static AdvancedCauseOfDeath Player_StunGrenade = Build(LanguageHandler.TAG_DEATH_PLAYER_STUN_GRENADE);
        public static AdvancedCauseOfDeath Player_EasterEgg = Build(LanguageHandler.TAG_DEATH_PLAYER_EASTER_EGG);

        public static AdvancedCauseOfDeath Player_Cruiser_Driver = Build(LanguageHandler.TAG_DEATH_PLAYER_CRUISER_DRIVER);
        public static AdvancedCauseOfDeath Player_Cruiser_Passenger = Build(LanguageHandler.TAG_DEATH_PLAYER_CRUISER_PASSENGER);
        public static AdvancedCauseOfDeath Player_Cruiser_Explode_Bystander = Build(LanguageHandler.TAG_DEATH_PLAYER_CRUISER_EXPLODE_BYSTANDER);
        public static AdvancedCauseOfDeath Player_Cruiser_Ran_Over = Build(LanguageHandler.TAG_DEATH_PLAYER_CRUISER_RAN_OVER);

        // Custom causes (facility pits)
        public static AdvancedCauseOfDeath Pit_Generic = Build(LanguageHandler.TAG_DEATH_PIT_GENERIC);
        public static AdvancedCauseOfDeath Pit_Facility_Pit = Build(LanguageHandler.TAG_DEATH_PIT_FACILITY_PIT);
        public static AdvancedCauseOfDeath Pit_Facility_Catwalk_Jump = Build(LanguageHandler.TAG_DEATH_PIT_FACILITY_CATWALK_JUMP);
        public static AdvancedCauseOfDeath Pit_Mine_Pit = Build(LanguageHandler.TAG_DEATH_PIT_MINE_PIT);
        public static AdvancedCauseOfDeath Pit_Mine_Cave = Build(LanguageHandler.TAG_DEATH_PIT_MINE_CAVE);
        public static AdvancedCauseOfDeath Pit_Mine_Elevator = Build(LanguageHandler.TAG_DEATH_PIT_MINE_ELEVATOR);

        // Custom causes (other)
        public static AdvancedCauseOfDeath Other_Landmine = Build(LanguageHandler.TAG_DEATH_OTHER_LANDMINE);
        public static AdvancedCauseOfDeath Other_Turret = Build(LanguageHandler.TAG_DEATH_OTHER_TURRET);
        public static AdvancedCauseOfDeath Other_Lightning = Build(LanguageHandler.TAG_DEATH_OTHER_LIGHTNING);
        public static AdvancedCauseOfDeath Other_Meteor = Build(LanguageHandler.TAG_DEATH_OTHER_METEOR);
        public static AdvancedCauseOfDeath Other_DepositItemsDesk = Build(LanguageHandler.TAG_DEATH_OTHER_DEPOSIT_ITEMS_DESK); // You never see this one since there's no report.
        public static AdvancedCauseOfDeath Other_Dropship = Build(LanguageHandler.TAG_DEATH_OTHER_ITEM_DROPSHIP);
        public static AdvancedCauseOfDeath Other_Spike_Trap = Build(LanguageHandler.TAG_DEATH_OTHER_SPIKE_TRAP);
        public static AdvancedCauseOfDeath Other_OutOfBounds = Build(LanguageHandler.TAG_DEATH_OTHER_OUT_OF_BOUNDS);

        public static AdvancedCauseOfDeath Build(string languageTag)
        {
            // This internal number might not be consistent between clients, but the Key should.
            int statusCode = NextId;
            string key = languageTag;
            NextId += 1;
            AdvancedCauseOfDeath result = new AdvancedCauseOfDeath()
            {
                statusCode = statusCode,
                languageTag = languageTag
            };
            Register(key, result);
            return result;
        }

        public static AdvancedCauseOfDeath BuildFromExisting(string languageTag, CauseOfDeath statusCode)
        {
            string key = languageTag;
            AdvancedCauseOfDeath result = new AdvancedCauseOfDeath()
            {
                statusCode = (int) statusCode,
                languageTag = languageTag
            };
            Register(key, result);
            return result;
        }

        private static void Register(string key, AdvancedCauseOfDeath value)
        {
            if (IsTagRegistered(key))
            {
                Plugin.Instance.PluginLogger.LogError($"Tried to register duplicate Cause of Death key ({key})!");
            }
            else if (IsCauseOfDeathRegistered(value))
            {
                Plugin.Instance.PluginLogger.LogError($"Tried to register Cause of Death twice ({value})!");
            }
            else
            {
                Registry.Add(key, value);
            }
        }

        public static AdvancedCauseOfDeath? Fetch(string? key)
        {
            if (key == null) return null;
            if (!Registry.ContainsKey(key)) return null;
            return Registry[key];
        }

        public static bool IsTagRegistered(string key)
        {
            return Registry.ContainsKey(key);
        }

        public static bool IsCauseOfDeathRegistered(AdvancedCauseOfDeath? value)
        {
            if (value == null) return false;
            return Registry.ContainsValue((AdvancedCauseOfDeath)value);
        }
        
        static AdvancedCauseOfDeath ConvertCauseOfDeath(CauseOfDeath causeOfDeath)
        {
            switch (causeOfDeath)
            {
                case CauseOfDeath.Unknown: return Unknown;
                case CauseOfDeath.Bludgeoning: return Bludgeoning;
                case CauseOfDeath.Gravity: return Gravity;
                case CauseOfDeath.Blast: return Blast;
                case CauseOfDeath.Strangulation: return Strangulation;
                case CauseOfDeath.Suffocation: return Suffocation;
                case CauseOfDeath.Mauling: return Mauling;
                case CauseOfDeath.Gunshots: return Gunshots;
                case CauseOfDeath.Crushing: return Crushing;
                case CauseOfDeath.Drowning: return Drowning;
                case CauseOfDeath.Abandoned: return Abandoned;
                case CauseOfDeath.Electrocution: return Electrocution;
                case CauseOfDeath.Burning: return Burning;
                case CauseOfDeath.Fan: return Fan;
                case CauseOfDeath.Stabbing: return Stabbing;
                default: return Unknown;
            }
        }

        //
        // Instance Fields
        // 

        private int statusCode;
        private string languageTag;

        public string GetLanguageTag()
        {
            return languageTag;
        }

        public string[] GetLanguageValues()
        {
            return Plugin.Instance.LanguageHandler.GetValuesByTag(languageTag);
        }

        public override bool Equals(object obj)
        {
            return obj is AdvancedCauseOfDeath code &&
                       statusCode == code.statusCode;
        }

        public override int GetHashCode() {
            return HashCode.Combine(statusCode, languageTag);
        }

        public static bool operator ==(AdvancedCauseOfDeath left, AdvancedCauseOfDeath right) => left.statusCode == right.statusCode;
        public static bool operator !=(AdvancedCauseOfDeath left, AdvancedCauseOfDeath right) => left.statusCode != right.statusCode;

        public static bool operator ==(CauseOfDeath left, AdvancedCauseOfDeath right) => (int) left == right.statusCode;
        public static bool operator !=(CauseOfDeath left, AdvancedCauseOfDeath right) => (int) left != right.statusCode;

        public static bool operator ==(AdvancedCauseOfDeath left, CauseOfDeath right) => left.statusCode == (int) right;
        public static bool operator !=(AdvancedCauseOfDeath left, CauseOfDeath right) => left.statusCode != (int) right;

        public static implicit operator AdvancedCauseOfDeath(CauseOfDeath value)
        {
            return ConvertCauseOfDeath(value);
        }

        public static implicit operator CauseOfDeath(AdvancedCauseOfDeath value)
        {
            return (CauseOfDeath) value.statusCode;
        }

        public override string ToString() {
            return $"AdvancedCauseOfDeath({languageTag})";
        }
    }
}