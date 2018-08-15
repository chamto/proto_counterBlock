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
            _text_stage = GameObject.Find("Text_stage").GetComponent<Text>();
            _text_info = GameObject.Find("Text_info").GetComponent<Text>();
            _button_retry = GameObject.Find("Button_retry");

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

        public void event_Button_Retry()
        {
            DebugWide.LogBlue("event Button Retry");
        }

    }
}//end namespace
