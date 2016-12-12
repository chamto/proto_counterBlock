using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExtendPart_Unity
{
	//extend part Transform 
	public class AnchorPoint
	{
		private Vector3 _anchorPoint; //real position standard center
		//private Vector3 _anchorRate;  //[-1~1]
		private Transform _tr = null;


		/// [temp value]
		Vector3 _movePosScale;
		Vector3 _movePosRotate;
		Vector3 _originalPos;


		public Vector3 anchorPoint
		{
			get
			{
				return _anchorPoint;
			}

		}
		///
		///this is position value what following anchorPoint
		/// 
		public Vector3 position
		{
			get
			{
				return _originalPos + _movePosScale;
			}
		}

		public AnchorPoint(Transform tr)
		{
			_anchorPoint = Vector3.zero;
			_movePosScale = Vector3.zero;
			_movePosRotate = Vector3.zero;

			_tr = tr;
			_originalPos = _tr.localPosition;
		}

		public void SetPosition(Vector3 pos)
		{
			this.calcMovePosScale ();

			_originalPos = pos - _movePosScale;
			_tr.localPosition = pos;
		}

		public void SetAnchorRate(Vector3 rate)
		{
			this.calcMovePosScale ();

			_anchorPoint = rate;
		}
		public void SetAnchorRateZ(float z)
		{
			this.calcMovePosScale ();

			_anchorPoint.z = z;
		}

		private void calcMovePosScale()
		{

			//anchor point calc
			//L = Length
			//L' = L * Scale
			//AP = anchor point
			//MV = move value
			//(L-L') * AP = MV

			Vector3 CUBE_LENGTH = Vector3.one;
			Vector3 ap = _anchorPoint;

			_movePosScale.x = (CUBE_LENGTH.x - (CUBE_LENGTH.x * _tr.localScale.x)) * ap.x;
			_movePosScale.y = (CUBE_LENGTH.y - (CUBE_LENGTH.y * _tr.localScale.y)) * ap.y;
			_movePosScale.z = (CUBE_LENGTH.z - (CUBE_LENGTH.z * _tr.localScale.z)) * ap.z;


		}

		public void Scale(Vector3 scale)
		{
			//1
			_tr.localScale = scale;

			//2
			this.calcMovePosScale ();

			//3
			_tr.localPosition = _originalPos + _movePosScale;
		}

		public void ScaleZ(float z)
		{
			Vector3 scale = _tr.localScale;
			scale.z = z;

			this.Scale (scale);
		}

		public void Rotate(Vector3 v3Degree)
		{
			_tr.Rotate (v3Degree.x, v3Degree.y, v3Degree.z);

			Vector3 apScale = Vector3.Scale (_tr.localScale, _anchorPoint);  //Scale AnchorPoint  when transform sequence  : 1.scale 2.rotate
			_movePosRotate = (_tr.localRotation * apScale) + apScale; 
			_tr.localPosition = _originalPos + _movePosRotate + _movePosScale;

			//Debug.Log (_tr.localRotation.eulerAngles + "    " + _movePosRotate); //chamto test
		}

		public void RotateX(float degree)
		{
			this.Rotate (new Vector3 (degree,0,0));
		}
	}
}
