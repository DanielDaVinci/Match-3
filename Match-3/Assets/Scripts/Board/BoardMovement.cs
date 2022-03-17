using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using static Utility.Addition;

[RequireComponent(typeof(Board))]
[RequireComponent(typeof(BoardController))]
public class BoardMovement : MonoBehaviour
{
    private Board board;
    private BoardController boardController;

    [HideInInspector]
    public bool inMoving
    {
        get;
        private set;
    }

    private void Awake()
    {
        inMoving = false;
        board = GetComponent<Board>();
        boardController = GetComponent<BoardController>();
    }

    public async void Displace(Vector2Int firstPosInBoard, Direction direct)
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
        List<GameObject> firstAndSecond = new List<GameObject> { board[firstPosInBoard.x, firstPosInBoard.y], board[secondPosInBoard.x, secondPosInBoard.y] };

        Swap(firstPosInBoard, secondPosInBoard);

        List<GameObject> destroyObjects = new List<GameObject>();
        List<GameObject> firstDestroyObjects = CheckMatch(firstPosInBoard);
        List<GameObject> secondDestroyObjects = CheckMatch(secondPosInBoard);
        destroyObjects.AddRange(firstDestroyObjects);
        destroyObjects.AddRange(secondDestroyObjects);

        await WaitEndMovement(firstAndSecond);

        if (firstDestroyObjects.Count < 3 && secondDestroyObjects.Count < 3)
        {
            Swap(firstPosInBoard, secondPosInBoard);
            await WaitEndMovement(firstAndSecond);

            inMoving = false;
            return;
        }

        DestroyObjects(destroyObjects);
        await Task.Yield(); // wait next frame
        MakeFallingObjects();

        inMoving = false;

        Debug.Log("Ok");
    }

    private void Swap(Vector2Int firstPosInBoard, Vector2Int secondPosInBoard, bool withSecond = true)
    {
        Vector2 firstPostion = board[firstPosInBoard.x, firstPosInBoard.y].transform.position;
        Vector2 secondPostion;
        if (withSecond)
            secondPostion = board[secondPosInBoard.x, secondPosInBoard.y].transform.position;
        else
            secondPostion = boardController.GetRelativePosition(secondPosInBoard);

        board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SquareMovement>().MoveTo(secondPostion);
        if (withSecond) board[secondPosInBoard.x, secondPosInBoard.y].GetComponent<SquareMovement>().MoveTo(firstPostion);

        board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SquareController>().setPositionInBoard(secondPosInBoard);
        if (withSecond) board[secondPosInBoard.x, secondPosInBoard.y].GetComponent<SquareController>().setPositionInBoard(firstPosInBoard);

        GameObject obj = board[firstPosInBoard.x, firstPosInBoard.y];
        if (withSecond) board[firstPosInBoard.x, firstPosInBoard.y] = board[secondPosInBoard.x, secondPosInBoard.y];
        board[secondPosInBoard.x, secondPosInBoard.y] = obj;
    }

    private async Task WaitEndMovement(List<GameObject> objects) // Waiting until these objects stop 
    {
        SquareMovement[] squareMovements = new SquareMovement[objects.Count];
        for (int i = 0; i < objects.Count; i++)
            squareMovements[i] = objects[i].GetComponent<SquareMovement>();

        for (bool all = true; all;)
        {
            all = false;

            for (int i = 0; i < objects.Count; i++)
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

        result = RemoveDuplicate(result);
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

        if (result.Count == 1)
            return new List<GameObject>();

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

    private List<GameObject> RemoveDuplicate(List<GameObject> objects)
    {
        IEnumerable<GameObject> enumerable = objects.Distinct();
        List<GameObject> noDuplicateList = new List<GameObject>();

        foreach (GameObject obj in enumerable)
            noDuplicateList.Add(obj);

        return noDuplicateList;
    }

    private void DestroyObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private void MakeFallingObjects()
    {
        Vector2 size = board.Prefab.GetComponent<SpriteRenderer>().size * board.Prefab.transform.localScale;

        for (int i = 0; i < board.SizeX; i++)
        {
            int countMadeObjects = 0;
            for (int j = 0; j < board.SizeY; j++)
            {
                if (board[i, j] == null)
                {
                    int posY = board.SizeY + countMadeObjects;
                    Vector2 position = boardController.GetRelativePosition(new Vector2Int(i, posY), size);

                    board[i, posY] = Instantiate(board.Prefab);
                    board[i, posY].transform.SetParent(transform, false);
                    board[i, posY].transform.localPosition = position;

                    SquareController sqController = board[i, posY].GetComponent<SquareController>();
                    sqController.setPositionInBoard(new Vector2Int(i, posY));

                    Debug.Log("asd");

                    countMadeObjects++;
                }
            }
        }
    }
}
