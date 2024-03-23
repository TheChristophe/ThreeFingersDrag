using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using ThreeFingerDragEngine.utils;
using ThreeFingerDragOnWindows.settings;
using ThreeFingerDragOnWindows.utils;

namespace ThreeFingerDragOnWindows.threefingerdrag;

public class ThreeFingerDrag
{
    public const int RELEASE_FINGERS_THRESHOLD_MS = 30; // Windows Precision Touchpad sends contacts about every 10ms

    private readonly DistanceManager _distanceManager = new();
    private readonly Timer _dragEndTimer = new();
    private readonly FingerCounter _fingerCounter = new();
    private bool _isDragging;

    public ThreeFingerDrag()
    {
        _dragEndTimer.AutoReset = false;
        _dragEndTimer.Elapsed += OnTimerElapsed;
    }

    public void OnTouchpadContact(TouchpadContact[] oldContacts, TouchpadContact[] contacts, long elapsed)
    {
        var hasFingersReleased = elapsed > RELEASE_FINGERS_THRESHOLD_MS;
        Debug.WriteLine("\nTFD: " + string.Join(", ", oldContacts.Select(c => c.ToString())) + " | " +
                        string.Join(", ", contacts.Select(c => c.ToString())) + " | " + elapsed);
        var areContactsIdsCommons = FingerCounter.AreContactsIdsCommons(oldContacts, contacts);

        var (_, longestDistDelta, longestDist2D) =
            _distanceManager.GetLongestDist2D(oldContacts, contacts, hasFingersReleased);
        var (fingersCount, shortDelayMovingFingersCount, longDelayMovingFingersCount, originalFingersCount) =
            _fingerCounter.CountMovingFingers(contacts, areContactsIdsCommons, longestDist2D, hasFingersReleased);

        Debug.WriteLine("    fingers: " + fingersCount + ", original: " + originalFingersCount + ", moving: " +
                        shortDelayMovingFingersCount + "/" + longDelayMovingFingersCount + ", dist: " + longestDist2D);

        if (fingersCount >= 3 && areContactsIdsCommons && longDelayMovingFingersCount == 3 &&
            originalFingersCount == 3 && !_isDragging)
        {
            // Start dragging
            _isDragging = true;
            Debug.WriteLine("    START DRAG, Left click down");
            switch (App.SettingsData.KeyToEmulate)
            {
                case SettingsData.Key.LEFT:
                    MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_LEFTDOWN);
                    break;
                case SettingsData.Key.RIGHT:
                    MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_RIGHTDOWN);
                    break;
                case SettingsData.Key.MIDDLE:
                    MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_MIDDLEDOWN);
                    break;
            }
        }
        else if ((shortDelayMovingFingersCount < 2 || (originalFingersCount != 3 && originalFingersCount >= 2)) &&
                 _isDragging)
        {
            // Stop dragging
            // Condition over originalFingersCount to catch cases where the drag has continued with only two or four fingers
            Debug.WriteLine("    STOP DRAG, Left click up");
            StopDrag();
        }
        else if (fingersCount >= 2 && originalFingersCount == 3 && areContactsIdsCommons && _isDragging)
        {
            // Dragging
            if (App.SettingsData.ThreeFingerDragCursorMove)
            {
                Debug.WriteLine("    MOVING, (x, y) = (" + longestDistDelta.x + ", " + longestDistDelta.y + ")");

                var delta = DistanceManager.ApplySpeedAndAcc(longestDistDelta, (int)elapsed);
                MouseOperations.ShiftCursorPosition(delta.x, delta.y);

                _dragEndTimer.Stop();
                _dragEndTimer.Interval = GetReleaseDelay();
                _dragEndTimer.Start();
            }
        }
    }

    private void OnTimerElapsed(object source, ElapsedEventArgs e)
    {
        if (_isDragging)
        {
            Debug.WriteLine("    STOP DRAG FROM TIMER, Left click up");
            StopDrag();
        }
    }

    private void StopDrag()
    {
        _isDragging = false;
        switch (App.SettingsData.KeyToEmulate)
        {
            case SettingsData.Key.LEFT:
                MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_LEFTUP);
                break;
            case SettingsData.Key.RIGHT:
                MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_RIGHTUP);
                break;
            case SettingsData.Key.MIDDLE:
                MouseOperations.MouseClick(MouseOperations.MOUSEEVENTF_MIDDLEUP);
                break;
        }
    }

    private int GetReleaseDelay()
    {
        // Delay after which the click is released if no input is detected
        return App.SettingsData.ThreeFingerDragAllowReleaseAndRestart
            ? Math.Max(App.SettingsData.ThreeFingerDragReleaseDelay, RELEASE_FINGERS_THRESHOLD_MS)
            : RELEASE_FINGERS_THRESHOLD_MS;
    }
}