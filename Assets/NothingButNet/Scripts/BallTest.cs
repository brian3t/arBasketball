using UnityEngine;
using System.Collections;

public class BallTest : MonoBehaviour {

	public Ball ball;
	// Use this for initialization
	void Start () {
		ball.OnNet += Handle_OnNet;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void Handle_OnNet(){
		Debug.Log ("Scored");
		}
}
