using System;
using System.Collections.Generic;
using GameNetcodeStuff;

namespace Coroner
{
    
    class AdvancedDeathTracker
    {
        public const int PLAYER_CAUSE_OF_DEATH_DROPSHIP = 300;

        public static readonly string[] FUNNY_NOTES = {
            Strings.FunnyNote1,
            Strings.FunnyNote2,
            Strings.FunnyNote3,
            Strings.FunnyNote4,
            Strings.FunnyNote5,
            Strings.FunnyNote6,
            Strings.FunnyNote7,
            Strings.FunnyNote8,
            Strings.FunnyNote9,
            Strings.FunnyNote10,
            Strings.FunnyNote11,
            Strings.FunnyNote12,
            Strings.FunnyNote13,
            Strings.FunnyNote14,
            Strings.FunnyNote15,
            Strings.FunnyNote16,
            Strings.FunnyNote17,
            Strings.FunnyNote18,
            Strings.FunnyNote19,
            Strings.FunnyNote20
        };

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
            if (result.Length == 1 || Plugin.Instance.PluginConfig.ShouldUseSeriousDeathMessages()) return result[0];
            else return result[random.Next(result.Length)];
        }

        public static string[] SelectCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath)
        {
            if (causeOfDeath == null) return FUNNY_NOTES;

            // NOTE: First cause of death in the list should be the "serious" entry.

            switch (causeOfDeath)
            {
                case AdvancedCauseOfDeath.Bludgeoning:
                    return new[] {
                        Strings.BludgeoningDeath1
                    };
                case AdvancedCauseOfDeath.Gravity:
                    return new[] {
                        Strings.GravityDeath1,
                        Strings.GravityDeath2
                    };
                case AdvancedCauseOfDeath.Blast:
                    return new[] {
                        Strings.BlastDeath1,
                        Strings.BlastDeath2,
                        Strings.BlastDeath3
                    };
                case AdvancedCauseOfDeath.Strangulation:
                    return new[] {
                        Strings.StrangulationDeath1
                    };
                case AdvancedCauseOfDeath.Suffocation:
                    return new[] {
                        Strings.SuffocationDeath1
                    };
                case AdvancedCauseOfDeath.Mauling:
                    return new[] {
                        Strings.MaulingDeath1
                    };
                case AdvancedCauseOfDeath.Gunshots:
                    return new[] {
                        Strings.GunshotsDeath1,
                        Strings.GunshotsDeath2
                    };
                case AdvancedCauseOfDeath.Crushing:
                    return new[] {
                        Strings.CrushingDeath1
                    };
                case AdvancedCauseOfDeath.Drowning:
                    return new[] {
                        Strings.DrowningDeath1
                    };
                case AdvancedCauseOfDeath.Abandoned:
                    return new[] {
                        Strings.AbandonedDeath1
                    };
                case AdvancedCauseOfDeath.Electrocution:
                    return new[] {
                        Strings.ElectrocutionDeath1
                    };
                case AdvancedCauseOfDeath.Kicking:
                    return new[] {
                        Strings.KickingDeath1
                    };

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return new[] {
                        Strings.Enemy_BrackenDeath1,
                        Strings.Enemy_BrackenDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return new[] {
                        Strings.Enemy_EyelessDogDeath1,
                        Strings.Enemy_EyelessDogDeath2,
                        Strings.Enemy_EyelessDogDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return new[] {
                        Strings.Enemy_ForestGiantDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return new[] {
                        Strings.Enemy_CircuitBeesDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return new[] {
                        Strings.Enemy_GhostGirlDeath1,
                        Strings.Enemy_GhostGirlDeath2,
                        Strings.Enemy_GhostGirlDeath3,
                        Strings.Enemy_GhostGirlDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return new[] {
                        Strings.Enemy_EarthLeviathanDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return new[] {
                        Strings.Enemy_BaboonHawkDeath1,
                        Strings.Enemy_BaboonHawkDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return new[] {
                        Strings.Enemy_JesterDeath1,
                        Strings.Enemy_JesterDeath2,
                        Strings.Enemy_JesterDeath3,
                        Strings.Enemy_JesterDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return new[] {
                        Strings.Enemy_CoilHeadDeath1,
                        Strings.Enemy_CoilHeadDeath2,
                        Strings.Enemy_CoilHeadDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return new[] {
                        Strings.Enemy_SnareFleaDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return new[] {
                        Strings.Enemy_HygrodereDeath1,
                        Strings.Enemy_HygrodereDeath2,
                        Strings.Enemy_HygrodereDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return new[] {
                        Strings.Enemy_HoarderBugDeath1,
                        Strings.Enemy_HoarderBugDeath2,
                        Strings.Enemy_HoarderBugDeath3,
                        Strings.Enemy_HoarderBugDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return new[] {
                        Strings.Enemy_SporeLizardDeath1,
                        Strings.Enemy_SporeLizardDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return new[] {
                        Strings.Enemy_BunkerSpiderDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_Thumper:
                    return new[] {
                        Strings.Enemy_ThumperDeath1,
                        Strings.Enemy_ThumperDeath2
                    };

                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear:
                    return new[] {
                        Strings.Enemy_MaskedPlayer_WearDeath1,
                        Strings.Enemy_MaskedPlayer_WearDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim:
                    return new[] {
                        Strings.Enemy_MaskedPlayer_VictimDeath1,
                        Strings.Enemy_MaskedPlayer_VictimDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked:
                    return new[] {
                        Strings.Enemy_Nutcracker_KickedDeath1,
                        Strings.Enemy_Nutcracker_KickedDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Shot:
                    return new[] {
                        Strings.Enemy_Nutcracker_ShotDeath1,
                        Strings.Enemy_Nutcracker_ShotDeath2
                    };

                case AdvancedCauseOfDeath.Player_Jetpack_Gravity:
                    return new[] {
                        Strings.Player_Jetpack_GravityDeath1,
                        Strings.Player_Jetpack_GravityDeath2,
                        Strings.Player_Jetpack_GravityDeath3
                    };
                case AdvancedCauseOfDeath.Player_Jetpack_Blast:
                    return new[] {
                        Strings.Player_Jetpack_BlastDeath1,
                        Strings.Player_Jetpack_BlastDeath2
                    };
                case AdvancedCauseOfDeath.Player_Murder_Melee:
                    return new[] {
                        Strings.Player_Murder_MeleeDeath1,
                        Strings.Player_Murder_MeleeDeath2,
                        Strings.Player_Murder_MeleeDeath3,
                        Strings.Player_Murder_MeleeDeath4
                    };
                case AdvancedCauseOfDeath.Player_Murder_Shotgun:
                    return new[] {
                        Strings.Player_Murder_ShotgunDeath1,
                        Strings.Player_Murder_ShotgunDeath2,
                        Strings.Player_Murder_ShotgunDeath3,
                        Strings.Player_Murder_ShotgunDeath4,
                        Strings.Player_Murder_ShotgunDeath5
                    };
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return new[] {
                        Strings.Player_QuicksandDeath1,
                        Strings.Player_QuicksandDeath2
                    };
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return new[] {
                        Strings.Player_StunGrenadeDeath1,
                        Strings.Player_StunGrenadeDeath2
                    };

                case AdvancedCauseOfDeath.Other_DepositItemsDesk:
                    // NOTE: Since there's no performance report on Gordion this never shows.
                    return new[] {
                        Strings.Other_DepositItemsDeskDeath1,
                        Strings.Other_DepositItemsDeskDeath2
                    };
                case AdvancedCauseOfDeath.Other_Dropship:
                    return new[] {
                        Strings.Other_DropshipDeath1,
                        Strings.Other_DropshipDeath2,
                        Strings.Other_DropshipDeath3
                    };
                case AdvancedCauseOfDeath.Other_Landmine:
                    return new [] {
                        Strings.Other_LandmineDeath1
                    };
                case AdvancedCauseOfDeath.Other_Turret:
                    return new [] {
                        Strings.Other_TurretDeath1
                    };
                case AdvancedCauseOfDeath.Other_Lightning:
                    return new [] {
                        Strings.Other_LightningDeath1
                    };

                default:
                    return new[] {
                        Strings.UnknownDeath1,
                        Strings.UnknownDeath2,
                        Strings.UnknownDeath3
                    };
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

        Other_Landmine,
        Other_Turret,
        Other_Lightning,
        Other_DepositItemsDesk,
        Other_Dropship,
    }
}