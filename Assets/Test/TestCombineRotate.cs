using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimpleParser : List<SimpleParser.Sentences>
{
	

	//text > sentence > commansGroup > command

	public class Sentences : List<Command> 
	{
		public enum eKind
		{
			None = 0,
			Translate,
			Rotate,
			Scale,

		}

		public eKind kind = eKind.None;
		public string text = "";

//		public Matrix4x4 GetMatrix()
//		{
//		}
	}


	public class Command
	{
		public enum eKind
		{
			None = 0,
			X,
			Y,
			Z,
			XYZ,
		}
		
		public Command(eKind kind, float degree) { this.degree = degree; this.kind = kind; }
		public Command(Vector3 xyz) { this.xyz = xyz; kind = eKind.XYZ;}

		public eKind kind;
		public float degree;
		public Vector3 xyz;
	}



	//"s(x y z) rz0 ry0 rx0 t(x y z)";
	public void Parsing(string text)
	{
		this.Clear ();
		List<string> list = this._incision (text);

		foreach (string stsText in list) 
		{
			this.Add ( this.SentenceDecomposition (stsText));
		}
	}



	public Sentences SentenceDecomposition(string text)
	{
		Sentences sts = new Sentences ();
		sts.text = text;
		if ('t' == text [0]) 
		{
			sts.kind = Sentences.eKind.Translate;
		}
		if ('r' == text [0]) 
		{
			sts.kind = Sentences.eKind.Rotate;
		}
		if ('s' == text [0]) 
		{
			sts.kind = Sentences.eKind.Scale;
		}
		CommandDecomposition (sts);

		return sts;
	}

	//"s(x y z) rz0 ry0 rx0 t(x y z)";
	public void CommandDecomposition(Sentences sts)
	{
		//int L = sts.text - 1;

		//int start = -1;
		string temp = "";
		string[] split = null;
		Vector3 xyz = Vector3.zero;

		if (Sentences.eKind.Translate == sts.kind || Sentences.eKind.Scale == sts.kind)
		{
			temp = sts.text.Trim ('t','s', ')',' ');
			temp = temp.Trim ('(', ' ');
			temp = temp.Trim ();
			split = temp.Split(' ');

			foreach(string s in split)
			{
				if (s.Length != 0 && 'x' == s[0]) 
				{
					temp = s.TrimStart ('x');
					if (false == float.TryParse (temp, out xyz.x)) {
						xyz.x = 0;
					}
				}
				if (s.Length != 0 && 'y' == s[0]) 
				{
					temp = s.TrimStart ('y');
					if (false == float.TryParse (temp, out xyz.y)) {
						xyz.y = 0;
					}
				}
				if (s.Length != 0 && 'z' == s[0]) 
				{
					temp = s.TrimStart ('z');
					if (false == float.TryParse (temp, out xyz.z)) {
						xyz.z = 0;
					}
				}
			}
			sts.Add (new Command (xyz));
		}
		if (Sentences.eKind.Rotate == sts.kind) 
		{
			temp = sts.text.Trim ('r',' ');

			if (temp.Length != 0 && 'x' == temp[0]) 
			{
				float value = 0;
				temp = temp.TrimStart ('x');
				if (false == float.TryParse (temp, out value)) {
					value = 0;
				}
				sts.Add(new Command(SimpleParser.Command.eKind.X, value));
			}
			if (temp.Length != 0 && 'y' == temp[0]) 
			{
				float value = 0;
				temp = temp.TrimStart ('y');
				if (false == float.TryParse (temp, out value)) {
					value = 0;
				}
				sts.Add(new Command(SimpleParser.Command.eKind.Y, value));
			}
			if (temp.Length != 0 && 'z' == temp[0]) 
			{
				float value = 0;
				temp = temp.TrimStart ('z');
				if (false == float.TryParse (temp, out value)) {
					value = 0;
				}
				sts.Add(new Command(SimpleParser.Command.eKind.Z, value));
			}

		}
	}


	private List<string> _incision(string text)
	{
		int L = text.Length - 1;

		int start = -1;
		int end = -1;

		List<string> list = new List<string> ();

		for(int i=0;i<text.Length;i++)
		{
			//scale , translate
			if ('s' == text[i] || 't' == text[i]) 
			{
				start = text.IndexOf ('(', i);
				end = text.IndexOf (')', start);

				list.Add( text.Substring (i, end - i + 1));
			}

			//rotate
			if ('r' == text [i]) 
			{
				end = -1;
				if (L >= i + 1) 
				{
					if('x' == text [i + 1] || 'y' == text [i + 1] || 'z' == text [i + 1]) 
					{
						end = text.IndexOf (' ', i);
						if (end < 0)
							end = L+1;
						
						list.Add( text.Substring (i, end - i));
						//DebugWide.LogBlue ("  : " + end + " " + i + "  " + (end - i)); //chamto test

					}
				}
			}
		}

		return list;
	}


	public void TestPrint(string text)
	{
		List<string> list = this._incision (text);
		foreach (string s in list) 
		{
			DebugWide.LogBlue (s);
		}
	}

}




public class TestCombineRotate : MonoBehaviour 
{
	public Transform _testTarget = null;
	public Transform _testAxis = null;
	public string _multiOrder = "s(x y z) rz0 ry0 rx0 t(x y z)";

	public bool _repeat = false;
	public bool _apply = false;

	private SimpleParser _parser = new SimpleParser();




	void Start () 
	{
		Matrix4x4 mrx = Matrix4x4.identity;
		Matrix4x4 mry = Matrix4x4.identity;
		Matrix4x4 mrz = Matrix4x4.identity;
		Matrix4x4 mt = Matrix4x4.identity;
		Matrix4x4 r = Matrix4x4.zero;

		mrx = this.GetRotateX (45f);
		mry = this.GetRotateY (45f);
		mrz = this.GetRotateY (0f);
		mt = this.GetTranslate(new Vector3(0,0,0));

		//r = mrx * mry * mrz * mt;
		r = mt* mrx * mry * mrz;
		//r = mrx * mry * mrz;

		//this.MatrixToGroupTransform (_testAxis, r);

		//_testTarget.position = r.MultiplyPoint (_testTarget.position);
		//_testTarget.position = mt.MultiplyPoint (_testTarget.position);

		this.SavePosition (_testAxis);
	}


	void Update () 
	{
		if (true == _apply) 
		{
			
			this.Parsing();

			//-----------
			prev_angles = dest_angles;
			this.MatrixToTransform(_testTarget, trs);
			dest_angles = _testTarget.eulerAngles;
			DebugWide.LogRed ("angles : "+_testTarget.eulerAngles + "\n" + trs);

			this.MatrixToGroupTransform (_testAxis, trs);
			//-----------

			elapsedTime = 0;
			_apply = false;
		}

		elapsedTime += Time.deltaTime;

		this.Repeat ();

	}

	float elapsedTime = 0;
	Vector3 prev_angles = Vector3.zero;
	Vector3 dest_angles = Vector3.zero;
	public void Repeat()
	{
		//interpolation
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



	Matrix4x4 trs = Matrix4x4.identity;
	public void Parsing()
	{
		_parser.Parsing(_multiOrder);

		trs = Matrix4x4.identity;
		foreach (SimpleParser.Sentences sts in _parser) 
		{
			if (0 == sts.Count)
				continue;

			switch (sts.kind) 
			{
			case SimpleParser.Sentences.eKind.Translate:
				trs = trs * this.GetTranslate (sts [0].xyz);
				break;
			case SimpleParser.Sentences.eKind.Rotate:
				{
					if (SimpleParser.Command.eKind.X == sts [0].kind)
						trs = trs * this.GetRotateX (sts [0].degree);
					if (SimpleParser.Command.eKind.Y == sts [0].kind)
						trs = trs * this.GetRotateY (sts [0].degree);
					if (SimpleParser.Command.eKind.Z == sts [0].kind)
						trs = trs * this.GetRotateZ (sts [0].degree);
				}
				break;
			case SimpleParser.Sentences.eKind.Scale:
				trs = trs * this.GetScale (sts [0].xyz);
				break;
			}
		}

	}

	//ref : http://answers.unity3d.com/questions/1134216/how-to-set-transformation-matrices-of-transform.html
	public void MatrixToTransform(Transform tr, Matrix4x4 mat)
	{
		tr.localPosition = mat.GetColumn( 3 );
		tr.localScale = new Vector3( mat.GetColumn( 0 ).magnitude, mat.GetColumn( 1 ).magnitude, mat.GetColumn( 2 ).magnitude );

		float w = Mathf.Sqrt( 1.0f + mat.m00 + mat.m11 + mat.m22 ) / 2.0f;
		tr.localRotation = new Quaternion( ( mat.m21 - mat.m12 ) / ( 4.0f * w ), ( mat.m02 - mat.m20 ) / ( 4.0f * w ), ( mat.m10 - mat.m01 ) / ( 4.0f * w ), w );
	}


	Dictionary<Transform ,Vector3> savePositions = new Dictionary<Transform ,Vector3> ();
	public void SavePosition(Transform groupTr)
	{
		savePositions.Clear ();
		foreach (Transform tr in groupTr.GetComponentsInChildren<Transform> ()) 
		{
			savePositions.Add (tr,tr.position);
		}
	}

	public void LoadPosition(Transform groupTr)
	{
		foreach (KeyValuePair<Transform, Vector3> p in savePositions) 
		{
			p.Key.position = p.Value;
		}
	}

	public void MatrixToGroupTransform(Transform groupTr, Matrix4x4 mat)
	{
		if (null == groupTr)
			return;

		this.LoadPosition (groupTr);
		foreach (Transform tr in groupTr.GetComponentsInChildren<Transform> ()) 
		{
			tr.position = mat.MultiplyPoint (tr.position);
		}
	}

	// 유니티엔진의 회전행렬 결합 순서 : y => x => z

	//열우선 행렬 : v' = m * v
	//v1: 00 01 02 03
	//v2: 10 11 12 13
	//v3: 20 21 22 23
	//v4: 30 31 32 33

	public Matrix4x4 GetScale(Vector3 scale)
	{
		//s1 : 00
		//s2 : 11
		//s3 : 22

		Matrix4x4 m = Matrix4x4.identity;
		m.m00 = scale.x;
		m.m11 = scale.y;
		m.m22 = scale.z;

		return m;
	}

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
