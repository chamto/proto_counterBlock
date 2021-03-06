﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct IKLink
{
	public Transform transform;
	public float length;
	public float damp_width;
	public int min_rz;
	public int max_rz;

	public Vector3 angles
	{
		get
		{
			return this.transform.localRotation.eulerAngles;
		}
	}

	public IKLink(Transform tr, float l)
	{
		this.transform = tr;
		this.length = l;
		damp_width = 10f;
		min_rz = -30;
		max_rz = 30;
	}


	public void AddLocalRotateX(float degreeX)
	{
		//Vector3 temp = this.transform.localRotation.eulerAngles;
		//temp.x = x;
		this.transform.Rotate (degreeX,0,0);
	}
	public void AddLocalRotateY(float degreeY)
	{
		//Vector3 temp = this.transform.localRotation.eulerAngles;
		//temp.y = y;
		this.transform.Rotate (0,degreeY,0);
	}
	public void AddLocalRotateZ(float degreeZ)
	{
		//Vector3 temp = this.transform.localRotation.eulerAngles;
		//temp.z = z;
		this.transform.Rotate (0,0,degreeZ);
	}

	public void SetLocalRotationZ(float degreeZ)
	{
//		Quaternion q = this.transform.localRotation;
//		Vector3 temp = q.eulerAngles;
//		temp.z = z;
//		q.eulerAngles = temp;
//		this.transform.localRotation = q;

		Vector3 angles = this.transform.localEulerAngles;
		angles.z = degreeZ;
		this.transform.localEulerAngles = angles;


	}
}



[ExecuteInEditMode]
public class IKChain : MonoBehaviour 
{
	
    public Transform 	_targetPos;
	public  bool 		_isOneCCDLink = false;
	public  int			_OneCCdLinkNum = 0;
	public	bool 		_isDamping = false;
	public	bool 		_isDOF_Restrict = false;
	private bool 		_toggleIK = false;

	public bool toggleIK
	{
		get {return _toggleIK; }
	}

	private List<IKLink> _linkes = new List<IKLink>();


    public void Start()
    {
    }

	public void ToggleOff()
	{
		//true => false
		if(false == this._toggleIK) return;
		this._toggleIK = false;
		//----------------------------------------
	}

	public void ToggleOn()
	{
		//false => true
		if(true == this._toggleIK) return;
		this._toggleIK = true;
		//----------------------------------------

		_linkes.Clear ();
		Transform next = transform;
		for (int i = 0; i < 10; i++) 
		{
			//DebugWide.LogBlue (i);
			if (null == next) break;

			//DebugWide.LogBlue (i+" "+next.name);
			_linkes.Add (new IKLink (next, 2f));
			foreach (Transform child in next.GetComponentsInChildren<Transform> ()) 
			{
				if (next != child) {
					next = child;
					break;
				}
				next = null; //더이상 추가할 자식링크가 없다.
			}
		}
	}


	public float VectorSquaredDistance (Vector3 a, Vector3 b)
	{
		return (a - b).sqrMagnitude;
	}

	//Cyclic Coordinate Descent (CCD)
	///////////////////////////////////////////////////////////////////////////////
	// Procedure:	ComputeOneCCDLink
	// Purpose:		Compute an IK Solution to an end effector position
	// Arguments:	End Target (x,y)
	// Returns:		TRUE if a solution exists, FALSE if the position isn't in reach
	///////////////////////////////////////////////////////////////////////////////		
	public bool ComputeOneCCDLink(Vector3 targetPos,int link)
	{
		/// Local Variables ///////////////////////////////////////////////////////////
		Vector3		rootPos,curEnd,desiredEnd,targetVector,curVector,crossResult;
		float		cosAngle,turnAngle,turnDeg;
		///////////////////////////////////////////////////////////////////////////////
		int EFFECTOR_POS = _linkes.Count - 1;

		//if (null == _linkes [link] || null == _linkes[EFFECTOR_POS])
		//	return false;

		rootPos = _linkes [link].transform.position;

		curEnd = _linkes [EFFECTOR_POS].transform.position;

		desiredEnd = targetPos;
		desiredEnd.z = 0; //ONLY DOING 2D NOW

		if ((curEnd-desiredEnd).sqrMagnitude > 1.0f)
		{

			//마지막 링크는 마지막 전 링크의 방향값으로 사용된다.
			//조종할 링크와 마지막 링크가 같을 경우 방향값이 동일해저 회전각을 구할 수가 없다. 
			curVector = curEnd - rootPos;
			curVector.Normalize ();

			targetVector = targetPos - rootPos;
			targetVector.z = 0.0f;						// ONLY DOING 2D NOW
			targetVector.Normalize();

			//마지막 링크의 방향값을 직접 구해 준다.
			if (link == EFFECTOR_POS) 
			{
				float angleZ = _linkes [link].transform.rotation.eulerAngles.z;
				angleZ = Mathf.Deg2Rad * angleZ;

				Vector3 cv2;
				cv2.x = Vector3.right.x * Mathf.Cos (angleZ) - Vector3.right.y * Mathf.Sin (angleZ);
				cv2.y = Vector3.right.x * Mathf.Sin (angleZ) + Vector3.right.y * Mathf.Cos (angleZ);
				cv2.z = 0;
				curVector = cv2;
				curVector.Normalize();
			}

			cosAngle = Vector3.Dot(targetVector,curVector);

			if (cosAngle < 0.99999f)
			{
				crossResult = Vector3.Cross (targetVector, curVector);

				if (crossResult.z > 0.0f)
				{
					turnAngle = Mathf.Acos(cosAngle);
					turnDeg = Mathf.Rad2Deg * turnAngle;
					_linkes [link].AddLocalRotateZ (-turnDeg);

				}
				else if (crossResult.z < 0.0f)
				{
					turnAngle = Mathf.Acos(cosAngle);
					turnDeg = Mathf.Rad2Deg * turnAngle;
					_linkes [link].AddLocalRotateZ (turnDeg);

				}

			}
		}
		return true;
	}


	///////////////////////////////////////////////////////////////////////////////
	// Procedure:	ComputeCCDLink
	// Purpose:		Compute an IK Solution to an end effector position
	// Arguments:	End Target (x,y)
	// Returns:		TRUE if a solution exists, FALSE if the position isn't in reach
	///////////////////////////////////////////////////////////////////////////////		
	public bool ComputeCCDLink(Vector3 endPos)
	{
		//DOF(Degree of Free)

		/// Local Variables ///////////////////////////////////////////////////////////
		Vector3		rootPos,curEnd,desiredEnd,targetVector,curVector,crossResult;
		float		cosAngle,turnAngle,turnDeg;
		int			link,tries;
		int 		EFFECTOR_POS = _linkes.Count - 1;
		float 		IK_POS_THRESH =	1.0f;	// THRESHOLD FOR SUCCESS
		int			MAX_IK_TRIES = 100;		// TIMES THROUGH THE CCD LOOP (TRIES = # / LINKS) 

		///////////////////////////////////////////////////////////////////////////////
		// START AT THE LAST LINK IN THE CHAIN
		link = EFFECTOR_POS - 1;
		tries = 0;						// LOOP COUNTER SO I KNOW WHEN TO QUIT
		do
		{
			
			// THE COORDS OF THE X,Y,Z POSITION OF THE ROOT OF THIS BONE IS IN THE MATRIX
			// TRANSLATION PART WHICH IS IN THE 12,13,14 POSITION OF THE MATRIX
			rootPos = _linkes[link].transform.position;

			// POSITION OF THE END EFFECTOR
			curEnd = _linkes[EFFECTOR_POS].transform.position;

			// DESIRED END EFFECTOR POSITION
			desiredEnd = endPos;
			desiredEnd.z = 0.0f;						// ONLY DOING 2D NOW

			// SEE IF I AM ALREADY CLOSE ENOUGH
			if (VectorSquaredDistance(curEnd, desiredEnd) > IK_POS_THRESH)
			{
				// CREATE THE VECTOR TO THE CURRENT EFFECTOR POS
				curVector = curEnd - rootPos;
				// CREATE THE DESIRED EFFECTOR POSITION VECTOR
				targetVector = endPos - rootPos;
				targetVector.z = 0.0f;						// ONLY DOING 2D NOW

				// NORMALIZE THE VECTORS (EXPENSIVE, REQUIRES A SQRT)
				curVector.Normalize();
				targetVector.Normalize();

				// THE DOT PRODUCT GIVES ME THE COSINE OF THE DESIRED ANGLE
				cosAngle = Vector3.Dot(targetVector,curVector);

				// IF THE DOT PRODUCT RETURNS 1.0, I DON'T NEED TO ROTATE AS IT IS 0 DEGREES
				if (cosAngle < 0.99999)
				{
					// USE THE CROSS PRODUCT TO CHECK WHICH WAY TO ROTATE
					crossResult = Vector3.Cross(targetVector,curVector);
					if (crossResult.z > 0.0f)	// IF THE Z ELEMENT IS POSITIVE, ROTATE CLOCKWISE
					{
						turnAngle = Mathf.Acos(cosAngle);	// GET THE ANGLE
						turnDeg = Mathf.Rad2Deg * turnAngle;		// COVERT TO DEGREES
						// DAMPING
						if (_isDamping && turnDeg > _linkes[link].damp_width) 
							turnDeg = _linkes[link].damp_width;
						
						_linkes[link].AddLocalRotateZ(-turnDeg); // ACTUALLY TURN THE LINK
						// DOF RESTRICTIONS 
						if (_isDOF_Restrict && _linkes[link].angles.z < (float)_linkes[link].min_rz) 
							_linkes[link].SetLocalRotationZ((float)_linkes[link].min_rz);
					}
					else if (crossResult.z < 0.0f)	// ROTATE COUNTER CLOCKWISE
					{
						turnAngle = Mathf.Acos(cosAngle);	// GET THE ANGLE
						turnDeg = Mathf.Rad2Deg * turnAngle;		// COVERT TO DEGREES
						// DAMPING
						if (_isDamping && turnDeg > _linkes[link].damp_width) 
							turnDeg = _linkes[link].damp_width;
						_linkes[link].AddLocalRotateZ(turnDeg); // ACTUALLY TURN THE LINK
						// DOF RESTRICTIONS
						if (_isDOF_Restrict && _linkes[link].angles.z > (float)_linkes[link].max_rz) 
							_linkes[link].SetLocalRotationZ((float)_linkes[link].max_rz);
					}
					// RECALC ALL THE MATRICES WITHOUT DRAWING ANYTHING
					//drawScene(FALSE);		// CHANGE THIS TO TRUE IF YOU WANT TO SEE THE ITERATION
				}
				if (--link < 0) link = EFFECTOR_POS - 1;	// START OF THE CHAIN, RESTART
			}
			// QUIT IF I AM CLOSE ENOUGH OR BEEN RUNNING LONG ENOUGH
		} while (tries++ < MAX_IK_TRIES && VectorSquaredDistance(curEnd, desiredEnd) > IK_POS_THRESH);

		
		return true;
	}


    public void LateUpdate()
    {
		if (_toggleIK) 
		{
			if(null == _targetPos || 0 == _linkes.Count) return;

			if (_isOneCCDLink) 
				this.ComputeOneCCDLink (_targetPos.position, _OneCCdLinkNum);
			else
				this.ComputeCCDLink (_targetPos.position);
			
		}
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
		if(null == _targetPos || 0 == _linkes.Count) return;

		if (Selection.Contains(_targetPos.gameObject) && _toggleIK)
        {
			
			Gizmos.color = Color.white;
			Vector3 prev = _linkes[0].transform.position;
			foreach(IKLink link in _linkes)
			{
				Gizmos.DrawLine(prev, link.transform.position);	
				prev = link.transform.position;
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(prev, _targetPos.position);	

        }
#endif

    }
    

}
