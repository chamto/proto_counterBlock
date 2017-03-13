
//===============================================================================
// @ IvQuat.h
// 
// Quaternion class
// ------------------------------------------------------------------------------
// Copyright (C) 2004 by Elsevier, Inc. All rights reserved.
//
//
//
//===============================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML
{
	
	public class Util
	{
		static public float  kEpsilon = 1.0e-6f; //허용오차
		//float.Epsilon : 실수 오차값이 너무 작아, 계산 범위에 못 들어 올 수 있다.

		static public bool IsZero( float a ) 
		{
			
			return ( Mathf.Abs(a) < kEpsilon );
			//return ( Mathf.Abs(a) < float.Epsilon );

		}

		static public float InvSqrt( float val )     
		{ 
			return 1.0f/ Mathf.Sqrt( val ); 
		}

		static public void SinCos( float a, out float sina, out float cosa )
		{
			sina = Mathf.Sin(a);
			cosa = Mathf.Cos(a);
		}

		static public string ToBit(float number)
		{
			byte[] arByte =  BitConverter.GetBytes (number);
			string buffer = "";
			foreach(byte b in arByte)
			{
				buffer += string.Format ("{0} ", Convert.ToString(b,2)); //2진수 문자열로 변환
			}

			return buffer;
		}

	}
}
