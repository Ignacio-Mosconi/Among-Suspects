using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class PuzzlePiece : MonoBehaviour
{
    [SerializeField, Range(0, 15)] uint pieceIndex = default;

    public bool IsMovable { get; set; } = false;

    RectTransform rectTransform;
    Vector2 gridCoordinates;
    Coroutine displacingRoutine;
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
        displacingRoutine = null;
    }

    public void MoveTo(BoardPosition boardPosition, float maxSpeed, float smoothTime)
    {
        gridCoordinates = boardPosition.gridCoordinates;

        displacingRoutine = StartCoroutine(DisplaceTo(boardPosition.spacePosition, maxSpeed, smoothTime));
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