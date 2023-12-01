using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Steamworks;

namespace Coroner {
    class AdvancedDeathTracker {
        private static readonly Dictionary<int, AdvancedCauseOfDeath> PlayerCauseOfDeath = new Dictionary<int, AdvancedCauseOfDeath>();

        public static void ClearDeathTracker() {
            PlayerCauseOfDeath.Clear();
        }

        public static void SetCauseOfDeath(int playerIndex, AdvancedCauseOfDeath causeOfDeath) {
            PlayerCauseOfDeath[playerIndex] = causeOfDeath;
        }

        public static void SetCauseOfDeath(int playerIndex, CauseOfDeath causeOfDeath) {
            SetCauseOfDeath(playerIndex, ConvertCauseOfDeath(causeOfDeath));
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, CauseOfDeath causeOfDeath) {
            SetCauseOfDeath((int) playerController.playerClientId, ConvertCauseOfDeath(causeOfDeath));
        }

        public static void SetCauseOfDeath(PlayerControllerB playerController, AdvancedCauseOfDeath causeOfDeath) {
            SetCauseOfDeath((int) playerController.playerClientId, causeOfDeath);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(int playerIndex) {
            PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[playerIndex];

            return GetCauseOfDeath(playerController);
        }

        public static AdvancedCauseOfDeath GetCauseOfDeath(PlayerControllerB playerController) {
            if (!PlayerCauseOfDeath.ContainsKey((int) playerController.playerClientId)) {
                return GuessCauseOfDeath(playerController);
            }
            return PlayerCauseOfDeath[(int) playerController.playerClientId];
        }

        public static AdvancedCauseOfDeath GuessCauseOfDeath(PlayerControllerB playerController) {
            if (playerController.isPlayerDead) {
                if (playerController.causeOfDeath == CauseOfDeath.Suffocation && playerController.isSinking) {
                    return AdvancedCauseOfDeath.Quicksand;
                } else if (IsHoldingJetpack(playerController)) {
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
            var heldObject = playerController.currentlyHeldObjectServer.gameObject.GetComponent<GrabbableObject>();
            if (heldObject == null) {
                return false;
            }
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
                    return "Exploded.";
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

                case AdvancedCauseOfDeath.Enemy_DepositItemsDesk:
                    return "Received a demotion.";
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

                case AdvancedCauseOfDeath.Quicksand:
                    return "Got stuck in quicksand.";
                case AdvancedCauseOfDeath.Jetpack_Gravity:
                    return "Flew too close to the sun.";
                case AdvancedCauseOfDeath.Jetpack_Blast:
                    return "Went up in a fiery blaze.";

                case AdvancedCauseOfDeath.Unknown:
                    return "Died somehow.";
                default:
                    return "Died somehow.";
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
        Enemy_DepositItemsDesk,
        Enemy_Bracken, // Also known as Flowerman
        Enemy_EyelessDog, // Also known as MouthDog
        Enemy_ForestGiant,
        Enemy_CircuitBees, // Also known as RedLocustBees
        Enemy_GhostGirl, // Also known as DressGirl
        Enemy_EarthLeviathan, // Also known as SandWorm
        Enemy_BaboonHawk, // Also known as BaboonBird

        // Custom causes (other)
        Quicksand,
        Enemy_Jester,
        Jetpack_Gravity,
        Jetpack_Blast,
    }
}