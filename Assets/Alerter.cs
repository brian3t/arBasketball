using UnityEngine;
using System.Collections;
using System.Text; 

[RequireComponent (typeof (TextMesh))]
public class Alerter : MonoBehaviour {
	
	/// <summary>
	/// time to traverse through the characters 
	/// </summary>
	private float _delayPerChar = 0.15f; 
	
	/// <summary>
	/// how long the messgae will be displayed for (once fully written)
	/// </summary>
	private float _delay = 1.0f; 		
	
	/// <summary>
	/// elapsedTime being displayed 
	/// </summary>
	private float _charElapsedTime = 0.0f; 
	
	/// <summary>
	/// how long the text has been displayed for 
	/// </summary>
	private float _elapsedTime = 0.0f; 
	
	/// <summary>
	/// local cache of the attached text mesh component 
	/// </summary>
	private TextMesh _textMesh; 
	
	/// <summary>
	/// the text to display 
	/// </summary>
	public string _textToDisplay = "";
	
	/// <summary>
	/// The index of the current char (_textToDisplay)
	/// </summary>
	private int _currentCharIndex = 0; 		
	
	/// <summary>
	/// what is currently being displayed 
	/// </summary>
	private StringBuilder _text = new StringBuilder(); 
	
	void Awake(){
		_textMesh = GetComponent<TextMesh>(); 		
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( _text.Length < _textToDisplay.Length ){
			_charElapsedTime += Time.deltaTime; 
			
			if( _charElapsedTime >= _delayPerChar ){
				_text.Append( _textToDisplay[_currentCharIndex++] );
				_textMesh.text = _text.ToString();		
				_charElapsedTime = 0.0f; 	
			}
			
		} else{
			_elapsedTime += Time.deltaTime; 
			
			if( _elapsedTime >= _delay ){
				Hide(); 	
			}	
		}
	}
	
	/// <summary>
	/// Show the specified text, delayPerChar and delay.
	/// </summary>
	/// <param name='text'>
	/// Text.
	/// </param>
	/// <param name='delayPerChar'>
	/// Delay per char.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public void Show( string text, float delayPerChar, float delay ){
		gameObject.SetActive(true);
		
		// positin in front of the camera (excluding z)
		Vector3 position = Camera.main.transform.position; 
		position.z = transform.position.z; 
		transform.position = position; 
		
		_textToDisplay = text; 
		_currentCharIndex = 0; 
		
		if( _text.Length > 0 ){
			_text.Remove( 0, _text.Length ); 
		}		
		
		_charElapsedTime = 0.0f; 				
		_elapsedTime = 0.0f; 
		
	}
	
	/// <summary>
	/// Force this game object to hide i.e. deactivate itself and all associated components (including children) after
	/// resetting the variables 
	/// </summary>
	public void Hide(){
		if( _text.Length > 0 ){
			_text.Remove( 0, _text.Length ); 
		}		
		_textMesh.text = _text.ToString();
		gameObject.SetActive(false); 
		
	}
	
	/// <summary>
	/// Gets a value indicating whether this instance is showing (active).
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance is showing; otherwise, <c>false</c>.
	/// </value>
	public bool IsShowing{
		get{
			return gameObject.activeSelf; 	
		}
	}
	
}