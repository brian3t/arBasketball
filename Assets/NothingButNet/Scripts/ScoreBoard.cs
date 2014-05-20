using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{

		public TextMesh pointsTextMesh;
		public TextMesh timeRemainingTextMesh;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void SetTime (float timeRemaining)
		{
				
				timeRemainingTextMesh.text = Mathf.Floor(timeRemaining/60).ToString() + ":" + (timeRemaining % 60).ToString();
		}

		public void setPoints (string points)
		{
				pointsTextMesh.text = points;
		}

}
