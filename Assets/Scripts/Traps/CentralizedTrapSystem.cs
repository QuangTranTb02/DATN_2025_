using System;
using UnityEngine;

namespace TrapSystem{
    /// <summary>
    /// This class will initialize all traps strategies, trap models, detection objects, etc.
    /// </summary>
    internal sealed class CentralizedTrapSystem : IDisposable{
        internal IVictimConverter _victimConvertHelper;

        internal CentralizedTrapSystem(){
            _victimConvertHelper = NullVictimConverter.nullConverter;
        }

        public void OnTrapRegistered(Trap trap){
            
        }
        public void OnTrapUnregistered(Trap trap){

        }

        private void OnTrapDetected(ITrapDetector detector, GameObject target)
        {
            Debug.Log($"Detected: {target.name}");
            IVictim victim = _victimConvertHelper.GetVictimFrom(target);
            if(victim == null) return;

            // TODO: retrieve a trap reaction strategy to trigger the trap logic to victim
            Debug.Log($"OnTrapDetected: {detector} -> {victim}");
        }

        public void Dispose()
        {
        }
    }

}