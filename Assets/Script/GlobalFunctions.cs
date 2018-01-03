using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


static public class GlobalFunctions 
{
	static public IEnumerator LoadScene(string sceneName , float waitTime)
	{
		
		yield return new WaitForSeconds (waitTime);

		SceneManager.LoadSceneAsync (sceneName);
	}

	static public IEnumerator Fade(CanvasRenderer renderer , float processTime, int mode) 
	{
		const int COUNT = 30;
		const float RATE = 1f / COUNT;
		float alpha = renderer.GetAlpha ();
		for (int f = 1; f <= COUNT ; f++) 
		{
			
			//==========================================
			if(0 == mode) //fade in : 점점 뚜렷해지다
				alpha = f * RATE;
			if(1 == mode) //fade out : 점점 사라지다
				alpha = 1f - (f * RATE);
			//==========================================

			//renderer.SetAlpha (alpha);
			GlobalFunctions.SetAlpha_Traversal (renderer, alpha);

			yield return new WaitForSeconds(processTime / COUNT);
		}
	}

	static public IEnumerator FadeOut(CanvasRenderer renderer , float processTime) 
	{
		GlobalFunctions.SetAlpha_Traversal (renderer, 1f);
		yield return GlobalFunctions.Fade (renderer, processTime, 1);

//		c.a = 1f;
//		material.color = c;
	}

	static public IEnumerator FadeIn(CanvasRenderer renderer , float processTime) 
	{
		GlobalFunctions.SetAlpha_Traversal (renderer, 0f);
		yield return GlobalFunctions.Fade (renderer, processTime, 0);

//		c.a = 1f;
//		material.color = c;
	}



	static public void SetAlpha_Traversal(CanvasRenderer dst , float alpha)
	{
		//1. 알고리즘 수행 
		CanvasRenderer can =  dst.GetComponent<CanvasRenderer> ();
		if (null != can) 
		{
			can.SetAlpha (alpha);
		}

		//2. traversal
		CanvasRenderer[] tfoList = dst.GetComponentsInChildren<CanvasRenderer> (true);
		foreach(CanvasRenderer child in tfoList)
		{
			if(child != dst)
			{
				GlobalFunctions.SetAlpha_Traversal(child , alpha);
			}
		}

	}
}

public static class ResolutionController
{

	public const float WIDTH_STANDARD = 1024;
	public const float HEIGHT_STANDARD = 600;
	public const float ASPECT_RATIO = WIDTH_STANDARD / HEIGHT_STANDARD;
	public const float REVERSE_ASPECT_RATIO = HEIGHT_STANDARD / WIDTH_STANDARD;


	static private void InitViewportRect(Canvas root, Camera mainCamera)
	{
		//ui canvas
		//뷰포트렉 크기 기준으로 해상도에 상관없이 크기조정 설정
		if (null != root) 
		{
			CanvasScaler cans = root.GetComponent<CanvasScaler>();
			cans.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cans.referenceResolution = new Vector2 (WIDTH_STANDARD,HEIGHT_STANDARD);
			cans.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand; //이 모드를 사용해야 에디터와 디바이스상의 위치값이 일치한다
		}


		//viewport
		mainCamera.aspect = ASPECT_RATIO; 
		Rect pr = mainCamera.pixelRect;
		pr.x = 0f;
		pr.y = 0f;
		pr.width = WIDTH_STANDARD;
		pr.height = HEIGHT_STANDARD;
		mainCamera.pixelRect = pr;

		//DebugWide.LogBlue (_camera.pixelRect);
	}


	static public void CalcViewportRect(Canvas root, Camera mainCamera)
	{
		//DebugWide.LogBlue ("CalcResolution");
		ResolutionController.InitViewportRect (root , mainCamera);

		//==================================
		int iHeight = (int)(Screen.width * REVERSE_ASPECT_RATIO);
		int iWidth = (int)(Screen.height * ASPECT_RATIO);
		Rect pr = mainCamera.pixelRect;
		//==================================

		float fScreenRate = (float)Screen.height / (float)Screen.width;
		if (fScreenRate > REVERSE_ASPECT_RATIO) 
		{ //기준해상도 비율에 비해 , 모바일 기기의 화면세로비율이 커졌거나, 가로비율이 작아진 경우

			//==================================
			pr.height = iHeight;
			pr.width = Screen.width;
			pr.y = (Screen.height - iHeight) * 0.5f; //화면중앙으로 이동시킴
			mainCamera.pixelRect = pr;
			//==================================
			//DebugWide.LogBlue (mainCamera.pixelRect + "  camera aspect:" + GetCameraAspect() + "  reverse:" + GetCameraReverseAspect() );

		} else if (fScreenRate < REVERSE_ASPECT_RATIO) 
		{ //기준해상도 비율에 비해 , 모바일 기기의 화면가로비율이 커졌거나, 세로비율이 작아진 경우

			//==================================
			pr.height = Screen.height;
			pr.width = iWidth;
			pr.x = (Screen.width - iWidth) * 0.5f; //화면중앙으로 이동시킴
			mainCamera.pixelRect = pr;
			//==================================
			//DebugWide.LogBlue (_camera.pixelRect + "  camera aspect:" + GetCameraAspect() + "  reverse:" + GetCameraReverseAspect() );
		} 


	}//end func


	static public void CalcScreenResolution()
	{

		//* 지정된 비율로 해상도를 재조정한다.
		//디바이스 상에서는 실제 화면비율에 따라 늘어나 보이게 된다
		//이 처리로는 에디터와 디바이스 화면이 다르게 보이는 것을 잡을수 없다

		//standard : h/w = 0.6
		//h/w > 0.6 : 기준보다 h값 비율이 크다. w값 비율이 작다.
		//h/w < 0.6 : 기준보다 h값 비율이 작다. w값 비율이 크다.

		float fScreenRate = (float)Screen.height / (float)Screen.width;
		if (fScreenRate > REVERSE_ASPECT_RATIO) //기준해상도 비율에 비해 , 모바일 기기의 화면세로비율이 커졌거나, 가로비율이 작아진 경우
		{
			int iHeight = (int)(Screen.width * REVERSE_ASPECT_RATIO);
			Screen.SetResolution(Screen.width, iHeight, false);
		}
		else if (fScreenRate < REVERSE_ASPECT_RATIO) //기준해상도 비율에 비해 , 모바일 기기의 화면가로비율이 커졌거나, 세로비율이 작아진 경우
		{
			int iWidth = (int)(Screen.height * ASPECT_RATIO);
			Screen.SetResolution(iWidth, Screen.height, false);
		}

	}//end func
}//end class
