using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using static Utility.Addition;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeY;
    [SerializeField] private float _distance;
    [SerializeField] private List<Color> _colors;

    [HideInInspector] private GameObject[,] objects;
    public GameObject Prefab
    {
        get { return _prefab; }
    }
    public int SizeX
    {
        get { return _sizeX; }
    }
    public int SizeY
    {
        get { return _sizeY;  }
    }
    public float Distance
    {
        get { return _distance; }
    }
    public List<Color> Colors
    {
        get { return _colors; }
    }

    public GameObject this [int i, int j]
    {
        get
        {
            if ((i < 0 || i >= _sizeX) || (j < 0 || j >= _sizeY))
                throw new ArgumentOutOfRangeException();

            return objects[i, j];
        }
        set
        {
            if ((i < 0 || i >= _sizeX) || (j < 0 || j >= _sizeY))
                throw new ArgumentOutOfRangeException();

            objects[i, j] = value;
        }
    }

    private void Awake()
    {
        objects = new GameObject[_sizeX, _sizeY];
    }
}
