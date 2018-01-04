using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MonoInputManager : MonoBehaviour 
{
	
	//private Vector3 			m_prevMousePosition;
	private Vector2				m_prevTouchMovedPos = Vector3.zero;
	private GameObject			m_TouchedObject = null;
	private bool				m_permitEvent_DropTouch = true;
	public bool permitEvent_DropTouch
	{
		get {
			return m_permitEvent_DropTouch;
		}
		set {
			m_permitEvent_DropTouch = value;
		}
	}

	public Vector2 prevTouchMovedPos
	{
		get
		{
			return m_prevTouchMovedPos;
		}
	}

	void Awake()
	{

		Input.simulateMouseWithTouches = false;	
		Input.multiTouchEnabled = false;

		//m_prevMousePosition = Input.mousePosition;
	}

	// Use this for initialization
	void Start () 
	{


	}


//	//startstartstartstartstartstartstartstartstartstartstartstartstartstartstartstartstart
//	#region Sources obsolete - test source 
//
//	void ProcessInput(RaycastHit2D hit2d , RaycastHit hit3d)
//	{
//		if(Input.touchCount <= 0 ) return;
//
//		if(Input.GetTouch(0).phase == TouchPhase.Began)
//		{
//			//Todo
//		}
//		else if(Input.GetTouch(0).phase == TouchPhase.Moved)
//		{
//			//Todo
//			if(hit2d.collider.CompareTag("drop"))
//			{
//				//drop move code
//				DebugWide.Log("2d TouchPhase Move Drop!!");
//			}
//			if(hit3d.collider.CompareTag("drop"))
//			{
//				//drop move code
//				DebugWide.Log("3d TouchPhase Move Drop!!");
//			}
//		}
//		else if(Input.GetTouch(0).phase == TouchPhase.Ended)
//		{
//			//Todo
//		}
//	}
//
//
//
//	RaycastHit2D RayCast2D(Ray ray )
//	{
//
//		return Physics2D.Raycast(ray.origin, ray.direction);
//
////		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
////		if(hit)
////		{
////			ProcessInput(hit,null);
////		}
//	}
//
//	RaycastHit RayCast3D(Ray ray )
//	{
//		RaycastHit hit;
//		Physics.Raycast(ray, out hit, Mathf.Infinity);
//		return hit;
//
////		if(Physics.Raycast(ray, out hit, Mathf.Infinity))
////		{
////			ProcessInput(null,hit);
////		}
//	}
//
//
//
//	// Update is called once per frame
//	void doNotUse_Update () 
//	{
//		//if(false)
//		////if(Input.mousePresent)
////		{
////			DebugWide.Log("Update Mouse Input!! -----"); //chamto test
////
////			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
////
////			
////			ProcessInput(
////				RayCast2D(ray),RayCast3D(ray));
////		}
//		if(Input.touchCount > 0 )
//		{
//			DebugWide.Log("Update Touch Input!! -----"); //chamto test
//
//			Vector2 pos = Input.GetTouch(0).position; 
//			Vector3 theTouch = new Vector3(pos.x , pos.y , 0.0f);
//			
//			Ray ray = Camera.main.ScreenPointToRay(theTouch);
//
//
//			ProcessInput(
//				RayCast2D(ray),RayCast3D(ray));
//			
//		}
//	}
//
//	#endregion
//	//endendendendendendendendendendendendendendendendendendendendendendendendendendendend
//


	//----------------------------------------------------------------------------------

	private void Init_PrevTouchMovedPos()
	{
		m_prevTouchMovedPos = Input_Unity.GetTouchPos(); //init touchedMovePos
	}

	/// <summary>
	/// TouchMoved 함수 호출후 갱신이 되어야 한다.
	/// </summary>
	private void Update_PrevTouchMovedPos()
	{
		m_prevTouchMovedPos = Input_Unity.GetTouchPos(); //update touchedMovePos
	}

	/// <summary>
	/// f : function
	/// 특정 함수 전용 멤버 변수. 다른 함수에서 접근하여 사용해서는 안된다
	/// 언어적 문법으로 막는 방법이 없다. (c++ 이라면 함수내 정적변수를 사용해 해결가능함)
	/// </summary>
	private bool				f_isEditorDraging = false;
	void Push_TouchEvent()
	{

		//DebugWide.Log(Input.touchCount);
		
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.touchCount > 0) {
				if(Input.GetTouch(0).phase == TouchPhase.Began)
				{
					//DebugWide.LogError("Update : TouchPhase.Began"); //chamto test
					Init_PrevTouchMovedPos();
					m_TouchedObject = SendMessage_TouchObject("TouchBegan",Input.GetTouch(0).position);
				}
				else if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
				{
					//DebugWide.LogError("Update : TouchPhase.Moved"); //chamto test

					if(null != m_TouchedObject)
						m_TouchedObject.SendMessage("TouchMoved",0,SendMessageOptions.DontRequireReceiver);

					Update_PrevTouchMovedPos();
					
				}
				else if(Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					//DebugWide.LogError("Update : TouchPhase.Ended"); //chamto test
					//checkInput("TouchEnded",Input.GetTouch(0).position);
					if(null != m_TouchedObject)
						m_TouchedObject.SendMessage("TouchEnded",0,SendMessageOptions.DontRequireReceiver);
					m_TouchedObject = null;
				}
				else
				{
					DebugWide.LogError("Update : Exception Input Event : " + Input.GetTouch(0).phase);
				}
			}
		}else if(Application.platform == RuntimePlatform.OSXEditor)
		{
			//Debug.Log("mousedown:" +Input.GetMouseButtonDown(0)+ "  mouseup:" + Input.GetMouseButtonUp(0) + " state:" +Input.GetMouseButton(0)); //chamto test
			if(Input.GetMouseButtonDown(0)) 
			{

				//Debug.Log ("______________ MouseBottonDown ______________" + m_TouchedObject); //chamto test

				//if(0 == (m_prevMousePosition - Input.mousePosition).sqrMagnitude)
				{	//mouse Down


					if(false == f_isEditorDraging)
					{
						//Init_PrevTouchMovedPos();
						m_TouchedObject = SendMessage_TouchObject("TouchBegan",Input.mousePosition);
						if(null != m_TouchedObject)
							f_isEditorDraging = true;
					}

					
				}

				//DebugWide.Log("--------------"); //chamto test
				
			}

			if(Input.GetMouseButtonUp(0))
			{	//mouse Up

				//Debug.Log ("______________ MouseButtonUp ______________" + m_TouchedObject); //chamto test
				f_isEditorDraging = false;
				
				if(null != m_TouchedObject)
					m_TouchedObject.SendMessage("TouchEnded",0,SendMessageOptions.DontRequireReceiver);
				m_TouchedObject = null;
				//checkInput("TouchEnded",Input.mousePosition);

			}

			//else
			if(Input_Unity.GetMouseButtonMove(0))
			{	//mouse Move
				
				if(f_isEditorDraging)
				{	///mouse Down + Move (Drag)
					
					//Debug.Log ("______________ MouseMoved ______________" + m_TouchedObject); //chamto test
					
					if(null != m_TouchedObject)					
						m_TouchedObject.SendMessage("TouchMoved",0,SendMessageOptions.DontRequireReceiver);
					
					//Update_PrevTouchMovedPos();
				}//if
			}//if
			
			//m_prevMousePosition = Input.mousePosition;
		}
	}


	void Update()
	//void FixedUpdate()
	{

		Push_TouchEvent();
	}


	//보조 함수 
	//내부멤버변수에 접근 안함
	//함수내부에서 값변환 없음
	GameObject SendMessage_TouchObject(string callbackMethod , Vector3 touchPos) 
	{
		//slow?
		//Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
		//Vector3 touchPos = new Vector2(wp.x, wp.y);
		//Collider2D hit = Physics2D.OverlapPoint(touchPos,LayerMask.NameToLayer("default"));

		//test process speed 
		Ray ray = Camera.main.ScreenPointToRay(touchPos);

		//20150917 chamto TODO - have to input code : first uisystem test , second raycast test
		//Debug.Log ("  -- currentSelectedGameObject : " + EventSystem.current.currentSelectedGameObject); //chamto test

		//1. uisystem input event test
		if (null != EventSystem.current && null != EventSystem.current.currentSelectedGameObject) 
		{
			return null;
		}

		//2. game input event test
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
		
		if(hit){
			//DebugWide.Log(hit.transform.gameObject.name); //chamto test
			hit.transform.gameObject.SendMessage(callbackMethod,0,SendMessageOptions.DontRequireReceiver);

//			if(hit.tag.Equals("collider"))
//			{
//				hit.transform.parent.gameObject.SendMessage(methodName,0,SendMessageOptions.DontRequireReceiver);
//			}else
//			{
//				hit.transform.gameObject.SendMessage(methodName,0,SendMessageOptions.DontRequireReceiver);
//			}

			return hit.transform.gameObject;
		}

		return null;
	}

	void TouchBegan() {}
	void TouchMoved() {}
	void TouchEnded() {}


}
