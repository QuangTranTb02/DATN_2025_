using TrapSystem.Utils;
using UnityEngine;
namespace TrapSystem.Savable
{
    internal class TrapCollectionSave : MonoBehaviour {
        [SerializeField] private ScriptableTrapSave trapSaveStorage;
        [SerializeField] private TrapConfiguration[] foundTrapConfiguration;

        #if UNITY_EDITOR
        [InspectorButton(nameof(Save), ButtonWidth = 120)] 
        [SerializeField] private bool saveAllTraps;
        private void Save(){
            for(int i = 0; i < foundTrapConfiguration.Length; i++){
                TrapConfiguration config = foundTrapConfiguration[i];
                TrapDataConfiguration dataConfig = trapSaveStorage.GetOrCreate(config.TrapId);
                config.MapSavedToConfiguration(dataConfig);
            }

            UnityEditor.EditorUtility.SetDirty(trapSaveStorage);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        [InspectorButton(nameof(Load), ButtonWidth = 120)]
        [SerializeField] private bool loadAllTraps;
        private void Load(){
            for(int i = 0; i < foundTrapConfiguration.Length; i++){
                TrapConfiguration config = foundTrapConfiguration[i];
                bool hasSavedConfig = trapSaveStorage.TryGetValue(config.TrapId, out TrapDataConfiguration foundConfig);
                if(hasSavedConfig){
                    config.LoadFrom(foundConfig);
                }
            }
        }

        [InspectorButton(nameof(FindAllTraps), ButtonWidth = 200)]
        [SerializeField] private bool findAllTraps;
        private void FindAllTraps(){
            foundTrapConfiguration = GameObject.FindObjectsByType<TrapConfiguration>(FindObjectsSortMode.None);
        }
        #endif
    }
}