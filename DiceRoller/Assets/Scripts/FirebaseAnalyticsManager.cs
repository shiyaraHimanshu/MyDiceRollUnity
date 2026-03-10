using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System.Collections.Generic;

public class FirebaseAnalyticsManager : MonoBehaviour
{
    public static FirebaseAnalyticsManager instance;

    private bool _isFirebaseInitialized = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                _isFirebaseInitialized = true;
                Debug.Log("Firebase Analytics Initialized Successfully");
                
                // Log an app_open event
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void LogEvent(string name)
    {
        if (!_isFirebaseInitialized) return;
        FirebaseAnalytics.LogEvent(name);
        Debug.Log($"Firebase Event Logged: {name}");
    }

    public void LogEvent(string name, string parameterName, string parameterValue)
    {
        if (!_isFirebaseInitialized) return;
        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        Debug.Log($"Firebase Event Logged: {name} | {parameterName}: {parameterValue}");
    }

    public void LogEvent(string name, string parameterName, double parameterValue)
    {
        if (!_isFirebaseInitialized) return;
        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        Debug.Log($"Firebase Event Logged: {name} | {parameterName}: {parameterValue}");
    }

    public void LogEvent(string name, Parameter[] parameters)
    {
        if (!_isFirebaseInitialized) return;
        FirebaseAnalytics.LogEvent(name, parameters);
        Debug.Log($"Firebase Event Logged: {name} with {parameters.Length} parameters");
    }

    // Helper methods for common events
    public void LogDiceRoll(int diceCount, int result, int range, string animation, string color, bool soundOn, bool vibrationOn)
    {
        Parameter[] parameters = {
            new Parameter("dice_count", diceCount),
            new Parameter("result_total", result),
            new Parameter("dice_range", range),
            new Parameter("animation_type", animation),
            new Parameter("dice_color", color),
            new Parameter("sound_enabled", soundOn ? "true" : "false"),
            new Parameter("vibration_enabled", vibrationOn ? "true" : "false")
        };
        LogEvent("dice_roll_complete", parameters);
    }

    public void LogAdEvent(string adType, string action)
    {
        Parameter[] parameters = {
            new Parameter("ad_type", adType),
            new Parameter("action", action)
        };
        LogEvent("ad_event", parameters);
    }

    public void LogSettingChanged(string settingName, string newValue)
    {
        LogEvent("setting_changed", new Parameter[] {
            new Parameter("setting_name", settingName),
            new Parameter("value", newValue)
        });
    }
}
