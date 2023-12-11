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
            "Secretly a lizard",
            "Believes in Santa Claus.",
            "Still uses a flip phone.",
            "Can't cook instant noodles right.",
            "Thinks the Earth is flat.",
            "Still afraid of the dark.",
            "Always the first in line for coffee.",
            "Lost a bet with a parrot.",
            "Can't remember their passwords.",
            "Secretly a superhero on weekends.",
            "Tried to high-five a mirror.",
            "Thinks pineapple belongs on pizza.",
            "Never finishes their... ",
            "Talks to plants.",
            "Has a pet rock named Steve.",
            "Wanted to be an astronaut.",
            "Sleeps with a night light.",
            "Once got stuck in a revolving door.",
            "Has a collection of rubber ducks.",
            "Dances when no one is watching.",
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
                        "Met the business end of a hammer.",
                        "Learned that gravity and rocks don't mix.",
                        "Discovered why pillows are safer than bricks.",
                        "Took a hard hit, literally.",
                        "Found out hammers aren't just for nails."
                    };
                case AdvancedCauseOfDeath.Gravity:
                    return new[] {
                        "Fell to their death.",
                        "Fell off a cliff.",
                        "Discovered flying isn't for everyone.",
                        "Took a dive, forgot to stop.",
                        "Learned that what goes up must come down.",
                        "Gravity: 1, Common Sense: 0."
                    };
                case AdvancedCauseOfDeath.Blast:
                    return new[] {
                        "Went out with a bang.",
                        "Exploded.",
                        "Was blown to smithereens.",
                        "Tried to juggle grenades.",
                        "Learned the hard way that red barrels explode.",
                        "Found out bombs are not toys."
                    };
                case AdvancedCauseOfDeath.Strangulation:
                    return new[] {
                        "Strangled to death.",
                        "Learned the hard way that neckties and machinery don't mix.",
                        "Discovered the downside of fashion scarves.",
                        "Played with a rope, it didn't end well.",
                        "Tried a new necktie, it was too tight."
                    };
                case AdvancedCauseOfDeath.Suffocation:
                    return new[] {
                        "Suffocated to death.",
                        "Forgot how to breathe.",
                        "Tried breathing in a vacuum.",
                        "Lost a game of hold-your-breath.",
                        "Found out air is important."
                    };
                case AdvancedCauseOfDeath.Mauling:
                    return new[] {
                        "Mauled to death.",
                        "Met a bear, it didn't go well.",
                        "Tried to pet a not-so-friendly animal.",
                        "Learned that wild animals aren't always cuddly.",
                        "Discovered that 'bear hug' can be literal."
                    };
                case AdvancedCauseOfDeath.Gunshots:
                    return new[] {
                        "Shot to death by a Turret.",
                        "Found out what a bullet feels like.",
                        "Learned guns aren't toys.",
                        "Played a deadly game of dodgeball.",
                        "Found out why it's called 'live' ammunition."
                    };
                case AdvancedCauseOfDeath.Crushing:
                    return new[] {
                        "Crushed to death.",
                        "Hugged too hard by a hydraulic press.",
                        "Found out what it feels like to be a pancake.",
                        "Learned that walls can be too close for comfort.",
                        "Discovered the weight of the world, literally."
                    };
                case AdvancedCauseOfDeath.Drowning:
                    return new[] {
                        "Drowned to death.",
                        "Tried to breathe underwater.",
                        "Forgot they weren't a fish.",
                        "Swimming lessons didn't pay off.",
                        "Discovered water is not air."
                    };
                case AdvancedCauseOfDeath.Abandoned:
                    return new[] {
                        "Abandoned by their coworkers.",
                        "Forgot to show up for the team meeting.",
                        "Learned the hard way that team work matters.",
                        "Left hanging, quite literally.",
                        "Discovered solitude isn't always peaceful."
                    };
                case AdvancedCauseOfDeath.Electrocution:
                    return new[] {
                        "Electrocuted to death.",
                        "Took a shocking turn of events.",
                        "Found out lightning does strike twice.",
                        "Learned why they say 'don't play with electricity'.",
                        "Discovered that power lines are not tightropes."
                    };
                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return new[] {
                        "Had their neck snapped by a Bracken.",
                        "Stared at a Bracken too long.",
                        "Tried to play hide and seek with a Bracken.",
                        "Thought a Bracken was a tree.",
                        "Gave a Bracken a funny look."
                    };
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return new[] {
                        "Got caught using a mechanical keyboard.",
                        "Was eaten by an Eyeless Dog.",
                        "Wasn't quiet around an Eyeless Dog.",
                        "Thought 'Eyeless' meant 'harmless'.",
                        "Tried to teach an Eyeless Dog new tricks."
                    };
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return new[] {
                        "Swallowed whole by a Forest Giant.",
                        "Tried to climb a Forest Giant.",
                        "Mistook a Forest Giant for a walking tree.",
                        "Played a game of tag with a Forest Giant.",
                        "Tried to take a selfie with a Forest Giant."
                    };
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return new[] {
                        "Electro-stung to death by Circuit Bees.",
                        "Tried to swat a Circuit Bee.",
                        "Thought Circuit Bees made digital honey.",
                        "Discovered Circuit Bees don't like water.",
                        "Tried to catch a Circuit Bee."
                    };
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return new[] {
                        "Died a mysterious death.",
                        "???",
                        "Played hide-and-seek with a Ghost Girl.",
                        "Tried to have a conversation with a Ghost Girl.",
                        "Thought a Ghost Girl was just a fog."
                    };
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return new[] {
                        "Swallowed whole by an Earth Leviathan.",
                        "Tried to pet an Earth Leviathan.",
                        "Mistook an Earth Leviathan for a mountain.",
                        "Got in the way of an Earth Leviathan.",
                        "Thought Earth Leviathans were friendly."
                    };

                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return new[] {
                        "Was eaten by a Baboon Hawk.",
                        "Was mauled by a Baboon Hawk.",
                        "Had a bad encounter with a Baboon Hawk.",
                        "Learned that Baboon Hawks aren't friendly.",
                        "Discovered that flying monkeys are scary."
                    };
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return new[] {
                        "Was the butt of the Jester's joke.",
                        "Got pranked by the Jester.",
                        "Got popped like a weasel.",
                        "Didn't find the Jester's joke funny.",
                        "Learned that laughter can be deadly."
                    };
                case AdvancedCauseOfDeath.Enemy_CoilHead:
                    return new[] {
                        "Got in a staring contest with a Coil Head.",
                        "Lost a staring contest with a Coil Head.",
                        "Tried to outstare a Coil Head.",
                        "Found out Coil Heads don't blink.",
                        "Discovered eyes can indeed be bigger than stomach."
                    };
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return new[] {
                        "Was suffocated a Snare Flea.",
                        "Got a hug from a Snare Flea.",
                        "Learned that fleas can be more than annoying.",
                        "Found out that some fleas are deadly.",
                        "Discovered not all fleas are tiny."
                    };
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return new[] {
                        "Was absorbed by a Hygrodere.",
                        "Got lost in the sauce.",
                        "Had an oopsie with a Hygrodere.",
                        "Learned that Hygroderes don't do handshakes.",
                        "Found out some hugs are too tight."
                    };
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return new[] {
                        "Was hoarded by a Hoarder Bug.",
                        "Found out what it's like to be a collectible.",
                        "Discovered that Hoarder Bugs don't share.",
                        "Learned that clutter can be deadly.",
                        "Got collected by a Hoarder Bug."
                    };
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return new[] {
                        "Was puffed by a Spore Lizard.",
                        "Met a Spore Lizard and it didn't go well.",
                        "Learned that spores aren't always harmless.",
                        "Found out that lizards can have deadly breath.",
                        "Discovered that not all lizards are cute."
                    };
                case AdvancedCauseOfDeath.Enemy_BunkerSpider:
                    return new[] {
                        "Ensnared in the Bunker Spider's web.",
                        "Met a spider, it was a big one.",
                        "Learned that some webs are stronger than others.",
                        "Found out that Bunker Spiders don't make good pets.",
                        "Discovered eight legs can be eight too many."
                    };

                case AdvancedCauseOfDeath.Jetpack_Gravity:
                    return new[] {
                        "Flew too close to the sun.",
                        "Ran out of fuel.",
                        "Learned that what goes up must come down.",
                        "Found out that flying isn't just flapping arms.",
                        "Discovered jetpacks have a 'down' button."
                    };
                case AdvancedCauseOfDeath.Jetpack_Blast:
                    return new[] {
                        "Turned into a firework.",
                        "Got blown up by bad piloting.",
                        "Decided to rocket to the moon, missed.",
                        "Learned that jetpacks are not toys.",
                        "Discovered the hard way that jetpacks are tricky."
                    };
                case AdvancedCauseOfDeath.Player_Murder:
                    return new[] {
                        "Was the victim of a murder.",
                        "Got murdered.",
                        "Found out that trust is overrated.",
                        "Learned not everyone plays nice.",
                        "Discovered betrayal hurts, literally."
                    };
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return new[] {
                        "Got stuck in quicksand.",
                        "Drowned in quicksand",
                        "Tried to make quicksand a friend.",
                        "Learned quicksand is not just in movies.",
                        "Found out standing still isn't always safe."
                    };
                case AdvancedCauseOfDeath.Player_DepositItemsDesk:
                    return new[] {
                        "Received a demotion.",
                        "Was put on disciplinary leave.",
                        "Found out the desk has a temper.",
                        "Learned paperwork can be deadly.",
                        "Discovered the desk is not always your friend."
                    };
                case AdvancedCauseOfDeath.Player_Dropship:
                    return new[] {
                        "Couldn't wait for their items.",
                        "Got too impatient for their items.",
                        "Learned patience is a virtue, the hard way.",
                        "Found out timing is everything.",
                        "Discovered that hurry can lead to worry."
                    };
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return new[] {
                        "Was the victim of a murder.",
                        "Found out what stars look like up close.",
                        "Learned that stun grenades are stunning.",
                        "Discovered that loud noises can be deadly.",
                        "Found out lights out means lights out."
                    };

                // case AdvancedCauseOfDeath.Unknown:
                default:
                    return new[] {
                        "Most sincerely dead.",
                        "Died somehow.",
                        "Passed away under mysterious circumstances.",
                        "Left this world in an unusual way.",
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