using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI.Extensions;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UICurve), true)]
    [CanEditMultipleObjects]
    public class UICurveEditor : GraphicEditor
    {

        [MenuItem("GameObject/UI/Curve", false, 4)]
        static void CreateWizard()
        {
            GameObject g = new GameObject("Curve");
            try
            {
                g.transform.parent = Selection.activeGameObject.transform;
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;
                g.AddComponent(typeof(UICurve));
                Selection.activeGameObject = g;
                g.GetComponent<UICurve>().points.Add(new Vector2(0, 0));
                g.GetComponent<UICurve>().points.Add(new Vector2(0.5f, 0));
                g.GetComponent<UICurve>().points.Add(new Vector2(0, 0.5f));
            }
            catch
            {
                Debug.Log("Error creating curve");
            }
        }

        SerializedProperty points;
        SerializedProperty resolution;
        SerializedProperty curveColor;
        SerializedProperty thickness;
        SerializedProperty fillAmount; // Add this line

        protected override void OnEnable()
        {
            base.OnEnable();

            points = serializedObject.FindProperty("points");
            resolution = serializedObject.FindProperty("resolution");
            curveColor = serializedObject.FindProperty("curveColor");
            thickness = serializedObject.FindProperty("thickness");
            fillAmount = serializedObject.FindProperty("fillAmount"); // Add this line
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(points);
            EditorGUILayout.PropertyField(resolution);
            EditorGUILayout.PropertyField(curveColor);
            EditorGUILayout.PropertyField(thickness);
            EditorGUILayout.PropertyField(fillAmount); // Add this line

            serializedObject.ApplyModifiedProperties();
        }

        // OnSceneGUI for visualizing and editing the curve points
        protected virtual void OnSceneGUI()
        {
            UICurve curve = (UICurve)target;

            // Draw and edit the points
            for (int i = 0; i < curve.points.Count; i++)
            {
                Vector3 point = curve.transform.TransformPoint(curve.points[i]);
                EditorGUI.BeginChangeCheck();
                point = Handles.PositionHandle(point, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    curve.points[i] = curve.transform.InverseTransformPoint(point);

                    curve.SetVerticesDirty();
                }
                Handles.Label(point, i.ToString());
            }
        }
    }
}