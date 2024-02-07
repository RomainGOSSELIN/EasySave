using static EasySave.Model.Enum;

namespace EasySave.Controller.Interfaces
{
    public interface ISettingsService
    {
        void ChangeLanguage(LanguageEnum language);
    }
}