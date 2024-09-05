# Modding Support

As of v2.1.0, Coroner now has an API which allows other mods to access causes of death and assign your own.

## Methods

- `SetCauseOfDeath(PlayerControllerB player, AdvancedCauseOfDeath? causeOfDeath)`
- `SetCauseOfDeath(int playerId, AdvancedCauseOfDeath? causeOfDeath)`
- `GetCauseOfDeath(PlayerControllerB player)`
- `GetCauseOfDeath(int playerId)`
- `Register(string key)`
- `IsRegistered(string key)`
- `StringifyCauseOfDeath(AdvancedCauseOfDeath causeOfDeath, Random? random)`

## Issues
Report any issues on the [Lethal Company Modding Discord](https://discord.gg/lcmod) or via the [GitHub Issue Tracker](https://github.com/EliteMasterEric/Coroner/issues).