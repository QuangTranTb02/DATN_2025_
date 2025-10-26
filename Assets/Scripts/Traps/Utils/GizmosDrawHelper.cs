#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TrapSystem.Utils
{
    public static class GizmosDrawHelper
    {
        #region EDITOR
#if UNITY_EDITOR
        public static void DrawWireCapsule(Vector3 pointA, Vector3 pointB, float radius, Color color = default)
        {
            if (color != default) Handles.color = color;

            Vector3 forward = pointB - pointA;
            Quaternion rotation = Quaternion.LookRotation(forward);
            float pointOffset = radius / 2f;
            float length = forward.magnitude;
            

            Matrix4x4 angleMatrix = Matrix4x4.TRS(pointA, rotation, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(angleMatrix))
            {
                // draw center 1
                var center1 = Vector3.zero;
                Handles.DrawWireDisc(center1, Vector3.forward, radius);
                Handles.DrawWireArc(center1, Vector3.up, Vector3.left * pointOffset, -180f, radius);
                Handles.DrawWireArc(center1, Vector3.left, Vector3.down * pointOffset, -180f, radius);
                
                // draw center 2
                var center2 = new Vector3(0f, 0f, length);
                Handles.DrawWireDisc(center2, Vector3.forward, radius);
                Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, radius);
                Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, radius);

                // draw horizontal lines in order: right, left, top, bottom
                DrawLine(radius, 0f, length);
                DrawLine(-radius, 0f, length);
                DrawLine(0f, radius, length);
                DrawLine(0f, -radius, length);
            }
        }

        private static void DrawLine(float arg1, float arg2, float forward)
        {
            Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }
#endif
        #endregion
    }
}