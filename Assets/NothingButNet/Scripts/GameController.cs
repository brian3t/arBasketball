using UnityEngine;
using System.Collections;
using System.Timers;

public class GameController : MonoBehaviour
{
		public Player player;
		private static GameController _instance = null;
		public ScoreBoard scoreBoard;// Reference to your games scoreboard 
		public Ball basketBall;// reference to the courts one and only basketball 
		public Alerter alerter;
		public Helper helper;
		public float _timeUpdateElapsedTime;
		public float throwRadius = 5.0f;// radius the player will be positioned for each throw 
		public enum GameStateEnum
		{
				Undefined,
				Menu,
				Paused,
				Ready,
				BallThrown,
				Missed,
				Scored,
				Disabled,
				WaitingOtherPlayer
	}
		;

	public const string GameStateEnum_Undefined = "GameStateEnum_Undefined";
	public const string GameStateEnum_Menu = "GameStateEnum_Menu";
	public const string GameStateEnum_Paused = "GameStateEnum_Paused";
	public const string GameStateEnum_Ready = "GameStateEnum_Ready";
	public const string GameStateEnum_BallThrown = "GameStateEnum_BallThrown";
	public const string GameStateEnum_Missed = "GameStateEnum_Missed";
	public const string GameStateEnum_Scored = "GameStateEnum_Scored";
	public const string GameStateEnum_Disabled = "GameStateEnum_Disabled";
	public const string GameStateEnum_WaitingOtherPlayer = "GameStateEnum_WaitingOtherPlayer";

	private static string _state = GameStateEnum_Undefined;// state of the current game - controls how
		//user interactions are interrupted and what is activivated and disabled 

		private int _gamePoints = 0;// Points accumulated by the user for this game session 
		private Vector3 _ballPosition;

		public static GameController SharedInstance {
				get {
						if (_instance == null) {
								_instance = GameObject.FindObjectOfType (typeof(GameController)) as GameController;
						}
						return _instance;
				}
		}

		// Use this for initialization
		void Start ()
		{
				basketBall.OnNet += HandleBasketballOnNet;
				State = GameStateEnum_Ready;
		}

		public void HandleBasketballOnNet ()
		{
				GamePoints += 3;
				_state = GameStateEnum_Scored;
		}
	
		void Awake ()
		{
				_instance = this;
				helper = new Helper ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
				if (_state == GameStateEnum_Undefined) {
						State = GameStateEnum_Menu;
				} else if (_state == GameStateEnum_Ready) {
						UpdateStateReady ();
				}

		}

		private void UpdateStateReady ()
		{


				// accumulate elapsed time 
				_timeUpdateElapsedTime += Time.deltaTime; 
		
				// has a second past? 
				if (_timeUpdateElapsedTime >= 1.0f) {
				}
		
				// after n seconds of the player being in the miss or score state reset the position and session 
				if ((player.State == Player.PlayerStateEnumMiss || player.State == Player.PlayerStateEnumScore)
						&& player.ElapseStateTime >= 3.0f) {
//todo reset game
			Debug.Log("3 seconds since miss or score");
						// check if the game is over 
		
								// set a new throw position 
//								Vector3 playersNextThrowPosition = _orgPlayerPosition;
								// offset x 
//								playersNextThrowPosition.x += Random.Range (-throwRadius, throwRadius); 
//								player.ShotPosition = playersNextThrowPosition; 			
				}
		}

		public float TimeUpdateElapsedTime {
				get {
						return _timeUpdateElapsedTime;
				}
				set {
						_timeUpdateElapsedTime = value;
				}
		}
	
		private void UpdateStateGameOver ()
		{		
				// TODO; to implement (next tutorial) 		
		}

		public void OnBallCollisionEnter (Collision collision)
		{
//		Debug.Log("GameController: ball collision occurred!");

		}

		public string State {
				get {
						return _state; 	
				}
				set {
						_state = value; 
			
						// MENU 
						if (_state == GameStateEnum_Menu) {
//				_menuController.Show(); 	
						}
			
			// PAUSED 
			else if (_state == GameStateEnum_Paused) {
								Time.timeScale = 0.0f; 
//				_menuController.Show(); 
						}
			
			// PLAY 
			else if (_state == GameStateEnum_Ready) {
								Time.timeScale = 1.0f; 
//				_menuController.Hide(); 								
				
								// notify user
								alerter.Show ("GAME ON", 0.2f, 2.0f); 
						}
			
			// GAME OVER 
			else if (_state == GameStateEnum_Disabled) {
								// notify user
								alerter.Show ("Please find a target image to start the game.", 0.2f, 2.0f); 	
						}								
				}
		
		}

		public void StartNewGame ()
		{
				GamePoints = 0;
				State = GameStateEnum_Ready;
		}

		public void ResumeGame ()
		{

				State = GameStateEnum_Ready;
		}

		public int GamePoints {
				get {
						return _gamePoints;
				}
				set {
						_gamePoints = value;
//			scoreBoard.setPoints(_gamePoints.ToString());
				}

		}

		public int TouchCount {
				get {
						if (helper.IsMobile) {
								return Input.touchCount; 
						} else {
								// if its not consdered to be mobile then query the left mouse button, returning 1 if down or 0 if not  
								if (Input.GetMouseButton (0)) {
										return 1; 	
								} else {
										return 0; 
								}
						}
				}	
		}
	
		public int TouchDownCount {
				get {
						if (helper.IsMobile) {
								int currentTouchDownCount = 0; 
								foreach (Touch touch in Input.touches) {
										if (touch.phase == TouchPhase.Began) {
												currentTouchDownCount++; 	
										}
								}
				
								return currentTouchDownCount;
						} else {
								if (Input.GetMouseButtonDown (0)) {
										return 1; 	
								} else {
										return 0; 
								}
						}
				}
		}
}
