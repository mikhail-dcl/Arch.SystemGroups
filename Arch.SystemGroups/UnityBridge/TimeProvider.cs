using System.Runtime.InteropServices;

namespace Arch.SystemGroups.UnityBridge
{
    /// <summary>
    ///     Can't call Unity API without Unity running
    /// </summary>
    public static class TimeProvider
    {
        /// <summary>
        /// Information about time, contains Fixed Time for Physics Systems Groups and Time for the rest
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct Info
        {
            /// <summary>
            /// <see cref="UnityEngine.Time.deltaTime"/> or <see cref="UnityEngine.Time.fixedDeltaTime"/>
            /// </summary>
            public readonly float DeltaTime;
        
            /// <summary>
            /// <see cref="UnityEngine.Time.unscaledDeltaTime"/> or <see cref="UnityEngine.Time.fixedUnscaledDeltaTime"/>
            /// </summary>
            public readonly float CurrentUnscaledTime;
        
            /// <summary>
            /// <see cref="UnityEngine.Time.time"/> or <see cref="UnityEngine.Time.fixedTime"/>
            /// </summary>
            public readonly float CurrentScaledTime;
        
            /// <summary>
            /// <see cref="UnityEngine.Time.realtimeSinceStartup"/>
            /// </summary>
            public readonly float Realtime;

            internal Info(float deltaTime, float currentUnscaledTime, float currentScaledTime, float realtime)
            {
                DeltaTime = deltaTime;
                CurrentUnscaledTime = currentUnscaledTime;
                CurrentScaledTime = currentScaledTime;
                Realtime = realtime;
            }
        }
    
        internal static Info GetFixedInfo() => new (FixedDeltaTime, UnscaledFixedDeltaTime, ScaledFixedTime, Realtime);
    
        internal static Info GetInfo() => new (DeltaTime, UnscaledDeltaTime, ScaledTime, Realtime);
    
        internal static float DeltaTime
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

        internal static float FixedDeltaTime
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

        internal static float ScaledTime
        {
            get
            {
#if OUTSIDE_UNITY
            return 0;
#else
                return UnityEngine.Time.time;
#endif
            }
        }

        internal static float ScaledFixedTime
        {
            get
            {
#if OUTSIDE_UNITY
            return 0;
#else
                return UnityEngine.Time.fixedTime;
#endif
            }
        }

        internal static float UnscaledDeltaTime
        {
            get
            {
#if OUTSIDE_UNITY
            return 0;
#else
                return UnityEngine.Time.unscaledDeltaTime;
#endif
            }
        }

        internal static float UnscaledFixedDeltaTime
        {
            get
            {
#if OUTSIDE_UNITY
            return 0;
#else
                return UnityEngine.Time.fixedUnscaledDeltaTime;
#endif
            }
        }

        internal static float Realtime
        {
            get
            {
#if OUTSIDE_UNITY
            return 0;
#else
                return UnityEngine.Time.realtimeSinceStartup;
#endif
            }
        }
    }
}