using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]
[RequireComponent (typeof (Rigidbody))]


public class Ball : MonoBehaviour {
	
	private Rigidbody _rigidBody;
	private Transform _transform;
	private Renderer _renderer;
	private SphereCollider _sphereCollider;
	private const float FrictionDown = 45f;
	private AudioSource[] _audioSources;
	private AudioSource _shootAs;
	private AudioSource _hitAs;
	private Vector3 tilt;
	private float rollingSpeed = 600f;

	private GameController _gameController;
	
	public delegate void Net();
	public Net OnNet = null;
	
	void Awake(){
		_transform = GetComponent<Transform> ();
		_rigidBody = GetComponent<Rigidbody> ();
		_sphereCollider = GetComponent<SphereCollider> ();
		_renderer = GetComponent<Renderer> ();

		_audioSources = GetComponents<AudioSource>();
		_shootAs = _audioSources [0];
		_hitAs = _audioSources [1];
	}
	
	// Use this for initialization
	void Start () {
		_gameController = GameController.SharedInstance;
		
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_METRO || UNITY_METRO_8_0 || UNITY_WINRT
		
		tilt.x = Input.acceleration.y;
		
		tilt.z = -Input.acceleration.x;
		
		
		
		if (tilt.sqrMagnitude > 1)
			
			tilt.Normalize();
		
		
		
		_rigidBody.AddForce(tilt * speed * Time.deltaTime);
		
		#else
		
//		tilt.z = Input.acceleration.y;
		
		tilt.x = Input.acceleration.x;
		
//		//todo remove debug
//		tilt.x = 0.02f;
		
		if (tilt.sqrMagnitude > 1)
			
			tilt.Normalize();

		_rigidBody.AddForce(tilt*rollingSpeed);
		
		#endif
	}
	
	public void OnCollisionEnter(Collision collision){
		Debug.Log ("Ball collided with " + collision.gameObject.name);
		if (collision.gameObject.name == "LeftHoop") {
			_rigidBody.AddForce(FrictionDown * Physics.gravity * rigidbody.mass);
				}
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
	public void Hide(){
		_renderer.enabled = false;
	}
	public void Show(){
		_renderer.enabled = true;
	}

	public Rigidbody BallRigidbody{
		get{
			return _rigidBody;
//			return null;
		}
	}
	public Transform BallTransform{
		get {
			return _transform;
//			return null;
		}
		set{
			_transform = value;
		}
	}
	public SphereCollider BallCollider{
		get {
			return _sphereCollider;
//			return null;

		}
	}
	public void PlayShootSound(){
		_shootAs.Play ();
	}
	public void PlayTouchSound(){
		_hitAs.Play ();
	}
}
