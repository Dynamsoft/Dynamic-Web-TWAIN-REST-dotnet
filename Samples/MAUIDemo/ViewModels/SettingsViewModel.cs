﻿using DynamicWebTWAIN.RestClient;
using DynamicWebTWAIN.ServiceFinder;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DWT_REST_MAUI.ViewModels;

internal class SettingsViewModel : INotifyPropertyChanged
{
    // Auto feeder
    private bool _autoFeeder;
    public bool AutoFeeder
    {
        get => _autoFeeder;
        set
        {
            if (_autoFeeder != value)
            {
                _autoFeeder = value;
                OnPropertyChanged();
            }
        }
    }
    // Auto feeder
    private bool _duplex;
    public bool Duplex
    {
        get => _duplex;
        set
        {
            if (_duplex != value)
            {
                _duplex = value;
                OnPropertyChanged();
            }
        }
    }
    // License Key
    private string _licenseKey;
    public string LicenseKey
    {
        get => _licenseKey;
        set
        {
            if (_licenseKey != value)
            {
                _licenseKey = value;
                OnPropertyChanged();
            }
        }
    }

    private string _findServiceButtonText = "Find services";
    public string FindServiceButtonText
    {
        get => _findServiceButtonText;
        set
        {
            if (_findServiceButtonText != value)
            {
                _findServiceButtonText = value;
                OnPropertyChanged();
            }
        }
    }
    private string _reloadButtonText = "Reload";
    public string ReloadButtonText
    {
        get => _reloadButtonText;
        set
        {
            if (_reloadButtonText != value)
            {
                _reloadButtonText = value;
                OnPropertyChanged();
            }
        }
    }


    // IP Address
    private string _ipAddress;
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }
    }

    // Scanner Models
    private List<string> _scannerModels = new List<string> { };
    public List<string> ScannerModels
    {
        get => _scannerModels;
        set
        {
            _scannerModels = value;
            OnPropertyChanged();
        }
    }
    private string _selectedScannerModel;
    public string SelectedScannerModel
    {
        get => _selectedScannerModel;
        set
        {
            if (_selectedScannerModel != value)
            {
                _selectedScannerModel = value;
                OnPropertyChanged();
            }
        }
    }

    // DPI Options
    private bool _is150Dpi;
    public bool Is150Dpi
    {
        get => _is150Dpi;
        set
        {
            if (_is150Dpi != value)
            {
                _is150Dpi = value;
                OnPropertyChanged();
                if (value) SelectedDpi = 150;
            }
        }
    }

    private bool _is300Dpi = true; // Default to 300 DPI
    public bool Is300Dpi
    {
        get => _is300Dpi;
        set
        {
            if (_is300Dpi != value)
            {
                _is300Dpi = value;
                OnPropertyChanged();
                if (value) SelectedDpi = 300;
            }
        }
    }

    private bool _is600Dpi;
    public bool Is600Dpi
    {
        get => _is600Dpi;
        set
        {
            if (_is600Dpi != value)
            {
                _is600Dpi = value;
                OnPropertyChanged();
                if (value) SelectedDpi = 600;
            }
        }
    }

    public int SelectedDpi { get; private set; } = 300;

    // Color Mode Options
    private bool _isBlackWhite = true; // Default to B&W
    public bool IsBlackWhite
    {
        get => _isBlackWhite;
        set
        {
            if (_isBlackWhite != value)
            {
                _isBlackWhite = value;
                OnPropertyChanged();
                if (value) SelectedColorMode = "BlackWhite";
            }
        }
    }

    private bool _isGrayscale;
    public bool IsGrayscale
    {
        get => _isGrayscale;
        set
        {
            if (_isGrayscale != value)
            {
                _isGrayscale = value;
                OnPropertyChanged();
                if (value) SelectedColorMode = "Grayscale";
            }
        }
    }

    private bool _isColor;
    public bool IsColor
    {
        get => _isColor;
        set
        {
            if (_isColor != value)
            {
                _isColor = value;
                OnPropertyChanged();
                if (value) SelectedColorMode = "Color";
            }
        }
    }

    public string SelectedColorMode { get; private set; } = "BlackWhite";

    // Save Command
    public ICommand SaveSettingsCommand { get; }
    public ICommand LoadScannersCommand { get; }
    public ICommand FindServiceCommand { get; }
    private readonly IDialogService _dialogService;
    public SettingsViewModel(IDialogService dialogService)
    {
        SaveSettingsCommand = new Command(ExecuteSaveSettings);
        LoadScannersCommand = new Command(LoadScanners);
        FindServiceCommand = new Command(FindServices);
        _dialogService = dialogService;
    }

    public async void FindServices()
    {
        if (FindServiceButtonText == "Finding...")
        {
            return;
        }
        FindServiceButtonText = "Finding...";
        IServiceInfo[] results = await ServiceFinder.DiscoverServicesAsync();
        FindServiceButtonText = "Find services";
        List<string> addresses = new List<string>();
        foreach (var service in results)
        {
            foreach (var address in service.Addresses)
            {
                string addressString = address.Address.ToString();
                if (addressString.StartsWith("https"))
                {
                    addresses.Add(addressString);
                }
            }
        }
#if WINDOWS
        addresses.Add(MainPage.defaultAddress);
#endif
        string result = await _dialogService.ShowActionSheetAsync("Select an address", "Cancel", null, addresses.ToArray());
        Debug.WriteLine(result);
        if (!string.IsNullOrEmpty(result))
        {
            if (result.StartsWith("https"))
            {
                IpAddress = result;
                LoadScanners();
            }
        }
    }

    public void LoadPreferences()
    {
        LicenseKey = Preferences.Get("License", "");
        IpAddress = Preferences.Get("IP", MainPage.defaultAddress);
        Duplex = Preferences.Get("Duplex", false);
        AutoFeeder = Preferences.Get("AutoFeeder", false);
        int DPI = Preferences.Get("DPI", 150);
        if (DPI == 150)
        {
            Is150Dpi = true;
        }
        else if (DPI == 300)
        {
            Is300Dpi = true;
        }
        else if (DPI == 600)
        {
            Is600Dpi = true;
        }
        SelectedColorMode = Preferences.Get("ColorMode", "Color");
        if (SelectedColorMode == "BlackWhite")
        {
            IsBlackWhite = true;
        }
        else if (SelectedColorMode == "Grayscale")
        {
            IsGrayscale = true;
        }
        else if (SelectedColorMode == "Color")
        {
            IsColor = true;
        }
    }

    public async void LoadScanners()
    {
        if (ReloadButtonText == "Reloading...")
        {
            return;
        }
        try
        {
            ReloadButtonText = "Reloading...";
            List<string> modelNames = new List<string>();
            var client = new DWTClient(new Uri(IpAddress), LicenseKey);
            var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_WIATWAINSCANNER | EnumDeviceTypeMask.DT_TWAINSCANNER);
            foreach (var scanner in scanners)
            {
                Debug.WriteLine(scanner.Name);
                modelNames.Add(scanner.Name);
            }
            ScannerModels = modelNames;
            var previousSelectedScanner = Preferences.Get("Scanner", "");
            if (modelNames.Contains(previousSelectedScanner))
            {
                SelectedScannerModel = previousSelectedScanner;
            }
            else
            {
                if (modelNames.Count > 0)
                {
                    SelectedScannerModel = modelNames[0];
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        ReloadButtonText = "Reload";

    }

    private void ExecuteSaveSettings()
    {
        Debug.WriteLine($"Saved");
        Debug.WriteLine($"Settings Saved:\n" +
                            $"License: {LicenseKey}\n" +
                            $"IP: {IpAddress}\n" +
                            $"Scanner: {SelectedScannerModel}\n" +
                            $"DPI: {SelectedDpi}\n" +
                            $"Color Mode: {SelectedColorMode}");
        var previousLicense = Preferences.Get("License", "");
        Preferences.Set("License", LicenseKey);
        Preferences.Set("IP", IpAddress);
        Preferences.Set("Scanner", SelectedScannerModel);
        Preferences.Set("DPI", SelectedDpi);
        Preferences.Set("AutoFeeder", AutoFeeder);
        Preferences.Set("Duplex", Duplex);
        Preferences.Set("ColorMode", SelectedColorMode);
        if (!previousLicense.Equals(LicenseKey))
        {
            Shell.Current.GoToAsync("../?licenseChanged=true");
        }
        else
        {
            Shell.Current.GoToAsync("../");
        }

    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
