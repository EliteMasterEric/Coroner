using BepInEx.Configuration;

#nullable enable

namespace Coroner
{
    class PluginConfig
    {
        // Config entries are null until bound.
        ConfigEntry<bool>? DisplayCauseOfDeath = null;
        ConfigEntry<bool>? SeriousDeathMessages = null;
        ConfigEntry<bool>? DisplayFunnyNotes = null;
        ConfigEntry<bool>? DeathReplacesNotes = null;
        ConfigEntry<string>? LanguagePicker = null;

        // Constructor
        public PluginConfig()
        {
        }

        // Bind config values to fields
        public void BindConfig(ConfigFile _config)
        {
            DisplayCauseOfDeath = _config.Bind("General", "DisplayCauseOfDeath", true, "Display the cause of death in the player notes.");
            SeriousDeathMessages = _config.Bind("General", "SeriousDeathMessages", false, "Cause of death messages are more to-the-point.");
            DisplayFunnyNotes = _config.Bind("General", "DisplayFunnyNotes", true, "Display a random note when the player has no notes.");
            DeathReplacesNotes = _config.Bind("General", "DeathReplacesNotes", true, "True to replace notes when the player dies, false to append.");

            LanguagePicker = _config.Bind("Language", "LanguagePicker", "en", "Select a language to use.");// + LanguageHandler.GetLanguageList());
        }

        public bool ShouldDisplayCauseOfDeath()
        {
            if (DisplayCauseOfDeath == null) {
                Plugin.Instance.PluginLogger.LogWarning("Invalid access to uninstantiated config value DisplayCauseOfDeath");
                return true; // default value
            }
            return DisplayCauseOfDeath.Value;
        }

        public bool ShouldUseSeriousDeathMessages()
        {
            if (SeriousDeathMessages == null) {
                Plugin.Instance.PluginLogger.LogWarning("Invalid access to uninstantiated config value SeriousDeathMessages");
                return false; // default value
            }
            return SeriousDeathMessages.Value;
        }

        public bool ShouldDisplayFunnyNotes()
        {
            if (DisplayFunnyNotes == null) {
                Plugin.Instance.PluginLogger.LogWarning("Invalid access to uninstantiated config value DisplayFunnyNotes");
                return true; // default value
            }
            return DisplayFunnyNotes.Value;
        }

        public bool ShouldDeathReplaceNotes()
        {
            if (DeathReplacesNotes == null) {
                Plugin.Instance.PluginLogger.LogWarning("Invalid access to uninstantiated config value DeathReplacesNotes");
                return true; // default value
            }
            return DeathReplacesNotes.Value;
        }

        public string GetSelectedLanguage()
        {
            if (LanguagePicker == null) {
                Plugin.Instance.PluginLogger.LogWarning("Invalid access to uninstantiated config value LanguagePicker");
                return "en"; // default value
            }
            return LanguagePicker.Value.Replace('-', '_');
        }
    }
}
