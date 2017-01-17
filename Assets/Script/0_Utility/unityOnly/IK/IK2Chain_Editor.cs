using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(IK2Chain))]
public class IK2Chan_Editor : Editor 
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		IK2Chain myScript = (IK2Chain)target;

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button(myScript.invert ? "Inverted" : "Not Inveted"))
            myScript.invert = !myScript.invert;

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button (myScript.toggleIK ? "IK ON" : "IK OFF")) 
		{
			//true -> false
			if(true == myScript.toggleIK)
				myScript.ToggleOff ();
			//false -> true
			else
				myScript.ToggleOn ();
			
		}

//        if (GUILayout.Button("Update Bones"))
//            myScript.UpdateBones();

        EditorGUILayout.EndHorizontal();
    }

}
#endif