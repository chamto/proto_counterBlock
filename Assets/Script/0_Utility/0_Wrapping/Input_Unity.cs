using UnityEngine;
using System.Collections;

/// <summary>
/// 20140724 chamto 
/// 유티니 플랫폼별 입력처리를 공통의 인터페이스로 묶은 중계라이브러리
/// </summary>
public class Input_Unity
{


	private static bool	f_isEditorDraging = false;
	public static TouchPhase GetTouchEvent()
	{

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					DebugWide.LogWhite ("Update : TouchPhase.Began"); //chamto test
					return TouchPhase.Began;
				} else if (Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch (0).phase == TouchPhase.Stationary) {
					DebugWide.LogWhite ("Update : TouchPhase.Moved"); //chamto test
					return TouchPhase.Moved;
				} else if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					DebugWide.LogWhite ("Update : TouchPhase.Ended"); //chamto test
					return TouchPhase.Ended;
				} else {
					DebugWide.LogWhite ("Update : Exception Input Event : " + Input.GetTouch (0).phase);
					return Input.GetTouch (0).phase;
				}
			}
		} else if (Application.platform == RuntimePlatform.OSXEditor) {
			if (Input.GetMouseButtonDown (0)) {
					
				if (false == f_isEditorDraging) {

					//DebugWide.LogWhite ("______________ MouseButtonDown ______________"); //chamto test

					f_isEditorDraging = true;

					return TouchPhase.Began;
				}

			}
			
			if (Input.GetMouseButtonUp (0)) {	//mouse Up
				
				//DebugWide.LogWhite ("______________ MouseButtonUp ______________"); //chamto test
				f_isEditorDraging = false;

				return TouchPhase.Ended;
			}
			
			//else
			if (Input_Unity.GetMouseButtonMove (0)) {	//mouse Move
				
				if (f_isEditorDraging) {	///mouse Down + Move (Drag)
					
					//DebugWide.LogWhite ("______________ MouseMoved ______________"); //chamto test
					
					return TouchPhase.Moved;
				}//if
			}//if
		}
		return TouchPhase.Canceled;
	}

	public static bool IsTouch()
	{
		//DebugWide.Log("IsTouchCount : " + Input.touchCount);

		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//DebugWide.Log("1  IsTouchCount : " + Input.touchCount);
			return (Input.touchCount > 0);
			//return (Input.touchCount > 0 || Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved);
			//return Input.GetMouseButton(0);
		}else if(Application.platform == RuntimePlatform.OSXEditor)
		{
			//DebugWide.Log("2  IsTouchCount : " + Input.touchCount);
			return Input.GetMouseButton(0);
		}
		
		return false;
	}

	public static Vector2 GetTouchPos()
	{
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Input.GetTouch(0).position;
		}else if(Application.platform == RuntimePlatform.OSXEditor)
		{
			return Input.mousePosition;
		}
		
		return Vector2.zero;
	}

	public static Vector3 GetTouchWorldPos()
	{
		Vector3 pos = Input_Unity.GetTouchPos ();

		return Camera.main.ScreenToWorldPoint (pos);
	}

	public static bool GetMouseButtonMove(int button)
	{
		if (Input.GetMouseButton (button) && Input.GetMouseButtonDown (button) == false) 
		{
			return true;
		}

		return false;
	}
}
