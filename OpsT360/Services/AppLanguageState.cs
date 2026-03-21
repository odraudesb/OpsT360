namespace OpsT360.Services;

public sealed class AppLanguageState : IAppLanguageState
{
    public bool IsEnglish { get; private set; } = true;

    public event Action<bool>? LanguageChanged;

    public void SetIsEnglish(bool isEnglish)
    {
        if (IsEnglish == isEnglish)
            return;

        IsEnglish = isEnglish;
        LanguageChanged?.Invoke(IsEnglish);
    }
}
