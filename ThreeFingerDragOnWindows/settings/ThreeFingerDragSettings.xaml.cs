using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.System;
using Microsoft.UI.Xaml;

namespace ThreeFingerDragOnWindows.settings;

public class KeyEntry
{
    public string Name { get; set; }
    public SettingsData.Key Value { get; set; }
}

public sealed partial class ThreeFingerDragSettings : INotifyPropertyChanged
{
    public ThreeFingerDragSettings()
    {
        InitializeComponent();

        var keys = new List<KeyEntry>();
        keys.Add(new KeyEntry { Name = "Left", Value = SettingsData.Key.LEFT });
        keys.Add(new KeyEntry { Name = "Right", Value = SettingsData.Key.RIGHT });
        keys.Add(new KeyEntry { Name = "Middle", Value = SettingsData.Key.MIDDLE });
        KeyBox.ItemsSource = keys;
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

    public SettingsData.Key KeyToEmulate
    {
        get => App.SettingsData.KeyToEmulate;
        set => App.SettingsData.KeyToEmulate = value;
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