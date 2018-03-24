/// <summary>
/// 
/// GameMode_Battle.cs
/// 
/// 20180325 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CounterBlock
{



	public class InitialData
	{
		public enum eOption : Int32
		{

			Position 	= 1<<1,
			Scale 		= 1<<2,
			Rotation 	= 1<<3,
			All 		= 0xffff,
		};

		private SpriteRenderer 	_spriteRender = null;
		private Transform 		_transform = null;
		public Sprite sprite 
		{
			get 
			{ 
				if (null == _spriteRender)
					return null;
				return _spriteRender.sprite;
			}
			set
			{
				if (null == _spriteRender)
					return;
				_spriteRender.sprite = value;
			}
		}
		public GameObject gameObject
		{
			get
			{ 
				return _transform.gameObject;
			}
		}
		public Transform transform
		{
			get
			{
				return _transform;
			}
		}
		public Color color
		{
			get
			{
				if (null == _spriteRender)
					return Color.white;
				return _spriteRender.color;
			}
			set
			{
				if (null == _spriteRender)
					return;
				_spriteRender.color = value;
			}
		}


		private Vector3 		_scale = Vector3.one;
		private Quaternion		_rotation = Quaternion.identity;

		private Vector3 		_localPosition = Vector3.zero;
		private Vector3 		_localScale = Vector3.one;
		private Quaternion		_localRotation = Quaternion.identity;

		public Vector3 Get_InitialLocalPostition()
		{
			return _localPosition;
		}
		public Vector3 Get_InitialLocalScale()
		{
			return _localScale;
		}
		public Quaternion Get_InitialLocalRotation()
		{
			return _localRotation;
		}

		public Vector3 Get_InitialPostition()
		{
			//return transform.parent.position + _localPosition;

			//플립효과를 주기위해 x_scale 에 -1 을 하기 때문에 생기는 문제이다. 
			//플립한 객체의 자식 객체에 직접 위치를 갱신하는데는 문제가 없는데, 월드 위치값을 가져와서 다시 적용하려 하면 scale값이 적용 안되 문제가 생긴다. 
			//!!임시로 부모의 x_scale을 곱해준다. 부모의 scale값이 1 또는 -1 이라고 가정한다.
			Vector3 temp = _localPosition; temp.x *= transform.parent.localScale.x;
			return  transform.parent.position + temp;
		}

		public InitialData(Transform trs)
		{
			_spriteRender = null;
			_transform = trs;

			_localPosition = _transform.localPosition;
			_localScale = _transform.localScale;
			_localRotation = _transform.localRotation;

			//_position = spr.transform.position;
			_scale = _transform.lossyScale;
			_rotation = _transform.rotation;
		}

		public  InitialData(SpriteRenderer spr)
		{
			_spriteRender = spr;
			_transform = _spriteRender.transform;

			_localPosition = spr.transform.localPosition;
			_localScale = spr.transform.localScale;
			_localRotation = spr.transform.localRotation;

			//_position = spr.transform.position;
			_scale = spr.transform.lossyScale;
			_rotation = spr.transform.rotation;
		}

		public void SelectAction(Character.eKind kind ,ResourceManager.eActionKind actionKind)
		{
			switch (kind) 
			{
			case Character.eKind.Seonbi:
				sprite = Single.resource.GetAction_Seonbi (actionKind);
				break;
			case Character.eKind.Biking:
				sprite = Single.resource.GetAction_Biking (actionKind);
				break;
			}


		}

		public void  Revert(eOption opt)
		{
			if(eOption.Position == (opt & eOption.Position))
				_transform.localPosition = _localPosition;

			if(eOption.Scale == (opt & eOption.Scale))
				_transform.localScale = _localScale;

			if(eOption.Rotation == (opt & eOption.Rotation))
				_transform.localRotation = _localRotation;
		}

	}//end class


	public class Mono_CrashMonitor : MonoBehaviour
	{

		private UI_BattleCard _ui_parent = null;

		void Start()
		{
			_ui_parent = this.gameObject.GetComponentInParent<UI_BattleCard> ();
		}

		void OnTriggerEnter(Collider other)
		{
			UI_BattleCard src = this.gameObject.GetComponentInParent<UI_BattleCard> ();
			UI_BattleCard dst = other.gameObject.GetComponentInParent<UI_BattleCard> ();
			if (null == dst || src._id == dst._id)
				return;
			DebugWide.LogBlue ("OnTriggerEnter:  " + " [" + src._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}
		void OnTriggerStay(Collider other)
		{
			UI_BattleCard src = this.gameObject.GetComponentInParent<UI_BattleCard> ();
			UI_BattleCard dst = other.gameObject.GetComponentInParent<UI_BattleCard> ();
			if (null == dst || src._id == dst._id)
				return;
			DebugWide.LogBlue ("OnTriggerStay:  " + " [" + src._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}
		void OnTriggerExit(Collider other)
		{
			UI_BattleCard src = this.gameObject.GetComponentInParent<UI_BattleCard> ();
			UI_BattleCard dst = other.gameObject.GetComponentInParent<UI_BattleCard> ();
			if (null == dst || src._id == dst._id)
				return;
			DebugWide.LogBlue ("OnTriggerExit:  " + " [" + src._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}

		//=====================================================================================

		void OnCollisionEnter (Collision other)
		{
			if(null != _ui_parent)
				_ui_parent.OnCollisionEnter (other);
		}
		void OnCollisionStay (Collision other)
		{
			if(null != _ui_parent)
				_ui_parent.OnCollisionStay (other);
		}
		void OnCollisionExit (Collision other)
		{
			if(null != _ui_parent)
				_ui_parent.OnCollisionExit (other);
		}
	}


	public struct CharDataBundle
	{
		public Character 		_data;
		public UI_BattleCard _ui;
		public GameObject 		_gameObject;
	}

	public class GameMode_Battle : MonoBehaviour
	{

		private Transform _1P_start = null;
		private Transform _2P_start = null;

		private Dictionary<uint, UI_BattleCard> _characters = new Dictionary<uint, UI_BattleCard> ();

		public const int START_POINT_LEFT = 1;
		public const int START_POINT_RIGHT = 2;
		//=================


		public void Init()
		{
			//this.transform.SetParent (Single.game_root, false);

			string parentPath = Single.hierarchy.GetFullPath (Single.game_root);
			_1P_start = Single.hierarchy.GetTransform (parentPath + "/startPoint_1");
			_2P_start = Single.hierarchy.GetTransform (parentPath + "/startPoint_2");

		}

		public UI_BattleCard GetCharacter(Character data)
		{
			return _characters [data.GetID ()];
		}

		public UI_BattleCard GetCharacter(uint idx)
		{
			return _characters [idx];
		}

		public UI_BattleCard AddCharacter(Character data)
		{
			uint id = data.GetID();
			UI_BattleCard card = UI_BattleCard.Create ("player_"+id.ToString("00"));
			//card.data = data;
			card.SetData(data);
			card._id = id;
			card._UI_Battle = this;
			_characters.Add (id, card);

			return card;
		}



		public void SetStartPoint(uint id, float delta_x , int pointNumber)
		{


			UI_BattleCard card = _characters [id];
			Vector3 pos = Vector3.zero;

			switch (pointNumber) 
			{
			case START_POINT_LEFT:
				pos = _1P_start.localPosition;
				card.TurnRight ();
				break;
			case START_POINT_RIGHT:
				pos = _2P_start.localPosition;
				card.TurnLeft ();
				break;
			}


			pos.x += delta_x;
			card.SetPosition (pos);

		}


		//void Update() {}  //chamto : 유니티 update 사용하지 말것. 호출순서를 코드에서 조작하기 위함

		public void Update_UI()
		{
			foreach (UI_BattleCard card in _characters.Values) 
			{
				card.Update_UI ();
			}
		}

	}

}//end namespace 
