using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestViewportRect : MonoBehaviour 
{
	
	private string text_width = "";
	private string text_height = "";
	private string text_info1 = "";
	private string text_info2 = "";
	private int input_width = 0;
	private int input_height = 0;

	private int org_width = 0;
	private int org_height = 0;


	const float WIDTH_STANDARD = 1024;
	const float HEIGHT_STANDARD = 600;
	const float ASPECT_RATIO = WIDTH_STANDARD / HEIGHT_STANDARD;
	const float REVERSE_ASPECT_RATIO = HEIGHT_STANDARD / WIDTH_STANDARD;

	Camera _camera = null;
	CanvasScaler _canvasScaler = null;

	// Use this for initialization
	void Start () 
    {
		_camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		_canvasScaler = GameObject.Find ("Canvas").GetComponent<CanvasScaler> ();


		InitViewportRect ();

		org_width = input_width = Screen.width;
		org_height = input_height = Screen.height;
		text_width = org_width.ToString();
		text_height = org_height.ToString();


		text_info1 = _camera.pixelRect.ToString ();
		text_info2 = "aspect -  st: " + ASPECT_RATIO + "  cu:" + GetCameraAspect();

		DebugWide.LogBlue (_camera.pixelRect + "  camera aspect:" + GetCameraAspect() + "  reverse:" + GetCameraReverseAspect() );
	}

	public float GetCameraAspect()
	{
		return (float)_camera.pixelWidth / (float)_camera.pixelHeight;
	}

	public float GetCameraReverseAspect()
	{
		return (float)_camera.pixelHeight / (float)_camera.pixelWidth;
	}

	public void InitViewportRect()
	{
		//ui canvas
		//뷰포트렉 크기 기준으로 해상도에 상관없이 크기조정 설정
		_canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		_canvasScaler.referenceResolution = new Vector2 (WIDTH_STANDARD,HEIGHT_STANDARD);
		_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand; //이 모드를 사용해야 에디터와 디바이스상의 위치값이 일치한다


		//viewport
		_camera.aspect = ASPECT_RATIO; 
		Rect pr = _camera.pixelRect;
		pr.x = 0f;
		pr.y = 0f;
		pr.width = WIDTH_STANDARD;
		pr.height = HEIGHT_STANDARD;
		_camera.pixelRect = pr;

		DebugWide.LogBlue (_camera.pixelRect);
	}


	public void CalcViewportRect()
	{
		DebugWide.LogBlue ("CalcResolution");
		InitViewportRect ();

		//==================================
		int iHeight = (int)(Screen.width * REVERSE_ASPECT_RATIO);
		int iWidth = (int)(Screen.height * ASPECT_RATIO);
		Rect pr = _camera.pixelRect;
		//==================================

		float fScreenRate = (float)Screen.height / (float)Screen.width;
		if (fScreenRate > REVERSE_ASPECT_RATIO) 
		{ //기준해상도 비율에 비해 , 모바일 기기의 화면세로비율이 커졌거나, 가로비율이 작아진 경우
			
			//==================================
			pr = _camera.pixelRect;
			pr.height = iHeight;
			pr.width = Screen.width;
			pr.y = (Screen.height - iHeight) * 0.5f; //화면중앙으로 이동시킴
			_camera.pixelRect = pr;
			//==================================
			DebugWide.LogBlue (_camera.pixelRect + "  camera aspect:" + GetCameraAspect() + "  reverse:" + GetCameraReverseAspect() );

		} else if (fScreenRate < REVERSE_ASPECT_RATIO) 
		{ //기준해상도 비율에 비해 , 모바일 기기의 화면가로비율이 커졌거나, 세로비율이 작아진 경우
			
			//==================================
			pr = _camera.pixelRect;
			pr.height = Screen.height;
			pr.width = iWidth;
			pr.x = (Screen.width - iWidth) * 0.5f; //화면중앙으로 이동시킴
			_camera.pixelRect = pr;
			//==================================
			DebugWide.LogBlue (_camera.pixelRect + "  camera aspect:" + GetCameraAspect() + "  reverse:" + GetCameraReverseAspect() );
		} 

		text_info1 = _camera.pixelRect.ToString ();
		text_info2 = "aspect -  st: " + ASPECT_RATIO + "  cu:" + GetCameraAspect();
	}

	// Update is called once per frame
	void Update () {
	
	}


    public void OnGUI()
    {
        
		if (GUI.Button(new Rect(10, 10, 150, 50), "SetResolution Excute"))
		{
			Screen.SetResolution(input_width, input_height, true);
		}
		if (GUI.Button(new Rect(10, 10 + 50, 150, 50), "Restore"))
		{
			Screen.SetResolution(org_width, org_height, true);

			Rect r = _camera.rect;
			r.x = 0f;
			r.y = 0f;
			r.width = 1f;
			r.height = 1f;
			_camera.rect = r;
		}
		if (GUI.Button(new Rect(10, 10 + 100, 150, 50), "CalcResolution"))
		{
			this.CalcViewportRect ();
			//Display.main.Activate ();

		}//endif
        
		//--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=

		GUI.TextArea(new Rect(10 + 150, 10, 150, 50), "Screen.width/height :" + Screen.width + "/" + Screen.height + " isFull : " + Screen.fullScreen.ToString());

		text_width = GUI.TextArea(new Rect(10 + 150, 10 + 50, 150, 50), text_width, 100);
		text_height = GUI.TextArea(new Rect(10 + 150, 10 + 100, 150, 50), text_height, 100);
		GUI.TextArea(new Rect(10 + 150, 10 + 150, 150, 50), text_info1, 100);
		GUI.TextArea(new Rect(10 + 150, 10 + 200, 150, 50), text_info2, 100);

		int.TryParse(text_width, out input_width);
		int.TryParse(text_height, out input_height);


    }

}
