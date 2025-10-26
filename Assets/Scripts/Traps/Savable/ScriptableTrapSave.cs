using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
namespace TrapSystem.Savable{

    [CreateAssetMenu(fileName = "ScriptableTrapSave", menuName = "Trap/ScriptableTrapSave", order = 0)]
    internal class ScriptableTrapSave : ScriptableObject{
        [SerializeField, SerializedDictionary("Trap Id", "Trap Configuration")] 
        private SerializedDictionary<SerializableGUID, TrapDataConfiguration> savableTrapConfigurations = new();

        public TrapDataConfiguration this[SerializableGUID guid] => savableTrapConfigurations[guid];

        public bool TryGetValue(SerializableGUID trapId, out TrapDataConfiguration config){
            return savableTrapConfigurations.TryGetValue(trapId, out config);
        }

        public void Add(SerializableGUID trapId, TrapDataConfiguration config){
            savableTrapConfigurations.Add(trapId, config);
        }
        public void Remove(SerializableGUID trapId){
            savableTrapConfigurations.Remove(trapId);
        }

        internal TrapDataConfiguration GetOrCreate(SerializableGUID trapId)
        {
            if(savableTrapConfigurations.TryGetValue(trapId, out TrapDataConfiguration config)){
                return config;
            }
            config = new TrapDataConfiguration();
            savableTrapConfigurations.Add(trapId, config);
            return config;
        }
    }
}