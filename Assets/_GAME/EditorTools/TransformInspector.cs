//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//[ExecuteInEditMode]
//[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
//public class TransformInspector : Editor
//{
//    private SerializedProperty positionPropety;

//    public void OnEnable()
//    {
//        this.positionPropety = this.serializedObject.FindProperty("m_LocalPosition");
//    }

//    public override void OnInspectorGUI()
//    {
//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button(new GUIContent("P", "Reset Position"), GUILayout.Width(20)))
//        {
//            positionPropety.vector3Value = new Vector3(0, 0, 0);
//        }
//        EditorGUILayout.EndHorizontal();
//    }
//}
