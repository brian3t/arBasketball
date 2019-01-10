using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(LeaderboardController))]
public class GameMenuController : MonoBehaviour
{

		public Texture2D backgroundTex;
		public Texture2D playButtonTex;
		public Texture2D resumeButtonTex;
		public Texture2D restartButtonTex;
		public Texture2D titleTex;
		public Texture2D leaderboardBgTex;
		public Texture2D loginCopyTex;
		public Texture2D fbButtonTex;
		public Texture2D instructionsTex;
		public GUISkin gameMenuGUISkinForSmall;
	public GUISkin gameMenuGUISkinForNormal;
	public GUISkin gameMenuGUISkinAlwaysOn;
	public float fadeSpeed = 1.0f;
		private float _globalTintAlpha = 0.0f;
		private GameController _gameController;
		private LeaderboardController _leaderboardController;
		private List<ScoreData> _scores = null;
		public const float kDesignWidth = 960f;
		public const float kDesignHeight = 640f;
		private float _scale = 1.0f;
		private Vector2 _scaleOffset = Vector2.one;
		private bool _showInstructions = false;
		private int _gamesPlayedThisSession = 0;

		void Awake ()
		{
				_gameController = GetComponent<GameController> ();
				_leaderboardController = GetComponent<LeaderboardController> ();

		}

		// Use this for initialization
		void Start ()
		{
				_scaleOffset.x = Screen.width / kDesignWidth;
				_scaleOffset.y = Screen.height / kDesignHeight;

				_scale = Mathf.Max (_scaleOffset.x, _scaleOffset.y);

				_leaderboardController.OnScoresLoaded += Handle_LeaderboardControlerOnScoresLoaded;
				_leaderboardController.FetchScores ();

		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void Handle_LeaderboardControlerOnScoresLoaded (List<ScoreData>scoresLoaded)
		{
				_scores = scoresLoaded;
		}

		void OnGUI ()
		{
				if (_scale < 1) {
						GUI.skin = gameMenuGUISkinForSmall; 
				} else {
						GUI.skin = gameMenuGUISkinForNormal;	
				}
				_globalTintAlpha = Mathf.Min (1.0f, Mathf.Lerp (_globalTintAlpha, 1.0f, Time.deltaTime * fadeSpeed));
				Color c = GUI.contentColor;
				c.a = _globalTintAlpha;
				GUI.contentColor = c;

				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTex); 
		
				if (_gameController.State == GameController.GameStateEnum.Paused) {				
						if (GUI.Button (new Rect (77 * _scaleOffset.x, 345 * _scaleOffset.y, 130 * _scale, 130 * _scale), resumeButtonTex, GUIStyle.none)) {
								_gameController.ResumeGame (); 
						}
			
						if (GUI.Button (new Rect (229 * _scaleOffset.x, 357 * _scaleOffset.y, 100 * _scale, 100 * _scale), restartButtonTex, GUIStyle.none)) {
								_gameController.StartNewGame (); 				
						}
				} else {
						if (GUI.Button (new Rect (77 * _scaleOffset.x, 345 * _scaleOffset.y, 130 * _scale, 130 * _scale), playButtonTex, GUIStyle.none)) {
								if (_showInstructions || _gamesPlayedThisSession > 0) {
										_showInstructions = false; 
										_gamesPlayedThisSession++; 
										_gameController.StartNewGame (); 
								} else {
										_showInstructions = true; 	
								}
						}
				}

				if (_showInstructions) {		
						GUI.DrawTexture (new Rect (67 * _scaleOffset.x, 80 * _scaleOffset.y, 510 * _scale, 309 * _scale), instructionsTex);										
				} else {
						GUI.DrawTexture (new Rect (67 * _scaleOffset.x, 188 * _scaleOffset.y, 447 * _scale, 113 * _scale), titleTex);
				}

				GUI.BeginGroup (new Rect (Screen.width - (214 + 10) * _scale, (Screen.height - (603 * _scale)) / 2, 215 * _scale, 603 * _scale)); 
		
				GUI.DrawTexture (new Rect (0, 0, 215 * _scale, 603 * _scale), leaderboardBgTex);
		
				Rect leaderboardTable = new Rect (17 * _scaleOffset.x, 50 * _scaleOffset.y, 180 * _scale, 534 * _scale); 
				if (_leaderboardController.IsFacebookAvailable && !_leaderboardController.IsLoggedIn) {			
						leaderboardTable = new Rect (17 * _scaleOffset.x, 50 * _scaleOffset.y, 180 * _scale, 410 * _scale); 
						GUI.DrawTexture (new Rect (29 * _scaleOffset.x, 477 * _scaleOffset.y, 156 * _scale, 42 * _scale), loginCopyTex);
						if (GUI.Button (new Rect (41 * _scaleOffset.x, 529 * _scaleOffset.y, 135 * _scale, 50 * _scale), fbButtonTex, GUIStyle.none)) {
								_leaderboardController.LoginToFacebook (); 
						}			
				} 			
				GUI.BeginGroup (leaderboardTable); 			
				if (_scores != null) {
						for (int i=0; i<_scores.Count; i++) {
								Rect nameRect = new Rect (5 * _scaleOffset.x, (20 * _scaleOffset.y) + i * 35 * _scale, 109 * _scale, 35 * _scale); 
								Rect scoreRect = new Rect (139 * _scaleOffset.x, (20 * _scaleOffset.y) + i * 35 * _scale, 52 * _scale, 35 * _scale); 
				
								GUI.Label (nameRect, _scores [i].name); 
								GUI.Label (scoreRect, _scores [i].points.ToString ()); 
						}
				}						
				GUI.EndGroup (); 	
				GUI.EndGroup ();
		
		}

		public void Show ()
		{
				// ignore if you are already enabled
				if (this.enabled) {
						return; 	
				}
				_globalTintAlpha = 0.0f; 
				_leaderboardController.FetchScores (); 
				this.enabled = true; 
		}
	
		public void Hide ()
		{
				this.enabled = false; 
		}
}
