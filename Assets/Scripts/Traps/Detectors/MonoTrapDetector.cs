using UnityEngine;

namespace TrapSystem{
    /// <summary>
    /// Simple detector with collider attached
    /// </summary>
    public sealed class MonoTrapDetector : MonoBehaviour, ITrapDetector{
        public event System.Action<ITrapDetector, GameObject> TrapDetectedEvent;

        void OnTriggerEnter(Collider other) => TrapDetectedEvent?.Invoke(this, other.gameObject);

        #if UNITY_EDITOR
        private void OnValidate(){
            if(null == gameObject.GetComponent<Collider>()){
                Debug.LogWarning($"MonoTrapDetector: No collider on {gameObject.name}");
            }
        }
        #endif
    }
}