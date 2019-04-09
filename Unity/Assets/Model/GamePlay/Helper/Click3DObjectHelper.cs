using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ETModel;

public class Click3DObjectHelper : MonoBehaviour,IPointerClickHandler
{
    public string objectId;
    public string virCamName;

    public void OnPointerClick(PointerEventData eventData)
    {
        Game.EventSystem.Run(EventIdType.ClickSceneObject, objectId,virCamName);
    }

}
