#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Creature))]
public class DebugCreature : Editor {

    protected static bool leftFoldout = true;
    protected static bool rightFoldout = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Creature cScript = (Creature)target;

        EditorGUILayout.Separator();

        leftFoldout = EditorGUILayout.Foldout(leftFoldout, "Left Leg:");
        if (leftFoldout)
        {
            EditorGUILayout.LabelField("m", cScript.genome.left.m.ToString());
            EditorGUILayout.LabelField("M", cScript.genome.left.M.ToString());
            EditorGUILayout.LabelField("o", cScript.genome.left.o.ToString());
            EditorGUILayout.LabelField("p", cScript.genome.left.p.ToString());
        }

        rightFoldout = EditorGUILayout.Foldout(rightFoldout, "Right Leg:");
        if (rightFoldout)
        {
            EditorGUILayout.LabelField("m", cScript.genome.right.m.ToString());
            EditorGUILayout.LabelField("M", cScript.genome.right.M.ToString());
            EditorGUILayout.LabelField("o", cScript.genome.right.o.ToString());
            EditorGUILayout.LabelField("p", cScript.genome.right.p.ToString());
        }
    }
}

#endif
