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
	namespace Figure
	{
		public struct Arc
		{
			public Vector3 pos;
			public Vector3 dir; //정규화 되어야 한다
			public float degree; //각도 
			public float radius;
			public float f
			{
				get
				{	//f = radius / sin
					return radius / Mathf.Sin( Mathf.Deg2Rad * degree );
				}
			}



			//ratio : [-1 ~ 1]
			//호에 원이 완전히 포함 [1]
			//호에 원의 중점까지 포함 [0]
			//호에 원의 경계까지 포함 [-1 : 포함 범위 가장 넒음] 
			public const float Fully_Included = 1f;
			public const float Focus_Included = 0f;
			public const float Boundary_Included = -1f;
			public Vector3 GetPosition(float ratio = Focus_Included)
			{
				if (0 == ratio)
					return pos; 
				
				return pos + dir * (f * ratio);
			}

			public Sphere sphere
			{
				get
				{ 
					Sphere sph;
					sph.pos = this.pos;
					sph.radius = this.radius;
					return sph;
				}

			}
		}
		public struct Sphere
		{
			public Vector3 pos;
			public float radius;
		}
	}
	public class Util
	{
		//사분면
		const int QUADRANT_1 = 1;
		const int QUADRANT_2 = 2;
		const int QUADRANT_3 = 3;
		const int QUADRANT_4 = 4;

		//acos 랑 뭐가 다른거지?
		static public float DegreeToCos(float degree) 
		{
			//(2,3 사분면 : 음수 )  (1,4 사분면 : 양수 )  
			//90': 1f = 9' : 0.1f  (비율값만 구할려는 것임. 왼쪽 비례식은 sin에 해당함)
			//A : B = a : b   (a값을 알때, b값을 구하려 한다 : Ba = Ab , b = Ba/A)
			//b = Ba/A  ,  b = 1f * Degree / 90'

			//45'  / 90 = 0  -> 1  ->  +1    
			//90'  / 90 = 1  -> 2  ->  -1   
			//180' / 90 = 2  -> 3  ->  -1   
			//270' / 90 = 3  -> 4  ->  +1   
			//360' / 90 = 4  -> 1  ->  +1   
			//450' / 90 = 5  -> 2  ->  -1   
			//...

			int quadrant = (int)degree / 90;
			quadrant = (quadrant % 4) + 1;
			if(QUADRANT_2 == quadrant && QUADRANT_3 == quadrant) 
				return (1f - (degree / 90f)) * -1f;
			
			//cos값으로 반전 시킨다.
			return 1f - (degree / 90f);
		}

		//1/2 = 0.5 , 1/90 = 0.1111...

		//만들다 보니 asin 
		static public float DegreeToSin(float degree)
		{
			//(3,4 사분면 : 음수 )  (1,2 사분면 : 양수 )  

			//45'  / 90 = 0  -> 1  ->  +1
			//90'  / 90 = 1  -> 2  ->  +1
			//180' / 90 = 2  -> 3  ->  -1
			//270' / 90 = 3  -> 4  ->  -1
			//360' / 90 = 4  -> 1  ->  +1
			//450' / 90 = 5  -> 2  ->  +1
			//...

			int quadrant = (int)degree / 90;
			quadrant = (quadrant % 4) + 1;
			if(QUADRANT_3 == quadrant && QUADRANT_4 == quadrant) 
				return (degree / 90f) * -1f;

			return (degree / 90f);
		}

		//호와 원의 충돌 검사 (2D 한정)
		static public bool Collision_Arc_VS_Sphere(Figure.Arc arc , Figure.Sphere sph , float ratio)
		{
			const float HALF_RADIUS_SUM = 0.5f;

			if (true == Util.Collision_Sphere (arc.sphere, sph, HALF_RADIUS_SUM)) 
			{
				float angle_arc = Util.DegreeToCos ( arc.degree * 0.5f); //각도를 반으로 줄여 넣는다. 1과 4분면을 구별 못하기 때문에 1사분면에서 검사하면 4사분면도 검사 결과에 포함된다. 즉 실제 검사 범위가 2배가 된다.

				Vector3 arc_sph_dir = sph.pos - arc.GetPosition (Figure.Arc.Focus_Included);
				arc_sph_dir.Normalize (); //노멀값 구하지 않는 계산식을 찾지 못했다. 

				float rate_cos = Vector3.Dot (arc.dir, arc_sph_dir);
				if(rate_cos > angle_arc) 
				{  
					return true;
				}	
			}
			 
			return false;
		}

		//ratio : 충돌 반지름합 비율 , 일정 비율이 넘었을 때만 충돌처리 되게 한다.
		public const float Fully_Included = 0.01f;	//완전겹침 처리가 필요할 경우
		public const float Focus_Included = 0.5f; 	//반지름합 1/2만 사용 ,  어느정도 겹쳤을 때 충돌처리 해야 할 경우
		public const float Boundary_Included = 1f; 	//반지름합 최대치 , 일반경우  
		static public bool Collision_Sphere(Figure.Sphere src , Figure.Sphere dst , float ratio = Boundary_Included)
		{
			//두원의 반지름을 더한후 제곱해 준다. 
			float sum_radius = (src.radius + dst.radius) * ratio;
			float sqr_standard_value = sum_radius * sum_radius;

			//두원의 중점 사이의 거리를 구한다. 피타고라스의 정리를 이용 , 제곱을 벗기지 않는다.
			float sqr_dis_between = Vector3.SqrMagnitude(src.pos - dst.pos);

			if (sqr_standard_value > sqr_dis_between)
				return true; //두원이 겹쳐짐 
			if (sqr_standard_value == sqr_dis_between)
				return true; //두원이 겹치지 않게 붙어 있음 
			if (sqr_standard_value < sqr_dis_between)
				return false; //두원이 겹쳐 있지 않음

			return false;
		}

		static public bool Collision_Sphere(Vector3 src_pos , float src_radius , Vector3 des_pos , float des_radius)
		{
			Figure.Sphere src, dst;
			src.pos = src_pos; src.radius = src_radius;
			dst.pos = des_pos; dst.radius = des_radius;
			return Util.Collision_Sphere (src, dst, 1f);
		}	


		//value 보다 target 값이 작으면 True 를 반환한다.
		static public bool Distance_LessThan(float Value , Vector3 target )
		{
			if(Mathf.Exp(Value * 0.5f) >=  Vector3.SqrMagnitude( target ))
			{
				return true;
			}

			return false;
		}
	}

	//자취의 모양
	public enum eTraceShape
	{
		None,
		Horizon,  //수평
		Vertical, //수직
		Straight, //직선
	}

	public class Behavior
	{		
		//--------<<============>>----------
		//    openTime_0 ~ openTime_1
		// 시간범위 안에 있어야 콤보가 된다
		public const float MAX_OPEN_TIME = 10f;
		public const float MIN_OPEN_TIME = 0f;

		public const float DEFAULT_DISTANCE = 14f;

		//===================================

		public float runningTime;	//동작 전체 시간 
		public float scopeTime_0;	//동작 유효 범위 : 0(시작) , 1(끝)  
		public float scopeTime_1;
		public float rigidTime;		//동작 완료후 경직 시간
		public float openTime_0; 	//다음 동작 연결시간 : 0(시작) , 1(끝)  
		public float openTime_1; 
		public float cloggedTime_0;	//막히는 시간 : 0(시작) , 1(끝)  
		public float cloggedTime_1;		


		//무기 움직임 정보 
		public eTraceShape 	attack_shape; 		//공격모양 : 종 , 횡 , 찌르기 , 던지기
		//	=== 범위형 움직임 === : 종,횡,찌르기,던지기 (무기 위치가 기준이 되는 범위이다)
		public float 		plus_range_0;		//더해지는 범위 최소 
		public float 		plus_range_1;		//더해지는 범위 최대
		//  === 이동형 움직임 === : 찌르기,던지기
		public float 		distance_travel;	//무기 이동 거리 : 상대방까지의 직선거리 , 근사치 , 판단을 위한 값임 , 정확한 충돌검사용 값이 아님.
		public float 		distance_maxTime;  //최대거리가 되는 시간 : 삼각형 모형
		public float 		velocity_up;		//무기 상승 속도
		public float 		velocity_down;		//무기 하강 속도	


		public  Behavior()
		{
			
			runningTime = 0f;
			scopeTime_0 = 0f;
			scopeTime_1 = 0f;
			rigidTime = 0f;
			openTime_0 = 0f;
			openTime_1 = 0f;
			cloggedTime_0 = 0f;
			cloggedTime_1 = 0f;
			plus_range_0 = 0f;
			plus_range_1 = 0f;
			attack_shape = eTraceShape.None;
			distance_travel = DEFAULT_DISTANCE;
			distance_maxTime = 0f;
			velocity_up = 0f;
			velocity_down = 0f;

		}

		public void Calc_Velocity()
		{
			//t * s = d
			//s = d/t
			this.velocity_up = distance_travel / distance_maxTime;
			this.velocity_down = distance_travel / (runningTime - distance_maxTime);
		}

		public float CurrentDistance(float currentTime)
		{
			//러닝타임 보다 더 큰 값이 들어오면 사용오류임
			if (runningTime < currentTime)
				return 0f; 
			
			if (currentTime <= distance_maxTime) 
			{
				return this.velocity_up * currentTime;
			}

			//if(distance_maxTime < currentTime)
			return  distance_travel - (this.velocity_down * currentTime);
		}

	}

	public struct Weapon
	{
		public enum eKind
		{
			None,
			Sword,	//칼 
			Lance,	//기사창
			Max,
		}
			
		public eKind 	kind;				//무기종류 
		//public Vector3  position;			//무기위치
		public float 	attack_range_min;	//최소 사거리
		public float 	attack_range_max; 	//최대 사거리 
		public float 	collider_sphere_radius;

		public void Default_Sword()
		{
			kind = eKind.Sword;
			//position = Vector3.zero;
			attack_range_min = 4f;
			attack_range_max = 14f;
			collider_sphere_radius = 2f;
		}
	}

	public class Character
	{
		public enum eState
		{
			None 		= 0,

			Start,
			Running,
			Waiting,
			End,

			Max,
		}

		public enum eSubState
		{
			None,

			Valid_Start,
			Valid_Running,
			Valid_End,

			Start,
			Running,
			End,

			Max
		}

		public enum eKind
		{
			None,
			Seonbi,
			Biking,
			Max
		};

		static public string StateToString (Character.eState state)
		{
			switch (state) 
			{
			case Character.eState.None:
				return "None";
			case Character.eState.Start:
				return "Start";
			case Character.eState.Running:
				return "Running";
			case Character.eState.Waiting:
				return "Waiting";
			case Character.eState.End:
				return "End";
			
			}

			return "None";
		}

		static public string SubStateToString (Character.eSubState state)
		{
			switch (state) 
			{
			case Character.eSubState.None:
				return "None";
			case Character.eSubState.Valid_Start:
				return "Valid_Start";
			case Character.eSubState.Valid_Running:
				return "Valid_Running";
			case Character.eSubState.Valid_End:
				return "Valid_End";

			}

			return "None";
		}

		//====================================

		//고유정보
		private uint 	_id;
		private int 	_hp_current;
		private int 	_hp_max;
		private Vector3 _position;
		private Vector3 _direction;
		private eKind	_kind;

		//무기정보 
		private Weapon 	_weapon;

		//동작정보
		private Behavior _behavior = null;
		private Skill 	_skill_current = null;
		private float 	_timeDelta = 0f; 	//시간변화량

		//상태정보
		private eState 	_state_current = eState.None;
		private eSubState _validState_current = eSubState.None; 	//유효상태
		private eSubState _giveState_current = eSubState.None; 		//준상태
		private eSubState _receiveState_current = eSubState.None; 	//받은상태 

		//판정
		private Judgment _judgment = new Judgment();

		//충돌모형
		private float _collider_sphere_radius;

		//====================================

		public Character()
		{
			_hp_current = 10;
			_hp_max = 10;
			_position = Vector3.zero;
			_direction = Vector3.right;
			_kind = eKind.Biking;

			_weapon.Default_Sword ();

			_collider_sphere_radius = 2f;  //임시로 넣어둔값


		}

		public void Init()
		{
			this.Idle ();
		}

		//====================================

		public CharacterManager ref_parent { get; set;} 

		public SkillBook ref_skillBook { get{ return CSingleton<SkillBook>.Instance; }}

		public eKind kind 
		{
			get { return _kind; }
			set { _kind = value; }
		}

		public Weapon weapon
		{
			get{ return _weapon; }
		}

		//====================================


		public Vector3 GetWeaponPosition()
		{
			return _position + (_behavior.CurrentDistance(_timeDelta) * _direction);
		}

		public void SetID(uint id)
		{
			this._id = id;
		}

		public uint GetID()
		{
			return this._id;
		}

		public Vector3 GetPosition()
		{
			return _position;
		}

		public void SetPosition(Vector3 pos)
		{
			_position = pos;
		}

		public Vector3 GetDirection()
		{
			return _direction;
		}

		public void SetDirection(Vector3 dir)
		{
			_direction = dir;
		}

		public float GetCollider_Sphere_Radius()
		{
			return _collider_sphere_radius;
		}

		public float GetTimeDelta()
		{
			return _timeDelta;
		}

		public Behavior GetBehavior()
		{
			return _behavior;
		}

		public eState CurrentState()
		{
			return _state_current;
		}

		public eSubState CurrentValidState()
		{
			return _validState_current;
		}

		public eSubState CurrentGiveState()
		{
			return _giveState_current;
		}

		public void SetState(eState setState)
		{
			_state_current = setState;
		}

		public void SetValidState(eSubState setSubState)
		{
			_validState_current = setSubState;
		}

		public void SetGiveState(eSubState setSubState)
		{
			_giveState_current = setSubState;
		}

		public void SetReceiveState(eSubState setSubState)
		{
			_receiveState_current = setSubState;
		}

		public float GetRangeMin()
		{
			return _behavior.plus_range_0 + _weapon.attack_range_min;
		}

		public float GetRangeMax()
		{
			return _behavior.plus_range_1 + _weapon.attack_range_max;
		}

		public Skill.eName CurrentSkillKind()
		{
			if(null != _skill_current)
				return _skill_current.name;

			return Skill.eName.None;
		}

		public int GetHP()
		{
			return _hp_current;
		}

		public int GetMaxHP()
		{
			return _hp_max;
		}
		public void SetHP(int hp)
		{
			_hp_current = hp;
		}

		public void AddHP(int amount)
		{
			_hp_current += amount;

			if (0 > _hp_current)
				_hp_current = 0;

			if (_hp_max < _hp_current)
				_hp_current = _hp_max;

		}


		public Judgment.eState GetJudgmentState()
		{
			return _judgment.state_current;
		}
		public void SetJudgmentState(Judgment.eState state)
		{
			
			this._judgment.state_current = state;
		}



		public float GetRunningTime()
		{
			return _behavior.runningTime;
		}

		public float GetScopeTime()
		{
			return _behavior.scopeTime_1 - _behavior.scopeTime_0;
		}

		public float GetOpenTime()
		{
			return _behavior.openTime_1 - _behavior.openTime_0;
		}

		public bool Valid_ScopeTime()
		{

			if (eState.Start == _state_current || eState.Running == _state_current) 
			{
				if (_behavior.scopeTime_0 <= _timeDelta && _timeDelta <= _behavior.scopeTime_1)
					return true;
			}

			return false;
		}

		public bool Valid_CloggedTime()
		{
			if (eState.Start == _state_current || eState.Running == _state_current) 
			{
				if (_behavior.cloggedTime_0 <= _timeDelta && _timeDelta <= _behavior.cloggedTime_1)
					return true;
			}

			return false;
		}

		public bool Valid_OpenTime()
		{
			if (eState.Running == _state_current) 
			{
				if (_behavior.openTime_0 <= _timeDelta && _timeDelta <= _behavior.openTime_1)
					return true;
			}

			return false;
		}

		public bool IsSkill_None()
		{
			if (Skill.eKind.None == _skill_current.kind )
				return true;

			return false;
		}

		public bool IsSkill_Attack()
		{
			if (Skill.eKind.Attack == _skill_current.kind)
				return true;

			return false;
		}

		public bool IsSkill_Block()
		{
			if (Skill.eKind.Block == _skill_current.kind)
				return true;

			return false;
		}


		public bool IsSkill_Counter()
		{
			if (Skill.eKind.Counter == _skill_current.kind)
				return true;

			return false;
		}

		public void SetSkill(Skill.eName kind)
		{
			_skill_current = ref_skillBook [kind];
			_behavior = _skill_current.FirstBehavior ();

			SetState (eState.Start);
			SetValidState (eSubState.None);
			SetGiveState (eSubState.None);
			//SetReceiveState (eSubState.None);

			this._timeDelta = 0f; //판정후 갱신되는 구조로 인해, 갱신되지 않은 상태에서 판정하는 문제 발생. => 스킬요청시 바로 초기화 시켜준다.  
		}


		public void Attack_1 ()
		{
			if (Skill.eName.Idle == _skill_current.name || true == this.Valid_OpenTime ()) 
			{
				//아이들상태거나 연결시간안에 행동이 들어온 경우
				SetSkill (Skill.eName.Attack_1);

				//DebugWide.LogBlue ("succeced!!! "); //chamto test
			}
		}
			

		public void Block()
		{
			if (Skill.eName.Idle == _skill_current.name || true == this.Valid_OpenTime ()) 
			{
				//아이들상태거나 연결시간안에 행동이 들어온 경우
				SetSkill (Skill.eName.Block_1);

				//DebugWide.LogBlue ("succeced!!! "); //chamto test
			}

		}


		public void Idle()
		{
			SetSkill (Skill.eName.Idle);
		}

		//상대로 부터 피해입다
		public void BeDamage(int damage)
		{
			this.AddHP (damage);

			SetSkill (Skill.eName.Hit);
		}


		public bool IsStart_AttackDamage()
		{
			if (Judgment.eState.Attack_Succeed == this.GetJudgmentState () &&
			   eSubState.Start == CurrentGiveState ())
				return true;

			return false;
		}

		public bool IsStart_BlockSucceed()
		{
			if (Judgment.eState.Block_Succeed == this.GetJudgmentState () &&
				eSubState.Start == CurrentGiveState ())
				return true;

			return false;
		}

		public bool IsStart_Damaged()
		{
			if (Judgment.eState.Damaged == this.GetJudgmentState () &&
				eSubState.Start == _receiveState_current )
				return true;

			return false;
		}





		//칼죽이기 가능한 거리인가?
		// * 내무기 범위와 상대방 무기위치로 판단한다.
		//의도 : 정확한 충돌처리를 위한 것이 아니다. 직선거리로 판정을 하기 위함이다.  
		public bool IsPossibleRange_Clog_VS(Character dst)
		{

			//1.들어온 무기 방향 검사 : 내앞에서 들어왔는가? 내뒤에서 들어왔는가?
			//2.내무기 범위 각도 검사 : 부채꼴 - 나중에 구현 

			//원을 반으로 나눠 앞쪽은 "같은 방향" , 뒤쪽은 "반대 방향" 으로 판단한다.
			float cos = Vector3.Dot (this.GetDirection(), dst.GetDirection ()); //두백터가 정규화 되었다면 cos삼각비가 나온다
			if (cos >= 0) 
			{	//같은 벡터 방향 
				return false;
			}
			//else if(angle < 0) {} //반대 벡터 방향


			//작은원 <= 대상 <= 큰원
			if(true == Util.Collision_Sphere (this._position, this.GetRangeMax(), dst.GetWeaponPosition(), dst.weapon.collider_sphere_radius)) 
			{	//큰원 보다 작고,
				if (false == Util.Collision_Sphere (this._position, this.GetRangeMin(), dst.GetWeaponPosition(), dst.weapon.collider_sphere_radius)) 
				{	//작은원 보다 크다. 
					return true;
				}

			}


			return false;
		}

		//공격이 상대방에 맞았나?
		//* 내무기 범위 또는 위치로 상대방 위치로 판단한다.
		//!!! 무기 범위가 방향성이 없다.  뒤나 앞이나 판정이 같다
		public bool Collision_Weaphon_Attack_VS(Character dst)
		{

			//정면에서 상대가 좌우 18도 안에 있을 때만 충돌처리 한다. (상하 18도 도 검사 된다. 추가 제한을 걸어 놓지 않았다. ) 
			//=======================================================================
			//0.1(1사분면) + 0.1(4사분면) = 0.2f  ,  90': 1f = 9' : 0.1f  ,  대략 9' * 2 안에 적이 있어야 공격이 가능하다. 
			const float ANGLE_SCOPE = 18f;
			float rate = 1f - ((ANGLE_SCOPE * 0.5f) / 90f); //1(단위원 최대값) - ((원하는각도 * 0.5) / 90도 )  ,  각도를 2로 나누는 이유 : 1,4사분면 부호가 같기 때문에 둘을 구별 할 수 없다. 의도와 다르게 2배 영역이 된다.
			Vector3 toDst = dst.GetPosition() - this.GetPosition();
			toDst.Normalize ();
			float cos = Vector3.Dot (this.GetDirection(), toDst);
			if(cos < rate) 
			{  //지정 각도보다 작으면 충돌검사 못함
				return false;
			} 
			//=======================================================================

			
			switch (this._behavior.attack_shape) 
			{
			case eTraceShape.Horizon: //todo : 추후 필요시 구현
			case eTraceShape.Vertical:
				{	//***** 내무기 범위 vs 상대방 위치 *****
					
					//작은원 <= 대상 <= 큰원
					if(true == Util.Collision_Sphere (this._position, this.GetRangeMax(), dst.GetPosition(), dst.GetCollider_Sphere_Radius())) 
					{	//큰원 보다 작고,
						if (false == Util.Collision_Sphere (this._position, this.GetRangeMin(), dst.GetPosition(), dst.GetCollider_Sphere_Radius())) 
						{	//작은원 보다 크다. 
							return true;
						}
					}

				}
				break;
			case eTraceShape.Straight:
				{	//***** 내무기 위치 vs 상대방 위치 *****

					if (true == Util.Collision_Sphere (this.GetWeaponPosition(), this.weapon.collider_sphere_radius, dst.GetPosition (), dst.GetCollider_Sphere_Radius ())) 
					{
						return true;
					}

				}
				break;
			}

			return false;
		}

		public void Test_Judge(Character dst)
		{
			//----------------------------------
			Judgment.eState jState = Judgment.eState.None;

			//============================
			//Attack_Vs_Attack
			//============================
			if (true == this.IsSkill_Attack () && true == dst.IsSkill_Attack () ) 
			{
				
				if (true == this.Valid_ScopeTime () && false == dst.Valid_ScopeTime()) 
				{	//먼저공격 나
					if(true == this.Collision_Weaphon_Attack_VS(dst))
					{
						jState = Judgment.eState.Attack_Succeed;
					}
				}
				if (false == this.Valid_ScopeTime () && true == dst.Valid_ScopeTime()) 
				{	//먼저공격 상대
					if(true == dst.Collision_Weaphon_Attack_VS(this))
					{
						jState = Judgment.eState.Damaged;
					}
				}
				else if (true == this.Valid_ScopeTime () && true == dst.Valid_ScopeTime()) 
				{	//칼겨루기
					if(this.IsPossibleRange_Clog_VS(dst))
					{
						jState = Judgment.eState.Attack_Withstand;		
					}
				}
				else if ( eState.Start == this.CurrentState () && true == dst.Valid_CloggedTime ()) 
				{	//칼죽이기
					if(this.IsPossibleRange_Clog_VS(dst))
					{
						jState = Judgment.eState.Attack_Weapon;		
					}
				}
				else if ( eState.Start == dst.CurrentState () && true == this.Valid_CloggedTime ()) 
				{	//칼죽이기 당함  
					if(dst.IsPossibleRange_Clog_VS(this))
					{
						jState = Judgment.eState.Attack_Clogged;		
					}
				}

			}

			//!!! 반대상태 연결 !!!
			//Attack_Succeed <==> Damaged
			//Attack_Blocked <==> Block_Succeed
			//Attack_Weapon <==> Attack_Clogged
			//Attack_Withstand <==> Attack_Withstand

			//============================
			//Attack_Vs_Block
			//============================
			if (true == this.IsSkill_Attack () && true == dst.IsSkill_Block ()) 
			{
				if (true == this.Collision_Weaphon_Attack_VS (dst)) 
				{
					if (true == this.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
					{
						jState = Judgment.eState.Attack_Blocked;
					}
					if (true == this.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
					{
						jState = Judgment.eState.Attack_Succeed;
					}
				}
			}

			//============================
			//Block_Vs_Attack
			//============================
			if (true == this.IsSkill_Block () && true == dst.IsSkill_Attack ()) 
			{
				if (true == dst.Collision_Weaphon_Attack_VS (this)) 
				{
					if (true == this.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
					{
						jState = Judgment.eState.Block_Succeed;
					}
					if (false == this.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
					{
						jState = Judgment.eState.Damaged;
					}
				}

			}
			//============================
			//Attack_Vs_None
			//============================
			if (true == this.IsSkill_Attack () && true == dst.IsSkill_None ()) 
			{
				if (true == this.Collision_Weaphon_Attack_VS (dst)) 
				{
					if (true == this.Valid_ScopeTime () ) 
					{
						jState = Judgment.eState.Attack_Succeed;
					}
				}

			}
			//============================
			//None_Vs_Attack
			//============================
			if (true == this.IsSkill_None () && true == dst.IsSkill_Attack ()) 
			{
				if (true == dst.Collision_Weaphon_Attack_VS (this)) 
				{
					if (true == dst.Valid_ScopeTime () ) 
					{
						jState = Judgment.eState.Damaged;
					}
				}

			}

			this.SetJudgmentState (jState);
			//----------------------------------
		}

		public void Judge(Character dst)
		{
			
			//if(1 == this.GetID())
			//	DebugWide.LogBlue ("[0:Judge : "+this.GetID() + "  !!!  "+_judgment.state_current); //chamto test

			switch (this.GetJudgmentState()) 
			{
			case Judgment.eState.Attack_Succeed:
				{
					//DebugWide.LogRed (this.GetID() + "  !!!  "+result.src + "  " + result.dst); //chamto test

					//한동작에서 일어난 사건
					if (eSubState.Start == CurrentGiveState ()) 
					{
						//apply jugement : HP
						dst.BeDamage (-1);
					}
						
				}
				break;
			}//end switch

		}//end func


		public void Update()
		{
			//this._timeDelta += Time.deltaTime;
			this._timeDelta += FrameControl.DeltaTime();
			
			switch (this._state_current) 
			{
			case eState.Start:
				{	//키입력에 의해 바로 Start상태에 이르게 된다. 키입력후 바로 Update 함수가 실행되면 Start상태는 바로 Running으로 변경되어 버려 UI에서 검출 할 수 없다.
					
					this._timeDelta = 0f;
					SetState (eState.Running);

					//DebugWide.LogRed ("[0: "+this._state_current);//chamto test
				}
				break;
			case eState.Running:
				{

					//====================================================
					// update sub_state 
					//====================================================



					switch (_validState_current) 
					{
					case eSubState.None:
						if (_behavior.scopeTime_0 <= _timeDelta && _timeDelta <= _behavior.scopeTime_1) {
							this.SetValidState (eSubState.Valid_Start);
						}
						break;
					case eSubState.Valid_Start:
						this.SetValidState (eSubState.Valid_Running);
						break;
					case eSubState.Valid_Running:
						if (!(_behavior.scopeTime_0 <= _timeDelta && _timeDelta < _behavior.scopeTime_1)) {
							this.SetValidState (eSubState.Valid_End);
						}

						break;
					case eSubState.Valid_End:
						this.SetValidState (eSubState.None);
						break;

					}

					//if(1 == this.GetID())
					//	DebugWide.LogBlue ("[1:_validState_current : "+_validState_current );//chamto test

					//====================================================
					// 실제 공격/방어 한 범위
					//====================================================
					switch (_giveState_current) 
					{
					case eSubState.None:
						{
							if (Judgment.eState.Attack_Succeed == this.GetJudgmentState () ||
								Judgment.eState.Block_Succeed == this.GetJudgmentState()) 
							{
								this.SetGiveState (eSubState.Start);
							}
						}
						break;
					case eSubState.Start:
						{
							this.SetGiveState (eSubState.Running);
						}
						break;
					case eSubState.Running:
						{
							
						}
						break;
					}

					//if(1 == this.GetID())
					//	DebugWide.LogBlue ("[2:_giveState_current : " + _giveState_current);
					
					//====================================================

					
					if (_behavior.runningTime <= this._timeDelta) 
					{
						//동작완료
						this.SetState (eState.Waiting);
					}	
				}
				break;
			case eState.Waiting:
				{
					//DebugWide.LogBlue (_behavior.rigidTime + "   " + (this._timeDelta - _behavior.allTime));
					if (_behavior.rigidTime <= (this._timeDelta - _behavior.runningTime)) 
					{
						this.SetState (eState.End);
					}	

				}
				break;
			case eState.End:
				{
					_behavior = _skill_current.NextBehavior ();

					if (null == _behavior) 
					{
						//스킬 동작을 모두 꺼냈으면 아이들상태로 들어간다
						Idle ();
					} else 
					{
						//다음 스킬 동작으로 넘어간다
						SetState (eState.Start);
					}
						
				}
				break;
			
			
			}

			//============================================================================

			switch (_receiveState_current) 
			{
			case eSubState.None:
				{
					
					if (Judgment.eState.Damaged == this.GetJudgmentState ())
					{
						this.SetReceiveState (eSubState.Start);
					}
				}
				break;
			case eSubState.Start:
				{
					this.SetReceiveState (eSubState.Running);
				}
				break;
			case eSubState.Running:
				{
					if (Judgment.eState.Damaged != this.GetJudgmentState ())
					{
						this.SetReceiveState (eSubState.None);
					}
				}
				break;
			}

			//if(1 == this.GetID())
			//	DebugWide.LogBlue ("[3:_receiveState_current : " + _receiveState_current);
			//============================================================================

		}//end func

	}



	public class CharacterManager : Dictionary<uint, Character>
	{
		private uint id_Sequence = 0;
		//todo : 제거된 id를 관리하는 리스트 필요 , 추후 메모리풀 구현

		public void Init(int characterCount)
		{
			
			for (int i = 0; i < characterCount; i++) 
			{
				this.AddCharacter ();
			}

			this.TestPrint ();
		}

		public uint GetIDSequence()
		{
			return ++this.id_Sequence;
		}

		public uint AddCharacter()
		{
			uint ID = this.GetIDSequence ();
			Character crt = new Character ();
			this.Add (ID, crt);

			crt.SetID (ID);
			crt.ref_parent = this;
			crt.Init ();

			return ID;
		}


		Judgment.Result _result_;
		Character _src_ = null;
		Character _dst_ = null;
		public void Update()
		{
			//1 =========== 
			foreach (Character crt in this.Values) 
			{
				crt.Update (); //호출순서중요 : judge 함수에 의해 상태시작후 Update 함수가 호출되면 UI에서 시작상태를 검출 할 수 없다.
			}


			//2 ===========
			//todo : 거리에 따라 판정대상 객체 거르는 처리가 필요 
			//캐릭터 전체 리스트 1대1조합 : 중복안됨, 순서없음
			for (int id_src = 0 ; id_src < (this.Values.Count-1) ; id_src++) 
			{
				for (int id_dst = (id_src+1) ; id_dst < this.Values.Count ; id_dst++) 
				{
					
					_src_ = this.Values.ElementAt (id_src);
					_dst_ = this.Values.ElementAt (id_dst);

					//========================================

					//상태갱신
					//_result_ = Judgment.Judge(_src_, _dst_);
					//_src_.SetJudgmentState (_result_.first);
					//_dst_.SetJudgmentState (_result_.second);
					_src_.Test_Judge(_dst_); //chamto test
					_dst_.Test_Judge(_src_); 

					//DebugWide.LogGreen ("1P: "+_src_.GetJudgmentState() + "  2P: " + _dst_.GetJudgmentState()); //chamto test
					//DebugWide.LogGreen ("a: "+_src_.CurrentSkillKind() + "  " + _dst_.CurrentSkillKind()); //chamto test
					//DebugWide.LogGreen (_result_.first + "   " + _src_.GetTimeDelta() + " , " + _result_.second + "  " + _dst_.GetTimeDelta()); //chamto test

					//갱신된 상태에 따른 처리 
					_src_.Judge (_dst_);
					_dst_.Judge (_src_);


					//DebugWide.LogGreen ("b: "+_src_.CurrentSkillKind() + "  " + _dst_.CurrentSkillKind()); //chamto test
				}
			}




			//DebugWide.LogRed ("====================="); //chamto test
		}

		public void TestPrint()
		{
			foreach (KeyValuePair<uint,Character> pair in this) 
			{
				DebugWide.LogGreen (pair.Key + "  " + pair.Value);
			}
		}

	}

	public class Judgment
	{

		public struct Result
		{
			public eState first;
			public eState second;

			public Result(eState s, eState d)
			{
				first = s;
				second = d;
			}

			public void Init()
			{
				first = eState.None;
				second = eState.None;
			}
		}

		public enum eState
		{
			None = 0,


			Attack_Succeed, 	//공격 성공 
			Attack_Weapon,		//상대 무기를 공격 :  상대는 부분막힘 상태가 된다. 
			Attack_Clogged, 	//공격 부분막힘 : 반격가능  
			Attack_Withstand,	//공격 버티기  : 칼이 서로 붙어 힘겨루기 하는 경우 , 서로 동시에 같은 공격한 경우 
			Attack_Blocked, 	//공격 완전막힘 
			Attack_Idle, 		//공격 헛 동작 : 멀리서 공격한 경우

			Block_Succeed,
			Block_Idle, 		//방어 헛 동작

			Damaged, 			//피해입음

			Max
		}
			
		//public uint targetID { get; set; }
		public eState state_prev { get; set; }
		public eState state_current { get; set; }

		public Judgment()
		{
			//targetID = 0;
			state_prev = eState.None;
			state_current = eState.None;
		}


		//==========================================



		static public Result Judge(Character src , Character dst)
		{
			Result result = new Result();
			result.Init ();


			//공격범위(무기와 기술에 영향을 받음)  ,  상대와의 거리  ,  공격타점의 이동시간(사거리의 이동시간)
			//무기범위 , 신체범위
			//판정 : 타점이 신체범위에 들어 왔는가?


			//============================
			//Attack_Vs_Attack
			//============================
			if (true == src.IsSkill_Attack () && true == dst.IsSkill_Attack()) 
			{
				if (true == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					
					result.first = eState.Attack_Blocked;
					result.second = eState.Attack_Blocked;

				}
				if (true == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Succeed;
					result.second = eState.Damaged;
				}
				if (false == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Damaged;
					result.second = eState.Attack_Succeed;
				}
				if (false == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Idle;
					result.second = eState.Attack_Idle;
				}

			}

			//============================
			//Attack_Vs_Block
			//============================
			if (true == src.IsSkill_Attack () && true == dst.IsSkill_Block()) 
			{
				if (true == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Blocked;
					result.second = eState.Block_Succeed;
				}
				if (true == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Succeed;
					result.second = eState.Damaged;
				}
				if (false == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Idle;
					result.second = eState.Block_Idle;
				}
				if (false == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Attack_Idle;
					result.second = eState.Block_Idle;
				}
			}

			//============================
			//Block_Vs_Attack
			//============================
			if (true == src.IsSkill_Block () && true == dst.IsSkill_Attack()) 
			{
				if (true == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Block_Succeed;
					result.second = eState.Attack_Blocked;
				}
				if (true == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Block_Idle;
					result.second = eState.Attack_Idle;
				}
				if (false == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Damaged;
					result.second = eState.Attack_Succeed;
				}
				if (false == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) 
				{
					result.first = eState.Block_Idle;
					result.second = eState.Attack_Idle;
				}

			}

			//============================
			//Attack_Vs_None
			//============================
			if (true == src.IsSkill_Attack () && true == dst.IsSkill_None ()) {
				if (true == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) {
					result.first = eState.Attack_Succeed;
					result.second = eState.Damaged;
				}
				if (true == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) {
					result.first = eState.Attack_Succeed;
					result.second = eState.Damaged;
				}
				if (false == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) {
					result.first = eState.Attack_Idle;
					result.second = eState.None;
				}
				if (false == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) {
					result.first = eState.Attack_Idle;
					result.second = eState.None;
				}
			}


			//============================
			//None_Vs_Attack
			//============================
			if (true == src.IsSkill_None () && true == dst.IsSkill_Attack ()) {
				if (true == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) {
					result.first = eState.Damaged;
					result.second = eState.Attack_Succeed;
				}
				if (true == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) {
					result.first = eState.None;
					result.second = eState.Attack_Idle;
				}
				if (false == src.Valid_ScopeTime () && true == dst.Valid_ScopeTime ()) {
					result.first = eState.Damaged;
					result.second = eState.Attack_Succeed;
				}
				if (false == src.Valid_ScopeTime () && false == dst.Valid_ScopeTime ()) {
					result.first = eState.None;
					result.second = eState.Attack_Idle;
				}
			}

			//============================
			//Exceptions
			//============================



			return result;
		}




	}//end class	

	public class SkillManager
	{
		
	}


	public class Skill : List<Behavior>
	{

		public enum eKind
		{
			None,
			Attack,
			Block,
			Counter,
			Max
		}

		public enum eName
		{
			None,
			Idle,
			Hit,

			Attack_1,
			Attack_2Combo,
			Attack_3Combo,

			Block_1,
			Block_2Combo,
			Block_3Combo,

			CounterBlock,
			Max
		}

		static public string NameToString (Skill.eName name)
		{
			switch (name) 
			{
			case Skill.eName.None:
				return "None";
			case Skill.eName.Idle:
				return "Idle";
			case Skill.eName.Hit:
				return "Hit";

			case Skill.eName.Attack_1:
				return "Attack_1";
			case Skill.eName.Attack_2Combo:
				return "Attack_2Combo";
			case Skill.eName.Attack_3Combo:
				return "Attack_3Combo";

			case Skill.eName.Block_1:
				return "Block_1";
			case Skill.eName.Block_2Combo:
				return "Block_2Combo";
			case Skill.eName.Block_3Combo:
				return "Block_3Combo";

			case Skill.eName.CounterBlock:
				return "CounterBlock";
			
			}

			return "None";
		}
		//========================================

		private int _index_current = 0;


		//========================================

		public eKind kind { get; set; }
		public eName name { get; set; }

		//========================================

		public Behavior FirstBehavior()
		{
			_index_current = 0; //index 초기화

			if (this.Count == 0)
				return null;

			return this [_index_current];
		}

		public Behavior NextBehavior()
		{
			if (this.Count > _index_current) 
			{
				//마지막 인덱스임
				if (this.Count == _index_current + 1)
					return null;

				_index_current++;
				return this [_index_current];
			}

			return null;
		}




		//========================================

		//스킬 명세서
		static public Skill Details_Idle()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.None;
			skinfo.name = eName.Idle;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;

			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Hit()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.None;
			skinfo.name = eName.Hit;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}


		static public Skill Details_Attack_1()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack;
			skinfo.name = eName.Attack_1;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1.0f;
			bhvo.scopeTime_0 = 0.6f;
			bhvo.scopeTime_1 = 0.8f;
			bhvo.rigidTime = 0.3f;
			bhvo.openTime_0 = 0.7f;
			bhvo.openTime_1 = 1f;
			bhvo.cloggedTime_0 = 0.2f;
			bhvo.cloggedTime_1 = 0.6f;
			bhvo.attack_shape = eTraceShape.Straight;
			bhvo.plus_range_0 = 2f;
			bhvo.plus_range_1 = 2f;
			bhvo.distance_travel = Behavior.DEFAULT_DISTANCE;
			bhvo.distance_maxTime = bhvo.scopeTime_1; //유효범위 끝시간에 최대 거리가 되게 한다.
			bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_3Combo()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack;
			skinfo.name = eName.Attack_3Combo;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}



		static public Skill Details_Block_1()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Block;
			skinfo.name = eName.Block_1;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 1f;
			bhvo.rigidTime = 0.1f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);


			return skinfo;
		}

		static public Skill Details_CounterBlock()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Counter;
			skinfo.name = eName.CounterBlock;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);


			return skinfo;
		}



	}


	public class SkillBook : Dictionary<Skill.eName, Skill>
	{
		public SkillBook()
		{
			this.Add (Skill.eName.Idle, Skill.Details_Idle ());
			this.Add (Skill.eName.Hit, Skill.Details_Hit ());
			this.Add (Skill.eName.Attack_1, Skill.Details_Attack_1 ());
			this.Add (Skill.eName.Attack_3Combo, Skill.Details_Attack_3Combo ());

			this.Add (Skill.eName.Block_1, Skill.Details_Block_1 ());

			this.Add (Skill.eName.CounterBlock, Skill.Details_CounterBlock ());

		}
	}


	public class ResourceManager
	{
		public enum eActionKind
		{
			None,
			Idle,
			AttackBefore,
			AttackValid,
			AttackAfter,

			BlockBefore,
			BlockValid,
			BlockAfter,

			CounterBlock,
			Max
		}

		public enum eSPRITE_NAME
		{
			NONE,
			EMPTY_CARD,

			P1_IDLE,
			P1_ATTACK_BEFORE,
			P1_ATTACK_VALID,
			P1_ATTACK_AFTER,
			P1_BLOCK_BEFORE,
			P1_BLOCK_VALID,
			P1_BLOCK_AFTER,

			P2_IDLE,
			P2_ATTACK_BEFORE,
			P2_ATTACK_VALID,
			P2_ATTACK_AFTER,
			P2_BLOCK_BEFORE,
			P2_BLOCK_VALID,
			P2_BLOCK_AFTER,

			EFFECT_FUCK,
			EFFECT_HIT,
			EFFECT_BLOCK,
			EFFECT_TING1,
			EFFECT_TING2,
			EFFECT_STRIKE_COMB, //빗살
			EFFECT_STRIKE_X,
			EFFECT_STRIKE_CROSS,

			MAX
		}


		private HierarchyPreLoader _ref_herch = null;

		private Dictionary<eSPRITE_NAME, string> _sprNameDict = null;
		private Dictionary<eSPRITE_NAME, Sprite> _loadedDict = new Dictionary<eSPRITE_NAME, Sprite> ();

		public ResourceManager()
		{
			_ref_herch = CSingleton<HierarchyPreLoader>.Instance;
		}


		public eSPRITE_NAME StringToEnum(string name)
		{
			//20170813 chamto fixme - value 값이 없을 때의 예외 처리가 없음 
			//ref : https://stackoverflow.com/questions/2444033/get-dictionary-key-by-value
			return _sprNameDict.FirstOrDefault(x => x.Value == name).Key;
		}

		public void Init()
		{
			_ref_herch.Init ();

			_sprNameDict = new Dictionary<eSPRITE_NAME, string> ()
			{
				{eSPRITE_NAME.EMPTY_CARD, "empty_card"},
				{eSPRITE_NAME.P1_IDLE, "p1_idle"},
				{eSPRITE_NAME.P1_ATTACK_BEFORE, "p1_attack_before"},
				{eSPRITE_NAME.P1_ATTACK_VALID, "p1_attack_valid"},
				{eSPRITE_NAME.P1_ATTACK_AFTER, "p1_attack_after"},
				{eSPRITE_NAME.P1_BLOCK_BEFORE, "p1_block_before"},
				{eSPRITE_NAME.P1_BLOCK_VALID, "p1_block_valid"},
				{eSPRITE_NAME.P1_BLOCK_AFTER, "p1_block_after"},

				{eSPRITE_NAME.P2_IDLE, "p2_idle"},
				{eSPRITE_NAME.P2_ATTACK_BEFORE, "p2_attack_before"},
				{eSPRITE_NAME.P2_ATTACK_VALID, "p2_attack_valid"},
				{eSPRITE_NAME.P2_ATTACK_AFTER, "p2_attack_after"},
				{eSPRITE_NAME.P2_BLOCK_BEFORE, "p2_block_before"},
				{eSPRITE_NAME.P2_BLOCK_VALID, "p2_block_valid"},
				{eSPRITE_NAME.P2_BLOCK_AFTER, "p2_block_after"},

				{eSPRITE_NAME.EFFECT_FUCK, "effect_0"},
				{eSPRITE_NAME.EFFECT_HIT, "effect_1"},
				{eSPRITE_NAME.EFFECT_BLOCK, "effect_2"},
				{eSPRITE_NAME.EFFECT_TING1, "effect_3"},
				{eSPRITE_NAME.EFFECT_TING2, "effect_4"},
				{eSPRITE_NAME.EFFECT_STRIKE_COMB, "effect_5"},
				{eSPRITE_NAME.EFFECT_STRIKE_X, "effect_6"},
				{eSPRITE_NAME.EFFECT_STRIKE_CROSS, "effect_7"},
			};


			this.Load_Sprite ();
		}

		public void Load_Sprite()
		{

			//=============================================
			//LOAD Texture/battleCard
			//=============================================
			Sprite[] sprites = Resources.LoadAll <Sprite>("Texture/battleCard");
			for(int i=0;i<sprites.Length;i++)
			{
				
				eSPRITE_NAME key = this.StringToEnum (sprites [i].name);
				_loadedDict.Add (key, sprites [i]);
			}


			//=============================================
			//LAOD Texture/effect
			//=============================================
			sprites = Resources.LoadAll <Sprite>("Texture/effect");
			for(int i=0;i<sprites.Length;i++)
			{

				eSPRITE_NAME key = this.StringToEnum (sprites [i].name);
				_loadedDict.Add (key, sprites [i]);
			}

		}

		public Sprite GetSprite(eSPRITE_NAME eName)
		{
			//20170813 chamto fixme - enum 값이 없을 때의 예외 처리가 없음 
			return _loadedDict [eName];
		}

		public Sprite GetAction_Seonbi(eActionKind actionKind)
		{
			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_IDLE);
			case eActionKind.AttackBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_BEFORE);
			case eActionKind.AttackValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_VALID);
			case eActionKind.AttackAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_AFTER);
			case eActionKind.BlockBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_BEFORE);
			case eActionKind.BlockValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_VALID);
			case eActionKind.BlockAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_AFTER);
			case eActionKind.CounterBlock:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.EMPTY_CARD); //chamto temp value

			}

			return null;
		}

		public Sprite GetAction_Biking(eActionKind actionKind)
		{
			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_IDLE);
			case eActionKind.AttackBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_BEFORE);
			case eActionKind.AttackValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_VALID);
			case eActionKind.AttackAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_AFTER);
			case eActionKind.BlockBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_BEFORE);
			case eActionKind.BlockValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_VALID);
			case eActionKind.BlockAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_AFTER);
			case eActionKind.CounterBlock:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.EMPTY_CARD); //chamto temp value

			}

			return null;
		}

		public ResourceManager.eSPRITE_NAME GetSpriteName_Seonbi(eActionKind actionKind)
		{
			ResourceManager.eSPRITE_NAME sprName = ResourceManager.eSPRITE_NAME.NONE;

			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				sprName = ResourceManager.eSPRITE_NAME.P1_IDLE;
				break;
			case eActionKind.AttackBefore:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_BEFORE;
				break;
			case eActionKind.AttackValid:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_VALID;
				break;
			case eActionKind.AttackAfter:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_AFTER;
				break;
			case eActionKind.BlockBefore:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_BEFORE;
				break;
			case eActionKind.BlockValid:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_VALID;
				break;
			case eActionKind.BlockAfter:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_AFTER;
				break;
			case eActionKind.CounterBlock:
				sprName = ResourceManager.eSPRITE_NAME.EMPTY_CARD; //chamto temp value
				break;

			}

			return sprName;
		}

		public ResourceManager.eSPRITE_NAME GetSpriteName_Biking(eActionKind actionKind)
		{
			ResourceManager.eSPRITE_NAME sprName = ResourceManager.eSPRITE_NAME.NONE;

			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				sprName = ResourceManager.eSPRITE_NAME.P2_IDLE;
				break;								  
			case eActionKind.AttackBefore:			  
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_BEFORE;
				break;								  
			case eActionKind.AttackValid:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_VALID;
				break;								  
			case eActionKind.AttackAfter:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_AFTER;
				break;								  
			case eActionKind.BlockBefore:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_BEFORE;
				break;								  
			case eActionKind.BlockValid:				
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_VALID;
				break;								  
			case eActionKind.BlockAfter:			
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_AFTER;
				break;
			case eActionKind.CounterBlock:
				sprName = ResourceManager.eSPRITE_NAME.EMPTY_CARD; //chamto temp value
				break;

			}

			return sprName;
		}

		public GameObject CreatePrefab(string prefabPath , Transform parent , string name)
		{
			const string root = "Prefab/";
			GameObject obj =  MonoBehaviour.Instantiate(Resources.Load(root + prefabPath)) as GameObject;
			obj.transform.SetParent (parent,false);
			obj.transform.name = name;

			_ref_herch.PreOrderTraversal (obj.transform);

			return obj;
		}



		public void TestPrint()
		{
			foreach (Sprite s in _loadedDict.Values) 
			{
				DebugWide.LogBlue (s.name);
			}
			foreach (eSPRITE_NAME s in _loadedDict.Keys) 
			{
				DebugWide.LogBlue (s);
			}
		}
	}//end class


	public class AnimationCard
	{

		public enum eState
		{
			None,
			Start,
			Running,
			End,
			Max
		}

		private eState _state = eState.None;
		private float _accumulate = 0f;
		private float _scaleDelta = 0f;

		private float _start = 0f;
		private float _end = 0f;
		private float _maxSecond = 0f;
		private Transform _dst = null;

		public void Start_Card_Move(Transform dst, float start, float end, float maxSecond)
		{
			_dst = dst;
			_start = start;
			_end = end;
			_maxSecond = maxSecond;

			_state = eState.Start;
		}

		public void Stop()
		{
			_state = eState.None;
		}

		public void Update()
		{

			switch (_state) 
			{
			case eState.None:
				break;
			case eState.Start:
				{
					_state = eState.Running;
					_accumulate = 0f;
					_scaleDelta = 0f;

				}
				break;

			case eState.Running:
				{
					_accumulate += Time.deltaTime;
					if (_maxSecond <= _accumulate) 
					{
						_state = eState.End;
						break;
					}

					//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

					_scaleDelta = Utility.Interpolation.easeOutElastic (_start, _end, _accumulate/_maxSecond);


					_dst.Translate(_scaleDelta,0,0);
					//DebugWide.LogBlue (_scaleDelta); //chamto test
					//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
				}
				break;
			case eState.End:
				{
					_state = eState.None;
				}
				break;
			}

		}//end func


	}//end class


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
		public Sprite sprite 
		{
			get 
			{ 
				return _spriteRender.sprite;
			}
			set
			{
				_spriteRender.sprite = value;
			}
		}
		public GameObject gameObject
		{
			get
			{ 
				return _spriteRender.gameObject;
			}
		}
		public Transform transform
		{
			get
			{
				return _spriteRender.transform;
			}
		}


		private Vector3 		_position = Vector3.zero;
		private Vector3 		_scale = Vector3.one;
		private Quaternion		_rotation = Quaternion.identity;

		public Vector3 Get_InitialPostition()
		{
			return _position;
		}
		public Vector3 Get_InitialScale()
		{
			return _scale;
		}
		public Quaternion Get_InitialRotation()
		{
			return _rotation;
		}

		public  InitialData(SpriteRenderer spr)
		{
			_spriteRender = spr;
			_position = spr.transform.localPosition;
			_scale = spr.transform.localScale;
			_rotation = spr.transform.localRotation;
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
				_spriteRender.transform.localPosition = _position;

			if(eOption.Scale == (opt & eOption.Scale))
				_spriteRender.transform.localScale = _scale;

			if(eOption.Rotation == (opt & eOption.Rotation))
				_spriteRender.transform.localRotation = _rotation;
		}

	}//end class

	public class UI_CharacterCard : MonoBehaviour
	{
		
		public enum eEffect
		{
			None,
			Empty,
			Text,
			Hit,
			Block,
			Max
		};


		public uint _id = 0;

		public TextMesh _text_explanation { get; set; }
		public TextMesh _text_time { get; set; }
		//public Slider _hp_bar { get; set; }

		public Transform _actionRoot { get; set; }
		public List<InitialData> _actions { get; set; }

		public Transform _effectRoot { get; set; }
		public Transform _effect_Texts { get; set; }
		public Dictionary<eEffect,InitialData> _effects { get; set; }

		//[SerializeField] 
		public bool _apply = false;


		//===============================================================

		public Character data
		{
			get;
			set;
		}

		//===============================================================


		public UI_CharacterCard()
		{
			this.data = null;
		}

		void Start()
		{
			

		}


		static public UI_CharacterCard Create(string name)
		{
			GameObject obj = Single.resource.CreatePrefab ("character_seonbi2", Single.game_root, name);

			string parentPath = Single.hierarchy.GetTransformFullPath (obj.transform);

			UI_CharacterCard ui = obj.AddComponent<UI_CharacterCard> ();
			ui._text_explanation = Single.hierarchy.Find<TextMesh> (parentPath + "/Text_explanation");
			ui._text_time = Single.hierarchy.Find<TextMesh> (parentPath + "/Text_time");
			//ui._hp_bar = Single.hierarchy.Find<Slider> (parentPath + "/Slider");

			//action
			ui._actionRoot = Single.hierarchy.Find<Transform> (parentPath + "/Images");
			ui._actions = new List<InitialData> ();
			//ui._action_originalPos = new List<Vector3> ();
			const int MAX_ACTION_CARD = 3;
			SpriteRenderer img = null;
			InitialData iData = null;
			for (int i = 0; i < MAX_ACTION_CARD; i++) 
			{
				img = Single.hierarchy.Find<SpriteRenderer> (parentPath + "/Images/Action_"+i.ToString("00"));
				iData = new InitialData (img);
				ui._actions.Add (iData);
				//ui._action_originalPos.Add (img.transform.localPosition);
			}

			//effect
			ui._effectRoot = Single.hierarchy.Find<Transform> (parentPath + "/Effects");
			ui._effect_Texts = Single.hierarchy.Find<Transform> (parentPath + "/Effects/texts");
			ui._effects = new Dictionary<eEffect, InitialData> ();
			img = Single.hierarchy.Find<SpriteRenderer> (parentPath + "/Effects/empty");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Empty,iData);
			img = Single.hierarchy.Find<SpriteRenderer> (parentPath + "/Effects/texts/hit");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Hit,iData);
			img = Single.hierarchy.Find<SpriteRenderer> (parentPath + "/Effects/block");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Block,iData);
			img = Single.hierarchy.Find<SpriteRenderer> (parentPath + "/Effects/texts/hit/fuck");
			iData = new InitialData (img);
			ui._effects.Add (eEffect.Text,iData);


			return ui;
		}



		public void RevertData_All()
		{

			foreach (InitialData iData in _actions) 
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

			data.SetDirection (Vector3.left);
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

			data.SetDirection (Vector3.right);
			//_hp_bar.direction = Slider.Direction.LeftToRight;
		}

		public void SetCharacter(Character.eKind kind)
		{
			data.kind = kind;
		}

		public void SetPosition(Vector3 pos)
		{

			data.SetPosition (pos);

			transform.localPosition = pos;

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

		//void Update() {}  //chamto : 유니티 update 사용하지 말것. 호출순서를 코드에서 조작하기 위함

		public void Update_UI()
		{
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
		}

		private void Update_UI_HPBAR()
		{
			//charUI._hp_bar.maxValue = charData.GetMaxHP ();
			//charUI._hp_bar.value = charData.GetHP ();
		}

		private void Update_UI_Explan()
		{
			
			this._text_explanation.text = 
				"  "  + Character.StateToString(data.CurrentState()) +
				"  sub:"+ Character.SubStateToString(data.CurrentValidState()) ;

			this._text_time.text = 
				Skill.NameToString(data.CurrentSkillKind()) + "   " +
				data.GetTimeDelta().ToString("0.0");

		}




		private void Update_UI_Card()
		{
			
			//if(2 == id)
			//	DebugWide.LogBlue (id+" : "+charData.CurrentState ()); //chamto test

			//if (Skill.eName.Idle == charData.CurrentSkillKind () ||
			//	Skill.eName.Hit == charData.CurrentSkillKind ()) 
			{
				switch (data.CurrentState ()) 
				{
				case Character.eState.Start:
					{


						this._actions [1].gameObject.SetActive (false);
						this._actions [2].gameObject.SetActive (false);

						this._actions [0].SelectAction (data.kind, ResourceManager.eActionKind.Idle);
					}
					break;
				}

			}


			if (Skill.eName.Attack_1 == data.CurrentSkillKind ()) 
			{

				switch (data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						this._actions[1].gameObject.SetActive (true);
						this._actions[2].gameObject.SetActive (false);

						this._actions[1].SelectAction(data.kind, ResourceManager.eActionKind.AttackBefore);
						this._actions[2].SelectAction (data.kind, ResourceManager.eActionKind.AttackValid);

						//iTween.Stop (charUI._actions [2].gameObject);
						//charUI.RevertData_All ();

					}
					break;
				case Character.eState.Running:
					{


						//====================================================
						//update sub_state
						//====================================================
						switch (data.CurrentValidState ()) 
						{
						case Character.eSubState.Valid_Start:
							{
								//DebugWide.LogBlue ("Valid_Start"); //chamto test

								CharDataBundle bundle;
								bundle._data = data;
								bundle._ui = this;
								bundle._gameObject = this._actions [2].gameObject;

								//StartCoroutine("AniStart_Attack_1",bundle); 
								StartCoroutine("AniStart_Attack_1_Random",bundle); 

							}
							break;
						case Character.eSubState.Valid_Running:
							{



							}
							break;
						case Character.eSubState.Valid_End:
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
						this._actions[1].SelectAction (data.kind, ResourceManager.eActionKind.AttackAfter);

					}

					break;
				case Character.eState.End:
					{
						//charUI._actions[1].gameObject.SetActive (false);
						//charUI._actions[2].gameObject.SetActive (false);
					}
					break;

				}//end switch


			}

			if (Skill.eName.Block_1 == data.CurrentSkillKind ()) 
			{



				switch (data.CurrentState ()) 
				{
				case Character.eState.Start:
					{
						this._actions[1].gameObject.SetActive (true);	
						this._actions[2].gameObject.SetActive (false);

						this._actions[1].SelectAction (data.kind, ResourceManager.eActionKind.BlockBefore);
					}
					break;
				case Character.eState.Running:
					{
						//=========================================

						switch (data.CurrentValidState ()) 
						{
						case Character.eSubState.Valid_Start:
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
			
			if (data.IsStart_Damaged ()) 
			{
				CharDataBundle bundle;
				bundle._data = data;
				bundle._ui = this;
				bundle._gameObject = this._effects [UI_CharacterCard.eEffect.Hit].gameObject;
				StartCoroutine("EffectStart_Damaged",bundle);

				bundle._gameObject = this._actionRoot.gameObject;
				StartCoroutine("EffectStart_Wobble",bundle);

			}


			if (data.IsStart_BlockSucceed ()) 
			{
				CharDataBundle bundle;
				bundle._data = data;
				bundle._ui = this;
				bundle._gameObject = this._effects [UI_CharacterCard.eEffect.Block].gameObject;
				StartCoroutine("EffectStart_Block",bundle);

				bundle._gameObject = this._actionRoot.gameObject;
				StartCoroutine("EffectStart_Endure",bundle);
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


		public IEnumerator AniStart_Attack_1(CharDataBundle bundle)
		{

			float time = bundle._data.GetScopeTime ();
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
			const int NODE_COUNT = 6;
			Vector3[] list = new Vector3[NODE_COUNT];

			if (PATH_01 == kind)
				pathName = "p01";
			if (PATH_02 == kind)
				pathName = "p02";
			if (PATH_03 == kind)
				pathName = "p03";

			list[0] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (0)").localPosition + start;
			list[1] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (1)").localPosition + start;
			list[2] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (2)").localPosition + start;
			list[3] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (3)").localPosition + start;
			list[4] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (4)").localPosition + start;
			list[5] = Single.hierarchy.Find<Transform> ("1_Paths/"+pathName+"/node (5)").localPosition + start;

			return list;
		}


		Vector3 _prev_position_ = Vector3.zero;
		public void AniUpdate_Attack_1_Random(Transform tr)
		{
			Vector3 dir = tr.localPosition - _prev_position_;
			if (0 == dir.sqrMagnitude || dir.sqrMagnitude <= float.Epsilon)
				return; //길이가 아주 작거나 0이면 각도 변화가 없는 상태이다. 

			Vector3 euler = tr.localEulerAngles;
			euler.z = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
			tr.localEulerAngles = euler;

			_prev_position_ = tr.localPosition;


			//DebugWide.LogBlue ("AniUpdate_Attack_1_Random : " + tr.localPosition + "  " + _prev_position_ + "  " );//chamto test
		}

		//ref : http://www.pixelplacement.com/itween/documentation.php
		public IEnumerator AniStart_Attack_1_Random(CharDataBundle bundle)
		{

			int rand = Single.rand.Next (1, 4);
			float time = bundle._data.GetScopeTime ();
			//float time = bundle._data.GetRunningTime();
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

			Vector3 start = bundle._ui._actions [2].transform.localPosition;
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
				,"onupdate","AniUpdate_Attack_1_Random"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);

		}



		//피해입다
		public IEnumerator EffectStart_Damaged(CharDataBundle bundle)
		{
			//DebugWide.LogBlue ("start");

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

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
			bundle._gameObject.transform.localPosition = Vector3.zero;

			iTween.ShakePosition(bundle._gameObject,new Vector3(1f,0,0), 0.5f);

			yield return new WaitForSeconds(0.5f);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.transform.localPosition = Vector3.zero;
		}

		//막다
		public IEnumerator EffectStart_Block(CharDataBundle bundle)
		{
			//DebugWide.LogBlue ("start");

			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();
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

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.transform.localEulerAngles = Vector3.zero;

			//iTween.ShakeRotation(gobj,new Vector3(0,45f,0), 0.5f);
			iTween.PunchRotation(bundle._gameObject,new Vector3(0,45f,0), 0.5f);

			yield return new WaitForSeconds(0.5f);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.transform.localEulerAngles = Vector3.zero;
		}

	}



	public struct CharDataBundle
	{
		public Character 		_data;
		public UI_CharacterCard _ui;
		public GameObject 		_gameObject;
	}

	public class UI_Battle : MonoBehaviour
	{

		private Transform _1P_start = null;
		private Transform _2P_start = null;

		private Dictionary<uint, UI_CharacterCard> _characters = new Dictionary<uint, UI_CharacterCard> ();

		public const int START_POINT_LEFT = 1;
		public const int START_POINT_RIGHT = 2;
		//=================

		
		public void Init()
		{
			//this.transform.SetParent (Single.game_root, false);

			string parentPath = Single.hierarchy.GetTransformFullPath (Single.game_root);
			_1P_start = Single.hierarchy.Find<Transform> (parentPath + "/startPoint_1");
			_2P_start = Single.hierarchy.Find<Transform> (parentPath + "/startPoint_2");
		
		}

		public UI_CharacterCard GetCharacter(uint idx)
		{
			return _characters [idx];
		}

		public UI_CharacterCard AddCharacter(Character data)
		{
			uint id = data.GetID();
			UI_CharacterCard card = UI_CharacterCard.Create ("player_"+id.ToString("00"));
			card.data = data;
			card._id = id;
			_characters.Add (id, card);

			return card;
		}



		public void SetStartPoint(uint id, float delta_x , int pointNumber)
		{
			

			UI_CharacterCard card = _characters [id];
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
			foreach (UI_CharacterCard card in _characters.Values) 
			{
				card.Update_UI ();
			}
		}

	}

	public class Effect : MonoBehaviour
	{
		//public GameObject _dst = null;

		static public void Add(GameObject dst)
		{
			Effect effect = dst.GetComponent<Effect> ();
			if (null == effect) {
				effect = dst.AddComponent<Effect> ();
				//DebugWide.LogRed (effect); //chamto test
			}

			//effect._dst = dst;
		}

		static public void FadeOut(GameObject dst , float time) 
		{
			Effect.Add (dst);

			iTween.ValueTo(dst, iTween.Hash(
				"from", 1.0f, "to", 0.0f,
				"time", time, "easetype", "linear",
				"onupdate", "SetAlpha"));

		}

		static public void FadeIn(GameObject dst , float time) 
		{
			Effect.Add (dst);

			iTween.ValueTo(dst, iTween.Hash(
				"from", 0f, "to", 1f,
				"time", time, 
				//"easetype", "linear",
				"easetype", "easeInBounce",
				"onUpdate", "SetAlpha"));

		}

		public void SetAlpha(float newAlpha) 
		{

			//DebugWide.LogBlue ("setAlpha"); //chamto test

			Color c;
			foreach (Image img in gameObject.GetComponentsInChildren<Image>(false)) 
			{
				c = img.color;
				c.a = newAlpha;
				img.color = c;
			}



		}//end setAlpha

	}//end class

}//end namespace 


