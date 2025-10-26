using UnityEngine;
namespace TrapSystem.Savable{

    /// <summary>
    /// A single trap configuration which can be saved and loaded
    /// </summary>
    internal class TrapConfiguration : MonoBehaviour{
        [SerializeField] SerializableGUID trapId;
        public SerializableGUID TrapId => trapId;
        [SerializeField] TrapDetectorConfiguration detectorConfiguration;
        public void LoadFrom(TrapDataConfiguration config){
            if(detectorConfiguration != null){
                Debug.Log($"Loading from config {config.ColliderConfigData}");
                detectorConfiguration.LoadFromConfig(config.ColliderConfigData);
            }
        }

        #if UNITY_EDITOR
        public void MapSavedToConfiguration(in TrapDataConfiguration config){
            config.ColliderConfigData = detectorConfiguration == null ? null : detectorConfiguration.GetColliderConfigData();
        }

        void OnValidate(){
            // find detector
            if(detectorConfiguration == null){
                detectorConfiguration = GetComponentInChildren<TrapDetectorConfiguration>();
            }
        }
        #endif
    }
}