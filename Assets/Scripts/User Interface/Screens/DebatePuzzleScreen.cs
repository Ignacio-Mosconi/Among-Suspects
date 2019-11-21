using System;
using UnityEngine;
using UnityEngine.EventSystems;

public struct BoardPosition
{
    public Vector2 spacePosition;
    public Vector2 gridCoordinates;
}

[RequireComponent(typeof(AnimatedMenuScreen))]
public class DebatePuzzleScreen : MonoBehaviour
{
    [Header("Board Properties")]
    [SerializeField] GameObject puzzlePiecesContainer = default;
    [SerializeField, Range(1, 4)] uint rows = 4;
    [SerializeField, Range(1, 4)] uint columns = 4;

    [Header("Piece Properties")]
    [SerializeField, Range(500f, 1000f)] float piecesMoveSpeed = 700f;
    [SerializeField, Range(0.1f, 0.3f)] float piecesSmoothTime = 0.2f;

    BoardPosition[,] board;
    PuzzlePiece[] puzzlePieces;
    PuzzlePiece[] movablePieces = new PuzzlePiece[4];
    BoardPosition emptyPosition;

    void Awake()
    {
        board = new BoardPosition[columns, rows];
        puzzlePieces = puzzlePiecesContainer.GetComponentsInChildren<PuzzlePiece>();
    }

    void Start()
    {
        uint pieceIndex = 0;

        for (int row = 0; row < rows; row++)
            for (int column = 0; column < columns; column++)
            {
                if (pieceIndex < puzzlePieces.Length)
                {
                    Vector2 gridCoordinates = new Vector2(column, row);

                    puzzlePieces[pieceIndex].SetGridCoordinates(gridCoordinates);
                    board[column, row].spacePosition = puzzlePieces[pieceIndex].RectTransform.anchoredPosition;
                    board[column, row].gridCoordinates = gridCoordinates;
                    pieceIndex++;
                }
            }

        float pieceWidth = puzzlePieces[0].RectTransform.rect.width;

        board[columns - 1, rows - 1].spacePosition = puzzlePieces[puzzlePieces.Length - 1].RectTransform.anchoredPosition + 
                                                    new Vector2(pieceWidth, 0f);
        board[columns - 1, rows - 1].gridCoordinates = new Vector2(columns - 1, rows - 1);

        emptyPosition = board[columns - 1, rows - 1];

        SetMovablePieces();
        SetMovablePiecesAvailability(enableMovement: true);

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            PuzzlePiece puzzlePiece = puzzlePieces[i];
            Debug.Log(puzzlePiece.name);

            puzzlePiece.AddButtonClickAction(() => MovePiece(puzzlePiece));
            puzzlePiece.OnFinishDisplacement.AddListener(() => SetMovablePiecesAvailability(enableMovement: true));
            Destroy(puzzlePiece.GetComponent<EventTrigger>());
        }
    }

    void SetMovablePiecesAvailability(bool enableMovement)
    {
        for (int i = 0; i < movablePieces.Length; i++)
            if (movablePieces[i])
                movablePieces[i].IsMovable = enableMovement;
    }

    void SetMovablePieces()
    {
        Vector2 leftCoordinates = new Vector2(emptyPosition.gridCoordinates.x - 1, emptyPosition.gridCoordinates.y);
        Vector2 rightCoordinates = new Vector2(emptyPosition.gridCoordinates.x + 1, emptyPosition.gridCoordinates.y);
        Vector2 upCoordinates = new Vector2(emptyPosition.gridCoordinates.x, emptyPosition.gridCoordinates.y - 1);
        Vector2 downCoordinates = new Vector2(emptyPosition.gridCoordinates.x, emptyPosition.gridCoordinates.y + 1);

        movablePieces[0] = Array.Find(puzzlePieces, pp => pp.GridCoordinates == leftCoordinates);
        movablePieces[1] = Array.Find(puzzlePieces, pp => pp.GridCoordinates == rightCoordinates);
        movablePieces[2] = Array.Find(puzzlePieces, pp => pp.GridCoordinates == upCoordinates);
        movablePieces[3] = Array.Find(puzzlePieces, pp => pp.GridCoordinates == downCoordinates);
    }

    void MovePiece(PuzzlePiece puzzlePiece)
    {
        if (puzzlePiece.IsMovable)
        {
            Vector2 newEmptySpacePosition = puzzlePiece.RectTransform.anchoredPosition;
            Vector2 newEmptyPositionGridCoordinates = puzzlePiece.GridCoordinates;

            SetMovablePiecesAvailability(enableMovement: false);
            puzzlePiece.MoveTo(emptyPosition.spacePosition, piecesMoveSpeed, piecesSmoothTime);
            emptyPosition.spacePosition = newEmptySpacePosition;
            emptyPosition.gridCoordinates = newEmptyPositionGridCoordinates;
            SetMovablePieces();
        }
    }
}