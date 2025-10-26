using UnityEngine;
namespace TrapSystem
{
    [System.Serializable]
    public sealed class CapsuleConfigData : ColliderConfigData{
        public override int ColliderType => 1;
        [SerializeField] public int Direction;
        [SerializeField] public float Radius;
        [SerializeField] public float Height;

        public override ColliderConfigData Clone()
        {
            return new CapsuleConfigData(){
                Is2D = Is2D,
                Offset = Offset,
                Direction = Direction,
                Radius = Radius,
                Height = Height
            };
        }
    }
    internal sealed class CapsuleDetectorConfigData : DetectorConfigData, IColliderConfigurable<CapsuleCollider>, IColliderConfigurable<CapsuleCollider2D> 
    {
        public readonly int Direction;
        public readonly float Radius;
        public readonly float Height;

        private CapsuleDetectorConfigData(ushort colliderType, int direction, float radius, float height) : base(colliderType)
        {
            Direction = direction;
            Radius = radius;
            Height = height;
        }

        internal static CapsuleDetectorConfigData CreateConfig3D(ushort colliderType, int direction, float radius, float height){
            return new CapsuleDetectorConfigData(colliderType, direction, radius, height);
        }
        internal static CapsuleDetectorConfigData CreateConfig2D(ushort colliderType, int direction, float radius, float height){
            if(direction > 1) throw new System.ArgumentException("Direction must be 0 or 1");
            return new CapsuleDetectorConfigData(colliderType, direction, radius, height);
        }


        public void ConfigureCollider(in CapsuleCollider collider)
        {
            collider.radius = Radius;
            collider.height = Height;
            collider.direction = Direction;
        }

        public void ConfigureCollider(in CapsuleCollider2D collider)
        {
            collider.size = new Vector2(Radius * 2f, Height);
            collider.direction = (CapsuleDirection2D)Direction;
        }
    }
}