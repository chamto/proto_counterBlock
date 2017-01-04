﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
[ExecuteInEditMode]
public class IK2Chain : MonoBehaviour 
{

    public Transform _targetPos = null;
	public Transform _joint_1 = null;
	public Transform _joint_2 = null;
	public float _joint_1_length = 0;	//upperArm Length 
	public float _joint_2_Length = 0;	//foreArm Length

    [HideInInspector]
    public bool invert;
    
	private bool _toggleIK = false;
	public bool toggleIK
	{
		get {return _toggleIK; }
	}

	private Vector3 _joint_1_initAngle;
	private Vector3 _joint_2_initAngle;


    public void Start()
    {
		
    }


	public void ToggleOn()
	{
		//false => true
		if(true == this._toggleIK) return;
		this._toggleIK = true;
		//----------------------------------------
		
		Vector3 v3UpperLength = (_joint_1.position - _joint_2.position);
		v3UpperLength.x = 0; //2D
		//_upperLength = Mathf.Abs (v3UpperLength.magnitude);

		_joint_1_initAngle = _joint_1.localRotation.eulerAngles;
		_joint_2_initAngle = _joint_2.localRotation.eulerAngles;
	}
	public void ToggleOff()
	{
		//true => false
		if(false == this._toggleIK) return;
		this._toggleIK = false;
		//----------------------------------------
		
		_joint_1.localRotation = Quaternion.Euler (_joint_1_initAngle);
		_joint_2.localRotation = Quaternion.Euler (_joint_2_initAngle);
	}

    float theta1 = 0;
    float theta2 = 0;
    bool outOfRange;
    public void LateUpdate()
    {
		

		if(false == toggleIK || null == _targetPos || null == _joint_2 || null == _joint_1)
			return;
		
		float z = _targetPos.position.z - _joint_1.position.z;
		float y = _targetPos.position.y - _joint_1.position.y;
		y = -y; //y->z 회전방향을 z->-y 회전방향으로 돌려서 계산한다. (x->y 에서의 식에 맞추기 위함)

		//제2코사인법칙 : a*a = b*b + c*c - 2bcCosA , //파타고라스의 정리 : z*z + y*y = x*x
        //float num = Mathf.Pow(z, 2) + Mathf.Pow(y, 2) - Mathf.Pow(d1, 2) - Mathf.Pow(d2, 2); 
		float num = _joint_1_length*_joint_1_length + _joint_2_Length*_joint_2_Length - (z*z + y*y);
        float denom = 2 * _joint_1_length * _joint_2_Length;
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
		float atz = y * (_joint_1_length + _joint_2_Length * Mathf.Cos(theta2)) - z * (_joint_2_Length * Mathf.Sin(theta2));
		float aty = z * (_joint_1_length + _joint_2_Length * Mathf.Cos(theta2)) + y * (_joint_2_Length * Mathf.Sin(theta2));

		theta1 = z==0 && y==0? 0 : Mathf.Atan2(atz, aty);

		_joint_1.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta1),0,0);
		_joint_2.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta2),0,0);
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
		if(null == _targetPos || null == _joint_2 || null == _joint_1)
			return;
		
        if (Selection.Contains(_targetPos.gameObject)&& toggleIK )
        {
            Gizmos.DrawIcon(_targetPos.position, "IKHandle.png", false);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(_joint_2.position, _targetPos.position);
			Gizmos.DrawLine(_joint_1.position, _targetPos.position);

			Gizmos.color =  outOfRange ? Color.red: Color.green;
			Vector3 foreJointPos = new Vector3(0,_joint_1.position.y + _joint_1_length * Mathf.Sin(-theta1), _joint_1.position.z + _joint_1_length * Mathf.Cos(-theta1)); //-theta1 ???
			Gizmos.DrawLine(_joint_1.position, foreJointPos);

        }
#endif

    }

}
