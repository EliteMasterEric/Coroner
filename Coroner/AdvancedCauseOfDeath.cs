using System;
using System.Collections.Generic;

using GameNetcodeStuff;

namespace Coroner
{

    class AdvancedDeathTracker
    {
        public const int PLAYER_CAUSE_OF_DEATH_DROPSHIP = 300;

        private static readonly Dictionary<int, AdvancedCauseOfDeath> PlayerCauseOfDeath = new Dictionary<int, AdvancedCauseOfDeath>();
        private static readonly Dictionary<int, string> PlayerNotes = new Dictionary<int, string>();

        public static void ClearDeathTracker()
        {
            PlayerCauseOfDeath.Clear();
            PlayerNotes.Clear();
        }

        public static void SetCauseOfDeath(int playerIndex, AdvancedCauseOfDeath causeOfDeath, bool broadcast = true)
        {
            PlayerCauseOfDeath[playerIndex] = causeOfDeath;
            if (broadcast) DeathBroadcaster.BroadcastCauseOfDeath(playerIndex, causeOfDeath);
        }

        public static void SetCauseOfDeath(int playerIndex, CauseOfDeath causeOfDeath, bool broadcast = true)
        {
            SetCauseOfDeath(playerIndex, ConvertCauseOfDeath(causeOfDeath), broadcast);
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, CauseOfDeath causeOfDeath, bool broadcast = true)
        {
            SetCauseOfDeath((int)playerController.playerClientId, ConvertCauseOfDeath(causeOfDeath), broadcast);
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, AdvancedCauseOfDeath causeOfDeath, bool broadcast = true)
        {
            SetCauseOfDeath((int)playerController.playerClientId, causeOfDeath, broadcast);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(int playerIndex)
        {
            PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[playerIndex];

            return GetCauseOfDeath(playerController);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(PlayerControllerB playerController)
        {
            if (!PlayerCauseOfDeath.ContainsKey((int)playerController.playerClientId))
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player {playerController.playerClientId} has no custom cause of death stored! Using fallback...");
                return GuessCauseOfDeath(playerController);
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug($"Player {playerController.playerClientId} has custom cause of death stored! {PlayerCauseOfDeath[(int)playerController.playerClientId]}");
                return PlayerCauseOfDeath[(int)playerController.playerClientId];
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

                return ConvertCauseOfDeath(playerController.causeOfDeath);
            }
            else
            {
                return AdvancedCauseOfDeath.Unknown;
            }
        }

        public static bool IsHoldingJetpack(PlayerControllerB playerController)
        {
            var heldObjectServer = playerController.currentlyHeldObjectServer;
            if (heldObjectServer == null) return false;
            var heldObjectGameObject = heldObjectServer.gameObject;
            if (heldObjectGameObject == null) return false;
            var heldObject = heldObjectGameObject.GetComponent<GrabbableObject>();
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

        public static AdvancedCauseOfDeath ConvertCauseOfDeath(CauseOfDeath causeOfDeath)
        {
            switch (causeOfDeath)
            {
                case CauseOfDeath.Unknown:
                    return AdvancedCauseOfDeath.Unknown;
                case CauseOfDeath.Bludgeoning:
                    return AdvancedCauseOfDeath.Bludgeoning;
                case CauseOfDeath.Gravity:
                    return AdvancedCauseOfDeath.Gravity;
                case CauseOfDeath.Blast:
                    return AdvancedCauseOfDeath.Blast;
                case CauseOfDeath.Strangulation:
                    return AdvancedCauseOfDeath.Strangulation;
                case CauseOfDeath.Suffocation:
                    return AdvancedCauseOfDeath.Suffocation;
                case CauseOfDeath.Mauling:
                    return AdvancedCauseOfDeath.Mauling;
                case CauseOfDeath.Gunshots:
                    return AdvancedCauseOfDeath.Gunshots;
                case CauseOfDeath.Crushing:
                    return AdvancedCauseOfDeath.Crushing;
                case CauseOfDeath.Drowning:
                    return AdvancedCauseOfDeath.Drowning;
                case CauseOfDeath.Abandoned:
                    return AdvancedCauseOfDeath.Abandoned;
                case CauseOfDeath.Electrocution:
                    return AdvancedCauseOfDeath.Electrocution;
                default:
                    return AdvancedCauseOfDeath.Unknown;
            }
        }

        public static string StringifyCauseOfDeath(CauseOfDeath causeOfDeath)
        {
            return StringifyCauseOfDeath(ConvertCauseOfDeath(causeOfDeath), Plugin.RANDOM);
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
                return result[random.Next(result.Length)];
            }
            else
            {
                return result[0];
            }
        }

        public static string[] SelectCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath)
        {
            if (causeOfDeath == null) return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_FUNNY_NOTES);

            // NOTE: First cause of death in the list should be the "serious" entry.
            
            switch (causeOfDeath)
            {
                case AdvancedCauseOfDeath.Bludgeoning:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_BLUDGEONING);
                case AdvancedCauseOfDeath.Gravity:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_GRAVITY);
                case AdvancedCauseOfDeath.Blast:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_BLAST);
                case AdvancedCauseOfDeath.Strangulation:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_STRANGULATION);
                case AdvancedCauseOfDeath.Suffocation:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_SUFFOCATION);
                case AdvancedCauseOfDeath.Mauling:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_MAULING);
                case AdvancedCauseOfDeath.Gunshots:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_GUNSHOTS);
                case AdvancedCauseOfDeath.Crushing:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_CRUSHING);
                case AdvancedCauseOfDeath.Drowning:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_DROWNING);
                case AdvancedCauseOfDeath.Abandoned:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_ABANDONED);
                case AdvancedCauseOfDeath.Electrocution:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_ELECTROCUTION);
                case AdvancedCauseOfDeath.Kicking:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_GENERIC_KICKING);

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_BRACKEN);
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_EYELESS_DOG);
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_FOREST_GIANT);
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_CIRCUIT_BEES);
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_GHOST_GIRL);
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_EARTH_LEVIATHAN);
                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_BABOON_HAWK);
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_JESTER);
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_COILHEAD);
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_SNARE_FLEA);
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_HYGRODERE);
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_HOARDER_BUG);
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_SPORE_LIZARD);
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_BUNKER_SPIDER);
                case AdvancedCauseOfDeath.Enemy_Thumper:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_THUMPER);

                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_MASKED_PLAYER_WEAR);
                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_MASKED_PLAYER_VICTIM);
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_NUTCRACKER_KICKED);
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Shot:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_ENEMY_NUTCRACKER_SHOT);

                case AdvancedCauseOfDeath.Player_Jetpack_Gravity:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_JETPACK_GRAVITY);
                case AdvancedCauseOfDeath.Player_Jetpack_Blast:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_JETPACK_BLAST);
                case AdvancedCauseOfDeath.Player_Murder_Melee:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_MURDER_MELEE);
                case AdvancedCauseOfDeath.Player_Murder_Shotgun:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_MURDER_SHOTGUN);
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_QUICKSAND);
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_PLAYER_STUN_GRENADE);
                    

                case AdvancedCauseOfDeath.Other_DepositItemsDesk:
                    // NOTE: Since there's no performance report on Gordion this never shows.
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_OTHER_DEPOSIT_ITEMS_DESK);
                case AdvancedCauseOfDeath.Other_Dropship:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_OTHER_ITEM_DROPSHIP);
                case AdvancedCauseOfDeath.Other_Landmine:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_OTHER_LANDMINE);
                case AdvancedCauseOfDeath.Other_Turret:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_OTHER_TURRET);
                case AdvancedCauseOfDeath.Other_Lightning:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_OTHER_LIGHTNING);

                default:
                    return LanguageHandler.GetValuesByTag(LanguageHandler.TAG_DEATH_UNKNOWN);
            }
        }

        internal static void SetCauseOfDeath(PlayerControllerB playerControllerB, object enemy_BaboonHawk)
        {
            throw new NotImplementedException();
        }
    }

    enum AdvancedCauseOfDeath
    {
        // Basic causes of death
        Unknown,
        Bludgeoning,
        Gravity,
        Blast,
        Strangulation,
        Suffocation,
        Mauling,
        Gunshots,
        Crushing,
        Drowning,
        Abandoned,
        Electrocution,
        Kicking, // New in v45

        // Custom causes (enemies)
        Enemy_BaboonHawk, // Also known as BaboonBird
        Enemy_Bracken, // Also known as Flowerman
        Enemy_CircuitBees, // Also known as RedLocustBees
        Enemy_CoilHead,  // Also known as SpringMan
        Enemy_EarthLeviathan, // Also known as SandWorm
        Enemy_EyelessDog, // Also known as MouthDog
        Enemy_ForestGiant,
        Enemy_GhostGirl, // Also known as DressGirl
        Enemy_Hygrodere, // Also known as Blob
        Enemy_Jester,
        Enemy_SnareFlea, // Also known as Centipede
        Enemy_SporeLizard, // Also known as Puffer
        Enemy_HoarderBug,
        Enemy_Thumper,
        Enemy_BunkerSpider,

        // Enemies from v45
        Enemy_MaskedPlayer_Wear, // Comedy mask
        Enemy_MaskedPlayer_Victim, // Comedy mask
        Enemy_Nutcracker_Kicked,
        Enemy_Nutcracker_Shot,

        // Custom causes (player)
        Player_Jetpack_Gravity,
        Player_Jetpack_Blast,
        Player_Quicksand,
        Player_Murder_Melee,
        Player_Murder_Shotgun,
        Player_StunGrenade, // TODO: Implement this.

        // Custom causes (other)
        Other_Landmine,
        Other_Turret,
        Other_Lightning,
        Other_DepositItemsDesk,
        Other_Dropship,
    }
}