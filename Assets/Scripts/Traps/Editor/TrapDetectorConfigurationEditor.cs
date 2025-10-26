
using System;
using UnityEditor;
using UnityEngine;

namespace TrapSystem.Editor{

    [CustomEditor(typeof(TrapDetectorConfiguration))]
    public class TrapDetectorConfigurationEditor : UnityEditor.Editor
    {
        private static readonly string[] s_ColliderNames = new string[] { "Circular", "Capsule", "Box" };
        private static readonly int[] s_ColliderValues = new int[] { 0, 1, 2 };

        SerializedProperty m_detectorTypeProp, m_is2DTypeProp, m_colliderConfigDataProp;

        void OnEnable(){
            m_detectorTypeProp = serializedObject.FindProperty("m_detectorType");
            m_colliderConfigDataProp = serializedObject.FindProperty("m_colliderConfigData");
            if(m_colliderConfigDataProp.managedReferenceValue != null){
                m_detectorTypeProp.intValue = (m_colliderConfigDataProp.managedReferenceValue as ColliderConfigData).ColliderType;
            }
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI(){
            
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            int detectorType = m_detectorTypeProp.intValue;
            detectorType = EditorGUILayout.IntPopup("Detector Type", detectorType, s_ColliderNames, s_ColliderValues);

            if(detectorType != m_detectorTypeProp.intValue || m_colliderConfigDataProp.managedReferenceValue == null){
                m_colliderConfigDataProp.managedReferenceValue = CreateColliderConfigData(detectorType);
            }

            // handle generic fields
            m_is2DTypeProp = m_colliderConfigDataProp.FindPropertyRelative("Is2D");
            m_is2DTypeProp.boolValue = EditorGUILayout.Toggle("Is 2D Collider", m_is2DTypeProp.boolValue, GUILayout.MaxWidth(200));
            
            SerializedProperty offsetProp = m_colliderConfigDataProp.FindPropertyRelative("Offset");
            if(m_is2DTypeProp.boolValue){
                offsetProp.vector3Value = EditorGUILayout.Vector2Field(label: "Offset",offsetProp.vector3Value);
            }
            else{
                offsetProp.vector3Value = EditorGUILayout.Vector3Field(label: "Offset",offsetProp.vector3Value);
            }

            // handle render fields each case
            switch (detectorType){
                case 0: // sphere
                HandleSphereConfig(m_colliderConfigDataProp);
                break;
                case 1: // capsule
                HandleCapsuleConfig(m_colliderConfigDataProp);
                break;
                case 2: // box
                HandleBoxConfig(m_colliderConfigDataProp);
                break;
            }

            m_detectorTypeProp.intValue = detectorType;
            serializedObject.ApplyModifiedProperties();
        }

        private object CreateColliderConfigData(int detectorType)
        {
            return detectorType switch
            {
                // sphere
                0 => new CircularConfigData(),
                // capsule
                1 => new CapsuleConfigData(),
                // box
                2 => new BoxConfigData(),
                _ => throw new System.NotImplementedException($"Unknown detector type: {detectorType}"),
            };
        }

        void HandleSphereConfig(SerializedProperty property){
            SerializedProperty radiusProp = property.FindPropertyRelative("Radius");
            EditorGUILayout.LabelField("Circular Configuration", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            {
                float maxWidth = 200f;
                float reservedWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = maxWidth / 2f;
                GUILayoutOption layoutOption = GUILayout.MaxWidth(maxWidth);

                radiusProp.floatValue = EditorGUILayout.FloatField(label: "Radius", radiusProp.floatValue, layoutOption);
                
                EditorGUIUtility.labelWidth = reservedWidth;
            }
            EditorGUI.indentLevel--;
        }
        
        void HandleBoxConfig(SerializedProperty property){
            SerializedProperty sizeProp = property.FindPropertyRelative("Size");
            EditorGUILayout.LabelField("Box Configuration", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            sizeProp.vector3Value = EditorGUILayout.Vector3Field(label: "Size",sizeProp.vector3Value);
            EditorGUI.indentLevel--;
        }

        #region Capsule Config
        private static readonly string[] s_CapsuleDirectionNames = new string[] { "X-axis", "Y-axis", "Z-axis" };
        private static readonly int[] s_CapsuleDirectionValues = new int[] { 0, 1, 2 };
        void HandleCapsuleConfig(SerializedProperty property){
            SerializedProperty directionProp = property.FindPropertyRelative("Direction");
            SerializedProperty radiusProp = property.FindPropertyRelative("Radius");
            SerializedProperty heightProp = property.FindPropertyRelative("Height");
            EditorGUILayout.LabelField("Capsule Configuration", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            {
                if (m_is2DTypeProp.boolValue == true)
                {
                    s_CapsuleDirectionNames[2] = null;
                }
                else
                {
                    s_CapsuleDirectionNames[2] = "Z-axis";
                }

                float maxWidth = 200f;
                float reservedWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = maxWidth / 2f;
                GUILayoutOption layoutOption = GUILayout.MaxWidth(maxWidth);

                directionProp.intValue = EditorGUILayout.IntPopup(label: "Direction", directionProp.intValue, s_CapsuleDirectionNames, s_CapsuleDirectionValues, layoutOption);
                radiusProp.floatValue = EditorGUILayout.FloatField(label: "Radius", radiusProp.floatValue, layoutOption);
                heightProp.floatValue = EditorGUILayout.FloatField(label: "Height", heightProp.floatValue, layoutOption);

                EditorGUIUtility.labelWidth = reservedWidth;
            }
            EditorGUI.indentLevel--;
        }
        #endregion
    }
}