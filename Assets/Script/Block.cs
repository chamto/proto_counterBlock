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
	float maxSecond = 0.8f;
	float angleSum = 0;
	float start = 0, end = -90f;
	public void RightBlock()
	{
		float scaleDelta = Interpolation.easeOutElastic (0,10f, accumulate/maxSecond);
		_rightUpperArmAnchor.SetEulerAngleX(scaleDelta);



		//float scaleDelta = Interpolation.easeInOutBack (start,end, accumulate/maxSecond);
		scaleDelta = Interpolation.easeOutElastic (start,end, accumulate/maxSecond);
		_rightForeArmAnchor.SetEulerAngleZ(scaleDelta);

		if (maxSecond <= accumulate) 
		{
			accumulate = 0; 			//a. 	repeat normal

			float temp = start;			//a-1. 	repeat ping pong 
			start = end;
			end = temp;

			//accumulate = maxSecond;	//b. 	one time
		
		}

		accumulate += Time.deltaTime;
	}
}
