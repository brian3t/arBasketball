/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;

public class SampleAppUIButton : ISampleAppUIElement
{
    public event System.Action TappedOn;
    
    /// <summary>
    /// Initializes a new instance button UI
    /// <param name='rect'> specifies button size
    /// <param name='path'> specifies path for assets to load from Resources
    public SampleAppUIButton(Rect rect, string[] path)
    {
        this.mRect = rect;
        mStyle = new GUIStyle();
        mStyle.normal.background = Resources.Load(path[0]) as Texture2D;
        mStyle.active.background = Resources.Load(path[1]) as Texture2D;
        mStyle.onNormal.background = Resources.Load(path[1]) as Texture2D;
    }

    public SampleAppUIButton(Rect rect, string[] path, string pathForImage)
    {
        this.mRect = rect;
        this.mButtonImage = Resources.Load (pathForImage) as Texture;
        mStyle = new GUIStyle();
        mStyle.normal.background = Resources.Load(path[0]) as Texture2D;
        mStyle.active.background = Resources.Load(path[1]) as Texture2D;
        mStyle.onNormal.background = Resources.Load(path[1]) as Texture2D;
        mStyle.alignment = TextAnchor.MiddleCenter;
    }
    
    public void Draw()
    {
        if(mButtonImage != null)
        {
            if(GUI.Button(mRect, mButtonImage, mStyle))
            {
                if(this.TappedOn != null){
                    this.TappedOn();
                    InputController.GUIInput = true;
                }
            }
        }
        else 
        {
            if(GUI.Button(mRect, "", mStyle))
            {
                if(this.TappedOn != null){
                    this.TappedOn();
                    InputController.GUIInput = true;
                }
            }
        }
    }

    private Texture mButtonImage;
    private Rect mRect;
    private GUIStyle mStyle;
}
