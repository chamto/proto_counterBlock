using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExtendPart_Unity
{
	//extend part Transform 
	public class AnchorPoint
	{
		private Vector3 _anchorRate; //[-1~1]
		private Transform _tr = null;


		/// [temp value]
		Vector3 _movePos;
		Vector3 _originalPos;


		public Vector3 anchorRate 
		{
			get
			{
				return _anchorRate;
			}

		}
		///
		///this is position value what following anchorRate 
		/// 
		public Vector3 position
		{
			get
			{
				return _originalPos + _movePos;
			}
		}

		public AnchorPoint(Transform tr)
		{
			_anchorRate = Vector3.zero;
			_movePos = Vector3.zero;

			_tr = tr;
			_originalPos = _tr.localPosition;
		}

		public void SetPosition(Vector3 pos)
		{
			this.calcMovePos ();

			_originalPos = pos - _movePos;
			_tr.localPosition = pos;
		}

		public void SetAnchorRate(Vector3 rate)
		{
			this.calcMovePos ();

			_anchorRate = rate;
		}
		public void SetAnchorRateZ(float z)
		{
			this.calcMovePos ();

			_anchorRate.z = z;
		}

		private void calcMovePos()
		{

			//anchor point calc
			//L = Length
			//L' = L * Scale
			//AP = anchor point
			//MV = move value
			//(L-L') * AP = MV

			const int CUBE_LENGTH = 1;

			_movePos.x = (CUBE_LENGTH - (CUBE_LENGTH * _tr.localScale.x)) * _anchorRate.x;
			_movePos.y = (CUBE_LENGTH - (CUBE_LENGTH * _tr.localScale.y)) * _anchorRate.y;
			_movePos.z = (CUBE_LENGTH - (CUBE_LENGTH * _tr.localScale.z)) * _anchorRate.z;
		}

		public void Scale(Vector3 scale)
		{
			//1
			_tr.localScale = scale;

			//2
			this.calcMovePos ();

			//3
			_tr.localPosition = _originalPos + _movePos;
		}

		public void ScaleZ(float z)
		{
			Vector3 scale = _tr.localScale;
			scale.z = z;

			this.Scale (scale);
		}
	}
}
