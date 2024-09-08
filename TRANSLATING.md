# Translating
In order to create a translation, create a copy of the [`Strings_en.xml`](https://github.com/EliteMasterEric/Coroner/blob/master/LanguageData/Strings_en.xml) file and change the file name to replace the `en` with your language's code from [ISO 639-1](https://www.iso.org/iso-639-language-code). A list of these codes can be easily found and searched via [this list on Wikipedia](https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes). It's best to copy from the English translation as it will always be the most up to date version of the translation values. If your language isn't apart of ISO 639-1, choose a two-character code (or append a two-character code if you're creating a translation for a locale, such as Simplified (`zh-cn`) or Traditional (`zh-tw`) Chinese, or American (`en-us`) or British (`en-uk`) English) which most accurately represents your language, provided it isn't already used in ISO 639-1.

When you've finalized your translation, ensure that you add or edit your translation's information (such as version) below under the [List of Available Translations](https://github.com/EliteMasterEric/Coroner/blob/master/TRANSLATING.md#list-of-available-translations), as well as adding yourself to the contributors with a link to your GitHub profile.

# Create or Edit Custom Death Messages
If you're trying to create your own variation of an already added language, please check [`MODDING.md`]("https://github.com/EliteMasterEric/Coroner/blob/master/MODDING.md#adding-or-replacing-language-strings") for more information on how to add or replace text in an already created translation.

# List of Available Translations
Below is a list of currently included languages, the version of the translation, and their contributor(s). Any language which isn't on the latest version is in need of translation. To join the list, create or edit a translation and submit a [pull request](https://github.com/EliteMasterEric/Coroner/pulls)!

- English (American) (`en-us`) [v3] | [EliteMasterEric (Eric)](https://github.com/EliteMasterEric)
- Chinese (Simplified) (`zh-cn`) [v2] | [RAINighty (颜绎)](https://github.com/RAINighty)
- Dutch (`nl`) [v0] | Ceva
- French (`fr`) [v0] | Fleetway
- German (`de`) [v0] | [YoBii](https://github.com/YoBii)
- Hungarian (`hu`) [v0] | [Kultercode (Kristóf Juhász)](https://github.com/Kultercode)
- Italian (`it`) [v0] | [MakinDay (Federico G.)](https://github.com/MakinDay)
- Korean (`ko`) [v0] | [sgkill8 (킬육)](https://github.com/sgkill6)
- Portuguese (Brazil) (`pt-br`) [v0] | Foxeru
- Russian (`ru`) [v3] | [NickolasFleim](https://github.com/NickolasFleim), [D4N9 (D4N9-Twitch)](https://github.com/D4N9-Twitch)
- Spanish (`es`) [v0] | Helado de Pato
- Debug (`test`) [v3]

# Versions of Translations
Please ensure when creating or amending a translation that the correct version number is noted at the top of the translation's file.
### v3
- Added v60 death messages
- Added specific pit death messages
### v2
- Added v50/55 death messages
### v1
- Added translation notes to assist translators
### v0