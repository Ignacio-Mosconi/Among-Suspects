using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PuzzleSolvingTier
{
    A, B, C, Count
}

public struct BoardPosition
{
    public Vector2 spacePosition;
    public Vector2 gridCoordinates;
    public uint currentPieceIndex;
}

[RequireComponent(typeof(AnimatedMenuScreen))]
[RequireComponent(typeof(PuzzleTimer))]
public class DebatePuzzleScreen : MonoBehaviour
{
    [Header("Board Properties")]
    [SerializeField] GridLayoutGroup puzzlePiecesContainer = default;
    [SerializeField, Range(1, 4)] uint rows = 4;
    [SerializeField, Range(1, 4)] uint columns = 4;

    [Header("Piece Properties")]
    [SerializeField, Range(500f, 1000f)] float piecesMoveSpeed = 700f;
    [SerializeField, Range(0f, 0.3f)] float piecesSmoothTime = 0.2f;

    [Header("Tier Values")]
    [SerializeField, Range(0, 15)] int[] tierMinutes = new int[(int)PuzzleSolvingTier.Count];

    [Header("Puzzle Finished")]
    [SerializeField] UIPrompt continueButtonPrompt = default;

    BoardPosition[,] board;
    PuzzlePiece[] puzzlePieces;
    PuzzlePiece[] movablePieces = new PuzzlePiece[4];
    PuzzleTimer puzzleTimer;
    BoardPosition emptyPosition;
    PuzzleSolvingTier tierAchieved;

    void Awake()
    {
        board = new BoardPosition[columns, rows];
        puzzleTimer = GetComponent<PuzzleTimer>();
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
                    board[column, row].currentPieceIndex = puzzlePieces[pieceIndex].PieceIndex;
                    pieceIndex++;
                }
            }

        float pieceWidth = puzzlePiecesContainer.cellSize.x;
        float pieceHeight = puzzlePiecesContainer.cellSize.y;
        float emptyHorizontalPosition = (pieceWidth + puzzlePiecesContainer.spacing.x) * (columns - 1) + pieceWidth * 0.5f;
        float emptyVerticalPosition = -((pieceHeight + puzzlePiecesContainer.spacing.y) * (rows - 1) + pieceHeight * 0.5f);
        Vector2 emptySpacePosition = new Vector2(emptyHorizontalPosition, emptyVerticalPosition);
        Vector2 emptyGridCoordinaes = new Vector2(columns - 1, rows - 1);

        board[columns - 1, rows - 1].spacePosition = emptySpacePosition;
        board[columns - 1, rows - 1].gridCoordinates = emptyGridCoordinaes;
        board[columns - 1, rows - 1].currentPieceIndex = pieceIndex;

        emptyPosition = board[columns - 1, rows - 1];

        SetMovablePieces();
        SetMovablePiecesAvailability(enableMovement: true);

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            PuzzlePiece puzzlePiece = puzzlePieces[i];
            EventTrigger eventTrigger = puzzlePiece.GetComponent<EventTrigger>();

            if (eventTrigger)
                Destroy(eventTrigger);

            puzzlePiece.AddButtonClickAction(() => MovePiece(puzzlePiece));
            puzzlePiece.OnFinishDisplacement.AddListener(OnPieceDisplacementFinished);
        }

        continueButtonPrompt.SetUp();
        continueButtonPrompt.Deactivate();

        GameManager.Instance.InvokeMethodInRealTime(() => puzzleTimer.StartTimer(), GetComponent<AnimatedMenuScreen>().ShowAnimationDuration);
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
            Vector2 previousPieceSpacePosition = puzzlePiece.RectTransform.anchoredPosition;
            Vector2 previousPieceGridCoordinates = puzzlePiece.GridCoordinates;

            board[(int)emptyPosition.gridCoordinates.x, (int)emptyPosition.gridCoordinates.y].currentPieceIndex = puzzlePiece.PieceIndex;
            board[(int)previousPieceGridCoordinates.x, (int)previousPieceGridCoordinates.y].currentPieceIndex = emptyPosition.currentPieceIndex;

            SetMovablePiecesAvailability(enableMovement: false);
            puzzlePiece.MoveTo(emptyPosition, piecesMoveSpeed, piecesSmoothTime);
            emptyPosition.spacePosition = previousPieceSpacePosition;
            emptyPosition.gridCoordinates = previousPieceGridCoordinates;
            SetMovablePieces();
        }
    }

    bool CheckPuzzleCompletion()
    {
        bool puzzleCompleted = true;
        int pieceIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (board[column, row].currentPieceIndex != pieceIndex)
                {
                    puzzleCompleted = false;
                    break;
                }
                pieceIndex++;
            }

            if (!puzzleCompleted)
                break;
        }

        return puzzleCompleted;
    }

    void OnPieceDisplacementFinished()
    {
        if (CheckPuzzleCompletion())
            EndPuzzle();
        else
            SetMovablePiecesAvailability(enableMovement: true);
    }

    void EndPuzzle()
    {
        continueButtonPrompt.Show();
        puzzleTimer.StopTimer();

        PuzzleTimespan timeToSolve = GetSolvingTime();

        if (timeToSolve.totalTimeInSeconds / 60f <= tierMinutes[(int)PuzzleSolvingTier.A])
            tierAchieved = PuzzleSolvingTier.A;
        else
            tierAchieved = (timeToSolve.totalTimeInSeconds / 60f <= tierMinutes[(int)PuzzleSolvingTier.B]) ?
                            PuzzleSolvingTier.B : PuzzleSolvingTier.C;
    }

    public PuzzleTimespan GetSolvingTime()
    {
        return puzzleTimer.TimeToSolve;
    }

    #region Properties

    public PuzzleSolvingTier TierAchieved
    {
        get { return tierAchieved; }
    }

    #endregion
}