using UnityEngine;

namespace TrapSystem{
    public sealed class MonoTrapDetector2D : MonoBehaviour, ITrapDetector{
        public event System.Action<ITrapDetector, GameObject> TrapDetectedEvent;

        void OnTriggerEnter2D(Collider2D other) => TrapDetectedEvent?.Invoke(this, other.gameObject);

        #if UNITY_EDITOR
        void OnValidate(){
            if(null == gameObject.GetComponent<Collider2D>()){
                Debug.LogWarning($"MonoTrapDetector2D: No collider on {gameObject.name}");
            }
        }
        #endif
    }
}