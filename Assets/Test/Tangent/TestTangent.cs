using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestTangent : MonoBehaviour 
{
	public Transform _joint = null;	
	public float 	_rot = 0;

	// Use this for initialization
	void Start () 
	{
		for (float t = 0; t < 360f; t++) 
		{
			
			//DebugWide.LogBlue (Mathf.Tan ());
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		_joint.Rotate (_rot, 0, 0);
	}


	void OnDrawGizmos()
	{
		if (false == Selection.Contains (_joint.gameObject))
			return;
		Vector3 from, to;
		from = Vector3.zero;
		to = from;

		//x,y:[-1~0~1]
		//x^2 + y^2 = 1
		const float SPACE_THETA = 0.5f;
		const float DIVISION = 9f;
		float x = 1f, y = 0;
		float dir = -1f;
		float dirY = 1f;
		int theta = 0;
		for (int  i = 0; i <= DIVISION * 8; i++) 
		{

			//x^2 + y^2 = 1
			y = 1f - (x * x);
			//y = Mathf.Pow (y, 0.5f);
			y = Mathf.Sqrt(y);
			y *= dirY;

			DebugWide.LogBlue ("theta: " + theta + "  x:"+x + "  y:" + y + "   y/x:" + y/x + "   tan:" + Mathf.Tan(theta*Mathf.Deg2Rad) + "   atan:" + Mathf.Atan2( y,x) * Mathf.Rad2Deg);
			//DebugWide.Log(" theta:" + theta + "  x:" + Mathf.Acos(x) * Mathf.Rad2Deg + "  y:" + Mathf.Asin(y) * Mathf.Rad2Deg);
			to.z += SPACE_THETA;
			to.y = y / x;
			Gizmos.color = Color.green;
			Gizmos.DrawLine (from , to);
			from = to;

			//------------------------
			//next value setting
			if (theta % 180 == 0 && theta != 0) 
			{
				DebugWide.LogRed (theta);
				//x = Mathf.Clamp (x, -1f, 1f);
				//dir = x * -1;
				//x = dir;
				dir *= -1;
				dirY *= -1f;
			}
			x +=  dir*(1f / DIVISION);
			theta += (int)(90 / DIVISION);


		}


	}
}
