using System;
using System.Collections.Generic;
namespace LowLevelLoop
{
    public static class TimerManager{
        private static readonly List<Timer> s_timers = new();

        internal static void Clear()
        {
            s_timers.Clear();
        }

        internal static void UpdateTimers()
        {
            for(int i = s_timers.Count - 1; i >= 0; --i){
                s_timers[i].Tick();
            }
        }
    }
}