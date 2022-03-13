using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // enum Direction
    public enum Direction
    {
        up,
        right,
        down,
        left
    }
    private Vector2Int DirectionToVector(Direction direct)
    {
        int x = direct == Direction.left ? -1 : (direct == Direction.right ? 1 : 0);
        int y = direct == Direction.down ? -1 : (direct == Direction.up ? 1 : 0);

        return new Vector2Int(x, y);
    }

    // -----------END-----------

    [SerializeField] private GameObject prefab;
    [SerializeField] private int sizeX = 2;
    [SerializeField] private int sizeY = 2;
    [SerializeField] private float distance = 1;

    private GameObject[,] board;

    public static bool inMoving;

    private void Awake()
    {
        inMoving = false;
        prefab.GetComponent<SquareController>().setBoard(gameObject);
    }

    void Start()
    {
        PositionToCenture();
        MakeBoard();
    }

    private void PositionToCenture()
    {
        Vector2 size = prefab.GetComponent<SpriteRenderer>().size * prefab.transform.localScale;

        bool even = (sizeX * sizeY) % 2 == 0;
        float deltaX = (size.x + distance) * (sizeX / 2) + (even ? -distance : size.x) / 2;
        float deltaY = (size.y + distance) * (sizeY / 2) + (even ? -distance : size.y) / 2;

        transform.position += new Vector3(-1 * deltaX, deltaY, 0);
    }

    private void MakeBoard()
    {
        board = new GameObject[sizeX, sizeY];

        Vector2 size = prefab.GetComponent<SpriteRenderer>().size * prefab.transform.localScale;

        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                float posX = i * (size.x + distance) + size.x / 2;
                float posY = j * (size.y + distance) + size.y / 2;
                Vector2 position = new Vector2(posX, -1 * posY);

                board[i, j] = Instantiate(prefab);
                board[i, j].transform.SetParent(transform, false);
                board[i, j].transform.localPosition = position;

                board[i, j].GetComponent<SquareController>().setPositionInBoard(new Vector2Int(i, j));
            }
        }
    }

    public async void Replacement(Vector2Int firstPosInBoard, Direction direct)
    {
        inMoving = true;

        Vector2Int secondPosInBoard = firstPosInBoard + DirectionToVector(direct);

        if ((secondPosInBoard.x < 0) || (secondPosInBoard.x >= sizeX) || (secondPosInBoard.y < 0) || (secondPosInBoard.y >= sizeY))
            return;

        Vector2 firstPostion = board[firstPosInBoard.x, firstPosInBoard.y].transform.position;
        Vector2 secondPostion = board[secondPosInBoard.x, secondPosInBoard.y].transform.position;

        board[firstPosInBoard.x, firstPosInBoard.y].GetComponent<SquareMovement>().MoveTo(secondPostion);
        board[secondPosInBoard.x, secondPosInBoard.y].GetComponent<SquareMovement>().MoveTo(firstPostion);

        GameObject[] squares = new GameObject[2] { board[firstPosInBoard.x, firstPosInBoard.y], board[secondPosInBoard.x, secondPosInBoard.y] };

        await CheckMovement(squares);

        Debug.Log("Ok");
    }
    
    public async Task<object> CheckMovement(GameObject[] squares)
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
