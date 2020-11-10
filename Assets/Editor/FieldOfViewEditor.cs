using Entities.Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView) target;
            Handles.color = Color.white;
            var position = fov.transform.position;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);
            Vector3 viewAngleA = fov.DirFromAngle(-fov.ViewAngle / 2, false);
            Vector3 viewAngleB = fov.DirFromAngle(fov.ViewAngle / 2, false);

            Handles.DrawLine(position, position + viewAngleA * fov.ViewRadius);
            Handles.DrawLine(position, position + viewAngleB * fov.ViewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.VisibleTargets)
            {
                Handles.DrawLine(fov.transform.position, visibleTarget.position);
            }
        }
    }
}
