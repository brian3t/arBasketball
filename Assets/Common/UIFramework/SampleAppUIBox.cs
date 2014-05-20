/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;

public class SampleAppUIBox : ISampleAppUIElement
{
    /// <summary>
    /// Initializes a new instance for a box UI
    /// <param name='rect'> specifies box size
    /// <param name='path'> specifies path for assets to load from Resources
    public SampleAppUIBox(Rect rect, string path)
    {
        this.mRect = rect;
        mStyle = new GUIStyle();
        mStyle.normal.background = Resources.Load(path) as Texture2D;
    }
    
    public void Draw()
    {
        GUI.Box(mRect, "", mStyle);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            InputController.GUIInput = true;
        }
    }
    
    private Rect mRect;
    private GUIStyle mStyle;
}


