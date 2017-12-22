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
	float maxSecond = 0.3f;
	float angleSum = 0;
	float startFore = 0, endFore = -90f;
	float startUpper = 0, endUpper = -10f;
	public void RightBlock()
	{
		float scaleDelta = Interpolation.easeOutElastic (startUpper,endUpper, accumulate/maxSecond);
		_rightUpperArmAnchor.SetEulerAngleZ(scaleDelta);



		//float scaleDelta = Interpolation.easeInOutBack (start,end, accumulate/maxSecond);
		scaleDelta = Interpolation.easeOutElastic (startFore,endFore, accumulate/maxSecond);
		_rightForeArmAnchor.SetEulerAngleZ(scaleDelta);

		if (maxSecond <= accumulate) 
		{
			accumulate = 0; 			//a. 	repeat normal
			//accumulate = maxSecond;	//b. 	one time

			float temp = startFore;		//a-1. 	repeat ping pong 
			startFore = endFore;
			endFore = temp;

			temp = startUpper;			//a-2. 	repeat ping pong 
			startUpper = endUpper;
			endUpper = temp;
		
		}

		accumulate += Time.deltaTime;
	}
}
