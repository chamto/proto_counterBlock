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
