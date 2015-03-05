using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DeltaDNA
{

/// <summary>
/// Notifications Plugin enables a game to register with Apple's push notification service.  It provides
/// some additional functionality not easily accessible from Unity.  By using the events, a game can be
/// notified when a game has registered with the service and when push notification has occured.  We use
/// these events to log notifications with the DeltaDNA platform.
/// </summary>
public class NotificationsPlugin : MonoBehaviour
{
	// Called with JSON string of the notification payload.
	public static event Action<string> OnDidLaunchWithPushNotification;

	// Called with JSON string of the notification payload.
	public static event Action<string> OnDidReceivePushNotification;

	// Called with the deviceToken.
	public static event Action<string> OnDidRegisterForPushNotifications;

	// Called with the error string.
	public static event Action<string> OnDidFailToRegisterForPushNotifications;

	void Awake()
	{
		gameObject.name = this.GetType().ToString();
		DontDestroyOnLoad(this);
	}

    #if UNITY_IPHONE

//    [DllImport("__Internal")]
//    private static extern void _registerForPushNotifications();

	/// <summary>
	/// Registers for push notifications.  Only iOS supported.
	/// </summary>
    public static void RegisterForPushNotifications()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
			NotificationServices.RegisterForRemoteNotificationTypes(
				RemoteNotificationType.Alert |
				RemoteNotificationType.Badge |
				RemoteNotificationType.Sound);
        }
    }

//    [DllImport("__Internal")]
//    private static extern void _unregisterForPushNotifications();

	/// <summary>
	/// Unregisters for push notifications.  Only iOS supported.
	/// </summary>
    public static void UnregisterForPushNotifications()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            NotificationServices.UnregisterForRemoteNotifications();
        }
    }

	#region Native Bridge

    public void DidLaunchWithPushNotification(string notification)
    {
		Logger.LogDebug("Did launch with push notification");

    	var payload = DeltaDNA.MiniJSON.Json.Deserialize(notification) as Dictionary<string, object>;
    	DDNA.Instance.RecordPushNotification(payload);

    	if (OnDidLaunchWithPushNotification != null) {
    		OnDidLaunchWithPushNotification(notification);
    	}
    }

    public void DidReceivePushNotification(string notification)
    {
		Logger.LogDebug("Did receive push notification");

		var payload = DeltaDNA.MiniJSON.Json.Deserialize(notification) as Dictionary<string, object>;
		DDNA.Instance.RecordPushNotification(payload);

    	if (OnDidReceivePushNotification != null) {
    		OnDidReceivePushNotification(notification);
    	}
    }

    public void DidRegisterForPushNotifications(string deviceToken)
    {
		Logger.LogDebug("Did register for push notifications: "+deviceToken);

        DDNA.Instance.PushNotificationToken = deviceToken;

        if (OnDidRegisterForPushNotifications != null) {
            OnDidRegisterForPushNotifications(deviceToken);
        }
    }

    public void DidFailToRegisterForPushNotifications(string error)
    {
		Logger.LogDebug("Did fail to register for push notifications: "+error);

        if (OnDidFailToRegisterForPushNotifications != null) {
            OnDidFailToRegisterForPushNotifications(error);
        }
    }

    #endregion

    #endif // UNITY_IPHONE
}

} // namespace DeltaDNA
