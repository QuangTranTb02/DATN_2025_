using UnityEngine;

namespace TrapSystem{
    public interface ITrapDetector{
        public event System.Action<ITrapDetector, GameObject> TrapDetectedEvent;
    }

    
}