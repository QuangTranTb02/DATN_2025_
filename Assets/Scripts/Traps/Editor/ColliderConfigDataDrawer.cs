using TrapSystem.Savable;
using UnityEditor;
using UnityEngine;

namespace TrapSystem.Editor
{
    [CustomPropertyDrawer(typeof(ColliderConfigData))]
    public class ColliderConfigDataDrawer : PropertyDrawer
    {

        private bool m_isExpanded;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_isExpanded = EditorGUI.Foldout(new Rect(10f, 0f, 40f, 20f), m_isExpanded, label);
            if (m_isExpanded == false) return;

            position.position += new Vector2(10f, 20f);

            property.Next(true); // get first child of your property
            do
            {
                EditorGUI.PropertyField(position, property, true); // Include children
                position.y += EditorGUIUtility.singleLineHeight + 2;
            }
            while (property.Next(false)); // get property's next sibling
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_isExpanded == false) return 20f;

            property.Next(true);
            float result = 20f;
            do
            {
                result += EditorGUI.GetPropertyHeight(property, true) + 2f; // include children
            }
            while (property.Next(false));
            return result;
        }
    }
}