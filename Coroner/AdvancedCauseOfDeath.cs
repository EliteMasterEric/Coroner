using System;
using System.Collections.Generic;
using GameNetcodeStuff;

namespace Coroner
{
    class AdvancedDeathTracker
    {
        public const int PLAYER_CAUSE_OF_DEATH_DROPSHIP = 300;

        public static readonly string[] FUNNY_NOTES = {
            "The goofiest goober.",
            "The cutest employee.",
            "Had the most fun.",
            "Had the least fun.",
            "The bravest employee.",
            "Did a sick flip.",
            "Stubbed their toe.",
            "The most likely to die next time.",
            "The least likely to die next time.",
            "Dislikes smoke.",
            "A team player.",
            "A real go-getter.",
            "Ate the most snacks.",
            "Passed GO and collected $200.",
            "Got freaky on a Friday night.",
            "I think this one's a serial killer.",
            "Perfectly unremarkable.",
            "Hasn't called their mother in a while.",
            "Has IP address 127.0.0.1.",
            "Secretly a lizard"
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
                        "Bludgeoned to death.",
                    };
                case AdvancedCauseOfDeath.Gravity:
                    return new[] {
                        "Fell to their death.",
                        "Fell off a cliff.",
                    };
                case AdvancedCauseOfDeath.Blast:
                    return new[] {
                        "Exploded.",
                        "Went out with a bang.",
                        "Was blown to smithereens."
                    };
                case AdvancedCauseOfDeath.Strangulation:
                    return new[] {
                        "Strangled to death.",
                    };
                case AdvancedCauseOfDeath.Suffocation:
                    return new[] {
                        "Suffocated to death.",
                    };
                case AdvancedCauseOfDeath.Mauling:
                    return new[] {
                        "Mauled to death.",
                    };
                case AdvancedCauseOfDeath.Gunshots:
                    return new[] {
                        "Shot to death.",
                        "Filled to the brim with bullets.",
                    };
                case AdvancedCauseOfDeath.Crushing:
                    return new[] {
                        "Crushed to death.",
                    };
                case AdvancedCauseOfDeath.Drowning:
                    return new[] {
                        "Drowned to death.",
                    };
                case AdvancedCauseOfDeath.Abandoned:
                    return new[] {
                        "Abandoned by their coworkers.",
                    };
                case AdvancedCauseOfDeath.Electrocution:
                    return new[] {
                        "Electrocuted to death.",
                    };
                case AdvancedCauseOfDeath.Kicking:
                    return new[] {
                        "Kicked to death.",
                    };

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return new[] {
                        "Had their neck snapped by a Bracken.",
                        "Stared at a Bracken too long.",
                    };
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return new[] {
                        "Was eaten by an Eyeless Dog.",
                        "Got caught using a mechanical keyboard.",
                        "Wasn't quiet around an Eyeless Dog.",
                    };
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return new[] {
                        "Swallowed whole by a Forest Giant.",
                    };
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return new[] {
                        "Electro-stung to death by Circuit Bees.",
                    };
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return new[] {
                        "Died a mysterious death.",
                        "Lost their mind.",
                        "Got a real bad headache.",
                        "???",
                    };
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return new[] {
                        "Swallowed whole by an Earth Leviathan.",
                    };
                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return new[] {
                        "Was eaten by a Baboon Hawk.",
                        "Was mauled by a Baboon Hawk.",
                    };
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return new[] {
                        "Mauled to death by a Jester.",
                        "Was the butt of the Jester's joke.",
                        "Got pranked by the Jester.",
                        "Got popped like a weasel.",
                    };
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return new[] {
                        "Mauled to death by a Coil Head.",
                        "Got in a staring contest with a Coil Head.",
                        "Lost a staring contest with a Coil Head.",
                    };
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return new[] {
                        "Was suffocated by a Snare Flea.",
                    };
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return new[] {
                        "Was absorbed by a Hygrodere.",
                        "Got lost in the sauce.",
                        "Had an oopsie with a Hygrodere.",
                    };
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return new[] {
                        "Was mauled by a Hoarder Bug.",
                        "Was swarmed by a Hoarder Bug.",
                        "Was hoarded by a Hoarder Bug.",
                        "Tried to steal from a Hoarder Bug.",
                    };
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return new[] {
                        "Was bitten by a Spore Lizard.",
                        "Was puffed by a Spore Lizard.",
                    };
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return new[] {
                        "Ensnared in the Bunker Spider's web.",
                    };
                case AdvancedCauseOfDeath.Enemy_Thumper:
                    return new[] {
                        "Was ravaged by a Thumper.",
                        "Got thumped by a Thumper.",
                    };

                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Wear:
                    return new[] {
                        "Donned the Mask.",
                        "Nobody cared who they were until they put on the Mask.",
                    };
                case AdvancedCauseOfDeath.Enemy_MaskedPlayer_Victim:
                    return new[] {
                        "Was killed by a Masked coworker.",
                        "Became a tragedy at the hands of the Mask.",
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Kicked:
                    return new[] {
                        "Was kicked to death by a Nutcracker.",
                        "Got their nuts cracked by a Nutcracker.",
                    };
                case AdvancedCauseOfDeath.Enemy_Nutcracker_Shot:
                    return new[] {
                        "Got shot by a Nutcracker.",
                        "Was at the wrong end of a 21-gun salute.",
                    };

                case AdvancedCauseOfDeath.Player_Jetpack_Gravity:
                    return new[] {
                        "Fell while using a jetpack.",
                        "Flew too close to the sun.",
                        "Ran out of fuel.",
                    };
                case AdvancedCauseOfDeath.Player_Jetpack_Blast:
                    return new[] {
                        "Blew up while using a Jetpack.",
                        "Turned into a firework.",
                    };
                case AdvancedCauseOfDeath.Player_Murder_Melee:
                    return new[] {
                        "Was bludgeoned to death by a coworker.",
                        "Was the victim of a murder.",
                        "Got murdered.",
                        "Got backstabbed by a coworker."
                    };
                case AdvancedCauseOfDeath.Player_Murder_Shotgun:
                    return new[] {
                        "Was shot to death by a coworker.",
                        "Was the victim of a murder.",
                        "Got murdered.",
                        "Got one-pumped by a coworker.",
                        "Got 360-noscoped by a coworker.",
                    };
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return new[] {
                        "Got stuck in quicksand.",
                        "Drowned in quicksand",
                    };
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return new[] {
                        "Got flashbanged by a coworker.",
                        "Was the victim of a murder.",
                    };

                case AdvancedCauseOfDeath.Other_DepositItemsDesk:
                    // NOTE: Since there's no performance report on Gordion this never shows.
                    return new[] {
                        "Received a demotion.",
                        "Was put on disciplinary leave.",
                    };
                case AdvancedCauseOfDeath.Other_Dropship:
                    return new[] {
                        "Was crushed by the Item Dropship.",
                        "Couldn't wait for their items.",
                        "Got too impatient for their items.",
                    };
                case AdvancedCauseOfDeath.Other_Landmine:
                    return new [] {
                        "Stepped on a landmine."
                    };
                case AdvancedCauseOfDeath.Other_Turret:
                    return new [] {
                        "Got shot by a turret."
                    };
                case AdvancedCauseOfDeath.Other_Lightning:
                    return new [] {
                        "Was struck by lightning."
                    };

                default:
                    return new[] {
                        "Died somehow.",
                        "Most sincerely dead.",
                        "Expired in an inexplicable manner."
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