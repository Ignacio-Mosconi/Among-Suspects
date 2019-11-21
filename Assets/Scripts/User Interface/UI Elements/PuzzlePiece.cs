using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class PuzzlePiece : MonoBehaviour
{
    [SerializeField, Range(0, 15)] uint pieceIndex;

    public bool IsMovable { get; set; } = false;

    RectTransform rectTransform;
    Vector2 gridCoordinates;
    Coroutine dispacingRoutine;
    Button button;

    UnityEvent onFinishDisplacement = new UnityEvent();

    const float NegligibleDistance = 0.01f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    IEnumerator DisplaceTo(Vector2 targetPosition, float maxSpeed, float smoothTime)
    {
        Vector2 currentVelocity = Vector2.zero;

        while ((rectTransform.anchoredPosition - targetPosition).sqrMagnitude > NegligibleDistance * NegligibleDistance)
        {
            rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, 
                                                                targetPosition, 
                                                                ref currentVelocity,
                                                                smoothTime,
                                                                maxSpeed);
            yield return new WaitForEndOfFrame();
        }

        rectTransform.anchoredPosition = targetPosition;
        onFinishDisplacement.Invoke();
        dispacingRoutine = null;
    }

    public void MoveTo(Vector2 targetPosition, float maxSpeed, float smoothTime)
    {
        Debug.Log("Previous Coords: " + GridCoordinates);

        if (Mathf.Abs(targetPosition.x - rectTransform.anchoredPosition.x) > NegligibleDistance)
            gridCoordinates.x += (targetPosition.x > rectTransform.anchoredPosition.x) ? 1 : -1;
        else
            gridCoordinates.y += (targetPosition.y < rectTransform.anchoredPosition.y) ? 1 : -1;

        dispacingRoutine = StartCoroutine(DisplaceTo(targetPosition, maxSpeed, smoothTime));

        Debug.Log("New Coords: " + GridCoordinates);
    }

    public void AddButtonClickAction(UnityAction unityAction)
    {
        button.onClick.AddListener(unityAction);
    }

    public void SetGridCoordinates(Vector2 coordinates)
    {
        gridCoordinates = coordinates;
    }

    #region Properties

    public UnityEvent OnFinishDisplacement
    {
        get { return onFinishDisplacement; }
    }
    
    public Vector2 GridCoordinates
    {
        get { return gridCoordinates; }
    }

    public RectTransform RectTransform
    {
        get { return rectTransform; }
    }
    
    public uint PieceIndex
    {
        get { return pieceIndex; }
    }
    
    #endregion
}