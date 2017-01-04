using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(IKChain))]
public class IKChan_Editor : Editor 
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		IKChain myScript = (IKChain)target;

        
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

        EditorGUILayout.EndHorizontal();
    }

}
#endif