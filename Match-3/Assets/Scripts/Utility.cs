using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class Addition
    {
        public enum Direction
        {
            left, right, up, down
        }

        public static Vector2Int DirectionToVector(Direction direction)
        {
            int x = direction == Direction.left ? -1 : (direction == Direction.right ? 1 : 0);
            int y = direction == Direction.down ? -1 : (direction == Direction.up ?    1 : 0);

            return new Vector2Int(x, y);
        }
    }
}
