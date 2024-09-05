# Modding Support

As of v2.1.0, Coroner now has an API which allows other mods to access causes of death and assign your own, as well as a config system which allows players to replace causes of death with custom ones provided by a modpack.

## Replacing Language Strings

Steps to creating a custom modpack which replaces or adds new language strings:

### Adding New Language Strings

1. Create a new Thunderstore mod, with a file `BepInEx/config/EliteMasterEric-Coroner/Strings_en_<suffix>.xml`. Replace `<suffix>` with something unique.
2. Fill the file with the following:

```xml
<base>
    <tags>
        <tag language="en" />
    </tags>

    <strings>

        <DeathEnemyBracken text="Was killed by a Bracken." />

    </strings>
</base>
```

3. Add tags in the `<strings>` section with your own values, see [the base language file](https://github.com/EliteMasterEric/Coroner/blob/master/LanguageData/Strings_en.xml) as an example.

You're done! When the mod is installed, all `Strings_en` language files get added together into a single large language file.

### Replacing Existing Language Strings

1. Create a new Thunderstore mod, with a file `BepInEx/config/EliteMasterEric-Coroner/Strings_en.xml`
2. Fill the file with the contents of [the base language file](https://github.com/EliteMasterEric/Coroner/blob/master/LanguageData/Strings_en.xml).
3. Modify the language file as desired.

Once you're complete, you should have a new mod which JUST contains the Thunderstore mod manifest file and the custom language file.

Installing this alongside Coroner will replace the language data, and you will see your custom language data in-game.

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

In `BepInEx/config/EliteMasterEric-Coroner/` in your mod upload, create a file named `Strings_<lang>_<suffix>.xml`, where `<lang>` should be the language code you want (`en` is the English language and the default for most players) and `suffix` is a value of your choice (try to choose something that another mod won't use on accident).

```xml

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