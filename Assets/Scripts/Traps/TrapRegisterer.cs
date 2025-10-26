using System;
using UnityEngine;

namespace TrapSystem{
    internal sealed class TrapRegisterer : MonoBehaviour{
        [SerializeField] private ScriptableTrapEventSource trapEventSource;
        [SerializeField] private Trap trap;

        private void OnTrapActive(bool active)
        {
            if(active){
                trapEventSource.RegisterTrap(trap);
            }
            else{
                trapEventSource.UnregisterTrap(trap);
            }
        }

        void OnDestroy(){
            if(trap != null){
                trap.TrapActiveEvent -= OnTrapActive;
            }
        }

#if UNITY_EDITOR
        private Trap m_referencedTrap;
        void OnValidate(){
            if(m_referencedTrap != null){
                m_referencedTrap.TrapActiveEvent -= OnTrapActive;
            }
            
            m_referencedTrap = trap;

            if(m_referencedTrap != null){
                m_referencedTrap.TrapActiveEvent += OnTrapActive;
            }
        }
#endif
    }
}