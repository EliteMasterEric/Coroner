# Changelog

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
