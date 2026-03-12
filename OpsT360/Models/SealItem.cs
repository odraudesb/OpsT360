using CommunityToolkit.Mvvm.ComponentModel;

namespace OpsT360.Models;

public partial class SealItem : ObservableObject
{
    public int Number { get; }

    [ObservableProperty] private string code = string.Empty;
    [ObservableProperty] private string tid = string.Empty;
    [ObservableProperty] private bool isLocked;

    public SealItem(int number)
    {
        Number = number;
    }
}