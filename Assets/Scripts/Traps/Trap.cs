using System;
using UnityEngine;
namespace TrapSystem{
    /// <summary>
    /// Trap attached to object
    /// </summary>
    public abstract class Trap : MonoBehaviour{
        public event Action<bool> TrapActiveEvent;
        public abstract void TriggerTrap(IVictim victim);


        private void OnEnable(){
            TrapActiveEvent?.Invoke(true);
            OnAfterTrapActivated();
        }

        private void OnDisable(){
            TrapActiveEvent?.Invoke(false);
            OnAfterTrapInactivated();
        }

        protected virtual void OnAfterTrapActivated(){}
        protected virtual void OnAfterTrapInactivated(){}
    }
}