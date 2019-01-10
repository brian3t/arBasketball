using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Animation))]
public class Player : MonoBehaviour
{

		public Ball basketBall;
		public float bounceForce = 1000f;
		private Transform _transform;
		private AnimationClip _currentAnimation = null;
		public AnimationClip animIdle;
		public AnimationClip animBounceDown;
		public AnimationClip animBounceUp;
		public AnimationClip animWalkForward;
		public AnimationClip animWalkBackward;
		public AnimationClip animPrepareThrow;
		public AnimationClip animThrow;
		public AnimationClip animScore;
		public AnimationClip animMiss;
		public float maxThrowForce = .00003f;
		public Vector3 throwDirection = new Vector3 (-0.5f, 1f, 0.0f);
		public float walkSpeed = 5.0f;
		protected float timeStartHoldingBall;
		protected float timeReleaseBall;
		private Vector3 _distance;


		public delegate void PlayerAnimationFinished (string animation);

		public PlayerAnimationFinished OnPlayerAnimationFinished = null;
		private Vector3 _shotPosition = Vector3.zero;

		public Vector3 ShotPosition {
				get {
						return _shotPosition; 
				}
				set {
						_shotPosition = value; 
			
						if (Mathf.Abs (_shotPosition.x - _transform.position.x) < 0.1f) {
								State = PlayerStateEnum_Idle; 	
						} else {
								State = PlayerStateEnum_Walking;
						}
				}
		}

		public const string PlayerStateEnum_Idle = "PlayerStateEnum_Idle";
		public const string PlayerStateEnum_HoldingBall = "PlayerStateEnum_HoldingBall";
		public const string PlayerStateEnum_PreparingToThrow = "PlayerStateEnum_PreparingToThrow";
		public const string PlayerStateEnum_Throwing = "PlayerStateEnum_Throwing";
		public const string PlayerStateEnum_Score = "PlayerStateEnum_IScore";
		public const string PlayerStateEnum_Miss = "PlayerStateEnum_Miss";
		public const string PlayerStateEnum_Walking = "PlayerStateEnum_Walking";
		private string _state = PlayerStateEnum_Idle;
		private float _elapsedStateTime = 0.0f;
		private Animation _animation;
		private CapsuleCollider _collider;
		private bool _holdingBall = true;
		private Transform _handTransform;

		void Awake ()
		{
				_transform = GetComponent<Transform> ();
				_animation = GetComponent<Animation> ();
				_collider = GetComponent<CapsuleCollider> ();
				_handTransform = _transform.Find (
			"BPlayerSkeleton/Pelvis/Hip/Spine/Shoulder_R/UpperArm_R/LowerArm_R/Hand_R");
				_shotPosition = _transform.position;

//		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
//		meshRenderer.enabled = false;
		}



		// Use this for initialization
		void Start ()
		{
				_animation = GetComponent<Animation> ();
				InitAnimations ();
				CurrentAnimation = animPrepareThrow;
				State = PlayerStateEnum_Idle;
		}
	
		// Update is called once per frame
		void Update ()
		{
		Vector3 hoopPos = GameObject.Find ("hoop").transform.position;
		_distance = hoopPos - basketBall.BallTransform.position;

		if (_holdingBall) {
						ResetBallPosition (); 	
			if (_distance.magnitude < 16) {
				basketBall.Hide ();
				GameController.SharedInstance.UpdateStatus("Too close!");
			} else {
				basketBall.Show();
				GameController.SharedInstance.UpdateStatus("Ready");
			}
		}
		_elapsedStateTime += Time.deltaTime;
				if (_state == PlayerStateEnum_Idle) {	
						if (_holdingBall) {
								if (GameController.SharedInstance.State == GameController.GameStateEnum.Play && GameController.SharedInstance.TouchDownCount >= 1) {
										State = PlayerStateEnum_PreparingToThrow;
										//save the first time when touchdown is > 1
										timeStartHoldingBall = Time.time;
										return; 
								}
						}
			
						if (_currentAnimation.name.Equals (animBounceDown.name)) {
								if (!_animation.isPlaying && _holdingBall) {
										// let go of ball
										_holdingBall = false;  
										// throw ball down 
										basketBall.BallRigidbody.AddRelativeForce (Vector3.up * bounceForce); 				
								} 				
						} else if (_currentAnimation.name.Equals (animBounceUp.name)) {						
								if (!_animation.isPlaying) {
										SetCurrentAnimation (animBounceDown); 
								}					
						}
				}
				if (_state == PlayerStateEnum_PreparingToThrow) {

						if (GameController.SharedInstance.State == GameController.GameStateEnum.Play &&
								GameController.SharedInstance.TouchCount == 0) {
				
								State = PlayerStateEnum_Throwing;
								
								_holdingBall = false; 
								timeReleaseBall = Time.time;
								float duration = timeReleaseBall - timeStartHoldingBall;
								float pumpedUpForce = maxThrowForce * duration;
								basketBall.BallRigidbody.useGravity = true;
								//recal throwdirection, we have to make it so that to ball always aim to the hoop, no matter where the hoop is
								throwDirection = _distance.normalized;
								throwDirection.y += 0.3f;
								basketBall.BallRigidbody.AddRelativeForce (throwDirection * (pumpedUpForce));
								basketBall.PlayShootSound ();
				GameController.SharedInstance.UpdateDistance(_distance.magnitude);
						}
				}
				if (_state == PlayerStateEnum_Throwing) {
						// turn on the collider as you want the ball to react to the player is it bounces back 
						if (!_animation.isPlaying && !_collider.enabled) {				
								_collider.enabled = true; 
						}
				}
				if (_state == PlayerStateEnum_Walking) {
						Vector3 pos = _transform.position; 
						pos = Vector3.Lerp (pos, _shotPosition, Time.deltaTime * walkSpeed); 
						_transform.position = pos; 
			
						if ((pos - _shotPosition).sqrMagnitude < 1.0f) {
								pos = _shotPosition;
								if (OnPlayerAnimationFinished != null) {
										OnPlayerAnimationFinished (_currentAnimation.name);
								}
						}
				}
		}

		private void InitAnimations ()
		{
				_animation.Stop ();
				_animation [animIdle.name].wrapMode = WrapMode.Once; 
				_animation [animBounceDown.name].wrapMode = WrapMode.Once; 
				_animation [animBounceUp.name].wrapMode = WrapMode.Once;         
				_animation [animWalkForward.name].wrapMode = WrapMode.Loop; 
				_animation [animWalkBackward.name].wrapMode = WrapMode.Loop; 
				_animation [animPrepareThrow.name].wrapMode = WrapMode.Once; 
				_animation [animThrow.name].wrapMode = WrapMode.Once; 
				_animation [animScore.name].wrapMode = WrapMode.Once; 
				_animation [animMiss.name].wrapMode = WrapMode.Once; 
		
				_animation [animBounceDown.name].speed = 2.0f; 
				_animation [animBounceUp.name].speed = 2.0f;         

		}

		public bool IsAnimating {
				get {
						return _animation.isPlaying; 	
				}
		}
	
		public AnimationClip CurrentAnimation {
				get {
						return _currentAnimation; 
				}
				set {
						SetCurrentAnimation (value);   
				}
		}

		public void SetCurrentAnimation (AnimationClip animationClip)
		{
				_currentAnimation = animationClip; 
				_animation [_currentAnimation.name].time = 0.0f; 
				_animation.CrossFade (_currentAnimation.name, 0.1f); 
		
				if (_currentAnimation.wrapMode != WrapMode.Loop) {
						Invoke ("OnAnimationFinished", _animation [_currentAnimation.name].length /
								_animation [_currentAnimation.name].speed);
				}
		}

		private void OnAnimationFinished ()
		{ 
		
				if (OnPlayerAnimationFinished != null) {
						OnPlayerAnimationFinished (_currentAnimation.name);    
				}
		}

		public bool IsHoldingBall {
				get {
						return _holdingBall;
				}
		}

		private void ResetBallPosition ()
		{
				_holdingBall = true; 
		
				Transform bTransform = basketBall.BallTransform; 
				SphereCollider bCollider = basketBall.BallCollider;  
				Rigidbody bRB = basketBall.BallRigidbody; 
		
				bRB.velocity = Vector3.zero; 
		
				bTransform.rotation = Quaternion.identity; 
				//todo handle ball pos here, not in gamecontroller
		
//				Vector3 bPos = bTransform.position;         
//				bPos = _handTransform.position;
//				bPos.y -= bCollider.radius; 
//				bTransform.position = bPos;                     

		}

		public string State {
				get {
						return _state; 
				}
				set {
//			Debug.Log("State is " + value);
						CancelInvoke ("OnAnimationFinished"); 
			
						_state = value; 
						_elapsedStateTime = 0.0f; 
			
						switch (_state) {
						case PlayerStateEnum_Idle:
								_collider.enabled = false; 
								ResetBallPosition (); 	
								basketBall.BallRigidbody.useGravity = false;
								SetCurrentAnimation (animBounceUp);				
								break;
						case PlayerStateEnum_PreparingToThrow:
								SetCurrentAnimation (animPrepareThrow); 
								break;
						case PlayerStateEnum_Throwing:				
								SetCurrentAnimation (animThrow); 
								break;
						case PlayerStateEnum_Score:
								SetCurrentAnimation (animScore); 
								break;
						case PlayerStateEnum_Miss:
								SetCurrentAnimation (animMiss); 
								break;
						case PlayerStateEnum_Walking:
								if (_shotPosition.x < _transform.position.x) {
										SetCurrentAnimation (animWalkForward); 
								} else {
										SetCurrentAnimation (animWalkBackward); 
								}
								break;
						}															 									
				}
		}

		public void OnTriggerEnter (Collider collider)
		{
				Debug.Log (string.Concat ("Player got triggered with", collider.name));
				if (_state == PlayerStateEnum_Idle) {
						if (!_holdingBall && collider.transform == basketBall.transform) {
								ResetBallPosition ();
								SetCurrentAnimation (animBounceUp);

						}
				}
		}

		public float ElapseStateTime {
				get {
						return _elapsedStateTime;
				}
		}
	public Vector3 Distance {
		get {
			return _distance;
		}
		set {
			_distance = value;
		}
	}

}
