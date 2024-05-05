using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(objectPoolTesting))]
public class objectPoolTestingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        objectPoolTesting OPT = (objectPoolTesting)target;

        if (GUILayout.Button("Test"))
        {
            OPT.Test();
        }
    }
}
#endif