/// <summary>
/// 
/// Simulation_Battle2
/// 
/// 20170825 - chamto - anrudco@gmail.com
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
	
	public class UI_CharacterCard : MonoBehaviour
	{

		public enum eAction
		{
			None,
			Idle,
			Action,
			Hilt,		//칼자루
			Blade,		//칼날
			Max
		}

		public enum eEffect
		{
			None,
			Empty,
			Text,
			Hit,
			Block,
			Wind,
			Max
		};

		public GameMode_Battle _UI_Battle = null;

		public uint _id = 0;
		public Character _data = null;

		public TextMesh _text_explanation { get; set; }
		public TextMesh _text_time { get; set; }
		//public Slider _hp_bar { get; set; }
		public AudioSource _audioSource { get; set; }

		public Transform _actionRoot { get; set; }
		public Dictionary<eAction,InitialData> _actions { get; set; }

		public Transform _effectRoot { get; set; }
		public Transform _effect_Texts { get; set; }
		public Dictionary<eEffect,InitialData> _effects { get; set; }

		//[SerializeField] 
		public bool _apply = false;

		public int _voiceSequence = 0;

		//===============================================================

//		public Character data
//		{
//			get;
//			set;
//		}

		//===============================================================

		public UI_CharacterCard()
		{
			//this.data = null;
		}

		void Start()
		{

		}

		static public UI_CharacterCard Create(string name)
		{
			GameObject obj = Single.resource.CreatePrefab ("character_seonbi2", Single.game_root, name);

			string parentPath = Single.hierarchy.GetFullPath (obj.transform);

			UI_CharacterCard ui = obj.AddComponent<UI_CharacterCard> ();
			ui._text_explanation = Single.hierarchy.GetTypeObject<TextMesh> (parentPath + "/Text_explanation") ;
			ui._text_time = Single.hierarchy.GetTypeObject<TextMesh> (parentPath + "/Text_time");
			//ui._hp_bar = Single.hierarchy.Find<Slider> (parentPath + "/Slider");
			ui._audioSource = obj.GetComponent<AudioSource>();

			//action
			ui._actionRoot = Single.hierarchy.GetTransform (parentPath + "/Images");
			ui._actions = new Dictionary<eAction, InitialData> ();

			SpriteRenderer img = null;
			Transform trs = null;
			InitialData iData = null;
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Images/Action_00");
			iData = new InitialData (img);
			ui._actions.Add (eAction.Idle, iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Images/Action_01");
			iData = new InitialData (img);
			ui._actions.Add (eAction.Action, iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Images/Hilt/Blade");
			iData = new InitialData (img);
			ui._actions.Add (eAction.Blade, iData);
			img.gameObject.AddComponent<Mono_CrashMonitor> (); //무기카드에 충돌감시기를 붙인다
			trs = Single.hierarchy.GetTransform (parentPath + "/Images/Hilt");
			iData = new InitialData (trs);
			ui._actions.Add (eAction.Hilt, iData);

			//effect
			ui._effectRoot = Single.hierarchy.GetTransform (parentPath + "/Effects");
			ui._effect_Texts = Single.hierarchy.GetTransform (parentPath + "/Effects/texts");
			ui._effects = new Dictionary<eEffect, InitialData> ();
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Effects/empty");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Empty,iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Effects/texts/hit");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Hit,iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Effects/block");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Block,iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Effects/wind");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Wind,iData);
			img = Single.hierarchy.GetTypeObject<SpriteRenderer> (parentPath + "/Effects/texts/hit/fuck");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Text,iData);

			return ui;
		}

		public void RevertData_All()
		{

			foreach (InitialData iData in _actions.Values) 
			{
				iData.Revert (InitialData.eOption.All);
			}

			foreach (InitialData iData in _effects.Values) 
			{
				iData.Revert (InitialData.eOption.All);
			}

		}

		// <--
		public void TurnLeft()
		{
			Vector3 scale = _actionRoot.localScale;
			scale.x = -1f;
			_actionRoot.localScale = scale;

			scale = _effectRoot.localScale;
			scale.x = -1f;
			_effectRoot.localScale = scale;

			//반전된 글자를 다시 반전시켜 원상태로 만든다. 
			scale = _effect_Texts.localScale;
			scale.x = -1f;
			_effect_Texts.localScale = scale;

			_data.SetDirection (Vector3.left);
			//_hp_bar.direction = Slider.Direction.RightToLeft;
		}

		// -->
		public void TurnRight()
		{
			Vector3 scale = _actionRoot.localScale;
			scale.x = 1f;
			_actionRoot.localScale = scale;

			scale = _effectRoot.localScale;
			scale.x = 1f;
			_effectRoot.localScale = scale;

			scale = _effect_Texts.localScale;
			scale.x = 1f;
			_effect_Texts.localScale = scale;

			_data.SetDirection (Vector3.right);
			//_hp_bar.direction = Slider.Direction.LeftToRight;
		}

		public void SetData(Character data)
		{
			_data = data;
		}

		public Character GetData()
		{
			return _data;
		}

		public void SetKind(Character.eKind kind)
		{
			_data.kind = kind;
		}

		public void SetPosition(Vector3 pos)
		{

			_data.SetPosition (pos);

			transform.localPosition = pos;

		}

		public Vector3 GetPosition()
		{
			return _data.GetPosition ();
		}

		public Sprite GetAction(Character.eKind ekind ,ResourceManager.eActionKind actionKind)
		{
			switch (ekind) 
			{
			case Character.eKind.Seonbi:
				return Single.resource.GetAction_Seonbi (actionKind);
			case Character.eKind.Biking:
				return Single.resource.GetAction_Biking (actionKind);
			}

			return null;
		}

		public float GetLength_Between_WeaponeCard()
		{
			return Mathf.Abs (_actions [eAction.Hilt].transform.localPosition.x);
		}

		public void Revert_ActionRoot()
		{
			_actionRoot.localEulerAngles = Vector3.zero;
			_actionRoot.localPosition = Vector3.zero;

			if (_data.GetDirection ().x > 0) 
			{  	//오른쪽을 보고 있음
				_actionRoot.localScale = new Vector3(1,1,1);
			} 
			else 
			{	//왼쪽을 보고 있음
				_actionRoot.localScale = new Vector3(-1,1,1);
			}
		}

		public void Revert_EffectRoot()
		{
			_effectRoot.localEulerAngles = Vector3.zero;
			_effectRoot.localPosition = Vector3.zero;

			if (_data.GetDirection ().x > 0) 
			{  	//오른쪽을 보고 있음
				_effectRoot.localScale = new Vector3(1,1,1);
			} 
			else 
			{	//왼쪽을 보고 있음
				_effectRoot.localScale = new Vector3(-1,1,1);
			}
		}

		public void StartSkill(Skill.eName skillName)
		{
			switch (skillName) 
			{
			case Skill.eName.Attack_Strong_1:
				_data.Attack_Strong ();
				break;
			case Skill.eName.Attack_Weak_1:
				_data.Attack_Weak ();
				break;
			case Skill.eName.Block_1:
				_data.Block ();
				break;
			}

		}


		//void Update() {}  //chamto : 유니티 update 사용하지 말것. 호출순서를 코드에서 조작하기 위함

		public void Update_UI()
		{
			//update position
			this.SetPosition(transform.position);

			if (true == _apply) 
			{
				//----------------------------------------

				//----------------------------------------
				_apply = false;
			}

			this.Update_UI_Explan ();
			this.Update_UI_Card ();
			this.Update_UI_HPBAR ();
			this.Update_UI_Effect ();
			this.Update_UI_Debug ();

		}

		Vector3 _debug_dir = Vector3.zero;
		Quaternion _debug_q = Quaternion.identity;
		Vector3 _debug_line = Vector3.zero;
		void OnDrawGizmos()
		{
			//*
			//공격 범위 - 안쪽원/바깥원
			Gizmos.color = Color.gray;
			Gizmos.DrawWireSphere(_data.GetPosition(), _data.GetRangeMin());
			Gizmos.DrawWireSphere(_data.GetPosition(), _data.GetRangeMax());

			//공격 범위 - 호/수직 : Vector3.forward
			//eTraceShape tr = eTraceShape.None;
			//_data.GetBehavior().attack_shape

			if (0 != _data.GetArc_Weapon ().degree) 
			{
				Gizmos.color = Color.yellow;
				_debug_q = Quaternion.AngleAxis (_data.GetArc_Weapon ().degree * 0.5f, Vector3.forward);
				_debug_dir = _debug_q * _data.GetArc_Weapon ().dir;
				Gizmos.DrawLine (_data.GetPosition (), _data.GetPosition () + _debug_dir * _data.GetArc_Weapon().radius_far);
				_debug_q = Quaternion.AngleAxis (_data.GetArc_Weapon ().degree * -0.5f, Vector3.forward);
				_debug_dir = _debug_q * _data.GetArc_Weapon ().dir;
				Gizmos.DrawLine (_data.GetPosition (), _data.GetPosition () + _debug_dir * _data.GetArc_Weapon().radius_far);
			}

			//공격 범위 - 호/수평 : Vector3.up

			//캐릭터카드 충돌원
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(_data.GetPosition(), _data.GetCollider_Sphere().radius);

			//캐릭터 방향 
			Gizmos.color = Color.black;
			Gizmos.DrawLine (_data.GetPosition (), _data.GetPosition () + _data.GetDirection () * 4);
			Gizmos.DrawSphere (_data.GetPosition () + _data.GetDirection () * 4, 0.4f);

			//공격 무기이동 경로
			_debug_line.y = -0.5f;
			Gizmos.color = Color.red;
			Gizmos.DrawLine (_data.GetPosition (), _data.GetWeaponPosition());
			Gizmos.DrawWireSphere (_data.GetWeaponPosition(), _data.weapon.collider_sphere_radius);
			//*/

			//칼죽이기 가능 범위
			_debug_line.y = -1f;
			Gizmos.color = Color.green;
			Gizmos.DrawLine (_data.GetWeaponPosition (_data.GetBehavior ().cloggedTime_0)+_debug_line, _data.GetWeaponPosition (_data.GetBehavior ().cloggedTime_1)+_debug_line);

			//공격점 범위 
			_debug_line.y = -1.5f;
			Gizmos.color = Color.red;
			Gizmos.DrawLine (_data.GetWeaponPosition (_data.GetBehavior().eventTime_0)+_debug_line, _data.GetWeaponPosition (_data.GetBehavior ().eventTime_1)+_debug_line);
		}

		private void Update_UI_Debug()
		{
			////!!!!!!!!! debug !!!!!!!!!!
			//Debug.DrawRay(_data.GetPosition(), _data.GetDirection() * 4, Color.black); //chamto test

		}

		private void Update_UI_HPBAR()
		{
			//charUI._hp_bar.maxValue = charData.GetMaxHP ();
			//charUI._hp_bar.value = charData.GetHP ();
		}

		private void Update_UI_Explan()
		{
			
			this._text_explanation.text = 
				"  "  + Character.StateToString(_data.CurrentState()) +
				"  sub:"+ Character.SubStateToString(_data.CurrentEventState()) ;

			this._text_time.text = 
				Skill.NameToString(_data.CurrentSkill().name) + "   " +
				_data.GetTimeDelta().ToString("0.0");

		}

		Coroutine _prev_coroutine_weaponCard_ = null;
		public void StopCoroutine_WeaponCard()
		{
			if (null != _prev_coroutine_weaponCard_)
				StopCoroutine (_prev_coroutine_weaponCard_);
		}

		private void Update_UI_Card()
		{
			
			//if(2 == id)
			//	DebugWide.LogBlue (id+" : "+charData.CurrentState ()); //chamto test

			//=====----=====----=====----=====----=====----=====----=====----=====----
			switch (_data.GetJudgment().GetState_Current()) 
			{
			case Judgment.eState.Attack_Clogged:
				{
					//DebugWide.LogBlue ("Attack_Clogged - 칼죽음"); //chamto test
				}
				break;
			case Judgment.eState.Attack_Weapon:
				{
					//DebugWide.LogBlue ("Attack_Weapon - 칼죽이기 성공"); //chamto test
				}
				break;
			case Judgment.eState.Attack_Withstand:
				{
					//DebugWide.LogBlue ("Attack_Withstand - 칼맞부딪침"); //chamto test
				}
				break;
			}
			//=====----=====----=====----=====----=====----=====----=====----=====----

			//공통 UI 출력 - idle
			{
				switch (_data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						
						this._actions [eAction.Action].gameObject.SetActive (false);
						this._actions [eAction.Hilt].gameObject.SetActive (false);

						this._actions [eAction.Idle].SelectAction (_data.kind, ResourceManager.eActionKind.Idle);

					}
					break;
				case Character.eState.End:
					{
						
//						if(0 != this._id && _data.CurrentSkill().name != Skill.eName.Idle)
//							DebugWide.LogBlue ("["+this._id + "]  " + "State.End  " + _data.CurrentSkill().name + 
//								"  " + _data.GetTimeDelta() + "  " + _data.GetBehavior().runningTime + "  " + _data.GetBehavior().rigidTime);

						//공격이 "막히지 않았을때" 만 카드 초기화 시켜준다  
						if(_data.GetJudgment().GetState_Current() != Judgment.eState.Attack_Clogged)
						{
							this.RevertData_All ();
						}

						//chamto test
						//칼충돌 이펙트 끄기
						Transform trEffect = Single.hierarchy.GetTransform("2_Effects/effect_6");
						trEffect.gameObject.SetActive(false);

						this.Revert_ActionRoot();
						this.Revert_EffectRoot();
					}
					break;
				}

			}

			//칼죽이기 UI 출력
			if (Skill.eName.Hit_Weapon == _data.CurrentSkill().name ||
				Skill.eName.Withstand_1 == _data.CurrentSkill().name) 
			{
				
				switch (_data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						this._actions[eAction.Action].gameObject.SetActive (true);
						this._actions[eAction.Hilt].gameObject.SetActive (true);

						this._actions[eAction.Action].SelectAction(_data.kind, ResourceManager.eActionKind.AttackBefore);
						this._actions[eAction.Hilt].SelectAction (_data.kind, ResourceManager.eActionKind.AttackValid);

						if (Skill.eName.Withstand_1 == _data.CurrentSkill ().name) 
						{
							this._effects [eEffect.Wind].gameObject.SetActive (true);
						}
							
						//================================================
						CharDataBundle bundle;
						bundle._data = _data;
						bundle._ui = this;
						bundle._gameObject = this._actions [eAction.Hilt].gameObject;

						StopCoroutine_WeaponCard ();
						if (Skill.eName.Hit_Weapon == _data.CurrentSkill ().name) 
						{
							_prev_coroutine_weaponCard_ = StartCoroutine("AniStart_Hit_Weapon",bundle); 
						}
						if (Skill.eName.Withstand_1 == _data.CurrentSkill().name) 
						{
							//bundle._gameObject = this._actions [eAction.Blade].gameObject;
							//bundle._gameObject = this._actionRoot.gameObject;
							_prev_coroutine_weaponCard_ = StartCoroutine("AniStart_Hit_Withstand_1",bundle); 
						}

						//================================================

					}
					break;
				case Character.eState.Running:
					{
						UI_CharacterCard dstCard = this._UI_Battle.GetCharacter (_data.CurrentTarget ());

						//칼버티기 상태에서의 칼밀기 애니 처리 
						if (Skill.eName.Withstand_1 == _data.CurrentSkill().name)
						{
							//상대와 나 사이의 중간위치가 최대거리가 되게 한다
							float max_dist = (this._actions [eAction.Idle].transform.position - dstCard._actions [eAction.Idle].transform.position).magnitude * 0.5f;
							float cur_dist = this._data.GetWeaponDistance ();

							//높이값의 분포를 이등변 삼각형모양으로 만들기 위해, 최대 거리를 넘어가는 길이는 목표점까지 점점 작아지게 변형한다
							if (cur_dist > max_dist)
								cur_dist = 2 * max_dist - cur_dist;
							
							float rate = cur_dist / max_dist; //최대길이값을 이용하여 0~1 선형 범위값으로 변환한다
							float inter_rate = Utility.Interpolation.easeInOutBounce(0,1,rate); //보간함수를 이용하여 선형 범위값을 변조시킨다
							const float MAX_HEIGHT = 5f;

							Vector3 temp = this._actions [eAction.Hilt].transform.position;
							temp.y = this._data.GetPosition ().y + (MAX_HEIGHT * inter_rate);
							temp.x = this._data.GetWeaponPosition ().x;
							this._actions [eAction.Hilt].transform.position = temp;

						}

						//====================================================
						//update sub_state
						//====================================================
						switch (_data.CurrentEventState ()) 
						{
						case Character.eSubState.Start:
							{
								
							}
							break;
						case Character.eSubState.Running:
							{
								
							}
							break;
						case Character.eSubState.End:
							{
								
							}
							break;
						}

						//====================================================
					}

					break;
				case Character.eState.Waiting:
					{
						//this._actions[eAction.Action].SelectAction (_data.kind, ResourceManager.eActionKind.AttackAfter);
					}

					break;
				case Character.eState.End:
					{
						this._effects [eEffect.Wind].gameObject.SetActive (false);
					}
					break;

				}//end switch

			}

			//공격 UI 출력 
			if (Skill.eName.Attack_Strong_1 == _data.CurrentSkill().name  || 
				Skill.eName.Attack_Weak_1 == _data.CurrentSkill().name ||
				Skill.eName.Attack_Counter_1 == _data.CurrentSkill().name) 
			{

				switch (_data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						//=================================================
						AudioClips clips = null;
						if (Character.eKind.Seonbi == _data.kind) {
							clips = Single.resource.GetVoiceClipMap ().GetClips (VoiceInfo.eKind.Eng_NaverMan_1);
						} else 
						{
							clips = Single.resource.GetVoiceClipMap ().GetClips (VoiceInfo.eKind.Eng_NaverWoman_2);
						}
						//_audioSource.Play (); //chamto test
						List<XML_Data.DictInfo.VocaInfo> seq = Single.resource.GetDictEng()._dictInfoMap[100].GetSequence(XML_Data.DictInfo.eKind.Sentence); //100 임시 처리
						//List<XML_Data.DictInfo.VocaInfo> seq = Single.resource.GetDictEng()._dictInfoMap[100].GetSequence(6); //100 , 9 임시 처리
						_audioSource.Stop ();
						_audioSource.PlayOneShot(clips[seq[_voiceSequence].hashKey]);
						_voiceSequence++;
						_voiceSequence = _voiceSequence % (seq.Count);
						//=================================================

						this._actions[eAction.Action].gameObject.SetActive (true);
						this._actions[eAction.Hilt].gameObject.SetActive (false);

						this._actions[eAction.Action].SelectAction(_data.kind, ResourceManager.eActionKind.AttackBefore);
						this._actions[eAction.Hilt].SelectAction (_data.kind, ResourceManager.eActionKind.AttackValid);

						//iTween.Stop (charUI._actions [2].gameObject);
						//charUI.RevertData_All ();

						//================================================
						CharDataBundle bundle;
						bundle._data = _data;
						bundle._ui = this;
						bundle._gameObject = this._actions [eAction.Hilt].gameObject;

						StopCoroutine_WeaponCard ();
						if (Skill.eName.Attack_Weak_1 == _data.CurrentSkill().name) 
						{
							_prev_coroutine_weaponCard_ = StartCoroutine("AniStart_Attack_Weak_1",bundle); 
						}
						if (Skill.eName.Attack_Strong_1 == _data.CurrentSkill().name) 
						{
							//this._effects [eEffect.Wind].gameObject.SetActive (true);
							StartCoroutine("Effect_AttackWind",bundle); 
							_prev_coroutine_weaponCard_ = StartCoroutine("AniStart_Attack_Strong_1",bundle); 
						}	
						if (Skill.eName.Attack_Counter_1 == _data.CurrentSkill().name) 
						{
							//this._effects [eEffect.Wind].gameObject.SetActive (true);
							_prev_coroutine_weaponCard_ = StartCoroutine("AniStart_Attack_Counter_1",bundle); 
						}	

						//================================================

					}
					break;
				case Character.eState.Running:
					{

						//====================================================
						//update sub_state
						//====================================================
						switch (_data.CurrentEventState ()) 
						{
						case Character.eSubState.Start:
							{
								if (Skill.eName.Attack_Strong_1 == _data.CurrentSkill().name) 
								{
									//this._effects [eEffect.Wind].gameObject.SetActive (true);
								}	

							}
							break;
						case Character.eSubState.Running:
							{

							}
							break;
						case Character.eSubState.End:
							{
								//DebugWide.LogBlue ("Valid_End"); //chamto test

								//charUI._actions[2].gameObject.SetActive (false);
								//iTween.Stop (charUI._actions [2].gameObject);
								//charUI.RevertData_All ();	
							}
							break;
						}

						//====================================================
					}

					break;
				case Character.eState.Waiting:
					{
						this._actions[eAction.Action].SelectAction (_data.kind, ResourceManager.eActionKind.AttackAfter);
						this._effects [eEffect.Wind].gameObject.SetActive (false);
					}

					break;
				case Character.eState.End:
					{
						this._effects [eEffect.Wind].gameObject.SetActive (false);
					}
					break;

				}//end switch

			}

			if (Skill.eName.Block_1 == _data.CurrentSkill().name) 
			{

				switch (_data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						this._actions[eAction.Action].gameObject.SetActive (true);	
						this._actions[eAction.Hilt].gameObject.SetActive (false);

						this._actions[eAction.Action].SelectAction (_data.kind, ResourceManager.eActionKind.BlockBefore);
					}
					break;
				case Character.eState.Running:
					{
						//=========================================

						switch (_data.CurrentEventState ()) 
						{
						case Character.eSubState.Start:
							{

							}
							break;
						}

						//=========================================
					}
					break;
				case Character.eState.Waiting:
					{

					}
					break;
				case Character.eState.End:
					{
						//charUI._actions[1].gameObject.SetActive (false);
					}
					break;

				}
			}

		}//end func

		private void Update_UI_Effect()
		{

			if (_data.Valid_CloggedTime ()) 
			{
				this._actions [eAction.Blade].color = Color.gray;

//				if(1 == _id)
//					DebugWide.LogBlue (_data.CurrentState() + "  "  + _data.GetBehavior().cloggedTime_0 + "   "  + _data.GetBehavior().cloggedTime_1 + "  "+ _data.GetTimeDelta() + "  ");

			} else 
			{
				this._actions [eAction.Blade].color = Color.white;

//				if(1 == _id)
//					DebugWide.LogBlue (_data.CurrentState() + " - --- - - - - - " + _data.GetTimeDelta() + "  " );
			}
			
			if (_data.GetJudgment().IsStart_Damaged ()) 
			{
				CharDataBundle bundle;
				bundle._data = _data;
				bundle._ui = this;
				bundle._gameObject = this._effects [UI_CharacterCard.eEffect.Hit].gameObject;
				StartCoroutine("EffectStart_Damaged",bundle);

				bundle._gameObject = this._actionRoot.gameObject;
				StartCoroutine("EffectStart_Endure",bundle);

			}

			if (_data.GetJudgment().IsStart_BlockSucceed ()) 
			{
				CharDataBundle bundle;
				bundle._data = _data;
				bundle._ui = this;
				bundle._gameObject = this._effects [UI_CharacterCard.eEffect.Block].gameObject;
				StartCoroutine("EffectStart_Block",bundle);

				bundle._gameObject = this._actionRoot.gameObject;
				StartCoroutine("EffectStart_Wobble",bundle);
			}

			//			switch (charData.GetJudgmentState ()) 
			//			{
			//			case Judgment.eState.AttackDamage_Start:
			//				{
			//					//DebugWide.LogBlue ("AttackDamage_Start"); //chamto test
			//
			//					//charUI._effect [UI_CharacterCard.eEffect.Block].gameObject.SetActive (true);
			//				}
			//				break;
			//			case Judgment.eState.Damaged_Start:
			//				{
			//					//charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject.SetActive (true);
			//					//Effect.FadeIn (charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject, 0.3f);
			//					//Effect.FadeOut (charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject, 0.3f);
			//
			//					//iTween.ShakeScale(charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject,new Vector3(0.2f,0.8f,0.2f), 1f); //!!!!
			//					//iTween.ScaleTo(charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject, new Vector3(1.2f,1.2f,1.2f), 0.7f);
			//					//iTween.ScaleFrom(charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject, Vector3.zero, 0.4f);
			//					StartCoroutine("EffectStart_1",charUI._effect [UI_CharacterCard.eEffect.Hit].gameObject);
			//
			//				}
			//				break;
			//			case Judgment.eState.BlockSucceed_Start:
			//				{
			//					StartCoroutine("EffectStart_2",charUI._effect [UI_CharacterCard.eEffect.Block].gameObject);
			//					//charUI._effect [UI_CharacterCard.eEffect.Block].gameObject.SetActive (true);
			//				}
			//				break;
			//			}

		}//end func

		//====================================================================================

		public void OnCollisionEnter (Collision other)
		{
			UI_CharacterCard dst = other.gameObject.GetComponentInParent<UI_CharacterCard> ();
			if (null == dst || this._id == dst._id)
				return;

			if(other.gameObject.tag.Equals("weapon"))
			{
				Transform trEffect = Single.hierarchy.GetTransform ("2_Effects/effect_6");
				//DebugWide.LogBlue (trEffect);
				trEffect.gameObject.SetActive(true);
				trEffect.transform.position = other.contacts [0].point;

				//chamto test
				//iTween.Stop (trEffect.gameObject);
				iTween.ShakeScale(trEffect.gameObject,new Vector3(0.5f,0.5f,0.1f), 1f);
			}
			//DebugWide.LogBlue ("OnCollisionEnter:  " + " [" + this._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}

		public void OnCollisionStay (Collision other)
		{
			UI_CharacterCard dst = other.gameObject.GetComponentInParent<UI_CharacterCard> ();
			if (null == dst || this._id == dst._id)
				return;
			//DebugWide.LogBlue ("OnCollisionStay:  " + " [" + this._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}

		public void OnCollisionExit (Collision other)
		{
			UI_CharacterCard dst = other.gameObject.GetComponentInParent<UI_CharacterCard> ();
			if (null == dst || this._id == dst._id)
				return;

			if(other.gameObject.tag.Equals("weapon"))
			{
				Transform trEffect = Single.hierarchy.GetTransform ("2_Effects/effect_6");
				//trEffect.gameObject.SetActive(false);
				//iTween.Stop (trEffect.gameObject);
			}
			//DebugWide.LogBlue ("OnCollisionExit:  " + " [" + this._id + "] " + other.gameObject.name + "  " + other.gameObject.tag  + "  " + dst._id );
		}

		//====================================================================================

		public IEnumerator AniStart_Attack_Strong_1(CharDataBundle bundle)
		{

			float time_pause = 0.3f;
			float time_before = bundle._data.GetBehavior().distance_maxTime - time_pause;
			float time_after = bundle._data.GetRunningTime () - time_before;
			int rand = Single.rand.Next (1, 5);
			rand = 4;
			//======================================
			yield return new WaitForSeconds(time_pause);

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			Vector3 start = bundle._ui._actions [eAction.Hilt].transform.localPosition;
			Vector3[] list = GetPaths (rand, start);

			_prev_position_ = list [0];

			//iTween.RotateBy (bundle._gameObject,new Vector3(0,0,60f),time_before); //표창느낌
			iTween.MoveTo(bundle._gameObject, iTween.Hash(
				"time", time_before - 0.1f //애니와 충돌순간이 맞지 않아서 애니를 0.1초 짧게 준다
				,"easetype",  "linear"//"easeInExpo"//"easeInOutBounce"//"easeOutCubic"
				,"path", list
				//,"orienttopath",true
				//,"axis","z"
				//,"looktarget",bundle._ui._actions [2].Get_InitialPostition ()
				,"islocal",true //로컬위치값을 사용하겠다는 옵션. 대상객체의 로컬위치값이 (0,0,0)이 되는 문제 있음. 직접 대상객체 로컬위치값을 더해주어야 한다.
				,"movetopath",false //현재객체에서 첫번째 노드까지 자동으로 경로를 만들겠냐는 옵션. 경로 생성하는데 문제가 있음. 비활성으로 사용해야함
				,"onupdate","Rotate_Towards_FrontGap"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			iTween.ShakePosition(bundle._ui._effectRoot.gameObject,new Vector3(0.5f,0.5f,0), time_before+time_after);

			//======================================
			yield return new WaitForSeconds(time_before);
			//iTween.MoveTo (bundle._gameObject, bundle._ui._actions [2].Get_InitialPostition(), time_after);
			iTween.MoveTo (bundle._gameObject, iTween.Hash (
				"time", time_after
				, "easetype", "easeOutExpo"//"easeInOutBounce"//"easeOutCubic"//"linear"
				, "position", bundle._ui._actions [eAction.Hilt].Get_InitialPostition ()
				, "onupdate", "Rotate_Towards_BehindGap"
				, "onupdatetarget", gameObject
				, "onupdateparams", bundle._gameObject.transform
			));

			//======================================
			yield return new WaitForSeconds(time_after);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);
			//======================================

			//DebugWide.LogBlue ("end");

		}

		public IEnumerator AniStart_Attack_Counter_1(CharDataBundle bundle)
		{

			float time_pause = 0.3f;
			float time_before = bundle._data.GetBehavior().distance_maxTime - time_pause;
			float time_after = bundle._data.GetRunningTime () - time_before;
			int path = 5;

			//======================================
			yield return new WaitForSeconds(time_pause);

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			Vector3 start = bundle._ui._actions [eAction.Hilt].transform.localPosition;
			Vector3[] list = GetPaths (path, start);

			_prev_position_ = list [0];

			//iTween.RotateBy (bundle._gameObject,new Vector3(0,0,60f),time_before); //표창느낌
			iTween.MoveTo(bundle._gameObject, iTween.Hash(
				"time", time_before - 0.1f //애니와 충돌순간이 맞지 않아서 애니를 0.1초 짧게 준다
				,"easetype",  "linear"//"easeInExpo"//"easeInOutBounce"//"easeOutCubic"
				,"path", list
				//,"orienttopath",true
				//,"axis","z"
				//,"looktarget",bundle._ui._actions [2].Get_InitialPostition ()
				,"islocal",true //로컬위치값을 사용하겠다는 옵션. 대상객체의 로컬위치값이 (0,0,0)이 되는 문제 있음. 직접 대상객체 로컬위치값을 더해주어야 한다.
				,"movetopath",false //현재객체에서 첫번째 노드까지 자동으로 경로를 만들겠냐는 옵션. 경로 생성하는데 문제가 있음. 비활성으로 사용해야함
				,"onupdate","Rotate_Towards_FrontGap"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			//======================================
			yield return new WaitForSeconds(time_before);
			//iTween.MoveTo (bundle._gameObject, bundle._ui._actions [2].Get_InitialPostition(), time_after);
			iTween.MoveTo (bundle._gameObject, iTween.Hash (
				"time", time_after
				, "easetype", "easeOutExpo"//"easeInOutBounce"//"easeOutCubic"//"linear"
				, "position", bundle._ui._actions [eAction.Hilt].Get_InitialPostition ()
				, "onupdate", "Rotate_Towards_BehindGap"
				, "onupdatetarget", gameObject
				, "onupdateparams", bundle._gameObject.transform
			));

			//======================================
			yield return new WaitForSeconds(time_after);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);
			//======================================

			//DebugWide.LogBlue ("end");

		}

		public IEnumerator AniStart_Attack_Weak_1(CharDataBundle bundle)
		{
			UI_CharacterCard target = bundle._ui._UI_Battle.GetCharacter (bundle._data.CurrentTarget ());
			Vector3 pos_targetWeapon = target._actions [eAction.Hilt].transform.position;
			GameObject obj_blade = bundle._ui._actions [eAction.Blade].gameObject;
			bundle._gameObject.SetActive (true);
			iTween.Stop (obj_blade);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			//float time = bundle._data.GetRunningTime () - bundle._data.GetBehavior().distance_maxTime; //after
			float time = bundle._data.GetBehavior().distance_maxTime; //before
			//float time = 3f; //chamto test
			float distance = bundle._data.GetBehavior().distance_travel - this.GetLength_Between_WeaponeCard();
			distance = distance * bundle._gameObject.transform.lossyScale.x; //반전시킨 것을 다시 곱하여 적용

			//<문제>itween 에서 객체의 로컬위치값으로만 적용됨 (조정 할수있는 해쉬값이 없음)
			//목표로의 순수벡터값만 구해 로컬위치값으로 사용되게 한다. 
			//pos_targetWeapon = pos_targetWeapon - bundle._gameObject.transform.position ; 
			//pos_targetWeapon.x = pos_targetWeapon.x * bundle._gameObject.transform.lossyScale.x; //반전적용

			//DebugWide.LogBlue ("["+_id + "]  pos_targetWeapon  "+pos_targetWeapon);

			_prev_position_ = bundle._gameObject.transform.localPosition;

			//iTween.PunchRotation(obj_blade,new Vector3(0,0,800),time);
			//iTween.RotateBy (obj_blade,new Vector3(0,0,60f),time);
			//iTween.PunchPosition(bundle._gameObject,pos_targetWeapon, time);

			iTween.PunchPosition(bundle._gameObject, iTween.Hash(
				"amount", pos_targetWeapon - bundle._gameObject.transform.position,
				"time", time
				,"space",Space.World
				,"onupdate","Rotate_Towards_FrontGap"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			//지정크기 만큼 객체를 이동 
			//내부에서 Translate 함수를 사용하여 이동함. 현재위치값 기준으로 동작함. 이동방향이 회전에 영향을 받음
			//월드좌표로 해야 이동방향이 회전에 영향을 안받음
//			iTween.MoveBy (bundle._gameObject, iTween.Hash (
//				"amount", pos_targetWeapon - bundle._gameObject.transform.position,
//				"time", time, "easetype", "easeOutCubic"//"easeOutCubic"//"easeInOutBounce"//
//				,"space",Space.World
//				, "onupdate", "Rotate_Towards_FrontGap"
//				, "onupdatetarget", gameObject
//				, "onupdateparams", bundle._gameObject.transform
//			));

			//목표위치까지 객체를 이동
//			iTween.MoveTo(bundle._gameObject, iTween.Hash(
//				"position", pos_targetWeapon,//bundle._data.GetDirection() * distance,
//				"time", time, "easetype",  "easeOutCubic"//"easeOutCubic"//"easeInOutBounce"//
//				//,"islocal",true //로컬위치값을 사용하겠다는 옵션. 대상객체의 로컬위치값이 (0,0,0)이 되는 문제 있음. 직접 대상객체 로컬위치값을 더해주어야 한다.
//				//,"movetopath",false //현재객체에서 첫번째 노드까지 자동으로 경로를 만들겠냐는 옵션. 경로 생성하는데 문제가 있음. 비활성으로 사용해야함
//				,"onupdate","Rotate_Towards_FrontGap"
//				,"onupdatetarget",gameObject
//				,"onupdateparams",bundle._gameObject.transform
//			));

			yield return new WaitForSeconds(time);

			iTween.Stop (obj_blade);
			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}

		public IEnumerator AniStart_Hit_Weapon(CharDataBundle bundle)
		{

			float time = bundle._data.GetRunningTime ();
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);

			//bundle._ui.RevertData_All ();
			//bundle._gameObject.transform.position = bundle._data.GetWeaponPosition (); //위치로 이동
			//DebugWide.LogBlue(bundle._data.GetBehavior ().distance_travel  + "  " + bundle._data.CurrentSkill().name); //chamto test

			//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",100,"y",100,"time",0.5f));
			//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",50,"loopType","loop","time",0.5f));
			//iTween.PunchRotation(bundle._gameObject,new Vector3(0,0,300f),time);
			//iTween.PunchPosition(bundle._gameObject, iTween.Hash("z",-200f,"time",time));	
//			iTween.MoveBy(bundle._gameObject, iTween.Hash(
//				"amount", new Vector3(0,0,30f),
//				"time", time, "easetype",  "easeInOutBounce"//"linear"
//			));
//			iTween.RotateBy(bundle._gameObject, iTween.Hash(
//				"amount", new Vector3(0,30f,0),
//				"time", time, "easetype",  "easeInOutBounce"//"linear"
//			));
			iTween.ShakeScale(bundle._gameObject,new Vector3(0.5f,0.5f,0.5f), time);

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}

		public void Withstand_Update_Position(Transform tr)
		{
			
		}

		public IEnumerator AniStart_Hit_Withstand_1(CharDataBundle bundle)
		{

			float time = bundle._data.GetRunningTime () + 1f;
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			//iTween.Stop (bundle._ui._actions[eAction.Blade].gameObject);

			//iTween.ShakeScale(bundle._ui._actionRoot.gameObject,new Vector3(0.1f,0.05f,0), time);
			//iTween.ShakeScale(bundle._gameObject,new Vector3(0.5f,0.5f,0), time);
			//iTween.ShakePosition(bundle._ui._actions[eAction.Hilt].gameObject,new Vector3(0.5f,0.5f,0), 10);
			//iTween.ShakePosition(bundle._gameObject,new Vector3(0.5f,0.5f,0),time);

			iTween.ShakeScale(bundle._ui._actionRoot.gameObject, iTween.Hash(
				"amount", new Vector3(0.1f,0.1f,0),
				"time", time
				,"space",Space.World
				,"onupdate","Rotate_Towards_FrontGap"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
				));

			iTween.ShakePosition(bundle._ui._effectRoot.gameObject,new Vector3(1.5f,1.5f,0), time);

			yield return new WaitForSeconds(time);

			//iTween.Stop (bundle._ui._actions[eAction.Blade].gameObject);
			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}

		public IEnumerator AniStart_Attack_1(CharDataBundle bundle)
		{

			float time = bundle._data.GetEventTime_Interval ();
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			//iTween.RotateBy (charUI._action[2].gameObject,new Vector3(0,0,-20f),0.5f);
			//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",100,"y",100,"time",0.5f));
			//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",50,"loopType","loop","time",0.5f));
			iTween.PunchRotation(bundle._gameObject,new Vector3(0,0,-45f),1f);
			iTween.PunchPosition(bundle._gameObject, iTween.Hash("x",10,"time",time));	
			//iTween.MoveBy(charUI._action[2].gameObject, iTween.Hash(
			//	"amount", new Vector3(300f,20f,0f),
			//	"time", 1f, "easetype",  "easeInOutBounce"//"linear"
			//));

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}

		public Vector3[] GetPaths(int kind , Vector3 start)
		{
			//DebugWide.LogBlue (kind); //chamto test

			//kind = 3;//chamto test

			string pathName = "";
			const int PATH_01 = 1;
			const int PATH_02 = 2;
			const int PATH_03 = 3;
			const int PATH_04 = 4;
			const int PATH_05 = 5;
			const int NODE_COUNT = 6;
			Vector3[] list = new Vector3[NODE_COUNT];

			if (PATH_01 == kind)
				pathName = "p01";
			if (PATH_02 == kind)
				pathName = "p02";
			if (PATH_03 == kind)
				pathName = "p03";
			if (PATH_04 == kind)
				pathName = "p04_strong";
			if (PATH_05 == kind)
				pathName = "p05_counter";

			list[0] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (0)").localPosition + start;
			list[1] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (1)").localPosition + start;
			list[2] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (2)").localPosition + start;
			list[3] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (3)").localPosition + start;
			list[4] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (4)").localPosition + start;
			list[5] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (5)").localPosition + start;

			return list;
		}

		Vector3 _prev_position_ = Vector3.zero;
		public void Rotate_Towards_FrontGap(Transform tr)
		{
			Vector3 dir = tr.localPosition - _prev_position_;
			if (dir.sqrMagnitude <= 0.5f)
				return; //길이가 아주 작거나 0이면 각도 변화가 없는 상태이다. 

			Vector3 euler = tr.localEulerAngles;
			float angle = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
			//90도 보다 작은 변화량만 적용한다. ITween 펀치에서 180도가 나와 적용한 코드이다
			if (90f > Mathf.Abs(angle))
				euler.z = angle;
			
			tr.localEulerAngles = euler;

//			if(1 == _id)
//				DebugWide.LogBlue ("Rotate_Towards_FrontGap : " + tr.localPosition + "  " + _prev_position_ + "  "  + dir.sqrMagnitude + "  " + dir + "  " + angle);//chamto test

			_prev_position_ = tr.localPosition;
		}

		public void Rotate_Towards_BehindGap(Transform tr)
		{
			Vector3 dir = _prev_position_ - tr.localPosition;
			if (dir.sqrMagnitude <= 0.01f)
				return; //길이가 아주 작거나 0이면 각도 변화가 없는 상태이다. 

			Vector3 euler = tr.localEulerAngles;
			euler.z = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
			tr.localEulerAngles = euler;

			_prev_position_ = tr.localPosition;
		}

		//ref : http://www.pixelplacement.com/itween/documentation.php
		public IEnumerator AniStart_Attack_1_Random(CharDataBundle bundle)
		{

			int rand = Single.rand.Next (1, 4);
			float time = bundle._data.GetEventTime_Interval ();
			//float time = bundle._data.GetRunningTime();
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			Vector3 start = bundle._ui._actions [eAction.Hilt].transform.localPosition;
			Vector3[] list = GetPaths (rand, start);

			_prev_position_ = list [0];

			iTween.MoveTo(bundle._gameObject, iTween.Hash(
				"time", time
				,"easetype",  "easeOutBack"
				,"path", list
				//,"orienttopath",true
				//,"axis","z"
				,"islocal",true //로컬위치값을 사용하겠다는 옵션. 대상객체의 로컬위치값이 (0,0,0)이 되는 문제 있음. 직접 대상객체 로컬위치값을 더해주어야 한다.
				,"movetopath",false //현재객체에서 첫번째 노드까지 자동으로 경로를 만들겠냐는 옵션. 경로 생성하는데 문제가 있음. 비활성으로 사용해야함
				//"looktarget",new Vector3(5,-5,7)
				,"onupdate","Rotate_Towards_FrontGap"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}

		public IEnumerator Effect_AttackWind(CharDataBundle bundle)
		{

			bundle._ui._effects[eEffect.Wind].gameObject.SetActive (false);
			yield return new WaitForSeconds(0.5f);
			bundle._ui._effects[eEffect.Wind].gameObject.SetActive (true);

		}

		//피해입다
		public IEnumerator EffectStart_Damaged(CharDataBundle bundle)
		{
			//DebugWide.LogBlue ("start");

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			this.Revert_ActionRoot();

			//gobj.transform.localScale = Vector3.one;
			iTween.ShakeScale(bundle._gameObject,new Vector3(1f,1f,1f), 0.5f);
			//iTween.ShakePosition(gobj,new Vector3(10f,10f,10f), 0.5f);
			//iTween.ShakeRotation(gobj,new Vector3(90f,1f,1f), 0.5f);

			yield return new WaitForSeconds(0.5f);

			iTween.Stop (bundle._gameObject);
			//gobj.transform.localScale = Vector3.one;
			bundle._gameObject.SetActive (false);

			//DebugWide.LogBlue ("end");
		}

		//휘청거리다 
		public IEnumerator EffectStart_Wobble(CharDataBundle bundle)
		{

			iTween.Stop (bundle._gameObject);
			this.Revert_ActionRoot();
			//bundle._ui.RevertData_All ();
			//bundle._gameObject.transform.localPosition = Vector3.zero;

			iTween.ShakePosition(bundle._gameObject,new Vector3(1f,0,0), 0.5f);

			yield return new WaitForSeconds(0.5f);

			iTween.Stop (bundle._gameObject);
			//bundle._gameObject.transform.localPosition = Vector3.zero;
		}

		//막다
		public IEnumerator EffectStart_Block(CharDataBundle bundle)
		{
			//DebugWide.LogBlue ("start");

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			this.Revert_ActionRoot();
			//gobj.transform.localScale = Vector3.one;

			iTween.ShakeScale(bundle._gameObject,new Vector3(1f,1f,1f), 0.5f);
			//iTween.ShakePosition(gobj,new Vector3(10f,10f,0), 0.5f);
			//iTween.ShakeRotation(gobj,new Vector3(90f,1f,1f), 0.5f);

			yield return new WaitForSeconds(0.5f);

			iTween.Stop (bundle._gameObject);
			//gobj.transform.localScale = Vector3.one;
			bundle._gameObject.SetActive (false);

			//DebugWide.LogBlue ("end");
		}

		//견디다
		public IEnumerator EffectStart_Endure(CharDataBundle bundle)
		{
			float time = 1.0f;
			iTween.Stop (bundle._gameObject);
			//bundle._gameObject.transform.localEulerAngles = Vector3.zero;
			this.Revert_ActionRoot();

			iTween.ShakeRotation(bundle._gameObject,new Vector3(0,100f,0), time);
			iTween.ShakePosition(bundle._gameObject,new Vector3(1f,0,0), time);
			//iTween.PunchRotation(bundle._gameObject,new Vector3(0,300f,0f), time);
//			iTween.RotateTo (bundle._gameObject, iTween.Hash (
//				"time", time
//				, "easetype", "easeInOutBounce"//"easeOutBack"
//				, "rotation", new Vector3(0,45f,0)
//				//,"looptype","pingPong"
//			));

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			//bundle._gameObject.transform.localEulerAngles = Vector3.zero;
		}

	}//end class
		

}//end namespace 
