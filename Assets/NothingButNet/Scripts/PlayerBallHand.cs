using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class PlayerBallHand : MonoBehaviour {


	private Player _player = null;

	void Awake(){
		}

	// Use this for initialization
	void Start () {
		//look up parents to search for _player component
		Transform parent = transform.parent;
		while (parent != null && _player == null) {
			Player parentPlayer = parent.GetComponent<Player>();
			if (parentPlayer == null){
				parent = parent.parent;
			}
			else {
					_player = parentPlayer;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//notify parents on trigger
	public void OnTriggerEnter(Collider collider){
//		_player.OnTriggerEnter (collider);
	}
}
