using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
[ExecuteInEditMode]
public class IK2Chain : MonoBehaviour 
{

    public Transform _targetPos = null;
	public Transform _joint2EndPos = null;
	public Transform _joint_1 = null;
	public Transform _joint_2 = null;
	public float _joint_1_Length = 0;	//upperArm Length 
	public float _joint_2_Length = 0;	//foreArm Length
	public bool		_isOneChain = false;
	public int 		_oneChainNum = 0;

	//DOF(Degree of Free)
	public Vector3 _DOF_Min = new Vector3(-180f,-180f,-180f);
	public Vector3 _DOF_Max = new Vector3(180f,180f,180f);


    [HideInInspector]
    public bool invert;
    
	public bool _toggleIK = false;
	public bool toggleIK
	{
		get{ return _toggleIK;}
		set{ _toggleIK = value;}
	}

	private Vector3 _joint_1_initAngle;
	private Vector3 _joint_2_initAngle;

	public Vector3 Joint2Dir()
	{
		return _joint2EndPos.position - _joint_2.position;
	}


    public void Start()
    {
		
    }


	public void ToggleOn()
	{
		//false => true
		if(true == this._toggleIK) return;
		this._toggleIK = true;
		//----------------------------------------
		
		Vector3 dir = (_joint_2.position - _joint_1.position);
		_joint_1_Length = dir.magnitude;

		dir = (_joint2EndPos.position - _joint_2.position);
		_joint_2_Length = dir.magnitude;

		_joint_1_initAngle = _joint_1.localRotation.eulerAngles;
		_joint_2_initAngle = _joint_2.localRotation.eulerAngles;
	}
	public void ToggleOff(bool restoreAngles)
	{
		//true => false
		if(false == this._toggleIK) return;
		this._toggleIK = false;
		//----------------------------------------

		if (true == restoreAngles) 
		{
			//_joint_1.localRotation = Quaternion.Euler (_joint_1_initAngle);
			//_joint_2.localRotation = Quaternion.Euler (_joint_2_initAngle);
			_joint_1.localEulerAngles = _joint_1_initAngle;
			_joint_2.localEulerAngles = _joint_2_initAngle;
		}

	}


	float theta1 = 0;
	float theta2 = 0;
	bool outOfRange;
	public void Compute2Chain()
	{
		if(false == _toggleIK || null == _targetPos || null == _joint_2 || null == _joint_1)
			return;

		float z = _targetPos.position.z - _joint_1.position.z;
		float y = _targetPos.position.y - _joint_1.position.y;
		y = -y; //y->z 회전방향을 z->-y 회전방향으로 돌려서 계산한다. (x->y 에서의 식에 맞추기 위함)

		//제2코사인법칙 : a*a = b*b + c*c - 2bcCosA , //파타고라스의 정리 : z*z + y*y = x*x
		//float num = Mathf.Pow(z, 2) + Mathf.Pow(y, 2) - Mathf.Pow(d1, 2) - Mathf.Pow(d2, 2); 
		float num = _joint_1_Length*_joint_1_Length + _joint_2_Length*_joint_2_Length - (z*z + y*y);
		float denom = 2 * _joint_1_Length * _joint_2_Length;
		float costheta2 = -(num / denom); //cosA = -cos(180-A)

		//삼각형 모양이 깨진 경우
		outOfRange = (Mathf.Abs(costheta2) > 1);
		costheta2 = Mathf.Clamp(costheta2, -1, 1);
		theta2 = Mathf.Acos(costheta2);

		if (invert)
			theta2 = -theta2;

		//y->z
		//ycos + -zsin 
		//ysin  + zcos
		//ref : http://www.darwin3d.com/gamedev/articles/col1198.pdf
		float atz = y * (_joint_1_Length + _joint_2_Length * Mathf.Cos(theta2)) - z * (_joint_2_Length * Mathf.Sin(theta2));
		float aty = z * (_joint_1_Length + _joint_2_Length * Mathf.Cos(theta2)) + y * (_joint_2_Length * Mathf.Sin(theta2));

		theta1 = z==0 && y==0? 0 : Mathf.Atan2(atz, aty);

		_joint_1.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta1),0,0);
		_joint_2.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta2),0,0);
	}

	void compute1_Chain0()
	{
		Vector3 dirFrom = _joint_2.position - _joint_1.position;
		Vector3 dirTo = _targetPos.position - _joint_1.position;
		dirFrom.Normalize ();
		dirTo.Normalize ();
		float cos = Vector3.Dot (dirFrom, dirTo);
		float theta = Mathf.Acos (cos);
		theta = theta * Mathf.Rad2Deg;

		Vector3 foward = dirFrom;
		Vector3 up = Vector3.Cross (dirFrom, dirTo);
		up.Normalize ();

		_joint_1.transform.Rotate (up,theta ,Space.World);
	}


	public Vector3 ClampV3(Vector3 value , Vector3 min , Vector3 max)
	{
		if (value.x < min.x) {
			value.x = min.x;
		} else if (value.x > max.x) {
			value.x = max.x;
		}

		if (value.y < min.y) {
			value.y = min.y;
		} else if (value.y > max.y) {
			value.y = max.y;
		}

		if (value.z < min.z) {
			value.z = min.z;
		} else if (value.z > max.z) {
			value.z = max.z;
		}

		return value;
	}

	void compute1_Chain1()
	{
		Vector3 dirTo = _targetPos.position - _joint_2.position;

		float degree = Vector3.Angle (_joint_2.forward, dirTo);

		Vector3 up = Vector3.Cross (_joint_2.forward, dirTo);

		_joint_2.transform.Rotate (up, degree, Space.World);


//		Vector3 test = _joint_2.transform.localEulerAngles;
//		if (test.x >= 180f) {
//			test.x -= 180f; 
//		}
		//Matrix4x4 m = Matrix4x4.identity;




		//DebugWide.LogBlue (Quaternion.FromToRotation(_joint_2.forward, dirTo).eulerAngles);
		DebugWide.LogBlue ("    "+_joint_2.transform.localEulerAngles);
		//_joint_2.transform.localEulerAngles = this.ClampV3 (_joint_2.transform.localEulerAngles, _DOF_Min, _DOF_Max);
		//DebugWide.LogBlue ("     "+_joint_2.transform.localEulerAngles);
	}

	public void Compute1Chain(int chainNum)
	{

		if (0 == chainNum) 
		{
			this.compute1_Chain0 ();
		}
		else if(1 == chainNum)
		{
			this.compute1_Chain1 ();
		}

	}

    
    public void LateUpdate()
    {
		
		if (true == this._toggleIK) 
		{
			if (true == _isOneChain) 
			{
				this.Compute1Chain (_oneChainNum);
			} else 
			{
				this.Compute2Chain ();
			}
		}

    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR

		if(null == _targetPos || null == _joint_2 || null == _joint_1)
			return;
		
        if (Selection.Contains(_targetPos.gameObject)&& _toggleIK )
        {
            Gizmos.DrawIcon(_targetPos.position, "IKHandle.png", false);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(_joint_2.position, _targetPos.position);
			Gizmos.DrawLine(_joint_1.position, _targetPos.position);

			Gizmos.color =  outOfRange ? Color.red: Color.green;
			Vector3 foreJointPos = new Vector3(0,_joint_1.position.y + _joint_1_Length * Mathf.Sin(-theta1), _joint_1.position.z + _joint_1_Length * Mathf.Cos(-theta1)); //-theta1 ???
			Gizmos.DrawLine(_joint_1.position, foreJointPos);

        }
#endif

    }

}
