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
                Plugin.Instance.PluginLogger.LogInfo($"Player {playerController.playerClientId} has no custom cause of death stored! Using fallback...");
                return GuessCauseOfDeath(playerController);
            }
            else
            {
                Plugin.Instance.PluginLogger.LogInfo($"Player {playerController.playerClientId} has custom cause of death stored! {PlayerCauseOfDeath[(int)playerController.playerClientId]}");
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
                        return AdvancedCauseOfDeath.Jetpack_Gravity;
                    }
                    else if (playerController.causeOfDeath == CauseOfDeath.Blast)
                    {
                        return AdvancedCauseOfDeath.Jetpack_Blast;
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
            if (result.Length == 1) return result[0];
            else return result[random.Next(result.Length)];
        }

        public static string[] SelectCauseOfDeath(AdvancedCauseOfDeath? causeOfDeath)
        {
            if (causeOfDeath == null) return FUNNY_NOTES;

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
                        "Went out with a bang.",
                        "Exploded.",
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
                        "Shot to death by a Turret.",
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

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return new[] {
                        "Had their neck snapped by a Bracken.",
                        "Stared at a Bracken too long.",
                    };
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return new[] {
                        "Got caught using a mechanical keyboard.",
                        "Was eaten by an Eyeless Dog.",
                        "Was eaten by an Eyeless Dog.",
                        "Was eaten by an Eyeless Dog.",
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
                        "Was the butt of the Jester's joke.",
                        "Got pranked by the Jester.",
                        "Got popped like a weasel.",
                    };
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return new[] {
                        "Got in a staring contest with a Coil Head.",
                        "Lost a staring contest with a Coil Head.",
                    };
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return new[] {
                        "Was suffocated a Snare Flea.",
                    };
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return new[] {
                        "Was absorbed by a Hygrodere.",
                        "Got lost in the sauce.",
                    };
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return new[] {
                        "Was hoarded by a Hoarder Bug.",
                    };
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return new[] {
                        "Was puffed by a Spore Lizard.",
                    };
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return new[] {
                        "Ensnared in the Bunker Spider's web.",
                    };

                case AdvancedCauseOfDeath.Jetpack_Gravity:
                    return new[] {
                        "Flew too close to the sun.",
                        "Ran out of fuel.",
                    };
                case AdvancedCauseOfDeath.Jetpack_Blast:
                    return new[] {
                        "Turned into a firework.",
                        "Got blown up by bad piloting.",
                    };
                case AdvancedCauseOfDeath.Player_Murder:
                    return new[] {
                        "Was the victim of a murder.",
                        "Got murdered.",
                    };
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return new[] {
                        "Got stuck in quicksand.",
                        "Drowned in quicksand",
                    };
                case AdvancedCauseOfDeath.Player_DepositItemsDesk:
                    return new[] {
                        "Received a demotion.",
                        "Was put on disciplinary leave.",
                    };
                case AdvancedCauseOfDeath.Player_Dropship:
                    return new[] {
                        "Couldn't wait for their items.",
                        "Got too impatient for their items.",
                    };
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return new[] {
                        "Was the victim of a murder.",
                    };

                // case AdvancedCauseOfDeath.Unknown:
                default:
                    return new[] {
                        "Most sincerely dead.",
                        "Died somehow.",
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

        // Custom causes (other)
        Jetpack_Gravity,
        Jetpack_Blast,
        Player_Quicksand,
        Player_Murder,
        Player_DepositItemsDesk,
        Player_Dropship,
        Player_StunGrenade, // TODO: Implement this.
    }
}