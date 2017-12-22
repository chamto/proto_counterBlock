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

		//기준원
		public struct Arc
		{
			public Vector3 pos;				//호의 시작점  
			public Vector3 dir; 			//정규화 되어야 한다
			public float degree; 			//각도 
			public float radius_near;		//시작점에서 가까운 원의 반지름 
			public float radius_far;		//시작점에서 먼 원의 반지름
			//public float radius;

			public const float STANDARD_COLLIDER_RADIUS = 2f;
			public float radius_collider_standard;	//기준이 되는 충돌원의 반지름 

			public float factor
			{
				get
				{	//f = radius / sin
					return radius_collider_standard / Mathf.Sin( Mathf.Deg2Rad * degree * 0.5f );
				}
			}

			//ratio : [-1 ~ 1]
			//호에 원이 완전히 포함 [1]
			//호에 원의 중점까지 포함 [0]
			//호에 원의 경계까지 포함 [-1 : 포함 범위 가장 넒음] 
			public const float Fully_Included = 1f;
			public const float Focus_Included = 0f;
			public const float Boundary_Included = -1f;
			public Vector3 GetPosition_Factor(float ratio = Focus_Included)
			{
				if (0 == ratio)
					return pos; 
				
				return pos + dir * (factor * ratio);
			}

			public Sphere sphere_near
			{
				get
				{ 
					Sphere sph;
					sph.pos = this.pos;
					sph.radius = this.radius_near;
					return sph;
				}

			}

			public Sphere sphere_far
			{
				get
				{ 
					Sphere sph;
					sph.pos = this.pos;
					sph.radius = this.radius_far;
					return sph;
				}

			}

			public string ToString()
			{

				return "pos: " + pos + "  dir: " + dir + "  degree: " + degree
				+ "  radius_near: " + radius_near + "  radius_far: " + radius_far + "  radius_collider_standard: " + radius_collider_standard + "  factor: " + factor;
			}
		}
		public struct Sphere
		{
			public Vector3 pos;
			public float radius;

			public Sphere(Vector3 p, float r)
			{
				pos = p;
				radius = r;
			}

			public string ToString()
			{
				return "pos: " + pos + "  radius: " + radius ;
			}
		}
	}
	public class Util
	{

		//코사인의 각도값을 비교 한다.
		//0 ~ 180도 사이만 비교 할수있다. (1,4사분면과 2,3사분면의 cos값이 같기 때문임)  
		//cosA > cosB : 1
		//cosA < cosB : 2
		//cosA == cosB : 0
		static public int Compare_CosAngle(float cos_1 , float cos_2)
		{
			//각도가 클수록 cos값은 작아진다 (0~180도 에서만 해당)
			if(cos_1 < cos_2)
				return 1;
			if (cos_1 > cos_2)
				return 2;
			
			return 0;
		}


		//호와 원의 충돌 검사 (2D 한정)
		static public bool Collision_Arc_VS_Sphere(Figure.Arc arc , Figure.Sphere sph)
		{
			//DebugWide.LogBlue ("1  srcPos" + arc.sphere_far.pos + " r:" + arc.sphere_far.radius + " dstPos:" + sph.pos + " r:" + sph.radius); //chamto test
			if (true == Util.Collision_Sphere (arc.sphere_far, sph, eSphere_Include_Status.Focus)) 
			{
				
				if (false == Util.Collision_Sphere (arc.sphere_near, sph, eSphere_Include_Status.Focus)) 
				{
					//각도를 반으로 줄여 넣는다. 1과 4분면을 구별 못하기 때문에 1사분면에서 검사하면 4사분면도 검사 결과에 포함된다. 즉 실제 검사 범위가 2배가 된다.
					float angle_arc = Mathf.Cos(arc.degree * 0.5f * Mathf.Deg2Rad);

					//DebugWide.LogBlue ( Mathf.Acos(angle_arc) * Mathf.Rad2Deg + " [arc] " + arc.ToString() + "   [sph] " + sph.ToString());//chamto test

					Vector3 arc_sph_dir = sph.pos - arc.GetPosition_Factor (Figure.Arc.Focus_Included);
					arc_sph_dir.Normalize (); //노멀값 구하지 않는 계산식을 찾지 못했다. 

					float rate_cos = Vector3.Dot (arc.dir, arc_sph_dir);
					if(rate_cos > angle_arc) 
					{  
						return true;
					}
				}
					
			}
			 
			return false;
		}


		public enum eSphere_Include_Status
		{
			Boundary = 1,	//두원의 닿는 경계까지 포함 
			Focus,			//작은원이 중점까지 포함
			Fully			//작은원이 완전포함 
		}
		//ratio : 충돌민감도 설정 , 기본 1f , 민감도올리기 1f 보다작은값 , 민감도낮추기 1f 보다큰값  
		static public bool Collision_Sphere(Figure.Sphere src , Figure.Sphere dst , eSphere_Include_Status eInclude ,float ratio = 1f)
		{
			
			float min_radius , max_radius , sum_radius , sqr_standard_value;
			if (src.radius > dst.radius) 
			{
				min_radius = dst.radius;
				max_radius = src.radius;
			} else 
			{
				min_radius = src.radius;
				max_radius = dst.radius;
			}

			//(src.r - dst.r) < src.r  < (src.r + dst.r)
			//완전포함 		  < 중점포함  < 경계포함
			//Fully           < Focus   < Boundary
			const float Minimum_Error_Value  = 1.0f; //최소오차값
			switch (eInclude) 
			{
			case eSphere_Include_Status.Fully:
				sum_radius = max_radius - min_radius;
				if (Minimum_Error_Value > sum_radius) //반지름 합값이 너무 작으면 판정을 못하므로 임의의 "최소오차값"을 할당해 준다.
					sum_radius = Minimum_Error_Value;
				break;
			case eSphere_Include_Status.Focus:
				sum_radius = max_radius;
				break;
			case eSphere_Include_Status.Boundary:
			default:
				//두원의 반지름을 더한후 제곱해 준다. 
				sum_radius = max_radius + min_radius;
				break;
			}
				
			sqr_standard_value = (sum_radius * sum_radius) * ratio;

			//두원의 중점 사이의 거리를 구한다. 피타고라스의 정리를 이용 , 제곱을 벗기지 않는다.
			float sqr_dis_between = Vector3.SqrMagnitude(src.pos - dst.pos);

			//DebugWide.LogBlue (" r+r: "+Mathf.Sqrt(sqr_standard_value) + " p-p: " + Mathf.Sqrt(sqr_dis_between));

			if (sqr_standard_value > sqr_dis_between)
			{
//				DebugWide.LogGreen ("T-----  include: " + eInclude.ToString() + "  std: "+Mathf.Sqrt(sqr_standard_value) + "   dis: " + Mathf.Sqrt(sqr_dis_between)
//					+ "  srcPos: "+src.pos + "   dstPos: "+ dst.pos); //chamto test
				return true; //두원이 겹쳐짐 
			}
			if (sqr_standard_value == sqr_dis_between)
			{
//				DebugWide.LogGreen ("T-----  include: " + eInclude.ToString() + "  std: "+Mathf.Sqrt(sqr_standard_value) + "   dis: " + Mathf.Sqrt(sqr_dis_between)
//					+ "  srcPos: "+src.pos + "   dstPos: "+ dst.pos); //chamto test
				return true; //포함조건과 똑같음
			}
			if (sqr_standard_value < sqr_dis_between)
			{
//				DebugWide.LogBlue ("F-----  include: " + eInclude.ToString() + "  std: "+Mathf.Sqrt(sqr_standard_value) + "   dis: " + Mathf.Sqrt(sqr_dis_between)
//					+ "  srcPos: "+src.pos + "   dstPos: "+ dst.pos); //chamto test
				return false; //두원이 겹쳐 있지 않음
			}

//			DebugWide.LogWhite ("***** unreachable !!! ******");
			return false;
		}

		static public bool Collision_Sphere(Vector3 src_pos , float src_radius , Vector3 des_pos , float des_radius , eSphere_Include_Status eInclude)
		{
			Figure.Sphere src, dst;
			src.pos = src_pos; src.radius = src_radius;
			dst.pos = des_pos; dst.radius = des_radius;
			return Util.Collision_Sphere (src, dst, eInclude);
		}	

		//!!test 필요
		//value 보다 target 값이 작으면 True 를 반환한다.
		static public bool Distance_LessThan(float Value , Vector3 target )
		{
			if(Mathf.Exp(Value * 0.5f) >=  Vector3.SqrMagnitude( target ))
			{
				return true;
			}

			return false;
		}



		//ref : https://stackoverflow.com/questions/27237776/convert-int-bits-to-float-bits
		//int i = ...;
		//float f = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
		public static unsafe int SingleToInt32Bits(float value) {
			return *(int*)(&value);
		}
		public static unsafe float Int32BitsToSingle(int value) {
			return *(float*)(&value);
		}

		//ref : https://ko.wikipedia.org/wiki/%EA%B3%A0%EC%86%8D_%EC%97%AD_%EC%A0%9C%EA%B3%B1%EA%B7%BC
		//함수해석 : https://zariski.wordpress.com/2014/10/29/%EC%A0%9C%EA%B3%B1%EA%B7%BC-%EC%97%AD%EC%88%98%EC%99%80-%EB%A7%88%EB%B2%95%EC%9D%98-%EC%88%98-0x5f3759df/
		//함수해석 : http://eastroot1590.tistory.com/entry/%EC%A0%9C%EA%B3%B1%EA%B7%BC%EC%9D%98-%EC%97%AD%EC%88%98-%EA%B3%84%EC%82%B0%EB%B2%95
		//뉴턴-랩슨법  : http://darkpgmr.tistory.com/58
		//sse 명령어 rsqrtss 보다 빠를수가 없다.
		//[ reciprocal square root ]
		// 제곱근의 역수이다. a 일때  역수는 1/a 이다.
		static public unsafe float RSqrt_Quick_2( float x )
		{

			const int SQRT_MAGIC_F  = 0x5f3759df;
			const float threehalfs = 1.5F;
			float xhalf = 0.5f * x;

			float ux;
			int ui;

			ui = * ( int * ) &x;
			ui = SQRT_MAGIC_F - (ui >> 1);  // gives initial guess y0
			ux  = * ( float * ) &ui;
			ux = ux * (threehalfs - xhalf * ux * ux);// Newton step, repeating increases accuracy 

			return ux;
		}


		//Algorithm: The Magic Number (Quake 3)
		static public unsafe float  Sqrt_Quick_2(float x)
		{
			const int SQRT_MAGIC_F  = 0x5f3759df;
			const float threehalfs = 1.5F;
			float xhalf = 0.5f * x;

			float ux;
			int ui;

			ui = * ( int * ) &x;
			ui = SQRT_MAGIC_F - (ui >> 1);  // gives initial guess y0
			ux  = * ( float * ) &ui;
			ux = x * ux * (threehalfs - xhalf * ux * ux);// Newton step, repeating increases accuracy 

			return ux;
		}

		//어셈코드 다음으로 속도가 가장 빠름. 매직넘버 "0x5f3759df" 코드보다 빠르지만 정확도는 더 떨어진다
		//ref : https://www.codeproject.com/Articles/69941/Best-Square-Root-Method-Algorithm-Function-Precisi
		//Algorithm: Dependant on IEEE representation and only works for 32 bits 
		static public unsafe float Sqrt_Quick_7(float x)
		{
			
			uint i = *(uint*) &x; 
			// adjust bias
			i  += 127 << 23;
			// approximation of square root
			i >>= 1; 
			return *(float*) &i;
		}   

	
		static public Vector3 Norm_Quick(Vector3 v3)
		{
			//float r_length = Util.RSqrt_Quick_2 (v3.sqrMagnitude);
			float r_length = 1f / Util.Sqrt_Quick_7 (v3.sqrMagnitude);
			return v3 * r_length;
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
		//1
		public float cloggedTime_0;	//막히는 시간 : 0(시작) , 1(끝)  
		public float cloggedTime_1;		
		//2
		public float eventTime_0;	//동작 유효 범위 : 0(시작) , 1(끝)  
		public float eventTime_1;
		//3
		public float openTime_0; 	//다음 동작 연결시간 : 0(시작) , 1(끝)  
		public float openTime_1; 
		//4
		public float rigidTime;		//동작 완료후 경직 시간



		//무기 움직임 정보 
		public eTraceShape 	attack_shape; 		//공격모양 : 종 , 횡 , 찌르기 , 던지기
		//	=== 범위형 움직임 === : 종,횡,찌르기,던지기 (무기 위치가 기준이 되는 범위이다)
		public float 		plus_range_0;		//더해지는 범위 최소 
		public float 		plus_range_1;		//더해지는 범위 최대
		public float		angle;				//범위 각도
		//  === 이동형 움직임 === : 찌르기,던지기
		public float 		distance_travel;	//공격점까지 이동 거리 : 상대방까지의 직선거리 , 근사치 , 판단을 위한 값임 , 정확한 충돌검사용 값이 아님.
		public float 		distance_maxTime;  	//최대거리가 되는 시간 : 공격점에 도달하는 시간
		public float 		velocity_before;	//공격점 전 속도 
		public float 		velocity_after;		//공격점 후 속도 	


		public  Behavior()
		{
			
			runningTime = 0f;
			eventTime_0 = 0f;
			eventTime_1 = 0f;
			rigidTime = 0f;
			openTime_0 = 0f;
			openTime_1 = 0f;
			cloggedTime_0 = 0f;
			cloggedTime_1 = 0f;
			plus_range_0 = 0f;
			plus_range_1 = 0f;
			angle = 45f; //chamto test
			//angle = 0f;
			attack_shape = eTraceShape.None;
			//distance_travel = DEFAULT_DISTANCE;
			distance_travel = 0f;
			distance_maxTime = 0f;
			velocity_before = 0f;
			velocity_after = 0f;

		}

		public Behavior Clone()
		{
			return this.MemberwiseClone() as Behavior;
		}

		public void Calc_Velocity()
		{
			//t * s = d
			//s = d/t
			if (0f == distance_maxTime)
				this.velocity_before = 0f;
			else
				this.velocity_before = distance_travel / distance_maxTime;
			
			this.velocity_after = distance_travel / (runningTime - distance_maxTime);

			DebugWide.LogBlue ("velocity_before : "+this.velocity_before + "   <-- 충돌점 -->   velocity_after : " +this.velocity_after + "  [distance_travel:" + distance_travel+"]"); //chamto test
		}

		public float CurrentDistance(float currentTime)
		{
			//* 러닝타임 보다 더 큰 값이 들어오면 사용오류임
			if (runningTime < currentTime)
				return 0f; 

			//* 최대거리에 도달하는 시간이 0이면 최대거리를 반환한다.
			if (0f == distance_maxTime) 
			{
				return distance_travel;
			}

			//1. 전진
			if (currentTime <= distance_maxTime) 
			{
				return this.velocity_before * currentTime;
			}

			//2. 후진
			//if(distance_maxTime < currentTime)
			return  this.velocity_after * (runningTime - currentTime);
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
		public float 	collider_sphere_radius; //무기의 반지름

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
			case Character.eSubState.Start:
				return "Valid_Start";
			case Character.eSubState.Running:
				return "Valid_Running";
			case Character.eSubState.End:
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
		private Figure.Sphere _collider;

		//숙련정보 
		// - 정확도 : 정확도가 낮으면 맞지 않는 공격비율이 올라간다 (마구 휘두르기 구현시 필요)
		//뒤에서 공격 당할시 피해가 커진다
		//시야정보

		//무기정보 
		private Weapon 	_weapon;

		//동작정보
		private Behavior _behavior = null;
		private Skill 	_skill_current = null;
		private Skill 	_skill_next = null;
		private float 	_timeDelta = 0f; 	//시간변화량

		//상태정보
		private eState 	_state_current = eState.None;
		private eSubState _eventState_current = eSubState.None; 	//유효상태
		//private eSubState _giveState_current = eSubState.None; 		//준상태
		//private eSubState _judgmentState_current = eSubState.None; 	//판정시작 상태  

		//판정
		private Judgment _judgment = new Judgment();

		//공격대상
		private Character _target = null;

		//====================================

		public Character()
		{
			_hp_current = 10;
			_hp_max = 10;
			_position = Vector3.zero;
			_direction = Vector3.right;
			_kind = eKind.Biking;
			_collider.pos = Vector3.zero;
			_collider.radius = 2f;

			_weapon.Default_Sword ();

			//_collider_sphere_radius = 2f;  //임시로 넣어둔값


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

		public Character CurrentTarget()
		{
			return _target;
		}

		public Judgment GetJudgment()
		{
			return _judgment;
		}

		public Figure.Arc GetArc_Weapon()
		{
			Figure.Arc arc = new Figure.Arc();
			arc.radius_near = this.GetRangeMin();
			arc.radius_far = this.GetRangeMax ();
			arc.pos = _position;
			arc.dir = _direction;
			arc.degree = _behavior.angle;
			arc.radius_collider_standard = Figure.Arc.STANDARD_COLLIDER_RADIUS; //임시처리임

			return arc;
		}

		public Figure.Sphere GetCollider_Sphere()
		{
			return _collider;
		}

		public Figure.Sphere GetWeaponCollider_Sphere()
		{
			Figure.Sphere sph = new Figure.Sphere ();
			sph.pos = this.GetWeaponPosition ();
			sph.radius = this.weapon.collider_sphere_radius;
			return sph;

		}


		public float GetWeaponDistance()
		{
			return _behavior.CurrentDistance (_timeDelta);
		}

		public Vector3 GetWeaponPosition(float time)
		{
			//DebugWide.LogBlue (_behavior.CurrentDistance (time) * _direction); //chamto test
			return _position + (_behavior.CurrentDistance(time) * _direction);
		}

		public Vector3 GetWeaponPosition()
		{
			return this.GetWeaponPosition (_timeDelta);

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
			_collider.pos = pos;
		}

		public Vector3 GetDirection()
		{
			return _direction;
		}

		public void SetDirection(Vector3 dir)
		{
			_direction = dir;
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

		public eSubState CurrentEventState()
		{
			return _eventState_current;
		}

//		public eSubState CurrentGiveState()
//		{
//			return _giveState_current;
//		}

		public void SetState(eState setState)
		{
			_state_current = setState;
		}

		public void SetEventState(eSubState setSubState)
		{
			_eventState_current = setSubState;
		}

//		public void SetGiveState(eSubState setSubState)
//		{
//			_giveState_current = setSubState;
//		}

//		public void SetSendState(eSubState setSubState)
//		{
//			_judgmentState_current = setSubState;
//		}

		public float GetRangeMin()
		{
			return _behavior.plus_range_0 + _weapon.attack_range_min;
		}

		public float GetRangeMax()
		{
			return _behavior.plus_range_1 + _weapon.attack_range_max;
		}

//		public Skill.eName CurrentSkillKind()
//		{
//			if(null != _skill_current)
//				return _skill_current.name;
//
//			return Skill.eName.None;
//		}

		public Skill CurrentSkill()
		{
			return _skill_current;
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


//		public Judgment.eState GetJudgmentState()
//		{
//			return _judgment.state_current;
//		}
//		public void SetJudgmentState(Judgment.eState state)
//		{
//			
//			this._judgment.state_current = state;
//		}



		public float GetRunningTime()
		{
			return _behavior.runningTime;
		}

		public float GetEventTime_Interval()
		{
			return _behavior.eventTime_1 - _behavior.eventTime_0;
		}

		public float GetOpenTime_Interval()
		{
			return _behavior.openTime_1 - _behavior.openTime_0;
		}

		public bool Valid_EventTime()
		{

			if (eState.Start == _state_current || eState.Running == _state_current) 
			{
				if (_behavior.eventTime_0 <= _timeDelta && _timeDelta <= _behavior.eventTime_1)
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

		public bool IsSkill_Unprotected()
		{
			if (Skill.eKind.None == _skill_current.kind || Skill.eKind.Hit == _skill_current.kind || 
				Skill.eKind.Withstand == _skill_current.kind) //1대1에서 제3자 끼어듬
				return true;

			return false;
		}

		public bool IsSkill_Attack()
		{
			if (Skill.eKind.Attack_Strong == _skill_current.kind || Skill.eKind.Attack_Weak == _skill_current.kind ||
				Skill.eKind.Attack_Counter == _skill_current.kind)
				return true;

			return false;
		}

		public bool IsSkill_Block()
		{
			if (Skill.eKind.Block == _skill_current.kind)
				return true;

			return false;
		}


		public bool IsSkill_HitWeapon()
		{
			if (Skill.eName.Hit_Weapon == _skill_current.name)
				return true;

			return false;
		}

		//[사용제한] 입력되는 스킬로 바로 적용한다  - (End 상태를 거치지 않는다)
		private void setSkill_None(Skill.eName name)
		{
			//_skill_next = null;
			_skill_current = ref_skillBook.Refer(name);
			_behavior = _skill_current.FirstBehavior ();

			//SetState (eState.Start);
			SetState (eState.None);
			SetEventState (eSubState.None);
			//SetGiveState (eSubState.None);
			//SetReceiveState (eSubState.None);

			this._timeDelta = 0f; //판정후 갱신되는 구조로 인해, 갱신되지 않은 상태에서 판정하는 문제 발생. => 스킬요청시 바로 초기화 시켜준다.  
		}
			

		public void SetSkill_AfterInterruption(Skill.eName name)
		{
			this.SetSkill_AfterInterruption(ref_skillBook.Refer(name));
		}

		//현재 스킬 중단후 다음스킬 시작 (현재 스킬을 end 상태로 바로 전환한다)  
		public void SetSkill_AfterInterruption(Skill skill)
		{
			//현재 스킬이 지정되어 있지 않으면 바로 요청 스킬로 지정한다
			//현재 상태가 end라면 스킬을 바로 지정한다
			if (null == _skill_current || eState.End == this._state_current) 
			{
				this.SetSkill_Start (skill);
				return;
			}

			_skill_next = skill;

			SetState (eState.End);
		}

		public void SetSkill_Start(Skill.eName name)
		{
			this.SetSkill_Start (ref_skillBook.Refer(name));
		}

		public void SetSkill_Start(Skill skill)
		{
			_skill_current = skill;
			_behavior = _skill_current.FirstBehavior ();
			SetState (eState.Start);
			this._timeDelta = 0f;
		}

		public void Attack_Strong ()
		{
			if (Skill.eKind.Withstand == _skill_current.kind) 
			{	//칼견디기 상태일때 칼밀기를 실행한다	
				this.Attack_SwordPush ();
			} else 
			{
				if (Skill.eName.Idle == _skill_current.name || true == this.Valid_OpenTime ()) 
				{
					//DebugWide.LogBlue ("Attack_Strong - " + this.Valid_OpenTime() + "  " + _skill_current.name); //chamto test

					//아이들상태거나 연결시간안에 행동이 들어온 경우
					SetSkill_AfterInterruption (Skill.eName.Attack_Strong_1);
				}

			}

		}

		//칼밀기 - 칼버티기시 실행된다
		public void Attack_SwordPush()
		{
			//DebugWide.LogBlue ("Attack_SwordPush !!! "); //chamto test
			this.GetBehavior ().distance_travel += 1f; //임시
		}

		public void Attack_Weak ()
		{
			//카운터스킬을 쓸수있는 조건
			//상대무기 공격을 성공한 경우 연결시간 조건과 상광없이 실행되게 한다
			if (Skill.eName.Hit_Weapon == this.CurrentTarget ().CurrentSkill ().name) 
			{
				//DebugWide.LogBlue ("denide  "+ this.GetTimeDelta() + "  " + this.CurrentSkill().name);
				//일정시간 뒤에 실행가능함
				if (Skill.eKind.Attack_Counter != this.CurrentSkill().kind && this.GetTimeDelta () > 0.5f) 
				{
					Attack_Counter ();
				}

			} 
			else 
			{
				
				if (Skill.eKind.Attack_Counter == this.CurrentTarget ().CurrentSkill ().kind && this.GetTimeDelta () > 0.3f) 
				{	//상대가 카운터 스킬을 발동했으면 연결시간과 상관없이 약공격을 쓸수있다
					SetSkill_AfterInterruption (Skill.eName.Attack_Weak_1);
				}
				else if (Skill.eName.Idle == this.CurrentSkill().name || true == this.Valid_OpenTime () )
				{	//일반스킬 사용조건
					
					//DebugWide.LogBlue ("Attack_Weak - " + this.Valid_OpenTime() + "  " + _skill_current.name); //chamto test
					SetSkill_AfterInterruption (Skill.eName.Attack_Weak_1);
				}
					
			}


		}

		//카운터공격 
		public void Attack_Counter()
		{
			SetSkill_AfterInterruption (Skill.eName.Attack_Counter_1);
		}
			

		public void Block()
		{
			if (Skill.eName.Idle == _skill_current.name || true == this.Valid_OpenTime ()) 
			{
				//아이들상태거나 연결시간안에 행동이 들어온 경우
				SetSkill_AfterInterruption (Skill.eName.Block_1);

				//DebugWide.LogBlue ("succeced!!! "); //chamto test
			}

		}


		public void Idle()
		{
			SetSkill_AfterInterruption (Skill.eName.Idle);
		}

		//상대로 부터 피해입다
		public void BeDamage(int damage)
		{
			this.AddHP (damage);

			SetSkill_AfterInterruption (Skill.eName.Hit_Body);
		}


//		public bool IsStart_AttackSucced()
//		{
//			if (Judgment.eState.Attack_Succeed == this.GetJudgmentState () &&
//			   eSubState.Start == CurrentGiveState ())
//				return true;
//
//			return false;
//		}
//
//		public bool IsStart_BlockSucceed()
//		{
//			if (Judgment.eState.Block_Succeed == this.GetJudgmentState () &&
//				eSubState.Start == CurrentGiveState ())
//				return true;
//
//			return false;
//		}
//
//		public bool IsStart_Damaged()
//		{
//			if (Judgment.eState.Damaged == this.GetJudgmentState () &&
//				eSubState.Start == _judgmentState_current )
//				return true;
//
//			return false;
//		}

		public bool IsAttackStart_Withstand(Character dst)
		{
			if(true == Util.Collision_Sphere (this.GetWeaponCollider_Sphere(), dst.GetWeaponCollider_Sphere(),
				Util.eSphere_Include_Status.Fully)) 
			{
				return true;
			}

			return false;
		}

		//칼죽이기 가능한 거리인가?
		// *A방식* 내무기 위치와 상대방 무기위치로 판단한다.
		// *B방식* 내 칼죽이기 범위와 상대방 무기위치로 판단한다. (최대 공격범위로만 판단한다)
		//의도 : 정확한 충돌처리를 위한 것이 아니다. 직선거리로 판정을 하기 위함이다.  
		public bool IsPossibleRange_Clog_VS(Character dst)
		{

			//1.들어온 무기 방향 검사 : 내앞에서 들어왔는가? 내뒤에서 들어왔는가?
			//2.내무기 범위 각도 검사 : 부채꼴 - 나중에 구현 

			//정면 180안에 상대가 있을 경우만 공격가능
			//=======================================================================
			Vector3 toDst = dst.GetPosition() - this.GetPosition();
			float cos = Vector3.Dot (this.GetDirection(), toDst);
			if(cos < 0) 
			{  
				return false;
			} 
			//=======================================================================

			//*B방식*
			if(true == Util.Collision_Sphere (this._position, this.GetRangeMax(), dst.GetWeaponPosition(), dst.weapon.collider_sphere_radius, 
				Util.eSphere_Include_Status.Focus)) 
			{
				//DebugWide.LogBlue ("Judgment.eState.Attack_Withstand !!!!"); //chamto test
				return true;
			}


			return false;
		}

		//공격이 상대방에 맞았나?
		//* 내무기 범위 또는 위치로 상대방 위치로 판단한다.
		//!!! 무기 범위가 방향성이 없다.  뒤나 앞이나 판정이 같다
		public bool Collision_Weaphon_Attack_VS(Character dst)
		{
//			if(_id == 1)
//				DebugWide.LogYellow ("****************Collision_Weaphon_Attack_VS**************");//chamto test
			
			switch (this._behavior.attack_shape) 
			{
			case eTraceShape.Horizon: //todo : 추후 필요시 구현
			case eTraceShape.Vertical:
				{	//***** 내무기 범위 vs 상대방 위치 *****
					
					return Util.Collision_Arc_VS_Sphere (this.GetArc_Weapon (), dst.GetCollider_Sphere());

				}
				//break;
			case eTraceShape.Straight:
				{	//***** 내무기 위치 vs 상대방 위치 *****


					//fixme : 원과 반직선 충돌 처리로 변경하는게 더 낫다. 현재 처리로는 부족하다.
					//정면 5도안에 상대가 있을 경우만 공격가능
					//=======================================================================
					const float ANGLE_SCOPE = 10f;
					//각도를 2로 나누는 이유 : 1,4사분면 부호가 같기 때문에 둘을 구별 할 수 없다. 의도와 다르게 2배 영역이 된다.
					float angle = Mathf.Cos(ANGLE_SCOPE * 0.5f * Mathf.Deg2Rad);
					Vector3 toDst = dst.GetPosition() - this.GetPosition();
					toDst.Normalize (); 
					float cos = Vector3.Dot (this.GetDirection(), toDst);
					if(2 == Util.Compare_CosAngle(angle, cos)) //angle 보다 cos이 작아야 함
					{  
						DebugWide.LogBlue ("std angle: " + angle + "   dst angle: " + Mathf.Acos(cos) * Mathf.Rad2Deg); //chamto test
						return false;	
					} 
					//=======================================================================

					if (true == Util.Collision_Sphere (new Figure.Sphere(this.GetWeaponPosition(), this.weapon.collider_sphere_radius), 
						dst.GetCollider_Sphere(),
						Util.eSphere_Include_Status.Focus)) 
					{
						return true;
					}

				}
				break;
			}

			return false;
		}

		public bool Collision_Weaphon_Withstand_VS(Character dst)
		{
			if (true == Util.Collision_Sphere (new Figure.Sphere(this.GetWeaponPosition(), this.weapon.collider_sphere_radius), 
				dst.GetCollider_Sphere(),
				Util.eSphere_Include_Status.Boundary,0.5f)) 
			{
				return true;
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
				
				if (true == this.Valid_EventTime () && false == dst.Valid_EventTime() 
					&& true == this.Collision_Weaphon_Attack_VS(dst)) 
				{	//먼저공격 나
					jState = Judgment.eState.Attack_Succeed;
				}
				if (false == this.Valid_EventTime () && true == dst.Valid_EventTime()
					&& true == dst.Collision_Weaphon_Attack_VS(this)) 
				{	//먼저공격 상대
					jState = Judgment.eState.Damaged;
				}
				else if ( Skill.eKind.Attack_Weak ==  this.CurrentSkill().kind &&
					eState.Start == this.CurrentState() &&
					true == dst.Valid_CloggedTime () &&
					this.IsPossibleRange_Clog_VS(dst)) 
				{	//칼죽이기
					jState = Judgment.eState.Attack_Weapon;	
				}
				else if (Skill.eKind.Attack_Weak ==  dst.CurrentSkill().kind &&
					eState.Start == dst.CurrentState() && 
					true == this.Valid_CloggedTime () &&
					dst.IsPossibleRange_Clog_VS(this)) 
				{	//칼죽이기 당함  
					jState = Judgment.eState.Attack_Clogged;
				}
				else if (Skill.eKind.Attack_Strong ==  this.CurrentSkill().kind  && Skill.eKind.Attack_Strong ==  dst.CurrentSkill().kind &&
					true == this.Valid_CloggedTime () && true == dst.Valid_CloggedTime() &&
					this.IsAttackStart_Withstand(dst)) 
				{	//칼맞부딪힘 
					jState = Judgment.eState.Attack_Withstand;
				}

//				if(eState.Start == this.CurrentState())
//					DebugWide.LogBlue (this.IsSkill_Attack ()+ "  " + jState.ToString());

			}

			//칼버티기 상태일때
			if (Skill.eKind.Withstand == this.CurrentSkill ().kind && Skill.eKind.Withstand == dst.CurrentSkill ().kind) 
			{
				if (true == this.Collision_Weaphon_Withstand_VS(dst)) 
				{	//먼저공격 나
					jState = Judgment.eState.Attack_Succeed;
				}
				if (true == dst.Collision_Weaphon_Withstand_VS(this)) 
				{	//먼저공격 상대
					jState = Judgment.eState.Damaged;
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
					if (true == this.Valid_EventTime () && true == dst.Valid_EventTime ()) 
					{
						jState = Judgment.eState.Attack_Blocked;
					}
					if (true == this.Valid_EventTime () && false == dst.Valid_EventTime ()) 
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
					if (true == this.Valid_EventTime () && true == dst.Valid_EventTime ()) 
					{
						jState = Judgment.eState.Block_Succeed;
					}
					if (false == this.Valid_EventTime () && true == dst.Valid_EventTime ()) 
					{
						jState = Judgment.eState.Damaged;
					}
				}

			}
			//============================
			//Attack_Vs_None
			//============================
			if (true == this.IsSkill_Attack () && true == dst.IsSkill_Unprotected ()) 
			{
				if (true == this.Collision_Weaphon_Attack_VS (dst)) 
				{
					if (true == this.Valid_EventTime () ) 
					{
						jState = Judgment.eState.Attack_Succeed;
					}
				}

			}
			//============================
			//None_Vs_Attack
			//============================
			if (true == this.IsSkill_Unprotected () && true == dst.IsSkill_Attack ()) 
			{
				if (true == dst.Collision_Weaphon_Attack_VS (this)) 
				{
					if (true == dst.Valid_EventTime () ) 
					{
						jState = Judgment.eState.Damaged;
					}
				}

			}

			this.GetJudgment ().SetState_Current (jState);
			//this.SetJudgmentState (jState);
			//----------------------------------
//			if (1 == _id && this.IsSkill_Attack ()) 
//			{
//				if(this.Valid_EventTime())
//				{
//					DebugWide.LogGreen (" id: "+ _id + "  dst_skill: " +dst.CurrentSkill().name + "  isEventTime: " + this.Valid_EventTime() + 
//						"  weaponPos: " + this.GetWeaponPosition() + "  state: "+jState.ToString() + "  timeDelta: "+this.GetTimeDelta()); //chamto test
//				}
//				else
//				{
//					DebugWide.LogBlue (" id: "+ _id + "  dst_skill: " +dst.CurrentSkill().name + "  isEventTime: " + this.Valid_EventTime() + 
//						"  weaponPos: " + this.GetWeaponPosition() + "  state: "+jState.ToString() + "  timeDelta: "+this.GetTimeDelta()); //chamto test		
//				}
//			}

//			if(this.Valid_EventTime() && this.IsSkill_Attack())
//				DebugWide.LogBlue (" id: "+ _id + "  isAttack: " +this.IsSkill_Attack ()+ "  isEventTime: " + this.Valid_EventTime() + 
//					"  weaponPos: " + this.GetWeaponPosition() + "  state: "+jState.ToString()); //chamto test
		}

		public void Judge(Character dst)
		{
			//============================
			//!! 판정에서 대상객체 정보를 넣는다
			this._target = dst;
			//============================

			//if(1 == this.GetID() &&  Judgment.eState.None != _judgment.state_current)
			//	DebugWide.LogBlue ("[0:Judge : "+this.GetID() + "  !!!  "+_judgment.state_current + "  " + CurrentGiveState ()); //chamto test

			switch (this.GetJudgment().GetState_Current()) 
			{
			case Judgment.eState.Attack_Succeed: //1 vs n
				{
//					DebugWide.LogRed ("**********;*********************;***********");
//					DebugWide.LogRed ("*********************" +this.GetID() + "  !!!  " + this.CurrentSkill().name + "***********************"); //chamto test


					//한동작에서 일어난 사건
					if (this.GetJudgment().IsStart()) 
					{
						//여러명에게 공격받았을 때의 처리 문제로, 상대의 피해함수를 호출해 준다
						//apply jugement : HP
						dst.BeDamage (-1);


						//칼버티기에서 공격성공함
						//start 서브상태에 안들어와 있으면 None 서브상태에서 먼저 수행되어, BeDamage 함수를 호출할수 없게 된다 
						if (Skill.eKind.Withstand == this.CurrentSkill ().kind )
						{
							this.Idle();
						}
					}
						
				}
				break;
			case Judgment.eState.Block_Succeed:
				{
				}
				break;
			case Judgment.eState.Damaged:
				{
				}
				break;
			
			case Judgment.eState.Attack_Withstand: //1 vs 1
				{
					//무기 위치값을 정지시킴
					//해당 3초간 공격키 연타 - 칼밀기 행동함
					//칼밀기를 많이 하여 칼이 상대몸에 도달하면 공격성공이 됨 
					float prev_distance = this.GetWeaponDistance (); 
					//Skill nextSkill = Skill.Details_Withstand_1 ();
					Skill nextSkill = ref_skillBook.Create(Skill.eName.Withstand_1);
					this.SetSkill_AfterInterruption (nextSkill);
					nextSkill.FirstBehavior ().distance_travel = prev_distance; //정지 거리를 넣어준다

					//DebugWide.LogBlue ("sdss");
				}
				break;
			case Judgment.eState.Attack_Weapon: //1 vs 1
				{

					//상대행동을 "Hit_Weapon"로 변경시킨다
					//Hit_Weapon 행동 : 1초간 무기 정지 
					float prev_distance = dst.GetWeaponDistance (); 
					//Skill nextSkill = Skill.Details_HitWeapon();
					Skill nextSkill = ref_skillBook.Create(Skill.eName.Hit_Weapon);
					dst.SetSkill_AfterInterruption (nextSkill);
					nextSkill.FirstBehavior ().distance_travel = prev_distance; //정지 거리를 넣어준다

					//DebugWide.LogBlue ("Attack_Weapon : " + dst.GetBehavior ().distance_travel);

				}
				break;
			}//end switch


			if (Skill.eKind.Withstand == this.CurrentSkill ().kind)
			{
				//양쪽의 현재스킬이 칼버티기 상태일때	
				if (Skill.eKind.Withstand == dst.CurrentSkill ().kind) 
				{
					const float MIN_LENGTH = 2f;
					float sqrLength = (this.GetWeaponPosition () - dst.GetWeaponPosition ()).sqrMagnitude;
					if (MIN_LENGTH * MIN_LENGTH > sqrLength) 
					{	//칼이 붙어 있을때 떨어뜨린다 
						this.GetBehavior ().distance_travel -= 0.2f; //임시
						dst.GetBehavior ().distance_travel -= 0.2f; //임시
					}
				}

			}

		}//end func


		public void Update()
		{
			//this._timeDelta += Time.deltaTime;
			this._timeDelta += FrameControl.DeltaTime();
			
			switch (this._state_current) 
			{
			case eState.None:
				{
					//===== 처리철차 ===== 
					//입력 -> ui갱신 -> (갱신 -> 판정)
					//공격키입력 -> 행동상태none 에서 start 로 변경 -> start 상태 검출
					//* 공격키입력으로 시작되는 상태는 None 이 되어야 한다. (바로 Start 상태가 되면 판정에서 Start상태인지 모른다)
					//* 상태변이에 의해 시작되는 상태는 Start 여야 한다. (None 으로 시작되면 한프레임을 더 수행하는게 되므로 Start로 시작하게 한다)
					this._timeDelta = 0f;
					SetState (eState.Start);
				}
				break;
			case eState.Start:
				{	
					this._timeDelta = 0f;
					SetState (eState.Running);
					SetEventState (eSubState.None);

					//DebugWide.LogRed ("[0: "+this._state_current);//chamto test
				}
				break;
			case eState.Running:
				{

					//====================================================
					// update sub_state 
					//====================================================



					switch (_eventState_current) 
					{
					case eSubState.None:
						if (_behavior.eventTime_0 <= _timeDelta && _timeDelta <= _behavior.eventTime_1) {
							this.SetEventState (eSubState.Start);
						}
						break;
					case eSubState.Start:
						this.SetEventState (eSubState.Running);
						break;
					case eSubState.Running:
						if (!(_behavior.eventTime_0 <= _timeDelta && _timeDelta < _behavior.eventTime_1)) {
							this.SetEventState (eSubState.End);
						}

						break;
					case eSubState.End:
						this.SetEventState (eSubState.None);
						break;

					}

					//if(1 == this.GetID())
					//	DebugWide.LogBlue ("[1:_validState_current : "+_validState_current );//chamto test

					//====================================================
					// 실제 공격/방어 한 범위
					//====================================================
//					switch (_giveState_current) 
//					{
//					case eSubState.None:
//						{
////							if (Judgment.eState.Attack_Succeed == this.GetJudgmentState () ||
////								Judgment.eState.Block_Succeed == this.GetJudgmentState()) 
////							{
////								this.SetGiveState (eSubState.Start);
////							}
//						}
//						break;
//					case eSubState.Start:
//						{
//							//DebugWide.LogBlue ("[0:Judge : "+this.GetID() + "  !!!  "+_judgment.state_current + "  " + _skill_current.name); //chamto test
//							this.SetGiveState (eSubState.Running);
//						}
//						break;
//					case eSubState.Running:
//						{
//							if (Judgment.eState.Attack_Succeed != this.GetJudgmentState () &&
//							    Judgment.eState.Block_Succeed != this.GetJudgmentState ()) 
//							{
//								this.SetGiveState (eSubState.End);
//							}
//						}
//						break;
//					case eSubState.End:
//						{
//							this.SetGiveState (eSubState.None);
//						}
//						break;
//					}

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
					//* 다음 스킬입력 처리  
					if (null != _skill_next) 
					{
						//DebugWide.LogBlue ("next : " + _skill_next.name);
						SetSkill_Start (_skill_next);
						_skill_next = null;
					} else 
					{
						//** 콤보 스킬 처리
						_behavior = _skill_current.NextBehavior ();
						if (null == _behavior) 
						//if(false == _skill_current.IsNextBehavior())
						{
							//스킬 동작을 모두 꺼냈으면 아이들상태로 들어간다
							Idle ();
						} else 
						{
							//_behavior = _skill_current.NextBehavior ().Clone();

							//다음 스킬 동작으로 넘어간다
							SetState (eState.Start);
							_timeDelta = 0f;

							DebugWide.LogBlue ("next combo !!");
						}
					}

				}
				break;
			
			
			}

			//============================================================================

			this.GetJudgment ().Update (); //판정 후처리 갱신

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


//		Judgment.Result _result_;
		Character _src_ = null;
		Character _dst_ = null;
		public void Update()
		{
			//처리철차 : 입력 -> ui갱신 -> (갱신 -> 판정)

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

		public enum eRunningState
		{
			None,
			Start,
			Running,
			End
		}
			
		private eState			_state_prev = eState.None;
		private eState 			_state_current = eState.None;
		private eRunningState 	_runningState = eRunningState.None;


		public Judgment()
		{
		}

		private eState GetState_Prev()
		{
			return _state_prev;
		}

		public eState GetState_Current()
		{
			return _state_current;
		}

		public void SetState_Current(eState current)
		{
			_state_prev = _state_current;
			_state_current = current;

			if (_state_prev != _state_current) 
			{
				_runningState = eRunningState.Start;
			}
		}
			
		public eRunningState GetRunningState()
		{
			return _runningState;
		}

		public bool IsStart()
		{
			return eRunningState.Start == _runningState;
		}

		public bool IsStart_AttackSucced()
		{
			if (Judgment.eState.Attack_Succeed == _state_current &&
				eRunningState.Start == _runningState )
				return true;

			return false;
		}

		public bool IsStart_BlockSucceed()
		{
			if (Judgment.eState.Block_Succeed == _state_current &&
				eRunningState.Start == _runningState )
				return true;

			return false;
		}

		public bool IsStart_Damaged()
		{
			if (Judgment.eState.Damaged == _state_current &&
				eRunningState.Start == _runningState )
				return true;

			return false;
		}

		public void Update()
		{
			
			switch (_runningState) 
			{
			case eRunningState.None:
				{

				}
				break;
			case eRunningState.Start:
				{
					_runningState = eRunningState.Running;

				}
				break;
			case eRunningState.Running:
				{
					if (_state_prev != _state_current) 
					{
						_runningState = eRunningState.End;
					}

				}
				break;
			case eRunningState.End:
				{
					_runningState = eRunningState.None;
				}
				break;
			}
		}

		//==========================================


//		public struct Result
//		{
//			public eState first;
//			public eState second;
//
//			public Result(eState s, eState d)
//			{
//				first = s;
//				second = d;
//			}
//
//			public void Init()
//			{
//				first = eState.None;
//				second = eState.None;
//			}
//		}

//		static public Result Judge(Character src , Character dst)
//		{
//			Result result = new Result();
//			result.Init ();
//
//
//			//공격범위(무기와 기술에 영향을 받음)  ,  상대와의 거리  ,  공격타점의 이동시간(사거리의 이동시간)
//			//무기범위 , 신체범위
//			//판정 : 타점이 신체범위에 들어 왔는가?
//
//
//			//============================
//			//Attack_Vs_Attack
//			//============================
//			if (true == src.IsSkill_Attack () && true == dst.IsSkill_Attack()) 
//			{
//				if (true == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					
//					result.first = eState.Attack_Blocked;
//					result.second = eState.Attack_Blocked;
//
//				}
//				if (true == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Succeed;
//					result.second = eState.Damaged;
//				}
//				if (false == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Damaged;
//					result.second = eState.Attack_Succeed;
//				}
//				if (false == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Idle;
//					result.second = eState.Attack_Idle;
//				}
//
//			}
//
//			//============================
//			//Attack_Vs_Block
//			//============================
//			if (true == src.IsSkill_Attack () && true == dst.IsSkill_Block()) 
//			{
//				if (true == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Blocked;
//					result.second = eState.Block_Succeed;
//				}
//				if (true == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Succeed;
//					result.second = eState.Damaged;
//				}
//				if (false == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Idle;
//					result.second = eState.Block_Idle;
//				}
//				if (false == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Attack_Idle;
//					result.second = eState.Block_Idle;
//				}
//			}
//
//			//============================
//			//Block_Vs_Attack
//			//============================
//			if (true == src.IsSkill_Block () && true == dst.IsSkill_Attack()) 
//			{
//				if (true == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Block_Succeed;
//					result.second = eState.Attack_Blocked;
//				}
//				if (true == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Block_Idle;
//					result.second = eState.Attack_Idle;
//				}
//				if (false == src.Valid_EventTime () && true == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Damaged;
//					result.second = eState.Attack_Succeed;
//				}
//				if (false == src.Valid_EventTime () && false == dst.Valid_EventTime ()) 
//				{
//					result.first = eState.Block_Idle;
//					result.second = eState.Attack_Idle;
//				}
//
//			}
//
//			//============================
//			//Attack_Vs_None
//			//============================
//			if (true == src.IsSkill_Attack () && true == dst.IsSkill_Unprotected ()) {
//				if (true == src.Valid_EventTime () && true == dst.Valid_EventTime ()) {
//					result.first = eState.Attack_Succeed;
//					result.second = eState.Damaged;
//				}
//				if (true == src.Valid_EventTime () && false == dst.Valid_EventTime ()) {
//					result.first = eState.Attack_Succeed;
//					result.second = eState.Damaged;
//				}
//				if (false == src.Valid_EventTime () && true == dst.Valid_EventTime ()) {
//					result.first = eState.Attack_Idle;
//					result.second = eState.None;
//				}
//				if (false == src.Valid_EventTime () && false == dst.Valid_EventTime ()) {
//					result.first = eState.Attack_Idle;
//					result.second = eState.None;
//				}
//			}
//
//
//			//============================
//			//None_Vs_Attack
//			//============================
//			if (true == src.IsSkill_Unprotected () && true == dst.IsSkill_Attack ()) {
//				if (true == src.Valid_EventTime () && true == dst.Valid_EventTime ()) {
//					result.first = eState.Damaged;
//					result.second = eState.Attack_Succeed;
//				}
//				if (true == src.Valid_EventTime () && false == dst.Valid_EventTime ()) {
//					result.first = eState.None;
//					result.second = eState.Attack_Idle;
//				}
//				if (false == src.Valid_EventTime () && true == dst.Valid_EventTime ()) {
//					result.first = eState.Damaged;
//					result.second = eState.Attack_Succeed;
//				}
//				if (false == src.Valid_EventTime () && false == dst.Valid_EventTime ()) {
//					result.first = eState.None;
//					result.second = eState.Attack_Idle;
//				}
//			}
//
//			//============================
//			//Exceptions
//			//============================
//
//
//
//			return result;
//		}
//



	}//end class	

	public class SkillManager
	{
		
	}


	public class Skill : List<Behavior>
	{

		public enum eKind
		{
			None,
			Attack_Strong,
			Attack_Weak,
			Attack_Counter,
			Withstand,
			Block,
			Hit,
			Max
		}

		public enum eName
		{
			None,
			Idle,
			Hit_Body,
			Hit_Weapon,

			Attack_Strong_1,
			Attack_Weak_1,
			Attack_Counter_1,

			Attack_3Combo,

			Withstand_1,
			Block_1,

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
			case Skill.eName.Hit_Body:
				return "Hit_Body";
			case Skill.eName.Hit_Weapon:
				return "Hit_Weapon";
			
			case Skill.eName.Attack_Strong_1:
				return "Attack_Strong_1";
			case Skill.eName.Attack_Weak_1:
				return "Attack_Weak_1";
			case Skill.eName.Attack_Counter_1:
				return "Attack_Counter_1";
			case Skill.eName.Attack_3Combo:
				return "Attack_3Combo";

			case Skill.eName.Withstand_1:
				return "Withstand_1";
			case Skill.eName.Block_1:
				return "Block_1";
			
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

		//다음 행동이 있나 질의한다
		public bool IsNextBehavior()
		{
			if (this.Count > _index_current) 
			{
				//마지막 인덱스임
				if (this.Count == _index_current + 1)
					return false;


				return true;
			}

			return false;
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

			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_HitBody()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Hit;
			skinfo.name = eName.Hit_Body;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_HitWeapon()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Hit;
			skinfo.name = eName.Hit_Weapon;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1.5f;
			//1
			bhvo.cloggedTime_0 = 0f;
			bhvo.cloggedTime_1 = 0f;
			//2
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			//3
			bhvo.openTime_0 = -1f; //연결 동작을 못 넣게 막는다. 0으로 설정시 연속입력을 허용하게 된다
			bhvo.openTime_1 = -1f;
			//4
			bhvo.rigidTime = 0f;


			bhvo.attack_shape = eTraceShape.Straight;
			bhvo.angle = 0f;
			bhvo.plus_range_0 = 0f;
			bhvo.plus_range_1 = 0f;
			bhvo.distance_travel = 0f;
			bhvo.distance_maxTime = 0f;
			//bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Withstand_1()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Withstand;
			skinfo.name = eName.Withstand_1;

			Behavior bhvo = new Behavior ();
			//bhvo.runningTime = 10.0f; //임시값
			bhvo.runningTime = 3.0f; 
			//1
			bhvo.cloggedTime_0 = 0f;
			bhvo.cloggedTime_1 = 0f;
			//2
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			//3
			bhvo.openTime_0 = -1f; //연결 동작을 못 넣게 막는다. 0으로 설정시 연속입력을 허용하게 된다
			bhvo.openTime_1 = -1f;
			//4
			bhvo.rigidTime = 0f;


			bhvo.attack_shape = eTraceShape.Straight;
			bhvo.angle = 0f;
			bhvo.plus_range_0 = 0f;
			bhvo.plus_range_1 = 0f;
			bhvo.distance_travel = 0f;
			bhvo.distance_maxTime = 0f;
			//bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_Weak()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_Weak;
			skinfo.name = eName.Attack_Weak_1;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1.5f;
			//1
			bhvo.cloggedTime_0 = 0.0f;
			bhvo.cloggedTime_1 = 1.3f;
			//2
			bhvo.eventTime_0 = 0.7f;
			bhvo.eventTime_1 = 1f;
			//3
			bhvo.openTime_0 = 1f;
			bhvo.openTime_1 = 1.3f;
			//4
			bhvo.rigidTime = 0f;


			bhvo.attack_shape = eTraceShape.Straight;
			//bhvo.attack_shape = eTraceShape.Vertical;
			bhvo.angle = 45f;
			bhvo.plus_range_0 = 0f;
			bhvo.plus_range_1 = -4f;
			bhvo.distance_travel = Behavior.DEFAULT_DISTANCE - 4f;
			//bhvo.distance_maxTime = bhvo.eventTime_0; //유효범위 시작시간에 최대 거리가 되게 한다. : 떙겨치기 , [시간증가에 따라 유효거리 감소]
			bhvo.distance_maxTime = bhvo.eventTime_1; //유효범위 끝시간에 최대 거리가 되게 한다. : 일반치기 , [시간증가에 따라 유효거리 증가]

			bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			//===== chamto test

//			bhvo = new Behavior ();
//			bhvo.runningTime = 1.0f;
//			//1
//			bhvo.cloggedTime_0 = 0.0f;
//			bhvo.cloggedTime_1 = 0.6f;
//			//2
//			bhvo.eventTime_0 = 0.7f;
//			bhvo.eventTime_1 = 0.8f;
//			//3
//			bhvo.openTime_0 = 0.7f;
//			bhvo.openTime_1 = 1f;
//			//4
//			bhvo.rigidTime = 0.2f;
//
//
//			bhvo.attack_shape = eTraceShape.Straight;
//			//bhvo.attack_shape = eTraceShape.Vertical;
//			bhvo.angle = 45f;
//			bhvo.plus_range_0 = 0f;
//			bhvo.plus_range_1 = -4f;
//			bhvo.distance_travel = Behavior.DEFAULT_DISTANCE;
//			//bhvo.distance_maxTime = bhvo.eventTime_0; //유효범위 시작시간에 최대 거리가 되게 한다. : 떙겨치기 , [시간증가에 따라 유효거리 감소]
//			bhvo.distance_maxTime = bhvo.eventTime_1; //유효범위 끝시간에 최대 거리가 되게 한다. : 일반치기 , [시간증가에 따라 유효거리 증가]
//			//bhvo.distance_maxTime = 0.6f; //chamto test
//			bhvo.Calc_Velocity ();
//			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_Counter()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_Counter;
			skinfo.name = eName.Attack_Counter_1;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1.2f;
			//1
			bhvo.cloggedTime_0 = 0.0f;
			bhvo.cloggedTime_1 = 0.7f;
			//2
			bhvo.eventTime_0 = 0.8f;
			bhvo.eventTime_1 = 1.0f;
			//3
			bhvo.openTime_0 = -1f;
			bhvo.openTime_1 = -1f;
			//4
			bhvo.rigidTime = 0.2f;


			bhvo.attack_shape = eTraceShape.Straight;
			bhvo.distance_travel = Behavior.DEFAULT_DISTANCE;
			//bhvo.distance_maxTime = bhvo.eventTime_0; //유효범위 시작시간에 최대 거리가 되게 한다. : 떙겨치기 , [시간증가에 따라 유효거리 감소]
			bhvo.distance_maxTime = bhvo.eventTime_1; //유효범위 끝시간에 최대 거리가 되게 한다. : 일반치기 , [시간증가에 따라 유효거리 증가]
			bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_Strong()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_Strong;
			skinfo.name = eName.Attack_Strong_1;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 2.0f;
			//1
			bhvo.cloggedTime_0 = 0.1f;
			bhvo.cloggedTime_1 = 1.0f;
			//2
			bhvo.eventTime_0 = 1.0f;
			bhvo.eventTime_1 = 1.2f;
			//3
			bhvo.openTime_0 = 1.5f;
			bhvo.openTime_1 = 1.8f;
			//4
			bhvo.rigidTime = 0.5f;

			bhvo.attack_shape = eTraceShape.Straight;
			//bhvo.attack_shape = eTraceShape.Vertical;
			bhvo.angle = 45f;
			bhvo.plus_range_0 = 2f;
			bhvo.plus_range_1 = 2f;
			bhvo.distance_travel = Behavior.DEFAULT_DISTANCE;
			//bhvo.distance_maxTime = bhvo.eventTime_0; //유효범위 시작시간에 최대 거리가 되게 한다. : 떙겨치기 , [시간증가에 따라 유효거리 감소]
			bhvo.distance_maxTime = bhvo.eventTime_1; //유효범위 끝시간에 최대 거리가 되게 한다. : 일반치기 , [시간증가에 따라 유효거리 증가]

			bhvo.Calc_Velocity ();
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_3Combo()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_Strong;
			skinfo.name = eName.Attack_3Combo;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 2f;
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 3f;
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 0f;
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
			bhvo.eventTime_0 = 0f;
			bhvo.eventTime_1 = 1f;
			bhvo.rigidTime = 0.1f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);


			return skinfo;
		}
			
	}

	/// <summary>
	/// Skill book.
	/// </summary>
	public class SkillBook //: Dictionary<Skill.eName, Skill>
	{
		private delegate Skill Details_Skill();
		private Dictionary<Skill.eName, Skill> _referDict = new Dictionary<Skill.eName, Skill>();	//미리 만들어진 정보로 빠르게 사용
		private Dictionary<Skill.eName, Details_Skill> _createDict = new Dictionary<Skill.eName, Details_Skill>(); //새로운 스킬인스턴스를 만들때 사용 

		public SkillBook()
		{
			this.Add (Skill.eName.Idle, Skill.Details_Idle );
			this.Add (Skill.eName.Hit_Body, Skill.Details_HitBody );
			this.Add (Skill.eName.Hit_Weapon, Skill.Details_HitWeapon );

			this.Add (Skill.eName.Withstand_1, Skill.Details_Withstand_1 );
			this.Add (Skill.eName.Block_1, Skill.Details_Block_1 );

			this.Add (Skill.eName.Attack_Strong_1, Skill.Details_Attack_Strong );
			this.Add (Skill.eName.Attack_Weak_1, Skill.Details_Attack_Weak );
			this.Add (Skill.eName.Attack_Counter_1, Skill.Details_Attack_Counter );

			this.Add (Skill.eName.Attack_3Combo, Skill.Details_Attack_3Combo );

		}

		private void Add(Skill.eName name , Details_Skill skillPtr)
		{
			_referDict.Add (name, skillPtr ());
			_createDict.Add (name, skillPtr);
		}

		//만들어진 객체를 참조한다 
		public Skill Refer(Skill.eName name)
		{
			return _referDict [name];
		}

		//요청객체를 생성한다
		public Skill Create(Skill.eName name)
		{
			return _createDict [name] ();
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



	/// <summary>
	/// frame skip 시 해당프레임의 deltaTime을 최소 프레임시간으로 설정한다.
	/// </summary>
	public class FrameControl
	{
		static private float _deltaTime_mix = 0f;
		static private float _deltaTime_max = 0f;

		static public void SetDeltaTime_30FPS()
		{
			FrameControl.SetDeltaTime_FPS (30f);
		}

		static public void SetDeltaTime_FPS(float fps)
		{
			_deltaTime_mix = 1f / fps;
			_deltaTime_max = _deltaTime_mix * 2f; //최소시간의 2배 한다. 
		}

		static public float DeltaTime_Mix()
		{
			return _deltaTime_mix;
		}

		static public float DeltaTime_Max()
		{
			return _deltaTime_max;
		}

		static public float DeltaTime()
		{
			//전프레임이 허용프레임 시간의 최대치를 넘었다면 최소시간을 반환한다.
			if (Time.deltaTime > _deltaTime_max) 
			{
				//DebugWide.LogBlue ("FrameControl - frameSkip detected !!! - DeltaTime : "+Time.deltaTime);//chamto test
				return _deltaTime_mix;
			}


			return Time.deltaTime;
		}
			
	}//end class

}//end namespace 


