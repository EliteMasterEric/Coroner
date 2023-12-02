using System;
using System.Collections.Generic;
using GameNetcodeStuff;

namespace Coroner {
    class AdvancedDeathTracker {
        public const int PLAYER_CAUSE_OF_DEATH_DROPSHIP = 300;

        private static readonly Dictionary<int, AdvancedCauseOfDeath> PlayerCauseOfDeath = new Dictionary<int, AdvancedCauseOfDeath>();

        public static void ClearDeathTracker() {
            PlayerCauseOfDeath.Clear();
        }

        public static void SetCauseOfDeath(int playerIndex, AdvancedCauseOfDeath causeOfDeath, bool broadcast = true) {
            PlayerCauseOfDeath[playerIndex] = causeOfDeath;
            if (broadcast) DeathBroadcaster.BroadcastCauseOfDeath(playerIndex, causeOfDeath);
        }

        public static void SetCauseOfDeath(int playerIndex, CauseOfDeath causeOfDeath, bool broadcast = true) {
            SetCauseOfDeath(playerIndex, ConvertCauseOfDeath(causeOfDeath), broadcast);
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, CauseOfDeath causeOfDeath, bool broadcast = true) {
            SetCauseOfDeath((int) playerController.playerClientId, ConvertCauseOfDeath(causeOfDeath), broadcast);
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, AdvancedCauseOfDeath causeOfDeath, bool broadcast = true) {
            SetCauseOfDeath((int) playerController.playerClientId, causeOfDeath, broadcast);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(int playerIndex) {
            PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[playerIndex];

            return GetCauseOfDeath(playerController);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(PlayerControllerB playerController) {
            if (!PlayerCauseOfDeath.ContainsKey((int) playerController.playerClientId)) {
                Plugin.Instance.PluginLogger.LogInfo($"Player {playerController.playerClientId} has no custom cause of death stored! Using fallback...");
                return GuessCauseOfDeath(playerController);
            } else {
                Plugin.Instance.PluginLogger.LogInfo($"Player {playerController.playerClientId} has custom cause of death stored! {PlayerCauseOfDeath[(int) playerController.playerClientId]}");
                return PlayerCauseOfDeath[(int) playerController.playerClientId];
            }
        }

        public static AdvancedCauseOfDeath GuessCauseOfDeath(PlayerControllerB playerController) {
            if (playerController.isPlayerDead) {
                if (IsHoldingJetpack(playerController)) {
                    if (playerController.causeOfDeath == CauseOfDeath.Gravity) {
                        return AdvancedCauseOfDeath.Jetpack_Gravity;
                    } else if (playerController.causeOfDeath == CauseOfDeath.Blast) {
                        return AdvancedCauseOfDeath.Jetpack_Blast;
                    }
                }

                return ConvertCauseOfDeath(playerController.causeOfDeath);
            } else {
                return AdvancedCauseOfDeath.Unknown;
            }
        }

        public static bool IsHoldingJetpack(PlayerControllerB playerController) {
            var heldObjectServer = playerController.currentlyHeldObjectServer;
            if (heldObjectServer == null) return false;
            var heldObjectGameObject = heldObjectServer.gameObject;
            if (heldObjectGameObject == null) return false;
            var heldObject = heldObjectGameObject.GetComponent<GrabbableObject>();
            if (heldObject == null) return false;

            if (heldObject is JetpackItem) {
                return true;
            } else {
                return false;
            }
        }

        public static AdvancedCauseOfDeath ConvertCauseOfDeath(CauseOfDeath causeOfDeath) {
            switch (causeOfDeath) {
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

        public static string StringifyCauseOfDeath(CauseOfDeath causeOfDeath) {
            return StringifyCauseOfDeath(ConvertCauseOfDeath(causeOfDeath));
        }

        public static string StringifyCauseOfDeath(AdvancedCauseOfDeath causeOfDeath) {
            switch (causeOfDeath) {
                case AdvancedCauseOfDeath.Bludgeoning:
                    return "Bludgeoned to death.";
                case AdvancedCauseOfDeath.Gravity:
                    return "Fell to their death.";
                case AdvancedCauseOfDeath.Blast:
                    return "Went out with a bang.";
                case AdvancedCauseOfDeath.Strangulation:
                    return "Strangled to death.";
                case AdvancedCauseOfDeath.Suffocation:
                    return "Suffocated to death.";
                case AdvancedCauseOfDeath.Mauling:
                    return "Mauled to death.";
                case AdvancedCauseOfDeath.Gunshots:
                    return "Shot to death by a turret.";
                case AdvancedCauseOfDeath.Crushing:
                    return "Crushed to death.";
                case AdvancedCauseOfDeath.Drowning:
                    return "Drowned to death.";
                case AdvancedCauseOfDeath.Abandoned:
                    return "Abandoned by their coworkers.";
                case AdvancedCauseOfDeath.Electrocution:
                    return "Electrocuted to death.";

                case AdvancedCauseOfDeath.Enemy_Bracken:
                    return "Had their neck snapped by a Bracken.";
                case AdvancedCauseOfDeath.Enemy_EyelessDog:
                    return "Was eaten by an Eyeless Dog.";
                case AdvancedCauseOfDeath.Enemy_ForestGiant:
                    return "Swallowed whole by a Forest Giant.";
                case AdvancedCauseOfDeath.Enemy_CircuitBees:
                    return "Electro-stung to death by Circuit Bees.";
                case AdvancedCauseOfDeath.Enemy_GhostGirl:
                    return "Died a mysterious death.";
                case AdvancedCauseOfDeath.Enemy_EarthLeviathan:
                    return "Swallowed whole by an Earth Leviathan.";
                case AdvancedCauseOfDeath.Enemy_BaboonHawk:
                    return "Was eaten by a Baboon Hawk.";
                case AdvancedCauseOfDeath.Enemy_Jester:
                    return "Was the butt of a joke.";
                case AdvancedCauseOfDeath.Enemy_SnareFlea:
                    return "Was suffocated a Snare Flea.";
                case AdvancedCauseOfDeath.Enemy_Hygrodere:
                    return "Was absorbed by a Hygrodere.";
                case AdvancedCauseOfDeath.Enemy_HoarderBug:
                    return "Was hoarded by a Hoarder Bug.";
                case AdvancedCauseOfDeath.Enemy_SporeLizard:
                    return "Was puffed by a Spore Lizard.";
                case AdvancedCauseOfDeath.Enemy_SandSpider:
                    return "Ensnared in the Sand Spider's web.";

                case AdvancedCauseOfDeath.Jetpack_Gravity:
                    return "Flew too close to the sun.";
                case AdvancedCauseOfDeath.Jetpack_Blast:
                    return "Turned into a firework.";
                case AdvancedCauseOfDeath.Player_Murder:
                    return "Was the victim of a murder.";
                case AdvancedCauseOfDeath.Player_Quicksand:
                    return "Got stuck in quicksand.";
                case AdvancedCauseOfDeath.Player_DepositItemsDesk:
                    return "Received a demotion.";
                case AdvancedCauseOfDeath.Player_Dropship:
                    return "Couldn't wait for their items.";
                case AdvancedCauseOfDeath.Player_StunGrenade:
                    return "Was the victim of a murder.";

                case AdvancedCauseOfDeath.Unknown:
                    return "Most sincerely dead.";
                default:
                    return "Most sincerely dead.";
            }
        }

        internal static void SetCauseOfDeath(PlayerControllerB playerControllerB, object enemy_BaboonHawk)
        {
            throw new NotImplementedException();
        }
    }

    enum AdvancedCauseOfDeath {
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
        Enemy_EarthLeviathan, // Also known as SandWorm
        Enemy_EyelessDog, // Also known as MouthDog
        Enemy_ForestGiant,
        Enemy_GhostGirl, // Also known as DressGirl
        Enemy_Jester,
        Enemy_SnareFlea, // Also known as Centipede
        Enemy_SporeLizard, // Also known as Puffer TODO: Implement this.
        Enemy_Hygrodere, // Also known as Blob TODO: Implement this.
        Enemy_SandSpider, // TODO: Implement this.
        Enemy_Thumper, // Also known as Crawler TODO: Implement this.
        Enemy_HoarderBug, // TODO: Implement this.

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