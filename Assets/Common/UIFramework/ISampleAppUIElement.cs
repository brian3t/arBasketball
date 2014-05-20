/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Custom ISampleAppUIElement that encapsulates Unity GUI Elements and runs a custom Draw() call.
/// All UIElements for this application must implement this interface
/// </summary>
public interface ISampleAppUIElement 
{
    void Draw();
}

