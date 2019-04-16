using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviorEventHelpr : MonoBehaviour
{
    public Action<bool> appFocusEvent;

    private void OnApplicationFocus(bool focus)
    {
        if (appFocusEvent != null)
        {
            appFocusEvent(focus);
        }
    }
}
