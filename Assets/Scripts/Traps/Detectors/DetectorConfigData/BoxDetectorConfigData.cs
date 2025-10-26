using UnityEngine;
namespace TrapSystem
{
    [System.Serializable]
    public sealed class BoxConfigData : ColliderConfigData
    {
        public override int ColliderType => 2;
        [SerializeField] public Vector3 Size;
        public override ColliderConfigData Clone()
        {
            return new BoxConfigData()
            {
                Is2D = Is2D,
                Offset = Offset,
                Size = Size
            };
        }
    }
    internal sealed class BoxDetectorConfigData : DetectorConfigData, IColliderConfigurable<BoxCollider2D>, IColliderConfigurable<BoxCollider>
    {
        public readonly float Width;
        public readonly float Height;
        public readonly float Depth;

        public BoxDetectorConfigData(ushort colliderType) : base(colliderType)
        {
        }

        public void ConfigureCollider(in BoxCollider2D collider)
        {
            collider.size = new Vector2(Width, Height);
        }
        public void ConfigureCollider(in BoxCollider collider)
        {
            collider.size = new Vector3(Width, Height, Depth);
        }
    }
}