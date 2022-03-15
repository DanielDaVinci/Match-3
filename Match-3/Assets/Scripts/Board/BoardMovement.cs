using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Vector2Int secondPosInBoard = firstPosInBoard + DirectionToVector(direct);

        if ((secondPosInBoard.x < 0) || (secondPosInBoard.x >= board.SizeX) || (secondPosInBoard.y < 0) || (secondPosInBoard.y >= board.SizeY))
            return;

        Color firstColor = board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SpriteRenderer>().color;
        Color secondColor = board[secondPosInBoard.x, secondPosInBoard.y].gameObject.GetComponent<SpriteRenderer>().color;

        if (firstColor == secondColor)
            return;

        // --------Start of movement--------

        inMoving = true;
        GameObject[] firstAndSecond = new GameObject[2] { board[firstPosInBoard.x, firstPosInBoard.y], board[secondPosInBoard.x, secondPosInBoard.y] };

        Swap(firstPosInBoard, secondPosInBoard);

        List<GameObject> destroyObjects = new List<GameObject>();
        List<GameObject> firstDestroyObjects = CheckMatch(firstPosInBoard);
        List<GameObject> secondDestroyObjects = CheckMatch(secondPosInBoard);

        await CheckMovement(firstAndSecond);

        if (firstDestroyObjects.Count < 3 && secondDestroyObjects.Count < 3)
        {
            Swap(firstPosInBoard, secondPosInBoard);
            await CheckMovement(firstAndSecond);

            inMoving = false;
            return;
        }

        inMoving = false;

        Debug.Log("Ok");
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

    private async Task CheckMovement(GameObject[] squares) // Waiting until these objects stop 
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
    }
    
    private List<GameObject> CheckMatchAll()
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < board.SizeX; i++)
        {
            for (int j = 0; j < board.SizeY; j++)
            {
                result.AddRange(CheckMatch(new Vector2Int(i, j)));
            }
        }

        return result;
    }

    private List<GameObject> CheckMatch(Vector2Int position)
    {
        List<GameObject> result = new List<GameObject>() { board[position.x, position.y] };

        List<GameObject> resultX = new List<GameObject>();
        resultX.AddRange(Wave(position, Direction.left));
        resultX.AddRange(Wave(position, Direction.right));

        if (resultX.Count >= 2)
            result.AddRange(resultX);

        List<GameObject> resultY = new List<GameObject>();
        resultY.AddRange(Wave(position, Direction.up));
        resultY.AddRange(Wave(position, Direction.down));

        if (resultY.Count >= 2)
            result.AddRange(resultY);

        return result;
    }

    private List<GameObject> Wave(Vector2Int firstPosition, Direction direction)
    {
        Vector2Int vectorDirect = DirectionToVector(direction);
        List<GameObject> result = new List<GameObject>();

        if ((firstPosition.x + vectorDirect.x < 0 || firstPosition.x + vectorDirect.x >= board.SizeX) ||
            (firstPosition.y + vectorDirect.y < 0 || firstPosition.y + vectorDirect.y >= board.SizeY))
            return result;

        Vector2Int secondPosition = firstPosition + vectorDirect;

        Color curentColor = board[firstPosition.x, firstPosition.y].GetComponent<SpriteRenderer>().color;
        Color nextColor = board[secondPosition.x, secondPosition.y].GetComponent<SpriteRenderer>().color;

        if (curentColor == nextColor)
        {
            result.Add(board[secondPosition.x, secondPosition.y]);
            result.AddRange(Wave(secondPosition, direction));
        }
        
        return result;
    }
}
