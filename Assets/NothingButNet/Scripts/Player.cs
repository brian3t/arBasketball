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
	public float maxThrowForce = 5000f;
	public Vector3 throwDirection = new Vector3 (-1.0f, 0.5f, 0.0f);
	public float walkSpeed = 5.0f;
	
	
	public delegate void PlayerAnimationFinished (string animation);
	
	public PlayerAnimationFinished OnPlayerAnimationFinished = null;
	private Vector3 _shotPosition = Vector3.zero;
	
	public Vector3 ShotPosition{
		get{
			return _shotPosition; 
		}
		set{
			_shotPosition = value; 
			
			if( Mathf.Abs( _shotPosition.x - _transform.position.x ) < 0.1f ){
				State = PlayerStateEnumIdle; 	
			} else{
				State = PlayerStateEnumInvalid;
			}
		}
	}


	public const int PlayerStateEnumIdle = 1;
	public const int PlayerStateEnumInvalid = 2;
	public const int PlayerStateEnumPreparingToThrow = 3;
	public const int PlayerStateEnumThrowing = 4;
	public const int PlayerStateEnumScore = 5;
	public const int PlayerStateEnumMiss = 6;
	public const int PlayerStateEnumWaitingOther = 7;

	private int _state = PlayerStateEnumIdle;
	private float _elapsedStateTime = 0.0f;
	private Animation _animation;
	private bool _holdingBall = true;
	private Transform _handTransform;
	
	void Awake ()
	{
		_transform = GetComponent<Transform> ();
		_animation = GetComponent<Animation> ();
		_shotPosition = _transform.position;
		
	}
	
	
	
	// Use this for initialization
	void Start ()
	{
		_animation = GetComponent<Animation> ();
		InitAnimations ();
		CurrentAnimation = animPrepareThrow;
		State = PlayerStateEnumIdle;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_elapsedStateTime += Time.deltaTime;
		if (_state.Equals( PlayerStateEnumIdle)) {	
			if (_holdingBall) {
				if (GameController.SharedInstance.State == GameController.GameStateEnum_Ready && GameController.SharedInstance.TouchDownCount >= 1) {
					State = PlayerStateEnumPreparingToThrow;
					return; 
				}
			}
			
				if (!_animation.isPlaying && _holdingBall) {
					// let go of ball
					_holdingBall = false;  
					// throw ball down 
					basketBall.BallRigidbody.AddRelativeForce (Vector3.up * bounceForce); 				
				} 				
		}
		if (_state == PlayerStateEnumPreparingToThrow) {
			if (GameController.SharedInstance.State == GameController.GameStateEnum_Ready &&
			    GameController.SharedInstance.TouchCount == 0) {
				//apply gravity here
				State = PlayerStateEnumThrowing;
				
				_holdingBall = false; 
				float pumpedUpForce = maxThrowForce * _animation [animPrepareThrow.name].normalizedTime;
				basketBall.BallRigidbody.AddRelativeForce (throwDirection * (pumpedUpForce));
			}
		}
		if (_state == PlayerStateEnumThrowing) {

		}
	}
	
	private void InitAnimations ()
	{

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
	
	private void AttachAndHoldBall ()
	{
		_holdingBall = true; 
		
//make sure gravity is not applied

	}
	
	public int State {
				get {
						return _state; 
				}
				set {
						CancelInvoke ("OnAnimationFinished"); 
			
						_state = value; 
						_elapsedStateTime = 0.0f; 
			
						switch (_state) {
						case PlayerStateEnumIdle:
								AttachAndHoldBall (); 			
								break;
						case PlayerStateEnumPreparingToThrow:
								SetCurrentAnimation (animPrepareThrow); 
								break;
						case PlayerStateEnumThrowing:				
								SetCurrentAnimation (animThrow); 
								break;
						case PlayerStateEnumScore:
								SetCurrentAnimation (animScore); 
								break;
						case PlayerStateEnumMiss:
								SetCurrentAnimation (animMiss); 
								break;
						}
			Debug.Log("Player state "+ value);
				}

		}
	
	public void OnTriggerEnter (Collider collider)
	{
		Debug.Log (string.Concat ("Player got triggered with", collider.name));
		if (_state == PlayerStateEnumIdle) {
			if (!_holdingBall && collider.transform == basketBall.transform) {
				AttachAndHoldBall ();
				SetCurrentAnimation (animBounceUp);
				
			}
		}
	}
	
	public float ElapseStateTime {
		get {
			return _elapsedStateTime;
		}
	}
}
