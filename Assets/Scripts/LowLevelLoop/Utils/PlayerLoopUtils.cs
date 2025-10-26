using System;
using Unity.Collections;
using UnityEngine.LowLevel;

namespace LowLevelLoop{

    public static class PlayerLoopUtils{

        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove){
            if(loop.subSystemList == null){
                return;
            }

            for(int i = 0; i < loop.subSystemList.Length; i++){
                PlayerLoopSystem subLoop = loop.subSystemList[i];
                if(subLoop.type == systemToRemove.type && subLoop.updateDelegate == systemToRemove.updateDelegate){
                    RemoveSystemAt(i, ref loop);
                    return;
                }
            }

            HandleSubSystemForRemoval<T>(ref loop, systemToRemove);
        }

        
        private static void HandleSubSystemForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove)
        {
            if(loop.subSystemList == null) return;

            for(int i = 0; i < loop.subSystemList.Length; ++i){
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
            }
        }

        static void RemoveSystemAt(int index, ref PlayerLoopSystem loop){
            NativeArray<PlayerLoopSystem> subSystemList = new(loop.subSystemList.Length - 1, Allocator.Temp);
            for(int i = 0; i < index; i++){
                subSystemList[i] = loop.subSystemList[i];
            }
            for(int i = index; i < subSystemList.Length; i++){
                subSystemList[i] = loop.subSystemList[i + 1];
            }
            loop.subSystemList = subSystemList.ToArray();

            subSystemList.Dispose();
        }

        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index){
            if(index < 0) return false;

            if(loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

            if(index > loop.subSystemList.Length) return false;

            NativeArray<PlayerLoopSystem> subSystemList = new(loop.subSystemList.Length + 1, Allocator.Temp);
            for(int i = 0; i < index; i++){
                subSystemList[i] = loop.subSystemList[i];
            }
            subSystemList[index] = systemToInsert;
            for(int i = index + 1; i < subSystemList.Length; i++){
                subSystemList[i] = loop.subSystemList[i - 1];
            }
            loop.subSystemList = subSystemList.ToArray();

            subSystemList.Dispose();
            return true;
        }

        private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index){
            if(loop.subSystemList == null){
                return false;
            }

            for(int i = 0; i < loop.subSystemList.Length; i++){
                if(InsertSystem<T>(ref loop.subSystemList[i], systemToInsert, index)){
                    return true;
                }
            }

            return false;
        }
    }
}