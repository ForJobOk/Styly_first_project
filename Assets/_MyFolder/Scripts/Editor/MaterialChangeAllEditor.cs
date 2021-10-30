using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialChangeAll))]
public class MaterialChangeAllEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var rootClass = target as MaterialChangeAll;

        //少しスペースを空ける
        EditorGUILayout.Space();

        // 押下時に実行したい処理
        if (GUILayout.Button("一括変更"))
        {
            rootClass?.ChangeAll();
        }
    }
}