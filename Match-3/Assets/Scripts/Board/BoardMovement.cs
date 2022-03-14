using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using static Utility.Addition;

[RequireComponent(typeof(Board))]
public class BoardMovement : MonoBehaviour
{
    private Board board;

    [HideInInspector] public bool inMoving = false;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    public async void Replacement(Vector2Int firstPosInBoard, Direction direct)
    {
        inMoving = true;
        Vector2Int secondPosInBoard = firstPosInBoard + DirectionToVector(direct);

        if ((secondPosInBoard.x < 0) || (secondPosInBoard.x >= board.SizeX) || (secondPosInBoard.y < 0) || (secondPosInBoard.y >= board.SizeY))
            return;

        Swap(firstPosInBoard, secondPosInBoard);

        GameObject[] squares = new GameObject[2] { board[firstPosInBoard.x, firstPosInBoard.y], board[secondPosInBoard.x, secondPosInBoard.y] };
        await CheckMovement(squares);

        inMoving = false;

        Debug.Log("Swapped");
    }

    private void Swap(Vector2Int firstPosInBoard, Vector2Int secondPosInBoard)
    {
        Vector2 firstPostion = board[firstPosInBoard.x, firstPosInBoard.y].transform.position;
        Vector2 secondPostion = board[secondPosInBoard.x, secondPosInBoard.y].transform.position;

        board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SquareMovement>().MoveTo(secondPostion);
        board[secondPosInBoard.x, secondPosInBoard.y].GetComponent<SquareMovement>().MoveTo(firstPostion);

        board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SquareController>().setPositionInBoard(secondPosInBoard);
        board[secondPosInBoard.x, secondPosInBoard.y].GetComponent<SquareController>().setPositionInBoard(firstPosInBoard);

        GameObject obj = board[firstPosInBoard.x, firstPosInBoard.y];
        board[firstPosInBoard.x, firstPosInBoard.y] = board[secondPosInBoard.x, secondPosInBoard.y];
        board[secondPosInBoard.x, secondPosInBoard.y] = obj;
    }

    private async Task<object> CheckMovement(GameObject[] squares)
    {
        int count = squares.Length;

        SquareMovement[] squareMovements = new SquareMovement[count];
        for (int i = 0; i < count; i++)
            squareMovements[i] = squares[i].GetComponent<SquareMovement>();

        bool all = true;
        while (all)
        {
            all = false;

            for (int i = 0; i < count; i++)
                all = all || squareMovements[i].isMoving;

            await Task.Yield();
        }

        return null;
    }
    
}
