using UnityEngine;

namespace TrapSystem{
    [System.Serializable]
    public sealed class CircularConfigData : ColliderConfigData{
        public override int ColliderType => 0;
        [SerializeField] public float Radius;

        public override ColliderConfigData Clone()
        {
            return new CircularConfigData(){
                Is2D = Is2D,
                Offset = Offset,
                Radius = Radius
            };
        }
    } 
    internal sealed class CircularDetectorConfigData : DetectorConfigData, IColliderConfigurable<CircleCollider2D>, IColliderConfigurable<SphereCollider>
    {
        public readonly float Radius;
        public CircularDetectorConfigData(ushort colliderType, float radius) : base(colliderType)
        {
            Radius = radius;
        }

        public void ConfigureCollider(in CircleCollider2D collider)
        {
            collider.radius = Radius;
        }

        public void ConfigureCollider(in SphereCollider collider)
        {
            collider.radius = Radius;
        }
    }
}