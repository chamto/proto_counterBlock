using UnityEngine;
using UnityEngine.UI;


//========================================================
//==================        U  I        ==================
//========================================================
namespace ProtoGame
{
    public class UI_Control : MonoBehaviour
    {
        public Text _text_stage = null;
        public Text _text_info = null;
        public GameObject _button_retry = null;

        private void Start()
        {
            
            _text_stage = this.FindUI("Text_stage").GetComponent<Text>();
            _text_info = this.FindUI("Text_info").GetComponent<Text>();
            _button_retry = this.FindUI("Button_retry").gameObject;

            _button_retry.SetActive(false);
        }


        //1초 = 1000ms , 1분 = 1000 * 60
        public void SetTextInfo(float s, uint score)
        {
            string t_s = s.ToString("00.00");

            _text_info.text = "시간 : " + t_s + "   점수 : " + score.ToString("00") + " ";

        }

        public void SetTextStage(uint stageNum)
        {
            _text_stage.text = "[ " + stageNum.ToString("00") + " 단계 ] ";
        }

        public Transform FindUI(string find_name)
        {
            foreach (Transform t in Single.canvasRoot.GetComponentsInChildren<Transform>(true))
            {
                //DebugWide.LogBlue(t.name); //chamto test
                if (t.name.Equals(find_name))
                {
                    return t;
                }
            }

            return null;
        }

        //____________________________________________
        //                 UI 활성/비활성  
        //____________________________________________

		public void Active_Button_Retry(bool value)
		{
            _button_retry.SetActive(value);
		}

		//___________________________________________
		//                콜백 이벤트 함수 
		//____________________________________________

		public void event_Button_Retry()
        {
            DebugWide.LogBlue("event Button Retry");
        }

    }
}//end namespace
