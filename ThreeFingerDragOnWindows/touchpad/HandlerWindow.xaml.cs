using System;
using System.Diagnostics;
using System.Timers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using ThreeFingerDragEngine.utils;
using ThreeFingerDragOnWindows.threefingerdrag;
using ThreeFingerDragOnWindows.utils;

namespace ThreeFingerDragOnWindows.touchpad;

public sealed partial class HandlerWindow : Window
{
    private readonly App _app;
    private readonly ContactsManager _contactsManager;
    private readonly ThreeFingerDrag _threeFingersDrag;
    private long _lastContactCtms = Ctms();

    // Called when a new set of contacts has been registered

    private TouchpadContact[] _oldContacts = Array.Empty<TouchpadContact>();
    public bool TouchpadExists;

    public bool
        TouchpadInitialized; // Became true when the touchpad check is done, but does not confirm that the touchpad has been registered, see TouchpadRegistered

    public bool TouchpadRegistered;

    public HandlerWindow(App app)
    {
        Debug.WriteLine("Starting HandlerWindow...");
        InitializeComponent();

        _app = app;
        _contactsManager = new ContactsManager(this);
        _threeFingersDrag = new ThreeFingerDrag();

        // Let the _handlerWindow to be defined in App.xaml.cs before initializing the source
        var queue = DispatcherQueue.GetForCurrentThread();
        Utils.runOnMainThreadAfter(100, () =>
        {
            _contactsManager.InitializeSource();

            var timer = new Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += (sender, args) => queue.TryEnqueue(() =>
            {
                if (!App.SettingsData.RegularTouchpadCheck)
                {
                    timer.Interval = 5000;
                    return;
                }

                if (App.SettingsData.RegularTouchpadCheckInterval > 0)
                    timer.Interval = App.SettingsData.RegularTouchpadCheckInterval * 1000;
                if (TouchpadInitialized &&
                    (!TouchpadRegistered || App.SettingsData.RegularTouchpadCheckEvenAlreadyRegistered))
                    _contactsManager.InitializeSource();
            });
            timer.Start();
        });
    }

    // TaskbarIcon Actions
    private void OpenSettingsWindow(object sender, ExecuteRequestedEventArgs e)
    {
        Debug.WriteLine("Opening SettingsWindow from HandlerWindow TaskbarIcon");
        _app.OpenSettingsWindow();
    }

    private void QuitApp(object sender, ExecuteRequestedEventArgs e)
    {
        Debug.WriteLine("Quitting App from HandlerWindow TaskbarIcon");
        _app.Quit();
    }


    // Touchpad
    // Called when the touchpad is detected and the events handlers are registered (or not)
    public void OnTouchpadInitialized(bool touchpadExists, bool touchpadRegistered)
    {
        TouchpadExists = touchpadExists;
        TouchpadRegistered = touchpadRegistered;
        if (!touchpadExists) Debug.WriteLine("Touchpad is not detected.");
        else if (!touchpadRegistered) Debug.WriteLine("Touchpad is detected but not registered.");
        else Debug.WriteLine("Touchpad is detected and registered.");

        TouchpadInitialized = true;
        _app.OnTouchpadInitialized();
    }

    public void OnTouchpadContact(TouchpadContact[] contacts)
    {
        if (App.SettingsData.ThreeFingerDrag)
            _threeFingersDrag.OnTouchpadContact(_oldContacts, contacts, Ctms() - _lastContactCtms);

        _app.OnTouchpadContact(contacts); // Transfer to App for displaying contacts in SettingsWindow
        _lastContactCtms = Ctms();
        _oldContacts = contacts;
    }

    private static long Ctms()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }
}