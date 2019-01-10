using UnityEngine;
using System.Collections;
[System.Serializable]
public class Helper{
	public bool IsMobile{
		get{
			return (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android); 	
		}
	}
}
