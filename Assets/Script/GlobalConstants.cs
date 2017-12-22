using UnityEngine;
using System.Collections;


public class GlobalConstants : MonoBehaviour 
{

	#if UNITY_EDITOR
	public const string CURRENT_PLATFORM = "UNITY_EDITOR";
	public static string ASSET_PATH = "file://" + UnityEngine.Application.dataPath + "/StreamingAssets/";
	#elif UNITY_IPHONE
	public const string CURRENT_PLATFORM = "UNITY_IPHONE";
	public static string ASSET_PATH = "file://" + UnityEngine.Application.dataPath + "/Raw/";
	#elif UNITY_ANDROID
	public const string CURRENT_PLATFORM = "UNITY_ANDROID";
	public static string ASSET_PATH = "jar:file://" + UnityEngine.Application.dataPath + "!/assets/";
	#elif SERVER
	public const string CURRENT_PLATFORM = "SERVER";
	public static string ASSET_PATH = "Data_KOR\\";
	#elif TOOL
	public const string CURRENT_PLATFORM = "TOOL";
	public static string ASSET_PATH = "Data_KOR\\";
	#endif

	
	public class Hierarchy
	{
//		public static Rect	gameViewArea;
//
//		public static void Init()
//		{
//			GameObject obj = GameObject.Find ("bound");
//			RectTransform trans =  obj.GetComponent<RectTransform> ();
//			Hierarchy.gameViewArea = trans.rect;
//
//			//DebugWide.LogWhite ("Complete : GlobalConstants.Hierarchy Init " + Hierarchy.gameViewArea);
//		}
	}


	public class Hash_Ani
	{
//		private static int _hide = Animator.StringToHash("Base Layer.state_hide");
//		public static int hide {get {	return _hide; }}
//
//		private static int _rush = Animator.StringToHash("Base Layer.state_rush");
//		public static int rush {get {	return _rush; }}
//
//		private static int _eat = Animator.StringToHash("Base Layer.state_eat");
//		public static int eat {get {	return _eat; }}

	}

	public class Layer
	{

		public class Num
		{
//			public static int default0 = LayerMask.NameToLayer("Default");
//			public static int building = LayerMask.NameToLayer("Building");
//			public static int superCat = LayerMask.NameToLayer("SuperCat");

		}

		public class Mask
		{
//			public static int building = (1 << Layer.Num.building);
//
//			///전체마스크에서 건물레이어만 제외함
//			public static int except_building = ~(1 << Layer.Num.building);

		}
	}

	public static void InitStatic()
	{
		//Hierarchy.Init ();
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
