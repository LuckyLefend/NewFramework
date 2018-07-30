using UnityEngine;

/// <summary>
/// 判断是否是触摸设备
/// </summary>
public static class PlatformUtil
{
    public static bool IsTouchDevice
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ||
                                    Application.platform == RuntimePlatform.Android;
        }
    }
}
