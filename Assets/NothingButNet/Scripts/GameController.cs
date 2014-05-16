using UnityEngine;
using System.Collections;
using System.Timers;

public class GameController : MonoBehaviour {

	private static GameController _instance = null;
	
	public Player player;// Reference to your player on the scene 
	public ScoreBoard scoreBoard;// Reference to your games scoreboard 
	public Ball basketBall;// reference to the courts one and only basketball 
	public Helper helper;
	public float _timeUpdateElapsedTime;

	public float throwRadius = 5.0f;// radius the player will be positioned for each throw 
	public enum GameStateEnum		{
		Undefined,
		Menu,
		Paused,
		Ready,
		BallThrown,
		Missed,
		Scored,
		Disabled,
		WaitingOtherPlayer
	};
	private static GameStateEnum _state = GameStateEnum.Undefined;// state of the current game - controls how
	//user interactions are interrupted and what is activivated and disabled 

	private int _gamePoints = 0;// Points accumulated by the user for this game session 
	private Vector3 _ballPosition;

	public static GameController SharedInstance{
		get {
			if (_instance == null){
				_instance = GameObject.FindObjectOfType(typeof (GameController)) as GameController;
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		basketBall.OnNet += HandleBasketballOnNet;
	}

	public void HandleBasketballOnNet() {
		GamePoints += 3;
		_state = GameStateEnum.Scored;
	}
	
	void Awake(){
		_instance = this;
		helper = new Helper ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_state == GameStateEnum.Undefined) {
			State=GameStateEnum.Menu;
				}
		else if(_state == GameStateEnum.Ready){
			UpdateStateReady();
		}

	}
	private void UpdateStateReady(){

		// accumulate elapsed time 
		_timeUpdateElapsedTime += Time.deltaTime; 
		
		// has a second past? 
		if( _timeUpdateElapsedTime >= 1.0f ){
		}
		
		// after n seconds of the player being in the miss or score state reset the position and session 
		if( (_state == GameStateEnum.Missed || _state == GameStateEnum.Scored)
		   && TimeUpdateElapsedTime >= 3.0f ){
			
				// set a new throw position 
				// offset x 
			}
		}


	public float TimeUpdateElapsedTime{
	get{
			return _timeUpdateElapsedTime;
	}
	set{
			_timeUpdateElapsedTime = value;
	}
}
	
	private void UpdateStateGameOver(){		
		// TODO; to implement (next tutorial) 		
	}
	public void OnBallCollisionEnter(Collision collision){
//		Debug.Log("GameController: ball collision occurred!");

	}

	public GameStateEnum State {
				get {
						return _state;
				}
		set {
			_state = value;
			//MENU
			if (_state == GameStateEnum.Menu){
				Debug.Log("state changed to Menu");
				//TODO: replace play state with menu (next)
				StartNewGame();
			}

			//PAUSED
			else if (_state == GameStateEnum.Paused) {
				Debug.Log("state changed to " + _state.ToString());
				//TODO: change play
			}

				}
		}
	public void StartNewGame(){
		GamePoints = 0;
		State = GameStateEnum.Ready;
	}

	public void ResumeGame(){

			State = GameStateEnum.Ready;
		}

	public int GamePoints{
		get{
			return _gamePoints;
		}
		set {
			_gamePoints = value;
//			scoreBoard.setPoints(_gamePoints.ToString());
		}

	}

	public int TouchCount {
		get{
			if( helper.IsMobile ){
				return Input.touchCount; 
			} else{
				// if its not consdered to be mobile then query the left mouse button, returning 1 if down or 0 if not  
				if( Input.GetMouseButton(0) ){
					return 1; 	
				} else{
					return 0; 
				}
			}
		}	
	}
	
	public int TouchDownCount {
		get{
			if( helper.IsMobile ){
				int currentTouchDownCount = 0; 
				foreach( Touch touch in Input.touches ){
					if( touch.phase == TouchPhase.Began ){
						currentTouchDownCount++; 	
					}
				}
				
				return currentTouchDownCount;
			} else{
				if( Input.GetMouseButtonDown(0) ){
					return 1; 	
				} else{
					return 0; 
				}
			}
		}
	}
}
