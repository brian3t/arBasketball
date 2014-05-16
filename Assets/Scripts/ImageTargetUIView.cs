/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;

public class ImageTargetUIView : ISampleAppUIView {
    
    #region PUBLIC_PROPERTIES
    public CameraDevice.FocusMode FocusMode
    {
        get {
            return m_focusMode;
        }
        set {
            m_focusMode = value;
        }
    }
    #endregion PUBLIC_PROPERTIES
    
    #region PUBLIC_MEMBER_VARIABLES
    public event System.Action TappedToClose;
    public SampleAppUIBox mBox;
    public SampleAppUICheckButton mAboutLabel;
    public SampleAppUILabel mImageTargetLabel;
    public SampleAppUICheckButton mExtendedTracking;
    public SampleAppUICheckButton mCameraFlashSettings;
    public SampleAppUICheckButton mAutoFocusSetting;
    public SampleAppUILabel mCameraLabel;
    public SampleAppUIRadioButton mCameraFacing;
    public SampleAppUIRadioButton mDataSet;
    public SampleAppUILabel mDataSetLabel;
    public SampleAppUIButton mCloseButton;
    #endregion PUBLIC_MEMBER_VARIABLES
    
    #region PRIVATE_MEMBER_VARIABLES
    private CameraDevice.FocusMode m_focusMode;
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region PUBLIC_METHODS
    
    public void LoadView()
    {
        mBox = new SampleAppUIBox(SampleAppUIConstants.BoxRect, SampleAppUIConstants.MainBackground);
        
        mImageTargetLabel = new SampleAppUILabel(SampleAppUIConstants.RectLabelOne, SampleAppUIConstants.ImageTargetLabelStyle);
        
        string[] aboutStyles = { SampleAppUIConstants.AboutLableStyle, SampleAppUIConstants.AboutLableStyle };
        mAboutLabel = new SampleAppUICheckButton(SampleAppUIConstants.RectLabelAbout, false, aboutStyles);
        
        string[] offTargetTrackingStyles = { SampleAppUIConstants.ExtendedTrackingStyleOff, SampleAppUIConstants.ExtendedTrackingStyleOn };
        mExtendedTracking = new SampleAppUICheckButton(SampleAppUIConstants.RectOptionOne, false, offTargetTrackingStyles);
        
        string[] cameraFlashStyles = {SampleAppUIConstants.CameraFlashStyleOff, SampleAppUIConstants.CameraFlashStyleOn};
        mCameraFlashSettings = new SampleAppUICheckButton(SampleAppUIConstants.RectOptionThree, false, cameraFlashStyles);
        
        string[] autofocusStyles = {SampleAppUIConstants.AutoFocusStyleOff, SampleAppUIConstants.AutoFocusStyleOn};
        mAutoFocusSetting = new SampleAppUICheckButton(SampleAppUIConstants.RectOptionTwo, false, autofocusStyles);
        
        mCameraLabel = new SampleAppUILabel(SampleAppUIConstants.RectLabelTwo, SampleAppUIConstants.CameraLabelStyle);
        
        string[,] cameraFacingStyles = new string[2,2] {{SampleAppUIConstants.CameraFacingFrontStyleOff, SampleAppUIConstants.CameraFacingFrontStyleOn},{ SampleAppUIConstants.CameraFacingRearStyleOff, SampleAppUIConstants.CameraFacingRearStyleOn}};
        SampleAppUIRect[] cameraRect = { SampleAppUIConstants.RectOptionFour, SampleAppUIConstants.RectOptionFive };
        mCameraFacing = new SampleAppUIRadioButton(cameraRect, 1, cameraFacingStyles);
        
        string[,] datasetStyles = new string[2,2] {{SampleAppUIConstants.StonesAndChipsStyleOff, SampleAppUIConstants.StonesAndChipsStyleOn}, {SampleAppUIConstants.TarmacOff, SampleAppUIConstants.TarmacOn}};
        SampleAppUIRect[] datasetRect = { SampleAppUIConstants.RectOptionSix, SampleAppUIConstants.RectOptionSeven};
        mDataSet = new SampleAppUIRadioButton(datasetRect, 0, datasetStyles);
        
        mDataSetLabel = new SampleAppUILabel(SampleAppUIConstants.RectLabelThree, SampleAppUIConstants.DatasetLabelStyle);
        
        string[] closeButtonStyles = {SampleAppUIConstants.closeButtonStyleOff, SampleAppUIConstants.closeButtonStyleOn };
        mCloseButton = new SampleAppUIButton(SampleAppUIConstants.CloseButtonRect, closeButtonStyles);    
    }
    
    public void UnLoadView()
    {
        mAboutLabel = null;
        mImageTargetLabel = null;
        mExtendedTracking = null;
        mCameraFlashSettings = null;
        mAutoFocusSetting = null;
        mCameraLabel = null;
        mCameraFacing = null;
        mDataSet = null;
        mDataSetLabel = null;
    }
    
    public void UpdateUI(bool tf)
    {
        if(!tf)
        {
            return;
        }
        
        mBox.Draw();
        mAboutLabel.Draw();
        mImageTargetLabel.Draw();
        mExtendedTracking.Draw();
        mCameraFlashSettings.Draw();
        mAutoFocusSetting.Draw();
        mCameraLabel.Draw();
        mCameraFacing.Draw();
        mDataSet.Draw();
        mDataSetLabel.Draw();
        mCloseButton.Draw();
    }

    public void OnTappedToClose ()
    {
        if(this.TappedToClose != null)
        {
            this.TappedToClose();
        }
    }
    #endregion PUBLIC_METHODS
}

