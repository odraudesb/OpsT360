namespace OpsT360.Services;

public interface IAppLanguageState
{
    bool IsEnglish { get; }
    event Action<bool>? LanguageChanged;
    void SetIsEnglish(bool isEnglish);
}
