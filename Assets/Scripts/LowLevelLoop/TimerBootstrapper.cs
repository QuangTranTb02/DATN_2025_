using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
namespace LowLevelLoop{
    internal static class TimerBootstrapper{
        static PlayerLoopSystem timerSystem;

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        // internal static void Initialize(){
        //     PlayerLoopSystem currentLoop = PlayerLoop.GetCurrentPlayerLoop();

        //     if(!InsertTimerManager<Update>(ref currentLoop, 0)){
        //         Debug.LogWarning("Failed to insert TimerManager");
        //         return;
        //     }

        //     #if UNITY_EDITOR
        //     UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        //     UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        //     #endif
        // }

        #if UNITY_EDITOR
        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if(state == UnityEditor.PlayModeStateChange.ExitingPlayMode){
                PlayerLoopSystem currentLoop = PlayerLoop.GetCurrentPlayerLoop();
                RemoveTimerManager<Update>(ref currentLoop);
                PlayerLoop.SetPlayerLoop(currentLoop);

                TimerManager.Clear();
            }
        }
        #endif

        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index){
            // todo check if can insert at index

            timerSystem = new PlayerLoopSystem(){
                type = typeof(TimerManager),
                updateDelegate = TimerManager.UpdateTimers,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index);
        }

        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop){
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in timerSystem);
        }
    }
}
