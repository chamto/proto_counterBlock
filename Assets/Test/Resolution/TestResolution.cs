using UnityEngine;
using System.Collections;

public class TestResolution : MonoBehaviour 
{

    private int in_width = 0;
    private int in_height = 0;
    private bool isFullScreen = false;

    private string vw_width = "";
    private string vw_height = "";
    private string vw_fullFlag = "";
	private string text_info1 = "";

    private int org_width = 0;
    private int org_height = 0;
    private bool org_fullFlag = false;

	const float WIDTH_800 = 800;
	const float HEIGHT_480 = 480;
	const float H_divide_W = HEIGHT_480 / WIDTH_800;
	const float W_divide_H = WIDTH_800 / HEIGHT_480;

	Camera _camera = null;

	// Use this for initialization
	void Start () 
    {
		_camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();

        org_width = in_width = Screen.width;
        org_height = in_height = Screen.height;
        org_fullFlag = isFullScreen = Screen.fullScreen;


        vw_width = org_width.ToString();
        vw_height = org_height.ToString();
        vw_fullFlag = org_fullFlag.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
    public void OnGUI()
    {
		int btn_x = 0;
		for (int i = 0; i < 10; i++) 
		{
			btn_x = 300 + i * 200;
			GUI.Button (new Rect (300 + i*200 , 10, 100, 100), btn_x+",10");
		}

		text_info1 = _camera.pixelRect.ToString();

		//===============================================================
        if (GUI.Button(new Rect(10, 10, 150, 50), "SetResolution Excute"))
        {
            Screen.SetResolution(in_width, in_height, isFullScreen);
        }
        if (GUI.Button(new Rect(10, 10 + 50, 150, 50), "Restore"))
        {
            Screen.SetResolution(org_width, org_height, org_fullFlag);
        }
        if (GUI.Button(new Rect(10, 10 + 100, 150, 50), "swingResolution"))
        {
			//* 지정된 비율로 해상도를 재조정한다.
			//디바이스 상에서는 실제 화면비율에 따라 늘어나 보이게 된다
			//이 처리로는 에디터와 디바이스 화면이 다르게 보이는 것을 잡을수 없다
			
			//standard : h/w = 0.6
			//h/w > 0.6 : 기준보다 h값 비율이 크다. w값 비율이 작다.
			//h/w < 0.6 : 기준보다 h값 비율이 작다. w값 비율이 크다.

            float fScreenRate = (float)Screen.height / (float)Screen.width;
			if (fScreenRate > H_divide_W) //기준해상도 비율에 비해 , 모바일 기기의 화면세로비율이 커졌거나, 가로비율이 작아진 경우
            {
				int iHeight = (int)(Screen.width * H_divide_W);
                Screen.SetResolution(Screen.width, iHeight, false);
            }
			else if (fScreenRate < H_divide_W) //기준해상도 비율에 비해 , 모바일 기기의 화면가로비율이 커졌거나, 세로비율이 작아진 경우
            {
				int iWidth = (int)(Screen.height * W_divide_H);
                Screen.SetResolution(iWidth, Screen.height, false);
            }

        }//endif




        //--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=

        GUI.TextArea(new Rect(10 + 150, 10, 150, 50), "Screen.width/height :" + Screen.width + "/" + Screen.height + " isFull : " + Screen.fullScreen.ToString());

        vw_width = GUI.TextArea(new Rect(10 + 150, 10 + 50, 150, 50), vw_width, 100);
        vw_height = GUI.TextArea(new Rect(10 + 150, 10 + 100, 150, 50), vw_height, 100);
        vw_fullFlag = GUI.TextArea(new Rect(10 + 150, 10 + 150, 150, 50), vw_fullFlag, 100);
		GUI.TextArea(new Rect(10 + 150, 10 + 200, 150, 50), text_info1, 100);

        int.TryParse(vw_width, out in_width);
        int.TryParse(vw_height, out in_height);

        if (true == bool.TryParse(vw_fullFlag, out isFullScreen))
        {
            //Debug.Log("isFullScreen : " + isFullScreen);
        }

    }
}
