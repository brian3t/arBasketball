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
/// cache of the leaderboard
/// </summary>
[System.Serializable]
public class ScoreDataCollection : object {
	
	#region singleton
	
	[System.NonSerialized]
	private static ScoreDataCollection _instance; 		
	
	public static ScoreDataCollection SharedInstance{
		get{						
			return _instance; 
		}
	}
	
	public static void InitSharedInstance(){
		if( _instance == null ){
			_instance = new ScoreDataCollection(); 	
			_instance.LoadDataCache(); 
		}
	}
	
	#endregion 
	
	private List<ScoreData> _scores = new List<ScoreData>(); 
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ScoreDataCollection"/> class.
	/// </summary>
	public ScoreDataCollection(){
			
	}
	
	public void LoadDataCache(){
		Debug.Log ( "ScoreDataCollection.LoadDataCache: START... " + Application.persistentDataPath );				
		
		if( File.Exists( Application.persistentDataPath + "/cachedscores.dat" ) ){				
			ScoreDataCollection obj = null; 
				
			try{
				using( StreamReader stream = File.OpenText( Application.persistentDataPath + "/cachedscores.dat" ) ){
					string xml = stream.ReadToEnd(); 
					obj = (ScoreDataCollection)DeserializeObject(xml); 
				}												
					
				this.Scores = obj._scores; 
			} catch( Exception e ){
				Debug.LogWarning( "Exception caught at ScoreDataCollection.LoadDataCache; " + e.Message );
			}
		}
	}
	
	private void SaveDataCache(){
		try{
			using( StreamWriter stream = File.CreateText( Application.persistentDataPath + "/cachedscores.dat" ) ){
				string xml = SerializeObject( this ); 
				stream.Write( xml ); 
			}
		} catch( Exception e ){
			Debug.LogWarning( "Exception caught at ScoreDataCollection.SaveDataCache; " + e.Message );
		}
	}
	
	public void AddScore( ScoreData score ){
		_scores.Add( score ); 
		_scores.Sort( CompareByPoints ); 
		SaveDataCache(); 
	}
	
	public List<ScoreData> Scores{
		get{
			return _scores; 	
		}
		set{
			_scores = value;
			_scores.Sort( CompareByPoints ); 
		}
	}
	
	#region Serialization methods
	
	/// <summary>
	/// Serializes the object.
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public string SerializeObject(object obj) {
	   string XmlizedString  = null;
	   MemoryStream memoryStream  = new MemoryStream();
	   XmlSerializer xs = new XmlSerializer(typeof(ScoreDataCollection));
	   XmlTextWriter xmlTextWriter  = new XmlTextWriter(memoryStream, Encoding.UTF8);
	   xs.Serialize(xmlTextWriter, obj);
	   memoryStream = xmlTextWriter.BaseStream as MemoryStream; // (MemoryStream)
	   XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
	   return XmlizedString;
	}
	
	/// <summary>
	/// Deserializes the object back into its original form
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='xmlizedString'>
	/// Xmlized string.
	/// </param>
	public object DeserializeObject( string xmlizedString ){
		XmlSerializer xs = new XmlSerializer(typeof(ScoreDataCollection));	
	   	//MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlizedString));
		StringReader stringReader = new StringReader(xmlizedString);
	    XmlTextReader xmlReader = new XmlTextReader(stringReader);
		
		//return xs.Deserialize(memoryStream);	
		
		object obj = xs.Deserialize(xmlReader); 
		
		xmlReader.Close();
    	stringReader.Close();
		
		return obj;	   
	}
	
	
	public string UTF8ByteArrayToString( byte[] characters ){     
	   UTF8Encoding encoding  = new UTF8Encoding();
	   string constructedString  = encoding.GetString(characters);
	   return (constructedString);
	}
	
	public byte[] StringToUTF8ByteArray( string xml ){
	   UTF8Encoding encoding = new UTF8Encoding();
	   byte[] byteArray = encoding.GetBytes(xml);
	   return byteArray;
	}
	
	#endregion 
	
	public static int CompareByPoints(ScoreData a, ScoreData b)
    {
        if (a == null)
        {
            if (b == null)
            {
                // If x is null and y is null, they're
                // equal. 
                return 0;
            }
            else
            {
                // If x is null and y is not null, y
                // is greater. 
                return -1;
            }
        }
        else
        {
            // If x is not null...
            //
            if (b == null)
                // ...and y is null, x is greater.
            {
                return 1;
            }
            else
            {
                return b.points.CompareTo( a.points ); 
            }
        }
    }
	
}
