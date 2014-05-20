using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]
[RequireComponent (typeof (Rigidbody))]


public class Ball : MonoBehaviour {

	private Rigidbody _rigidBody;
	private Transform _transform;
	private SphereCollider _sphereCollider;

	private GameController _gameController;

	public delegate void Net();
	public Net OnNet = null;

	void Awake(){
		_transform = GetComponent<Transform> ();
		_rigidBody = GetComponent<Rigidbody> ();
		_sphereCollider = GetComponent<SphereCollider> ();

		}

	// Use this for initialization
	void Start () {
		_gameController = GameController.SharedInstance;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter(Collision collision){
		Debug.Log ("Ball collided with " + collision.gameObject.name);
		_gameController.OnBallCollisionEnter (collision);
		}

	public void OnTriggerEnter(Collider collider){
		Debug.Log ("Ball trigger with " + collider.name.ToString ());
		if (collider.transform.name.Equals("LeftHoop_001")){
			if (OnNet != null){
				OnNet();
			}
		}
	}

	public Rigidbody BallRigidbody{
		get{
			return _rigidBody;
		}
	}
	public Transform BallTransform{
		get {
			return _transform;
		}
	}
	public SphereCollider BallCollider{
		get {
			return _sphereCollider;
		}
	}
}
