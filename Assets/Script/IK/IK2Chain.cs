using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
[ExecuteInEditMode]
public class IK2Chain : MonoBehaviour {

    public Transform _targetPos;
	public Transform _child;
	public float _d1 = 0;	//upperArm Length 
	public float _d2 = 0;	//foreArm Length

    [HideInInspector]
    public bool invert;
    [HideInInspector]
    public bool toggleIK=false;

    public void Start()
    {
    }

	/*
	//Cyclic Coordinate Descent (CCD)
	///////////////////////////////////////////////////////////////////////////////
	// Procedure:	ComputeOneCCDLink
	// Purpose:		Compute an IK Solution to an end effector position
	// Arguments:	End Target (x,y)
	// Returns:		TRUE if a solution exists, FALSE if the position isn't in reach
	///////////////////////////////////////////////////////////////////////////////		
	BOOL COGLView::ComputeOneCCDLink(CPoint endPos,int link)
	{
		/// Local Variables ///////////////////////////////////////////////////////////
		tVector		rootPos,curEnd,desiredEnd,targetVector,curVector,crossResult;
		double		cosAngle,turnAngle,turnDeg;
		///////////////////////////////////////////////////////////////////////////////

		rootPos.x = m_Link[link].matrix.m[12];
		rootPos.y = m_Link[link].matrix.m[13];
		rootPos.z = m_Link[link].matrix.m[14];

		curEnd.x = m_Link[EFFECTOR_POS].matrix.m[12];
		curEnd.y = m_Link[EFFECTOR_POS].matrix.m[13];
		curEnd.z = m_Link[EFFECTOR_POS].matrix.m[14];

		desiredEnd.x = (float)endPos.x;
		desiredEnd.y = (float)endPos.y;
		desiredEnd.z = 0.0f;						// ONLY DOING 2D NOW
		if (VectorSquaredDistance(&curEnd, &desiredEnd) > 1.0f)
		{
			curVector.x = curEnd.x - rootPos.x;
			curVector.y = curEnd.y - rootPos.y;
			curVector.z = curEnd.z - rootPos.z;

			targetVector.x = endPos.x - rootPos.x;
			targetVector.y = endPos.y - rootPos.y;
			targetVector.z = 0.0f;						// ONLY DOING 2D NOW

			NormalizeVector(&curVector);
			NormalizeVector(&targetVector);

			cosAngle = DotProduct(&targetVector,&curVector);

			if (cosAngle < 0.99999)
			{
				CrossProduct(&targetVector, &curVector, &crossResult);

				if (crossResult.z > 0.0f)
				{
					turnAngle = acos((float)cosAngle);
					turnDeg = RADTODEG(turnAngle);
					m_Link[link].rot.z -= (float)turnDeg;
				}
				else if (crossResult.z < 0.0f)
				{
					turnAngle = acos((float)cosAngle);
					turnDeg = RADTODEG(turnAngle);
					m_Link[link].rot.z += (float)turnDeg;
				}
				drawScene(FALSE);		// CHANGE THIS TO TRUE IF YOU WANT TO SEE THE ITERATION
			}
		}
		return TRUE;
	}

	///////////////////////////////////////////////////////////////////////////////
	// Procedure:	ComputeCCDLink
	// Purpose:		Compute an IK Solution to an end effector position
	// Arguments:	End Target (x,y)
	// Returns:		TRUE if a solution exists, FALSE if the position isn't in reach
	///////////////////////////////////////////////////////////////////////////////		
	BOOL COGLView::ComputeCCDLink(CPoint endPos)
	{
		/// Local Variables ///////////////////////////////////////////////////////////
		tVector		rootPos,curEnd,desiredEnd,targetVector,curVector,crossResult;
		double		cosAngle,turnAngle,turnDeg;
		int			link,tries;
		///////////////////////////////////////////////////////////////////////////////
		// START AT THE LAST LINK IN THE CHAIN
		link = EFFECTOR_POS - 1;
		tries = 0;						// LOOP COUNTER SO I KNOW WHEN TO QUIT
		do
		{
			// THE COORDS OF THE X,Y,Z POSITION OF THE ROOT OF THIS BONE IS IN THE MATRIX
			// TRANSLATION PART WHICH IS IN THE 12,13,14 POSITION OF THE MATRIX
			rootPos.x = m_Link[link].matrix.m[12];
			rootPos.y = m_Link[link].matrix.m[13];
			rootPos.z = m_Link[link].matrix.m[14];

			// POSITION OF THE END EFFECTOR
			curEnd.x = m_Link[EFFECTOR_POS].matrix.m[12];
			curEnd.y = m_Link[EFFECTOR_POS].matrix.m[13];
			curEnd.z = m_Link[EFFECTOR_POS].matrix.m[14];

			// DESIRED END EFFECTOR POSITION
			desiredEnd.x = (float)endPos.x;
			desiredEnd.y = (float)endPos.y;
			desiredEnd.z = 0.0f;						// ONLY DOING 2D NOW

			// SEE IF I AM ALREADY CLOSE ENOUGH
			if (VectorSquaredDistance(&curEnd, &desiredEnd) > IK_POS_THRESH)
			{
				// CREATE THE VECTOR TO THE CURRENT EFFECTOR POS
				curVector.x = curEnd.x - rootPos.x;
				curVector.y = curEnd.y - rootPos.y;
				curVector.z = curEnd.z - rootPos.z;
				// CREATE THE DESIRED EFFECTOR POSITION VECTOR
				targetVector.x = endPos.x - rootPos.x;
				targetVector.y = endPos.y - rootPos.y;
				targetVector.z = 0.0f;						// ONLY DOING 2D NOW

				// NORMALIZE THE VECTORS (EXPENSIVE, REQUIRES A SQRT)
				NormalizeVector(&curVector);
				NormalizeVector(&targetVector);

				// THE DOT PRODUCT GIVES ME THE COSINE OF THE DESIRED ANGLE
				cosAngle = DotProduct(&targetVector,&curVector);

				// IF THE DOT PRODUCT RETURNS 1.0, I DON'T NEED TO ROTATE AS IT IS 0 DEGREES
				if (cosAngle < 0.99999)
				{
					// USE THE CROSS PRODUCT TO CHECK WHICH WAY TO ROTATE
					CrossProduct(&targetVector, &curVector, &crossResult);
					if (crossResult.z > 0.0f)	// IF THE Z ELEMENT IS POSITIVE, ROTATE CLOCKWISE
					{
						turnAngle = acos((float)cosAngle);	// GET THE ANGLE
						turnDeg = RADTODEG(turnAngle);		// COVERT TO DEGREES
						// DAMPING
						if (m_Damping && turnDeg > m_Link[link].damp_width) 
							turnDeg = m_Link[link].damp_width;
						m_Link[link].rot.z -= (float)turnDeg;	// ACTUALLY TURN THE LINK
						// DOF RESTRICTIONS
						if (m_DOF_Restrict &&
							m_Link[link].rot.z < (float)m_Link[link].min_rz) 
							m_Link[link].rot.z = (float)m_Link[link].min_rz;
					}
					else if (crossResult.z < 0.0f)	// ROTATE COUNTER CLOCKWISE
					{
						turnAngle = acos((float)cosAngle);
						turnDeg = RADTODEG(turnAngle);
						// DAMPING
						if (m_Damping && turnDeg > m_Link[link].damp_width) 
							turnDeg = m_Link[link].damp_width;
						m_Link[link].rot.z += (float)turnDeg;	// ACTUALLY TURN THE LINK
						// DOF RESTRICTIONS
						if (m_DOF_Restrict &&
							m_Link[link].rot.z > (float)m_Link[link].max_rz) 
							m_Link[link].rot.z = (float)m_Link[link].max_rz;
					}
					// RECALC ALL THE MATRICES WITHOUT DRAWING ANYTHING
					drawScene(FALSE);		// CHANGE THIS TO TRUE IF YOU WANT TO SEE THE ITERATION
				}
				if (--link < 0) link = EFFECTOR_POS - 1;	// START OF THE CHAIN, RESTART
			}
			// QUIT IF I AM CLOSE ENOUGH OR BEEN RUNNING LONG ENOUGH
		} while (tries++ < MAX_IK_TRIES && 
			VectorSquaredDistance(&curEnd, &desiredEnd) > IK_POS_THRESH);
		return TRUE;
	}
	*/

    float theta1 = 0;
    float theta2 = 0;
    bool outOfRange;
    public void LateUpdate()
    {

        float z = _targetPos.position.z - transform.position.z;
		float y = _targetPos.position.y - transform.position.y;
		y = -y; //y->z 회전방향을 z->-y 회전방향으로 돌려서 계산한다. (x->y 에서의 식에 맞추기 위함)

		//제2코사인법칙 : a*a = b*b + c*c - 2bcCosA , //파타고라스의 정리 : z*z + y*y = x*x
        //float num = Mathf.Pow(z, 2) + Mathf.Pow(y, 2) - Mathf.Pow(d1, 2) - Mathf.Pow(d2, 2); 
		float num = _d1*_d1 + _d2*_d2 - (z*z + y*y);
        float denom = 2 * _d1 * _d2;
		float costheta2 = -(num / denom); //cosA = -cos(180-A)

		//삼각형 모양이 깨진 경우
		outOfRange = (Mathf.Abs(costheta2) > 1);
		costheta2 = Mathf.Clamp(costheta2, -1, 1);
        theta2 = Mathf.Acos(costheta2);

        if (invert)
            theta2 = -theta2;

		//y->z
		//ycos + -zsin 
		//ysin  + zcos
		//ref : http://www.darwin3d.com/gamedev/articles/col1198.pdf
		float atz = y * (_d1 + _d2 * Mathf.Cos(theta2)) - z * (_d2 * Mathf.Sin(theta2));
		float aty = z * (_d1 + _d2 * Mathf.Cos(theta2)) + y * (_d2 * Mathf.Sin(theta2));

		theta1 = z==0 && y==0? 0 : Mathf.Atan2(atz, aty);

        if (toggleIK)
        {
            transform.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta1),0,0);
            _child.localEulerAngles = new Vector3((Mathf.Rad2Deg * theta2),0,0);
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Selection.Contains(_targetPos.gameObject)&& toggleIK )
        {
            Gizmos.DrawIcon(_targetPos.position, "IKHandle.png", false);

			Gizmos.color = Color.white;
            Gizmos.DrawLine(_child.position, _targetPos.position);
            Gizmos.DrawLine(transform.position, _targetPos.position);

			Gizmos.color =  outOfRange ? Color.red: Color.green;
			Vector3 childPos = new Vector3(0,transform.position.y + _d1 * Mathf.Sin(-theta1), transform.position.z + _d1 * Mathf.Cos(-theta1)); //-theta1 ???
			Gizmos.DrawLine(transform.position, childPos);

        }
#endif

    }

}
