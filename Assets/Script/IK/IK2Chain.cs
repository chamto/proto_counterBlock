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

        UpdateBones();
    }

    public void UpdateBones()
    {
		float rot1, rot2;
		rot1 = transform.localEulerAngles.x;
		rot2 = _child.localEulerAngles.x;

        transform.localEulerAngles = new Vector3(rot1,0,0) ;
        _child.localEulerAngles = new Vector3(rot2,0,0);
    }


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
