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


	float accumulate = 0;
	float maxSecond = 1.0f;
	float direct = 0;
	float angleSum = 0;
	public void RightBlock()
	{
		//_rightUpperArmAnchor.RotateZ(directAngle);
		//_rightForeArmAnchor.RotateZ (directAngle);

		//[0~   1]
		//[0~ -90]

		//float scaleDelta = Interpolation.punch (-90f, accumulate/maxSecond);

		//0~1 : (1)-0~(1)-1 => 1~0 , (0)-0~(0)-1 => 0~ -1
		//1~0 : (1)-1~(1)-0 => 0~1
		float scaleDelta = Interpolation.easeInQuad (0,-90f, Mathf.Abs(direct-accumulate/maxSecond));
		_rightForeArmAnchor.SetEulerAngleZ(scaleDelta);

		if (maxSecond <= accumulate) 
		{
			accumulate = 0; 			//a. repeat
			//accumulate = maxSecond;	//b. one time

//			if (1 == direct)
//				direct = 0;
//			else
//				direct = 1;


			//1=>0   (-1)^1=-1
			//0=>1   (-1)^0= 1
			direct = direct+Mathf.Pow(-1,direct);	//a-2. ping pong
		
		}

		accumulate += Time.deltaTime;
	}
}
