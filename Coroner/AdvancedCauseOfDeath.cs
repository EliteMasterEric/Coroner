using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using GameNetcodeStuff;

namespace Coroner
{
    
    class AdvancedDeathTracker
    {
        public const int PLAYER_CAUSE_OF_DEATH_DROPSHIP = 300;

        public static readonly string[] FUNNY_NOTES = {
            LangStrings.FunnyNote1,
            LangStrings.FunnyNote2,
            LangStrings.FunnyNote3,
            LangStrings.FunnyNote4,
            LangStrings.FunnyNote5,
            LangStrings.FunnyNote6,
            LangStrings.FunnyNote7,
            LangStrings.FunnyNote8,
            LangStrings.FunnyNote9,
            LangStrings.FunnyNote10,
            LangStrings.FunnyNote11,
            LangStrings.FunnyNote12,
            LangStrings.FunnyNote13,
            LangStrings.FunnyNote14,
            LangStrings.FunnyNote15,
            LangStrings.FunnyNote16,
            LangStrings.FunnyNote17,
            LangStrings.FunnyNote18,
            LangStrings.FunnyNote19,
            LangStrings.FunnyNote20
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
                        LangStrings.BludgeoningDeath1
                    };
                case AdvancedCauseOfDeath.Gravity:
                    return new[] {
                        LangStrings.GravityDeath1,
                        LangStrings.GravityDeath2
                    };
                case AdvancedCauseOfDeath.Blast:
                    return new[] {
                        LangStrings.BlastDeath1,
                        LangStrings.BlastDeath2,
                        LangStrings.BlastDeath3
                    };
                case AdvancedCauseOfDeath.Strangulation:
                    return new[] {
                        LangStrings.StrangulationDeath1
                    };
                case AdvancedCauseOfDeath.Suffocation:
                    return new[] {
                        LangStrings.SuffocationDeath1
                    };
                case AdvancedCauseOfDeath.Mauling:
                    return new[] {
                        LangStrings.MaulingDeath1
                    };
                case AdvancedCauseOfDeath.Gunshots:
                    return new[] {
                        LangStrings.GunshotsDeath1,
                        LangStrings.GunshotsDeath2
                    };
                case AdvancedCauseOfDeath.Crushing:
                    return new[] {
                        LangStrings.CrushingDeath1
                    };
                case AdvancedCauseOfDeath.Drowning:
                    return new[] {
                        LangStrings.DrowningDeath1
                    };
                case AdvancedCauseOfDeath.Abandoned:
                    return new[] {
                        LangStrings.AbandonedDeath1
                    };
                case AdvancedCauseOfDeath.Electrocution:
                    return new[] {
                        LangStrings.ElectrocutionDeath1
                    };
                case AdvancedCauseOfDeath.Kicking:
                    return new[] {
                        LangStrings.KickingDeath1
                    };

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return new[] {
                        LangStrings.Enemy_BrackenDeath1,
                        LangStrings.Enemy_BrackenDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return new[] {
                        LangStrings.Enemy_EyelessDogDeath1,
                        LangStrings.Enemy_EyelessDogDeath2,
                        LangStrings.Enemy_EyelessDogDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return new[] {
                        LangStrings.Enemy_ForestGiantDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return new[] {
                        LangStrings.Enemy_CircuitBeesDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return new[] {
                        LangStrings.Enemy_GhostGirlDeath1,
                        LangStrings.Enemy_GhostGirlDeath2,
                        LangStrings.Enemy_GhostGirlDeath3,
                        LangStrings.Enemy_GhostGirlDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return new[] {
                        LangStrings.Enemy_EarthLeviathanDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return new[] {
                        LangStrings.Enemy_BaboonHawkDeath1,
                        LangStrings.Enemy_BaboonHawkDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return new[] {
                        LangStrings.Enemy_JesterDeath1,
                        LangStrings.Enemy_JesterDeath2,
                        LangStrings.Enemy_JesterDeath3,
                        LangStrings.Enemy_JesterDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return new[] {
                        LangStrings.Enemy_CoilHeadDeath1,
                        LangStrings.Enemy_CoilHeadDeath2,
                        LangStrings.Enemy_CoilHeadDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return new[] {
                        LangStrings.Enemy_SnareFleaDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return new[] {
                        LangStrings.Enemy_HygrodereDeath1,
                        LangStrings.Enemy_HygrodereDeath2,
                        LangStrings.Enemy_HygrodereDeath3
                    };
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return new[] {
                        LangStrings.Enemy_HoarderBugDeath1,
                        LangStrings.Enemy_HoarderBugDeath2,
                        LangStrings.Enemy_HoarderBugDeath3,
                        LangStrings.Enemy_HoarderBugDeath4
                    };
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return new[] {
                        LangStrings.Enemy_SporeLizardDeath1,
                        LangStrings.Enemy_SporeLizardDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return new[] {
                        LangStrings.Enemy_BunkerSpiderDeath1
                    };
                case AdvancedCauseOfDeath.Enemy_Thumper:
                    return new[] {
                        LangStrings.Enemy_ThumperDeath1,
                        LangStrings.Enemy_ThumperDeath2
                    };

                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear:
                    return new[] {
                        LangStrings.Enemy_MaskedPlayer_WearDeath1,
                        LangStrings.Enemy_MaskedPlayer_WearDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim:
                    return new[] {
                        LangStrings.Enemy_MaskedPlayer_VictimDeath1,
                        LangStrings.Enemy_MaskedPlayer_VictimDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked:
                    return new[] {
                        LangStrings.Enemy_Nutcracker_KickedDeath1,
                        LangStrings.Enemy_Nutcracker_KickedDeath2
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Shot:
                    return new[] {
                        LangStrings.Enemy_Nutcracker_ShotDeath1,
                        LangStrings.Enemy_Nutcracker_ShotDeath2
                    };

                case AdvancedCauseOfDeath.Player_Jetpack_Gravity:
                    return new[] {
                        LangStrings.Player_Jetpack_GravityDeath1,
                        LangStrings.Player_Jetpack_GravityDeath2,
                        LangStrings.Player_Jetpack_GravityDeath3
                    };
                case AdvancedCauseOfDeath.Player_Jetpack_Blast:
                    return new[] {
                        LangStrings.Player_Jetpack_BlastDeath1,
                        LangStrings.Player_Jetpack_BlastDeath2
                    };
                case AdvancedCauseOfDeath.Player_Murder_Melee:
                    return new[] {
                        LangStrings.Player_Murder_MeleeDeath1,
                        LangStrings.Player_Murder_MeleeDeath2,
                        LangStrings.Player_Murder_MeleeDeath3,
                        LangStrings.Player_Murder_MeleeDeath4
                    };
                case AdvancedCauseOfDeath.Player_Murder_Shotgun:
                    return new[] {
                        LangStrings.Player_Murder_ShotgunDeath1,
                        LangStrings.Player_Murder_ShotgunDeath2,
                        LangStrings.Player_Murder_ShotgunDeath3,
                        LangStrings.Player_Murder_ShotgunDeath4,
                        LangStrings.Player_Murder_ShotgunDeath5
                    };
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return new[] {
                        LangStrings.Player_QuicksandDeath1,
                        LangStrings.Player_QuicksandDeath2
                    };
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return new[] {
                        LangStrings.Player_StunGrenadeDeath1,
                        LangStrings.Player_StunGrenadeDeath2
                    };

                case AdvancedCauseOfDeath.Other_DepositItemsDesk:
                    // NOTE: Since there's no performance report on Gordion this never shows.
                    return new[] {
                        LangStrings.Other_DepositItemsDeskDeath1,
                        LangStrings.Other_DepositItemsDeskDeath2
                    };
                case AdvancedCauseOfDeath.Other_Dropship:
                    return new[] {
                        LangStrings.Other_DropshipDeath1,
                        LangStrings.Other_DropshipDeath2,
                        LangStrings.Other_DropshipDeath3
                    };
                case AdvancedCauseOfDeath.Other_Landmine:
                    return new [] {
                        LangStrings.Other_LandmineDeath1
                    };
                case AdvancedCauseOfDeath.Other_Turret:
                    return new [] {
                        LangStrings.Other_TurretDeath1
                    };
                case AdvancedCauseOfDeath.Other_Lightning:
                    return new [] {
                        LangStrings.Other_LightningDeath1
                    };

                default:
                    return new[] {
                        LangStrings.UnknownDeath1,
                        LangStrings.UnknownDeath2,
                        LangStrings.UnknownDeath3
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