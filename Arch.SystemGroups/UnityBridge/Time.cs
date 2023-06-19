namespace Arch.SystemGroups.UnityBridge;

/// <summary>
/// Can't call Unity API without Unity running
/// </summary>
internal static class Time
{
    public static float DeltaTime
    {
        get
        {
#if OUTSIDE_UNITY
            return 0;
#else
            return UnityEngine.Time.deltaTime;
#endif
        }
    }

    public static float FixedDeltaTime
    {
        get
        {
#if OUTSIDE_UNITY
            return 0;
#else
            return UnityEngine.Time.fixedDeltaTime;
#endif
        }
    }
}