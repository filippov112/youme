using Application.Constants;

namespace Application.Models
{
    public class GlobalConfig
    {
        public string StructurePromptGlobal { get; set; } = $"{SettingConstants.KEY_WORD_INPUT}\n`````\n{SettingConstants.KEY_WORD_PROJECT}\n`````\n{SettingConstants.KEY_WORD_REQUEST}\n\n{SettingConstants.KEY_WORD_SETTINGS}";
        public string UserSettingsPrompt { get; set; } = "При ответе будь краток и лаконичен.";
        public string StyleFileBlock { get; set; } = $"Файл: {SettingConstants.KEY_WORD_PATH}\n````\n{SettingConstants.KEY_WORD_CONTENT}\n````\n";
        public GlobalConfig Copy() => new()
        {
            StructurePromptGlobal = StructurePromptGlobal,
            UserSettingsPrompt = UserSettingsPrompt,
            StyleFileBlock = StyleFileBlock
        };
    }
}
