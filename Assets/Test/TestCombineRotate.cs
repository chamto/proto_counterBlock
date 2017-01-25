using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCombineRotate : MonoBehaviour 
{
	public Transform _testTarget = null;
	public string _multiOrder = " x  y  z";
	//public string _multiOrder = " x90  y90  z";

	public bool _repeat = false;
	public bool _apply = false;
	public void Parse()
	{
			
		Matrix4x4 r = Matrix4x4.identity;
		
		float degree = 0;
		string temp;
		string[] s =  _multiOrder.Split(" "[0]);
		foreach (string item in s) 
		{
			temp = item.Trim ();
			if (0 == temp.Length)
				continue;
			

			if (temp [0] == 'x') 
			{
				temp = temp.TrimStart (temp [0]);
				//DebugWide.LogBlue (temp);
				if (false == float.TryParse (temp, out degree))
					degree = 0;
				
				r = r * this.GetRotateX (degree);
			} else if (temp [0] == 'y') 
			{
				temp = temp.TrimStart (temp [0]);
				if (false == float.TryParse (temp, out degree))
					degree = 0;
				
				r = r * this.GetRotateY (degree);
			} else if (temp [0] == 'z') 
			{
				temp = temp.TrimStart (temp [0]);
				if (false == float.TryParse (temp, out degree))
					degree = 0;
				
				r = r * this.GetRotateZ (degree);
			}
		}

		prev_angles = dest_angles;
		this.MatrixToTransform(_testTarget, r);
		dest_angles = _testTarget.eulerAngles;
		elapsedTime = 0;

		DebugWide.LogRed ("angles : "+_testTarget.eulerAngles);
	}

	float elapsedTime = 0;
	Vector3 prev_angles = Vector3.zero;
	Vector3 dest_angles = Vector3.zero;
	void Update () 
	{
		if (true == _apply) 
		{
			this.Parse ();	

			_apply = false;
		}

		//interpolation

		elapsedTime += Time.deltaTime;


		if (true == _repeat)
		{
			elapsedTime = Mathf.Repeat (elapsedTime, 2f);

			//0~1
			if(0 <= elapsedTime && elapsedTime <= 1f)
				_testTarget.eulerAngles = Vector3.Lerp (dest_angles, prev_angles, elapsedTime);
			//1~2
			else if(1f < elapsedTime && elapsedTime <= 2f)
			_testTarget.eulerAngles = Vector3.Lerp (prev_angles, dest_angles, elapsedTime -1f);
		}
		else
			_testTarget.eulerAngles = Vector3.Lerp (prev_angles, dest_angles, elapsedTime);


	}

	void Start () 
	{
//		Matrix4x4 r = Matrix4x4.zero;
//		Matrix4x4 m1 = this.GetRotateZ(45f);
//		Matrix4x4 m2 = this.GetRotateY(45f);
//		Matrix4x4 m3 = this.GetRotateX(45f);

		//r = m1 * m2 * m3;
		//r = m3 * m2 * m1;

		//DebugWide.LogBlue (m1);
		//DebugWide.LogBlue (m2);
		//DebugWide.LogRed (r);

		//this.MatrixToTransform(_testTarget, r);

		//DebugWide.LogRed (_testTarget.position);
	}

	//ref : http://answers.unity3d.com/questions/1134216/how-to-set-transformation-matrices-of-transform.html
	public void MatrixToTransform(Transform tr, Matrix4x4 mat)
	{
		tr.localPosition = mat.GetColumn( 3 );
		tr.localScale = new Vector3( mat.GetColumn( 0 ).magnitude, mat.GetColumn( 1 ).magnitude, mat.GetColumn( 2 ).magnitude );
		float w = Mathf.Sqrt( 1.0f + mat.m00 + mat.m11 + mat.m22 ) / 2.0f;
		tr.localRotation = new Quaternion( ( mat.m21 - mat.m12 ) / ( 4.0f * w ), ( mat.m02 - mat.m20 ) / ( 4.0f * w ), ( mat.m10 - mat.m01 ) / ( 4.0f * w ), w );
	}



	//열우선 행렬 : v' = m * v
	//v1: 00 01 02 03
	//v2: 10 11 12 13
	//v3: 20 21 22 23
	//v4: 30 31 32 33

	public Matrix4x4 GetTranslate(Vector3 pos)
	{
		//t1 : 03
		//t2 : 13
		//t3 : 23

		Matrix4x4 m = Matrix4x4.identity;
		m.SetColumn (3, new Vector4 (pos.x, pos.y, pos.z, 1));

		return m;
	}

	public Matrix4x4 GetRotateX(float degree)
	{
		// 1   0    0 
		// 0  cos -sin 
		// 0  sin  cos

		float theta = degree * Mathf.Deg2Rad;
		float cos = Mathf.Cos (theta);
		float sin = Mathf.Sin (theta);

		Matrix4x4 m = Matrix4x4.identity;
		m.SetRow (0, new Vector4 (1,   0,    0, 0));
		m.SetRow (1, new Vector4 (0, cos, -sin, 0));
		m.SetRow (2, new Vector4 (0, sin,  cos, 0));
		m.SetRow (3, new Vector4 (0,   0,    0, 1));

		return m;
		 
	}

	public Matrix4x4 GetRotateY(float degree)
	{
		// cos  0  sin
		//  0   1   0
		//-sin  0  cos

		float theta = degree * Mathf.Deg2Rad;
		float cos = Mathf.Cos (theta);
		float sin = Mathf.Sin (theta);

		Matrix4x4 m = Matrix4x4.identity;
		m.SetRow (0, new Vector4 ( cos,  0,  sin, 0));
		m.SetRow (1, new Vector4 (   0,  1,    0, 0));
		m.SetRow (2, new Vector4 (-sin,  0,  cos, 0));
		m.SetRow (3, new Vector4 (   0,  0,    0, 1));

		return m;

	}

	public Matrix4x4 GetRotateZ(float degree)
	{
		// cos -sin  0
		// sin  cos  0
		//  0    0   1

		float theta = degree * Mathf.Deg2Rad;
		float cos = Mathf.Cos (theta);
		float sin = Mathf.Sin (theta);

		Matrix4x4 m = Matrix4x4.identity;
		m.SetRow (0, new Vector4 (cos, -sin, 0, 0));
		m.SetRow (1, new Vector4 (sin,  cos, 0, 0));
		m.SetRow (2, new Vector4 (  0,    0, 1, 0));
		m.SetRow (3, new Vector4 (  0,    0, 0, 1));

		return m;
	}
	


}
