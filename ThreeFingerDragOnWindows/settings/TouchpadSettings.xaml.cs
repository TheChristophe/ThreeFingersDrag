using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ThreeFingerDragOnWindows.settings;

public sealed partial class TouchpadSettings
{
    public TouchpadSettings()
    {
        InitializeComponent();
        if (App.Instance.HandlerWindow == null || !App.Instance.HandlerWindow.TouchpadInitialized)
        {
            Loader.Visibility = Visibility.Visible;
            TouchpadStatus.Visibility = Visibility.Collapsed;
            ContactsDebug.Visibility = Visibility.Collapsed;
        }
        else
        {
            OnTouchpadInitialized();
        }
    }

    public bool RegularTouchpadCheckProperty
    {
        get => App.SettingsData.RegularTouchpadCheck;
        set => App.SettingsData.RegularTouchpadCheck = value;
    }

    public int RegularTouchpadCheckIntervalProperty
    {
        get => App.SettingsData.RegularTouchpadCheckInterval;
        set => App.SettingsData.RegularTouchpadCheckInterval = value;
    }

    public bool RegularTouchpadCheckEvenAlreadyRegisteredProperty
    {
        get => App.SettingsData.RegularTouchpadCheckEvenAlreadyRegistered;
        set => App.SettingsData.RegularTouchpadCheckEvenAlreadyRegistered = value;
    }

    public void UpdateContactsText(string text)
    {
        ContactsDebug.Title = "Inputs:\n" + text;
    }

    public void OnTouchpadInitialized()
    {
        Loader.Visibility = Visibility.Collapsed;
        TouchpadStatus.Visibility = Visibility.Visible;

        if (App.Instance.HandlerWindow.TouchpadExists)
        {
            if (App.Instance.HandlerWindow.TouchpadRegistered)
            {
                TouchpadStatus.Title = "Touchpad exists and is registered !";
                TouchpadStatus.Severity = InfoBarSeverity.Success;
                ContactsDebug.Visibility = Visibility.Visible;
            }
            else
            {
                TouchpadStatus.Title = "Touchpad exists, but can't be registered.";
                TouchpadStatus.Severity = InfoBarSeverity.Warning;
                ContactsDebug.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            TouchpadStatus.Title = "Touchpad not detected, make sure to have a Windows Precision compatible touchpad.";
            TouchpadStatus.Severity = InfoBarSeverity.Error;
            ContactsDebug.Visibility = Visibility.Collapsed;
        }
    }
}