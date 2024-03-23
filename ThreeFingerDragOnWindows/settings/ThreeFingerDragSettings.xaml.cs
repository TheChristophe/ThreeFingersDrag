using System;
using System.ComponentModel;
using Windows.System;
using Microsoft.UI.Xaml;

namespace ThreeFingerDragOnWindows.settings;

public sealed partial class ThreeFingerDragSettings : INotifyPropertyChanged
{
    public ThreeFingerDragSettings()
    {
        InitializeComponent();
    }


    public bool EnabledProperty
    {
        get => App.SettingsData.ThreeFingerDrag;
        set => App.SettingsData.ThreeFingerDrag = value;
    }

    public bool AllowReleaseAndRestartProperty
    {
        get => App.SettingsData.ThreeFingerDragAllowReleaseAndRestart;
        set => App.SettingsData.ThreeFingerDragAllowReleaseAndRestart = value;
    }

    public int ReleaseDelayProperty
    {
        get => App.SettingsData.ThreeFingerDragReleaseDelay;
        set => App.SettingsData.ThreeFingerDragReleaseDelay = value;
    }

    public bool CursorMoveProperty
    {
        get => App.SettingsData.ThreeFingerDragCursorMove;
        set => App.SettingsData.ThreeFingerDragCursorMove = value;
    }

    public float CursorSpeedProperty
    {
        get => App.SettingsData.ThreeFingerDragCursorSpeed;
        set
        {
            if (App.SettingsData.ThreeFingerDragCursorSpeed != value)
            {
                App.SettingsData.ThreeFingerDragCursorSpeed = value;
                OnPropertyChanged(nameof(CursorSpeedProperty));
            }
        }
    }

    public float CursorAccelerationProperty
    {
        get => App.SettingsData.ThreeFingerDragCursorAcceleration;
        set
        {
            if (App.SettingsData.ThreeFingerDragCursorAcceleration != value)
            {
                App.SettingsData.ThreeFingerDragCursorAcceleration = value;
                OnPropertyChanged(nameof(CursorAccelerationProperty));
            }
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        _ = Launcher.LaunchUriAsync(new Uri("ms-settings:devices-touchpad"));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}