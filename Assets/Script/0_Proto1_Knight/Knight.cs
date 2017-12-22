using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendPart_Unity;
using Utility;


public struct KnightData
{
	public uint hp;
}

public class Knight : MonoBehaviour 
{
	private KnightData _data;

	public uint hp 
	{
		get
		{
			return _data.hp;
		}
		set
		{
			_data.hp = value;
		}
	}

	void Start()
	{
		_data.hp = 5;
	}
}
