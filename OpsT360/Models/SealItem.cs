using CommunityToolkit.Mvvm.ComponentModel;

namespace OpsT360.Models;

public partial class SealItem : ObservableObject
{
    public int Number { get; }

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private bool isLocked;

    [ObservableProperty]
    private bool hasImage;

    public string Label => $"Seal #{Number}";

    public SealItem(int number)
    {
        Number = number;
    }
}
