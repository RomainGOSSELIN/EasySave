using static EasySave.Model.Enum;

namespace EasySave.Controller.Interfaces
{
    public interface ISettingsService
    {
        void ChangeOptions(LanguageEnum language, LogTypeEnum logtype);
    }
}