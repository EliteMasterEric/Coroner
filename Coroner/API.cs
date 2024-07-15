using System;
using GameNetcodeStuff;

#nullable enable

namespace Coroner
{
    public class API
    {
        /// <summary>
        /// Sets the cause of death for a player object.
        /// You can set this to a default one (see `Coroner.AdvancedCauseOfDeath`) or a custom one (see `Register()`).
        /// </summary>
        /// <param name="player">The player to set the cause of death for.</param>
        /// <param name="causeOfDeath">The cause of death to use. Set to `null` to clear.</param>
        /// <example> SetCauseOfDeath(player, AdvancedCauseOfDeath.Enemy_ForestGiant); </example>
        public static void SetCauseOfDeath(PlayerControllerB player, AdvancedCauseOfDeath? causeOfDeath)
        {
            // Call the proper internal method.
            AdvancedDeathTracker.SetCauseOfDeath(player, causeOfDeath);
        }

        /// <summary>
        /// Sets the cause of death for a player with the given ID.
        /// You can set this to a default one (see `Coroner.AdvancedCauseOfDeath`) or a custom one (see `Register()`).
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <param name="causeOfDeath">The cause of death to use. Set to `null` to clear.</param>
        /// <example> SetCauseOfDeath(0, AdvancedCauseOfDeath.Enemy_ForestGiant); </example>
        public static void SetCauseOfDeath(int playerId, AdvancedCauseOfDeath? causeOfDeath)
        {
            // Call the proper internal method.
            AdvancedDeathTracker.SetCauseOfDeath(playerId, causeOfDeath);
        }

        /// <summary>
        /// Gets the cause of death for a player object.
        /// </summary>
        /// <param name="player">The player to get the cause of death for.</param>
        /// <returns>The cause of death for the player. May be null if none is set, or a custom value provided by a mod.</returns>
        /// <example> GetCauseOfDeath(player); </example>
        public static AdvancedCauseOfDeath? GetCauseOfDeath(PlayerControllerB player)
        {
            // Call the proper internal method.
            return AdvancedDeathTracker.GetCauseOfDeath(player);
        }

        /// <summary>
        /// Gets the cause of death for a player with the given ID.
        /// </summary>
        /// <param name="playerId">The ID of the player.</param>
        /// <returns>The cause of death for the player.</returns>
        /// <example> GetCauseOfDeath(0); </example>
        public static AdvancedCauseOfDeath? GetCauseOfDeath(int playerId)
        {
            // Call the proper internal method.
            return AdvancedDeathTracker.GetCauseOfDeath(playerId);
        }

        /// <summary>
        /// Register a new cause of death. Useful for mods.
        /// Choose one that is unique to your mod, and store the value statically for reuse.
        /// Then, call SetCauseOfDeath(player, customCauseOfDeath) to use it.
        /// </summary>
        /// <param name="key">The language key to use for the cause of death.</param>
        /// <returns>The newly registered cause of death.</returns>
        public static AdvancedCauseOfDeath Register(string key)
        {
            // Call the proper internal method.
            return AdvancedCauseOfDeath.Build(key);
        }

        // <summary>
        // Determine whether a cause of death is registered.
        // </summary>
        // <param name="key">The language key to use for the cause of death.</param>
        // <returns>Whether that cause of death is already registered.</returns>
        public static bool IsRegistered(string key)
        {
            return AdvancedCauseOfDeath.IsTagRegistered(key);
        }

        /// <summary>
        /// Convert a cause of death to a language string as used in-game.
        /// </summary>
        /// <param name="causeOfDeath">The cause of death to convert.</param>
        /// <param name="random">Optionally specify a random number generator to use. If you seed this the same between clients, they'll produce the same value.</param>
        /// <returns>One of the available language strings for that cause of death.</returns>
        public static string StringifyCauseOfDeath(AdvancedCauseOfDeath causeOfDeath, Random? random)
        {
            // Call the proper internal method.
            return AdvancedDeathTracker.StringifyCauseOfDeath(causeOfDeath, random != null ? random : Plugin.RANDOM);
        }
    }
}