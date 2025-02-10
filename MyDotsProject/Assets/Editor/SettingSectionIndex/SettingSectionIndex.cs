using UnityEngine;
using UnityEditor;

public class SettingSectionIndex : EditorWindow
{
    public GameObject[] objs;
    SerializedObject so;

    [MenuItem("MyDotsProject/SettingSectionIndex")]
    private static void ShowWindow()
    {
        var window = GetWindow<SettingSectionIndex>();
        window.titleContent = new GUIContent("SettingSectionIndex");
        window.maxSize = new Vector2(300.0f, 500.0f);
        window.minSize = new Vector2(300.0f, 500.0f);
        window.Show();
    }

    private void OnEnable() {
        ScriptableObject target = this;
        so = new SerializedObject(target);
    }

    private void OnGUI()
    {
        GUILayout.Label("Array:");
        so.Update();
        SerializedProperty titleProperty = so.FindProperty("objs");
        EditorGUILayout.PropertyField(titleProperty, true);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Button"))
        {
            foreach (GameObject obj in objs)
            {
                Debug.Log(obj.name);
            }
        }
    }
}