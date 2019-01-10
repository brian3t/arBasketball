using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class LeaderboardController : MonoBehaviour {
			
	public delegate void ScoresLoaded(List<ScoreData> scores);
	public ScoresLoaded OnScoresLoaded = null;
	
	void Awake(){
		StartCoroutine("DoInitScoreDataCollection");
	}
	
	// Use this for initialization
	void Start () {
		 			
	}
		
	
	private IEnumerator DoInitScoreDataCollection(){
		ScoreDataCollection.InitSharedInstance(); 
		yield return true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public bool IsLoggedIn{
		get{
			return false; 	
		}
	}
	
	public bool IsFacebookAvailable{
		get{
			return true; 	
		}
	}
	
	public void LoginToFacebook(){
			
	}
	
	public void AddPlayersScore( int score ){
		// ignore if the score is 0 
		if( score == 0 ){
			return; 	
		}
		
		Debug.Log ( "Adding players score" );
		
		// add score 
		ScoreData newScore = new ScoreData();
		newScore.local = true; 
		newScore.name = "YOU"; 
		newScore.points = score; 
		newScore.fbId = PlayersFacebookId; 
			
		if( IsLoggedIn && NetworkAvailable ){
			PlayerPrefs.DeleteKey( "syncTimestamp" ); // force a sync for the next score request 
			// TODO; Sync scores with server 
		} else{
			Debug.Log ( "Adding players score to ScoreDataCollection.SharedInstance" );
			ScoreDataCollection.SharedInstance.AddScore( newScore ); 			
		}
	}	
	
	public string PlayersFacebookId{
		get{
			if( PlayerPrefs.HasKey( "fbId" ) ){
				return PlayerPrefs.GetString( "fbId" ); 	
			} else {
				return "";
			}
		}
		set{
			PlayerPrefs.SetString( "fbId", value ); 
		}
	}
	
	public bool NetworkAvailable{
		get{
			return Application.internetReachability != NetworkReachability.NotReachable; 			
		}
	}
	
	public bool SyncRequired{
		get{
			if( !IsLoggedIn || !NetworkAvailable ){
				return false; 	
			}
			
			bool syncRequired = false; 
			System.DateTime currentDate = System.DateTime.Now;
			
			if( !PlayerPrefs.HasKey( "syncTimestamp" ) ){				
				syncRequired = true; 
			} else{
	        	long temp = long.Parse( PlayerPrefs.GetString( "syncTimestamp" ) );
	
	        	//Convert the old time from binary to a DataTime variable
	        	System.DateTime lastSyncDate = System.DateTime.FromBinary(temp);
						
				System.TimeSpan diff = currentDate.Subtract( lastSyncDate );
				
				if( diff.TotalMinutes > 10.0f ){
					syncRequired = true; 
				}						
			}
			
			return syncRequired; 
		}
	}
	
	public void FetchScores(){
		StartCoroutine("DoFetchScores"); 
	}
	
	private IEnumerator DoFetchScores(){
		if( SyncRequired ){
			// TODO; Fetch and Sync scores 
			
		} else{
			if( OnScoresLoaded != null ){
				OnScoresLoaded( ScoreDataCollection.SharedInstance.Scores ); 	
			}			
		}
		
		yield return true;
	}
}
