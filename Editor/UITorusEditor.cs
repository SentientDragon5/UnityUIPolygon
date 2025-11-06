using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI.Extensions;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UITorus), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom editor for RawImage.
    /// Extend this class to write a custom editor for a component derived from RawImage.
    /// </summary>
    public class UITorusEditor : GraphicEditor
    {

        [MenuItem("GameObject/UI/Torus", false, 4)]
        static void CreateWizard()
        {
            GameObject g = new GameObject("Torus");
            try
            {
                g.transform.parent = Selection.activeGameObject.transform;
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;
                g.AddComponent(typeof(UITorus));
                Selection.activeGameObject = g;
            }
            catch
            {
                Debug.Log("Error creating torus");
            }
        }

        SerializedProperty innerRadiusProp;
        SerializedProperty outerRadiusProp;
        SerializedProperty segmentsProp;
        SerializedProperty tubeSegmentsProp;
        SerializedProperty fillAmountProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            innerRadiusProp = serializedObject.FindProperty("innerRadius");
            outerRadiusProp = serializedObject.FindProperty("outerRadius");
            segmentsProp = serializedObject.FindProperty("segments");
            tubeSegmentsProp = serializedObject.FindProperty("tubeSegments");
            fillAmountProp = serializedObject.FindProperty("fillAmount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(innerRadiusProp);
            EditorGUILayout.PropertyField(outerRadiusProp);
            EditorGUILayout.PropertyField(segmentsProp);
            EditorGUILayout.PropertyField(tubeSegmentsProp);
            EditorGUILayout.PropertyField(fillAmountProp);

            serializedObject.ApplyModifiedProperties();

            // Draw the default Graphic inspector controls
            base.OnInspectorGUI();
        }
    }

}