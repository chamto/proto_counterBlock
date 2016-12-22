using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

public class DebugWide 
{

#if UNITY_EDITOR
    static bool m_DebugLogView = true;
#else
	static bool m_DebugLogView = true;
#endif

    //static CommonNet.CEzLog m_Log = null;  
    static string m_strCommon = "";

    public enum eDebugLogMode
    {
        _None = 0,
        _Waring = 1,
        _Error = 2,
        _Exception = 3,
    }

    public static void Close()
    {
       // CDefine.m_Log.Close();
       // m_Log = null;
    }

    public static void _BaseLog(object message , Object context , System.Exception exception ,eDebugLogMode eMode)
    {
        //CommonNet.CNetLog.OutputDebugString((string)message);

		//m_strCommon = "[Thr:"+System.AppDomain.GetCurrentThreadId().ToString("00000") + "] "; // window only? 
		//m_strCommon = CommonNet.CUnitUtil.TimeStamp();
		message = m_strCommon + message;

        if (m_DebugLogView == true)
        {
            switch (eMode)
            {
                case eDebugLogMode._None:
                    {
                        Debug.Log(message, context); 
                    }
                    break;
                case eDebugLogMode._Waring:
                    {
                        Debug.LogWarning(message, context);
                    }
                    break;
                case eDebugLogMode._Error:
                    {
                        Debug.LogError(message, context);
                    }
                    break;
                case eDebugLogMode._Exception:
                    {
                        Debug.LogException(exception, context);    

                        message = exception.ToString();
                    }
                    break;
            }
            
        }

        
//        if (null == m_Log)
//            m_Log = new CommonNet.CEzLog("Client_debugLog.txt");
//        else
//        {
//            //m_Log.ReCreate("Client_debugLog.txt"); 
//            m_Log.Log(message.ToString(), CommonNet.LOG_TYPE.INFO);
//            //m_Log.Close();
//        }

    }

	public static void LogBool(bool boolColor, object message)
	{
		if (true == boolColor) 
		{
			_BaseLog("<color=white>"+message+"</color>",null,null, eDebugLogMode._None);
		} 
		else 
		{
			_BaseLog("<color=red>"+message+"</color>",null,null, eDebugLogMode._None);
		}
	}

	public static void LogWhite(object message)
	{
		_BaseLog("<color=white>"+message+"</color>",null,null, eDebugLogMode._None);
	}
	public static void LogYellow(object message)
	{
		_BaseLog("<color=yellow>"+message+"</color>",null,null, eDebugLogMode._None);
	}
	public static void LogGreen(object message)
	{
		_BaseLog("<color=green>"+message+"</color>",null,null, eDebugLogMode._None);
	}
	public static void LogRed(object message)
	{
		_BaseLog("<color=red>"+message+"</color>",null,null, eDebugLogMode._None);
	}
	public static void LogBlue(object message)
	{
		_BaseLog("<color=blue>"+message+"</color>",null,null, eDebugLogMode._None);
	}

	public static void Log(object message)
    {
        
        _BaseLog(message,null,null, eDebugLogMode._None);
    }

    public static void Log(object message, Object context)
    {
        _BaseLog(message, context,null, eDebugLogMode._None);
    }

    public static void LogError(object message)
    {
        _BaseLog(message, null,null, eDebugLogMode._Error);
    }

    public static void LogError(object message, Object context)
    {
        _BaseLog(message, context,null, eDebugLogMode._Error);
    }

    public static void LogException(System.Exception exception)
    {
       _BaseLog(null, null,exception, eDebugLogMode._Exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        _BaseLog(null, context,exception, eDebugLogMode._Exception);
    }

    public static void LogWarning(object message)
    {
        _BaseLog(message, null,null, eDebugLogMode._Waring);
    }

    public static void LogWarning(object message, Object context)
    {
        _BaseLog(message, context,null, eDebugLogMode._Waring);
    }


	//ref : http://fishpoint.tistory.com/677
//	public static bool CheckBoxToBox(Rect _rt1, Rect _rt2)
//	{
//
//		if(_rt1.right > _rt2.left && 
//		   _rt1.left <_rt2.right &&
//		   _rt1.top > _rt2.bottom &&
//		   _rt1.bottom < _rt2.top) return true;
//		
//		return false;		
//	}

}

