# Changelog

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
