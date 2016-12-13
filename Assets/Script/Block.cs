using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendPart_Unity;
using Utility;


public class Block : MonoBehaviour 
{
	public Transform _rightUpperArm	= null;
	public Transform _rightForeArm	= null;
	public bool 	_active = false;

	private AnchorPoint _rightUpperArmAnchor = null;
	private AnchorPoint _rightForeArmAnchor = null;

	void Start () 
	{
		_rightUpperArmAnchor = new AnchorPoint (_rightUpperArm);
		_rightUpperArmAnchor.SetAnchorRateZ (0.5f);
		
		_rightForeArmAnchor = new AnchorPoint (_rightForeArm);
		_rightForeArmAnchor.SetAnchorRateX (0.5f);
	}
	

	void Update () 
	{
		this.RightBlock ();

		if (true == _active) 
		{
			
		}
		_active = false;
	}

	float directAngle = 5f;
	float angleSum = 0;
	public void RightBlock()
	{
		//_rightUpperArmAnchor.RotateZ(directAngle);
		_rightForeArmAnchor.RotateZ (directAngle);
		if (0f < angleSum || angleSum < -90f) {
			directAngle *= -1;
		}
		angleSum += directAngle;
	}
}
