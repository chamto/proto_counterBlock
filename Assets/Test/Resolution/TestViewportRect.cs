using UnityEngine;
using System.Collections;

public class TestViewportRect : MonoBehaviour 
{
	const float WIDTH_STANDARD = 1024;
	const float HEIGHT_STANDARD = 600;
	const float ASPECT_RATIO = WIDTH_STANDARD / HEIGHT_STANDARD;

	Camera _camera = null;

	// Use this for initialization
	void Start () 
    {
		_camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		_camera.aspect = ASPECT_RATIO; 
		Rect r = _camera.rect;
		r.y = 0.1f;
		r.height = 0.8f;
		_camera.rect = r;

		DebugWide.LogBlue (Screen.width + "  " + Screen.height);

		//2:1 = w : h  ,  w/h = 2/1 = 2 = a  , 
		//w/h=a , w=a*h , h=w/a      
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
    public void OnGUI()
    {
        
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
	//*/
}
