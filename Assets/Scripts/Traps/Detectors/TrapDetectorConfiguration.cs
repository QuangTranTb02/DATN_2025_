using System;
using TrapSystem.Utils;
using UnityEngine;
namespace TrapSystem
{
    public class TrapDetectorConfiguration : MonoBehaviour{
        [SerializeField] int m_detectorType;
        [SerializeField] bool m_is2D; // 2D or 3D
        [SerializeReference] ColliderConfigData m_colliderConfigData;
        public DetectorConfigData GetConfiguredData(){
            throw new System.NotImplementedException();
        }

        public void LoadFromConfig(ColliderConfigData data){
            m_colliderConfigData = data.Clone();
        }
        public ColliderConfigData GetColliderConfigData() => m_colliderConfigData.Clone();

        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if(m_colliderConfigData != null){
                DrawAShapeFrom(m_colliderConfigData);
            }
        }

        private void DrawAShapeFrom(ColliderConfigData colliderConfigData)
        {
            if(colliderConfigData is BoxConfigData boxConfigData){
                Gizmos.DrawWireCube(this.transform.position + colliderConfigData.Offset, boxConfigData.Size);
            }
            else if(colliderConfigData is CapsuleConfigData capsuleConfigData){
                Vector3 direction = capsuleConfigData.Direction == 0 ? Vector3.left : Vector3.up;
                Vector3 from = this.transform.position + colliderConfigData.Offset - direction * capsuleConfigData.Height / 2f;
                Vector3 to = from + direction * capsuleConfigData.Height;
                GizmosDrawHelper.DrawWireCapsule(from, to, capsuleConfigData.Radius, Color.green);
            }
            else if(colliderConfigData is CircularConfigData circleConfigData){
                Gizmos.DrawWireSphere(this.transform.position + colliderConfigData.Offset, circleConfigData.Radius);
            }
        }
        #endif
    }
}