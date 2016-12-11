using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour 
{
	public Transform _RightUpperArm	= null;
	public Transform _RightForeArm	= null;
	public bool 	_Active = false;


	void Start () 
	{
		
	}
	

	void Update () 
	{
		if (true == _Active) 
		{
			
		}
		_Active = false;
	}
}
