using UnityEngine;
using System; 
using System.Collections;
using System.Collections.Generic;
using System.IO; 
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Data object that contains score data for a single score entry 
/// </summary>
[System.Serializable]
public class ScoreData : object
{
	/// <summary>
	/// name of the person (YOU if local) 
	/// </summary>
	public string name = ""; 
	/// <summary>
	/// points for this entry 
	/// </summary>
	public int points = 0; 
	/// <summary>
	/// date the score was added 
	/// </summary>
	public string date = DateTime.Today.ToString( "dd-MM-yy" );
	/// <summary>
	/// associated fb id 
	/// </summary>
	public string fbId = "";
	/// <summary>
	/// score belongs to the local user 
	/// </summary>
	public bool local = true; 
	
	public ScoreData(){
		
	}					
}

