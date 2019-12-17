using UnityEngine;
using UnityEngine.EventSystems;

public class ExtendedInputModule : StandaloneInputModule
{
    public GameObject GetHoveredObject(int pointerId = StandaloneInputModule.kMouseLeftId)
    {
        GameObject hoveredObject = null;
        PointerEventData lastPointerData = GetLastPointerEventData(pointerId);
        
        if (lastPointerData != null)
            hoveredObject = lastPointerData.pointerCurrentRaycast.gameObject;
        
        return hoveredObject;
    }
}