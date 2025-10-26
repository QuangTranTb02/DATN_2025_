using UnityEngine;

namespace TrapSystem
{
    public abstract class DetectorConfigData{
        public readonly ushort ColliderType;
        internal DetectorConfigData(ushort colliderType){
            ColliderType = colliderType;
        }
    }
    [System.Serializable]
    public abstract class ColliderConfigData{
        public abstract int ColliderType {get;}
        [SerializeField] public bool Is2D;
        [SerializeField] public Vector3 Offset;

        public abstract ColliderConfigData Clone();
    }

    internal interface IColliderConfigurable<TCollider>{
        void ConfigureCollider(in TCollider collider);
    }
}