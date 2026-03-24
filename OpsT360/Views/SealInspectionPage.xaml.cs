using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class SealInspectionPage : ContentPage
{
    private readonly SealInspectionViewModel _vm;

    public SealInspectionPage(SealInspectionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    private void OnSealCompleted(object? sender, EventArgs e)
    {
        if (sender is not Entry entry || !int.TryParse(entry.ClassId, out var sealNumber))
            return;

        _vm.ReadSealCommand.Execute(sealNumber.ToString());
    }

    private async void OnReadSealClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || !int.TryParse(button.ClassId, out var sealNumber))
            return;

        if (!button.IsEnabled)
            return;

        button.IsEnabled = false;
        try
        {
            var sdkCaptured = await _vm.TryCaptureSealFromSdkAsync(sealNumber);
            if (sdkCaptured)
            {
                _vm.ReadSealCommand.Execute(sealNumber.ToString());
                return;
            }

            if (string.IsNullOrWhiteSpace(_vm.StatusText))
                _vm.StatusText = "No EPC received from RFID SDK. Verify the handheld is in UHF mode and not barcode mode.";
        }
        catch (Exception ex)
        {
            _vm.StatusText = $"Read seal button error: {ex.Message}";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private async void OnReadNextSealClicked(object? sender, EventArgs e)
    {
        try
        {
            await _vm.TryCaptureRemainingSealsFromSdkAsync();
        }
        catch (Exception ex)
        {
            _vm.StatusText = $"Read seals button error: {ex.Message}";
        }
    }


    private async void OnPanelPreviewTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is null || !int.TryParse(e.Parameter.ToString(), out var panelNumber))
            return;

        var index = panelNumber - 1;
        if (index < 0 || index >= _vm.SealImages.Count)
            return;

        var panel = _vm.SealImages[index];
        if (!panel.HasImage)
        {
            await DisplayAlert("No image", $"{panel.Label} has no uploaded evidence yet.", "OK");
            return;
        }

        await ShowPreviewModalAsync(
            $"{panel.Label} ({panel.ValidationStatus})",
            panel.PreviewImage!);
    }

    private async void OnContainerPreviewTapped(object? sender, TappedEventArgs e)
    {
        if (!_vm.ContainerImage.HasImage)
        {
            await DisplayAlert("No image", "There is no container photo yet.", "OK");
            return;
        }

        await ShowPreviewModalAsync(
            $"Container image ({_vm.ContainerImage.ValidationStatus})",
            _vm.ContainerImage.PreviewImage!);
    }

    private async Task ShowPreviewModalAsync(string title, ImageSource source)
    {
        var closeButton = new Label
        {
            Text = "✕",
            FontSize = 20,
            TextColor = Color.FromArgb("#5B6475"),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start
        };

        var modal = new ContentPage
        {
            BackgroundColor = Color.FromArgb("#CC000000"),
            Content = new Grid
            {
                Padding = 16,
                Children =
                {
                    new Border
                    {
                        BackgroundColor = Colors.White,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(14) },
                        Padding = 12,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Grid
                                {
                                    ColumnDefinitions = new ColumnDefinitionCollection
                                    {
                                        new ColumnDefinition{ Width = GridLength.Star },
                                        new ColumnDefinition{ Width = GridLength.Auto }
                                    },
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = title,
                                            FontAttributes = FontAttributes.Bold,
                                            TextColor = Color.FromArgb("#22304A"),
                                            VerticalTextAlignment = TextAlignment.Center
                                        },
                                        closeButton
                                    }
                                },
                                new Image
                                {
                                    Source = source,
                                    Aspect = Aspect.AspectFit,
                                    HeightRequest = 560
                                }
                            }
                        }
                    }
                }
            }
        };

        closeButton.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await Navigation.PopModalAsync())
        });
        await Navigation.PushModalAsync(modal);
    }

    private void OnMenuTapped(object? sender, TappedEventArgs e)
    {
        Element? current = this;
        while (current is not null && current is not MainMenuPage)
            current = current.Parent;

        if (current is MainMenuPage menu)
            menu.OpenMenu();
    }
}
