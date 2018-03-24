using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;

public class UI_MonoBase : MonoBehaviour
{
//	protected Camera _mainCamera = null;
//	protected Canvas _canvasRoot = null;
//	protected CanvasRenderer _panelRoot = null;

	public void Init_UI()
	{
		//=================================================
		//                    초 기 화 
		//=================================================
//		_mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
//		_canvasRoot = GameObject.Find ("Canvas").GetComponent<Canvas>().rootCanvas; 
//		_panelRoot = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		Single.hierarchy.Init ();

		ResolutionController.CalcViewportRect (Single.canvasRoot, Single.mainCamera); //화면크기조정
		//=================================================
	}

	private bool _process_title_ = false;
	public void LoadScene_Title()
	{
		if (true == _process_title_)
			return;
		//DebugWide.LogBlue ("Load...");
		_process_title_ = !_process_title_;
		StartCoroutine (GlobalFunctions.FadeOut (Single.panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.TITLE, 3.0f));
	}

	private bool _process_menu_ = false;
	public void LoadScene_Menu()
	{
		if (true == _process_menu_)
			return;
		//DebugWide.LogBlue ("Load...");
		_process_menu_ = !_process_menu_;
		StartCoroutine (GlobalFunctions.FadeOut (Single.panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.MAIN_MENU, 3.0f));
	}

	private bool _process_game_couple_ = false;
	public void LoadScene_Game_Couple()
	{
		if (true == _process_game_couple_)
			return;
		//DebugWide.LogBlue ("Load...");
		_process_game_couple_ = !_process_game_couple_;
		StartCoroutine (GlobalFunctions.FadeOut (Single.panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.GAME, 3.0f));
	}

	private bool _process_proto_singing_ = false;
	public void LoadScene_Proto_Singing()
	{
		if (true == _process_proto_singing_)
			return;
		//DebugWide.LogBlue ("Load...");
		_process_proto_singing_ = !_process_proto_singing_;
		StartCoroutine (GlobalFunctions.FadeOut (Single.panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.Proto_Singing, 3.0f));
	}
}
