using UnityEngine;
using System.Collections;

public class ScoreBoardTest : MonoBehaviour {

	public ScoreBoard scoreBoard;
	// Use this for initialization
	void Start () {
		scoreBoard.setPoints ("100");
		scoreBoard.SetTime (120);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
