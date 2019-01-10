using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardControllerTest : MonoBehaviour {

	public LeaderboardController leaderboard;
	// Use this for initialization
	void Start () {
		leaderboard.OnScoresLoaded += Handle_OnScoresLoaded;
		leaderboard.AddPlayersScore (100);
		leaderboard.AddPlayersScore (200);

		leaderboard.FetchScores ();
	}

	public void Handle_OnScoresLoaded(List<ScoreData> scores){
		foreach (ScoreData score in scores) {
			Debug.Log(score.points);		
		}
		}
	
	// Update is called once per frame
	void Update () {
	
	}
}
