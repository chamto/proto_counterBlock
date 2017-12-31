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

    private int org_width = 0;
    private int org_height = 0;
    private bool org_fullFlag = false;

    const int WIDTH800 = 800;
    const int HEIGHT480 = 480;

	// Use this for initialization
	void Start () 
    {

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
        
        //20130729 chamto - 테스트를 위한 자체서버 접속용 추가
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
            float fScreenRate = (float)Screen.height / (float)Screen.width;
            if (fScreenRate > 0.6f) //기준해상도 비율에 비해 , 모바일 기기의 화면세로비율이 커졌거나, 가로비율이 작아진 경우
            {
                float fRate = (float)Screen.width / WIDTH800;
                int iHeight = (int)(HEIGHT480 * fRate);
                //CDefine.DebugLog("------------------Screen.height / Screen.width :" + Screen.height + " / " + Screen.width + "  iHeight : " + iHeight);//chamto test
                Screen.SetResolution(Screen.width, iHeight, false);
            }
            else if (fScreenRate < 0.6f) //기준해상도 비율에 비해 , 모바일 기기의 화면가로비율이 커졌거나, 세로비율이 작아진 경우
            {
                float fRate = (float)Screen.height / HEIGHT480;
                int iWidth = (int)(WIDTH800 * fRate);
                //CDefine.DebugLog("-----------------  iHeight : " + iWidth + "    Screen.height / Screen.width :" + Screen.height + " / " + Screen.width);//chamto test
                Screen.SetResolution(iWidth, Screen.height, false);
            }

        }//endif


        //--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=--=-=-=-=-=-=

        GUI.TextArea(new Rect(10 + 150, 10, 150, 50), "Screen.width/height :" + Screen.width + "/" + Screen.height + " isFull : " + Screen.fullScreen.ToString());

        vw_width = GUI.TextArea(new Rect(10 + 150, 10 + 50, 150, 50), vw_width, 100);
        vw_height = GUI.TextArea(new Rect(10 + 150, 10 + 100, 150, 50), vw_height, 100);
        vw_fullFlag = GUI.TextArea(new Rect(10 + 150, 10 + 150, 150, 50), vw_fullFlag, 100);

        int.TryParse(vw_width, out in_width);
        int.TryParse(vw_height, out in_height);

        if (true == bool.TryParse(vw_fullFlag, out isFullScreen))
        {
            //Debug.Log("isFullScreen : " + isFullScreen);
        }

    }
}
