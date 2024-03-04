# Changelog

# 1.6.1
# Fixed
- Improved the clarity of the error messages for when people install the mod incorrectly (you won't get this error if you use R2Modman!).

# 1.6.0
## Added
- Added new custom messages for specific causes of death:
    - Holding a Stun Grenade
    - Extension Ladder
- Added support for the expanded Unicode character set while [FontFixer](https://thunderstore.io/c/lethal-company/p/EliteMasterEric/FontFixer/) is installed.
- Added a new Spanish localization.
- Added a new German localization.
- Added a new Italian localization.
- Added a new Korean localization.
- Added a new Hungarian localization.
- Added a new Chinese (Simplified) localization.
- Added a new Portuguese (Brazilian) localization.
## Changed
- Numerous translation changes.
    - The English Translation now has additional notes to assist other translators.
- Moved the localization files into the config folder.
    - This should allow modpacks to properly override them.
- Changed translation handling to fallback to English if specific lines are missing.
    - This applies when some lines are present and some aren't.
## Fixed
- Fixes for Lethal Company v49.

# 1.5.3
## Fixed
- Fixed a build issue with 1.5.2.

# 1.5.2
## Fixed
- Attempting to fix an issue caused when installing Coroner without R2Modman.

# 1.5.1
## Added
- Added a new French localization.
## Changed
- I have removed LC_API as a dependency from the manifest. Coroner on its own does a reasonable job of synchronizing cause of death messages across clients without it, in my experience, to a greater extent than I originally thought. I do not anticipate making it a dependency again in the future. Apologies for the inconvenience.
- Improved the Dutch localization.

# 1.5.0
## Added
- Added a new localization feature! Coroner now has official support for English, Dutch, and Russian.
    - If you want to help localize Coroner for other languages, please visit the Lethal Company Modding Discord or submit a pull request on GitHub, I would GREATLY appreciate your contributions.
## Changed
- LC_API has been readded as a dependency after the 3.0.0 update fixed all the weird bu.
## Fixed
- Fixed an issue where vanilla notes would be replaced with funny notes (funny notes should only display if no vanilla ones were given)
- Fixed an issue where "Funny Notes" were not randomized when serious death messages were turned on.

# 1.4.2
## Added
- Now includes the mod lel

# 1.4.1
## Removed
- I decided you folks had it too good *yoinks mod away*

# 1.4.0
## Additions
- Added custom death messages for specific types of explosions:
    - Landmines
    - Jetpack
    - Lightning
- Added custom death message for Turrets.
- Added a `SeriousDeathMessages` config option to display only more to-the-point death messages. Defaults to `false`.
## Changed
- The death report now says "Cause of Death" instead of "Notes" when a player dies.
- Decreased log verbosity to improve performance.

# 1.3.1
## Fixed
- Fixed an issue where an exception in one of the cause-of-death patches would cause the player to not die.
- Fixed an issue where not having LC_API installed would cause an exception to occur.

# 1.3.0
## Additions
- Additional death messages for other death types.
- Added new death messages for the enemies from v45.
## Fixed
- Fixed some bugs related to v45.

# 1.2.0
## Additions
- Each death type can now randomly display one of several messages.
- Messages should now match across clients, even when randomized.
- Added new death messages for the enemies that didn't have them.
    - Bunker Spider
    - Coil Head
    - Hoarder Bug
    - Hygrodere
    - Spore Lizard
    - Thumper
## Changes
- Removed LC_API as a dependency from the manifest (Thunderstore was enforcing it as mandatory rather than optional). It is still recommended that players install LC_API alongside this if possible.
## Bug Fixes
- Fixed an issue where Bunkers Spiders were called Sand Spiders.
- Fixed an issue where the mod ZIP was included inside the mod ZIP, causing a console error.

## 1.1.0
## Additions
- Added a custom cause of death for the dropship.
- Added an optional dependency on LC_API and used it to improve accuracy of cause of death reports over multiplayer.
## Bug Fixes
- Fixed a softlock/crash related to checking if the player has a Jetpack.
- Fixed the cause of death not being evaluated properly when being crushed by a ladder.
- Fixed the cause of death not being evaluated properly when drowning in Quicksand.
- Fixed a bug where notes would not start with "Notes:".
- Fixed an issue where BepInEx was not listed as a mandatory dependency.
- Fixed an issue with enemy-related custom deaths not working

## 1.0.0
Initial release.
- Added cause of death to the results screen.
- Added advanced cause-of-death tracking for specific enemies, falling back to built-in tracking on failure.
