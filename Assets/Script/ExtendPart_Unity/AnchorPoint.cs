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
				return _originalPos + _movePosScale + _movePosRotate;
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
			this.calcMovePos ();

			_originalPos = pos - _movePosScale - _movePosRotate;
			_tr.localPosition = pos;
		}

		public void SetAnchorRate(Vector3 rate)
		{
			//fix me : need calc : rate => point

			this.calcMovePos ();

			_anchorPoint = rate;
		}

		public void SetAnchorRateX(float x)
		{
			this.calcMovePos ();

			_anchorPoint.x = x;
		}

		public void SetAnchorRateZ(float z)
		{
			this.calcMovePos ();

			_anchorPoint.z = z;
		}

		private void calcMovePos()
		{

			//anchor point calc
			//L = Length
			//L' = L * Scale
			//AP = anchor point
			//MV = move value
			//(L-L') * AP = MV

			Vector3 CUBE_LENGTH = Vector3.one;

			_movePosScale.x = (CUBE_LENGTH.x - (CUBE_LENGTH.x * _tr.localScale.x)) * _anchorPoint.x;
			_movePosScale.y = (CUBE_LENGTH.y - (CUBE_LENGTH.y * _tr.localScale.y)) * _anchorPoint.y;
			_movePosScale.z = (CUBE_LENGTH.z - (CUBE_LENGTH.z * _tr.localScale.z)) * _anchorPoint.z;

			Vector3 apScale = Vector3.Scale (_tr.localScale, _anchorPoint);  //Scale AnchorPoint  when transform sequence  : "scale => rotate" or "rotate => scale"" 
			_movePosRotate = apScale - (_tr.localRotation * apScale); 
			//Debug.Log (apScale + "   " + _movePosRotate); //chamto test

		}

		public void Scale(Vector3 scale)
		{
			//1
			_tr.localScale = scale;

			//2
			this.calcMovePos ();

			//3
			_tr.localPosition = _originalPos + _movePosScale + _movePosRotate;
		}

		public void ScaleZ(float z)
		{
			Vector3 scale = _tr.localScale;
			scale.z = z;

			this.Scale (scale);
		}

		public Vector3 Rotate(Vector3 v3Degree)
		{
			//1
			_tr.Rotate (v3Degree.x, v3Degree.y, v3Degree.z);

			//2
			this.calcMovePos();

			//3
			_tr.localPosition = _originalPos + _movePosRotate + _movePosScale;

			//Debug.Log (_tr.localRotation.eulerAngles + "    " + _movePosRotate); //chamto test

			return _tr.localEulerAngles;
		}

		public float RotateX(float degree)
		{
			return this.Rotate (new Vector3 (degree,0,0)).x;
		}

		public float RotateZ(float degree)
		{
			return this.Rotate (new Vector3 (0,0,degree)).z;
		}

		public void SetEulerAngles(Vector3 v3Degree)
		{
			//1
			_tr.eulerAngles = v3Degree;

			//2
			this.calcMovePos();

			//3
			_tr.localPosition = _originalPos + _movePosRotate + _movePosScale;
		}

		public void SetEulerAngleX(float x)
		{
			Vector3 angles = _tr.eulerAngles;
			angles.x = x;
			this.SetEulerAngles (angles);
		}

		public void SetEulerAngleZ(float z)
		{
			Vector3 angles = _tr.eulerAngles;
			angles.z = z;
			this.SetEulerAngles (angles);
		}


	}
}
