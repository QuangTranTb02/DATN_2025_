using System;
using UnityEngine;

namespace TrapSystem
{
    [CreateAssetMenu(fileName = "ScriptableTrapEventSource", menuName = "Trap/TrapEventSource")]
    public sealed class ScriptableTrapEventSource : ScriptableObject, ITrapEventSource
    {
        public event Action<Trap> TrapRegisteredEvent;
        public event Action<Trap> TrapUnregisteredEvent;

        public void RegisterTrap(Trap trap){
            TrapRegisteredEvent?.Invoke(trap);
        }

        public void UnregisterTrap(Trap trap){
            TrapUnregisteredEvent?.Invoke(trap);
        }
    }
}