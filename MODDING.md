# Modding Support

As of v2.1.0, Coroner provides an API which allows other mods and their developers (such as [CoronerMimics](https://thunderstore.io/c/lethal-company/p/EliteMasterEric/CoronerMimics/)) to access, create, and assign a player's cause of death. Players are also able to create or edit their own custom death messages via a mod, which can be shared to others by including it in a modpack (such as on [Thunderstore](https://thunderstore.io/c/lethal-company/?section=modpacks)), or by distributing the mod directly.

## Adding or Replacing Language Strings
To begin, follow these instructions for both adding and replacing:
1. Create a new Thunderstore mod. For more information, check H3VR Modding Wiki's [guide to creating a Thunderstore mod/package](https://h3vr-modding.github.io/wiki/creating/thunderstore/uploading.html#manual-creation) from the **Manual Creation** section.
2. Create a folder `yourmod/BepInEx/config/EliteMasterEric-Coroner` in your mod's folder (where the `yourmod` folder has `manifest.json`).

> **NOTE:** It is important to update your mod whenever there is an update to Lethal Company which adds a new way of dying, otherwise the mod will use its default values for death messages.

### Adding New Language Strings

1. Create the file `Strings_<lang>_<suffix>.xml` in the folder `EliteMasterEric-Coroner`. Replace `<lang>` with the language you want to target (most commonly `en-us`) and `<suffix>` with something unique. You should end up with a folder structure like this:

```
yourmod/
    BepInEx/config/EliteMasterEric-Coroner/Strings_<lang>_<suffix>.xml
    icon.png
    manifest.json
    README.md
```
2. Add the following code to the file `Strings_<lang>_<suffix>.xml`:

```xml
<base>
    <tags>
        <tag language="<lang>" />
        <!-- Ensure you replace <lang> with the language you want to target, otherwise it won't work -->
    </tags>

    <strings>

        <DeathEnemyBracken text="Was killed by a Bracken." />

    </strings>
</base>
```

3. Add tags in the `<strings>` section with the values you want to replace. For all available tags, see [`Strings_en-us.xml`](https://github.com/EliteMasterEric/Coroner/blob/master/LanguageData/Strings_en-us.xml).
4. You're done! Whenever your mod is installed alongside Coroner, all `Strings_<lang>.xml` and `Strings_<lang>_<suffix>.xml` files will be added together into one xml file in memory and Coroner will pick a death message from the combined file.

### Replacing Existing Language Strings

1. Create the file `Strings_<lang>.xml` in the folder `EliteMasterEric-Coroner`. Replace `<lang>` with the language you want to target (most commonly `en-us`). You should end up with a folder structure like this:

```
yourmod/
    BepInEx/config/EliteMasterEric-Coroner/Strings_<lang>.xml
    icon.png
    manifest.json
    README.md
```
2. Copy the contents of the language you want to target (most commonly [`Strings_en-us.xml`](https://github.com/EliteMasterEric/Coroner/blob/master/LanguageData/Strings_en-us.xml)) into `Strings_<lang>.xml`.
3. Modify the language file as desired.
4. You're done! Whenever your mod is installed alongside Coroner, all death messages from your mod will replace the death messages that were included with the mod's default language file for your targeted language.

## Creating Custom Causes of Death

This is the most common operation to perform with Coroner's API.

First, build a new AdvancedCauseOfDeath, and store it statically so it can be used later.

```cs
static const MIMIC_LANGUAGE_KEY = "Enemy_Mimic";
static AdvancedCauseOfDeath MIMIC = Coroner.API.Register(MIMIC_LANGUAGE_KEY);
```

Then, when the player dies, apply this cause of death for the player:

```cs
Coroner.API.SetCauseOfDeath(player, MIMIC);
```

### Adding Language Strings

The above will currently display `{Enemy_Mimic}` as the cause of death rather than your desired string. You need to provide an XML config which includes your language strings:

In `BepInEx/config/EliteMasterEric-Coroner/` in your mod upload, create a file named `Strings_<lang>_<suffix>.xml`, where `<lang>` should be the language code you want (`en-us` is the English (American) language and the default for most players) and `suffix` is a value of your choice (try to choose something that another mod won't use on accident). Add tags to `<strings>` for each cause of death you want to add. You can include multiple tags for each cause of death and the game will randomize between them.

```xml
<base>
    <tags>
        <tag language="en-us" />
    </tags>

    <strings>
        <!-- NOTE: If SeriousDeathNotes is turned on, it displays only the first entry, so make the first entry literal! -->
        <DeathEnemyMimic text="Was killed by a Mimic." />
        <DeathEnemyMimic text="Fell for a Mimic." />
    </strings>
</base>
```

## List of Methods

- `Coroner.API.Register(string key)`
    - Registers a new cause of death that uses a given language key.
    - Returns: An `AdvancedCauseOfDeath` corresponding to the new cause of death. Store this statically and reuse it.
- `Coroner.API.IsRegistered(string key)`
    - Check whether a given language key is already registered.
    - Returns: A boolean value.
- `Coroner.API.GetCauseOfDeath(int playerId)`
    - Retrieves the currently known cause of death for the player by their client ID, if any.
    - Returns: An `AdvancedCauseOfDeath`, or `null` if no cause of death is known.
- `Coroner.API.GetCauseOfDeath(PlayerControllerB player)`
    - Retrieves the currently known cause of death for the player by reference, if any.
    - Returns: An `AdvancedCauseOfDeath`, or `null` if no cause of death is known.
- `Coroner.API.SetCauseOfDeath(PlayerControllerB player, AdvancedCauseOfDeath? causeOfDeath)`
    - Applies a given cause of death to the player by their client ID.
    - Provide the `AdvancedCauseOfDeath` via argument. `AdvancedCauseOfDeath` has static constants for all the vanilla causes of death, or you can use one created by `Coroner.API.Register()`.
- `Coroner.API.SetCauseOfDeath(int playerId, AdvancedCauseOfDeath? causeOfDeath)`
    - Applies a given cause of death to the player by reference.
    - Provide the `AdvancedCauseOfDeath` via argument. `AdvancedCauseOfDeath` has static constants for all the vanilla causes of death, or you can use one created by `Coroner.API.Register()`.
- `Coroner.API.StringifyCauseOfDeath(AdvancedCauseOfDeath causeOfDeath, Random? random)`
    - Translate a cause of death to the user's current language.
    - Pass this the result of `Coroner.API.GetCauseOfDeath()` for best results.
    - The `Random` argument is optional, only pass it in if you need to modify how language strings are chosen. The default works well and syncs properly across clients.

## Issues
Report any issues on the [Lethal Company Modding Discord](https://discord.gg/lcmod) or via the [GitHub Issue Tracker](https://github.com/EliteMasterEric/Coroner/issues).