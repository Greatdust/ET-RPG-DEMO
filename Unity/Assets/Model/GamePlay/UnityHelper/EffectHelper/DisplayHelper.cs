using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayHelper : MonoBehaviour
{
    [System.Serializable]
    public struct GoState
    {
        public GameObject go;
        public float displayDelay;
    }
    public GoState[] goStates;

    private void OnEnable()
    {
        if (goStates != null)
        {
            foreach (var v in goStates)
            {
                Display(v.go,v.displayDelay).Coroutine();
            }
        }
    }

    async ETVoid Display(GameObject go,float delayTime)
    {
        await TimerComponent.Instance.WaitAsync(delayTime);
        go.SetActive(true);
    }

    private void OnDisable()
    {
        if (goStates != null)
        {
            foreach (var v in goStates)
            {
                v.go.SetActive(false);
            }
        }
    }
}
