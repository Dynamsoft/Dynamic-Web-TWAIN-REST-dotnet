using DWT_REST_MAUI.ViewModels;

namespace DWT_REST_MAUI;

public interface IDialogService
{
    Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
}
public class DialogService : IDialogService
{
    public async Task<string> ShowActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
    {
        return await Application.Current.MainPage.DisplayActionSheet(
            title, cancel, destruction, buttons);
    }
}
public partial class SettingsPage : ContentPage
{
	private SettingsViewModel viewModel = new SettingsViewModel(new DialogService());
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = viewModel;
		LoadSettings();
    }

	private void LoadSettings() {
		viewModel.LoadPreferences();
		viewModel.LoadScanners();
	}
}