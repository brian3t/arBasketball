using UnityEngine;
using System.Collections;
using System.Timers;

public class GameController : MonoBehaviour {

	private static GameController _instance = null;
	
	public Player player;// Reference to your player on the scene 
	public ScoreBoard scoreBoard;// Reference to your games scoreboard 
	public Ball basketBall;// reference to the courts one and only basketball 
	public Helper helper;
	
	public float gameSessionTime = 120.0f;// time for a single game session (in seconds) 
	public float throwRadius = 5.0f;// radius the player will be positioned for each throw 
	public enum GameStateEnum		{
		Undefined,
		Menu,
		Paused,
		Play,
		GameOver
	};
	private static GameStateEnum _state = GameStateEnum.Undefined;// state of the current game - controls how
	//user interactions are interrupted and what is activivated and disabled 

	private int _gamePoints = 0;// Points accumulated by the user for this game session 
	private float _timeRemaining = 0.0f;// The time remaining for current game session 
	// we only want to update the count down every second; so we'll accumulate the time in this variable 
	// and update the remaining time after each second 	
	private float _timeUpdateElapsedTime = 0.0f;
	private Vector3 _orgPlayerPosition;// The original player position - each throw position will be offset based on this and a random value  
	// between-throwRadius and throwRadius 

	private GameMenuController _menuController;
	private LeaderboardController _leaderboardController;
	public Alerter alerter;

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
		player.OnPlayerAnimationFinished += HandlePlayerOnPlayerAnimationFinished;
	}

	public void HandleBasketballOnNet() {
		GamePoints += 3;
		if (player.Distance.magnitude > 35) {
			GamePoints+= 4;
				}
		player.State = Player.PlayerStateEnum_Score;
		scoreBoard.statusTextMesh.text = "Scored!! ";
	}

	public void HandlePlayerOnPlayerAnimationFinished(string animationName){
		if (player.State == Player.PlayerStateEnum_Walking) {
			player.State = Player.PlayerStateEnum_Idle;}
		}

	void Awake(){
		_instance = this;
		helper = new Helper ();
		_menuController = GetComponent<GameMenuController> ();
		_leaderboardController = GetComponent<LeaderboardController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_state == GameStateEnum.Undefined) {
			State=GameStateEnum.Menu;
				}
		else if(_state == GameStateEnum.Play){
			UpdateStatePlay();
		}
		else if(_state == GameStateEnum.GameOver){
			UpdateStateGameOver();
		}
	}
	private void UpdateStatePlay(){
		_timeRemaining -= Time.deltaTime; 
		
		// accumulate elapsed time 
		_timeUpdateElapsedTime += Time.deltaTime; 
		
		// has a second past? 
		if( _timeUpdateElapsedTime >= 1.0f ){
			TimeRemaining = Mathf.Round(_timeRemaining); 
		}
		//after 5 seconds of the Player being in the throwing state, if nothing happends, set the state as miss
		if (player.State == Player.PlayerStateEnum_Throwing && player.ElapseStateTime >= 1.2f) {
			player.State = Player.PlayerStateEnum_Miss;
		}
		
		// after n seconds of the player being in the miss or score state reset the position and session 
		if( (player.State == Player.PlayerStateEnum_Miss || player.State == Player.PlayerStateEnum_Score)
		   && player.ElapseStateTime >= 2.0f ){
			
			// check if the game is over 
			if( _timeRemaining <= 0.0f ){
				State = GameStateEnum.GameOver;
			} else{				
				// set a new throw position 
//				Vector3 playersNextThrowPosition = _orgPlayerPosition;
				Vector3 playersNextThrowPosition = Camera.main.transform.position;
				// offset x 
//				playersNextThrowPosition.z -= 3;
				playersNextThrowPosition.y -= 1;
				playersNextThrowPosition.x = 1;

//				playersNextThrowPosition.x +=  0.2f * Random.Range(-throwRadius, throwRadius); 
				player.ShotPosition = playersNextThrowPosition; 	
				Vector3 cameraPosition = Camera.main.transform.position; 
				
				Transform bskTrans =  basketBall.BallTransform;
				Vector3 bskPos = cameraPosition;
				bskPos.x += 0.3866521f;
				bskPos.y += -6.904152f;
				bskPos.z += 14.36236f;
				bskTrans.position = bskPos;
				basketBall.BallTransform = bskTrans;
				player.State = Player.PlayerStateEnum_Idle;
			}
		}
	}
	
	private void UpdateStateGameOver(){		
		// TODO; to implement (next tutorial) 		
	}
	public void OnBallCollisionEnter(Collision collision){
		Debug.Log("GameController: ball collision occurred!" + collision.ToString());
		if (!player.IsHoldingBall) {
			if ((collision.transform.name == "Ground" ||
			     collision.transform.name == "Court")
			    &&
			    player.State == Player.PlayerStateEnum_Throwing)
			{
				player.State = Player.PlayerStateEnum_Miss;
			}
				}

	}

	public GameStateEnum State {
		get{
			return _state; 	
		}
		set{
			_state = value; 
			
			// MENU 
			if( _state == GameStateEnum.Menu ){
				player.State = Player.PlayerStateEnum_Idle;	
				_menuController.Show(); 	
			}
			
			// PAUSED 
			else if( _state == GameStateEnum.Paused ){
				Time.timeScale = 0.0f; 
				_menuController.Show(); 
			}
			
			// PLAY 
			else if( _state == GameStateEnum.Play ){
				Time.timeScale = 1.0f; 
				_menuController.Hide(); 								
				
				// notify user
				alerter.Show( "GAME ON", 0.2f, 2.0f ); 
			}
			
			// GAME OVER 
			else if( _state == GameStateEnum.GameOver ){
				// add score 
				if( _gamePoints > 0 ){
					_leaderboardController.AddPlayersScore( _gamePoints ); 	
				}
				
				// notify user
				alerter.Show( "GAME OVER", 0.2f, 2.0f ); 	
			}								
		}

	}
	public void StartNewGame(){
		GamePoints = 0;
		_timeRemaining = gameSessionTime;
		player.State = Player.PlayerStateEnum_Idle;
		State = GameStateEnum.Play;
	}

	public void ResumeGame(){
		if (_timeRemaining < 0) {
						StartNewGame ();
				} else {
			State = GameStateEnum.Play;
				}
		}

	public int GamePoints{
		get{
			return _gamePoints;
		}
		set {
			_gamePoints = value;
			scoreBoard.setPoints(_gamePoints.ToString());
		}

	}

	public float TimeRemaining{
		get{
			return _timeRemaining;
		}
		set {
			_timeRemaining=value;
			scoreBoard.SetTime (_timeRemaining);
			//rese t elapsed time
			_timeUpdateElapsedTime = 0.0f;
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

	public void UpdateDistance(float distance){
		scoreBoard.UpdateDistance (distance);
	}
	public void UpdateStatus(string status){
		scoreBoard.statusTextMesh.text = status;
	}
}
