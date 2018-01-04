using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;

public class UI_MonoBase : MonoBehaviour
{
	protected Camera _mainCamera = null;
	protected Canvas _canvasRoot = null;
	protected CanvasRenderer _panelRoot = null;

	public void Init_UI()
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		_mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		_canvasRoot = GameObject.Find ("Canvas").GetComponent<Canvas>().rootCanvas; 
		_panelRoot = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		Single.hierarchy.Init ();

		ResolutionController.CalcViewportRect (_canvasRoot, _mainCamera); //화면크기조정
		//=================================================
	}
}
