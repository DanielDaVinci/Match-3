using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject     _prefab;
    [SerializeField] private GameObject     _scoreLabel;
                     private int            _score;
    [Min(1)][SerializeField] private int    _sizeX;
    [Min(1)][SerializeField] private int    _sizeY;
    [Min(0)][SerializeField] private float  _distance;
    [SerializeField] private List<Color>    _colors;

    [HideInInspector] private GameObject[,] objects;

    public GameObject Prefab
    {
        get { return _prefab; }
    }
    public GameObject ScoreLabel
    {
        get { return _scoreLabel; }
    }
    public int Score
    {
        get 
        { 
            return _score; 
        }
        set 
        {
            PlayerPrefs.SetInt("Score", value);
            _scoreLabel.GetComponent<Text>().text = "Score: " + value.ToString();
            _score = value;
        }
    }
    public int SizeX
    {
        get { return _sizeX; }
    }
    public int SizeY
    {
        get { return _sizeY; }
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
            return objects[i, j];
        }
        set
        {
            if ((i < 0 || i >= _sizeX) || (j < 0 || j >= _sizeY * 2))
                throw new ArgumentOutOfRangeException();

            objects[i, j] = value;
        }
    }

    private void Awake()
    {
        // Multiply by 2 for falling objects
        objects = new GameObject[_sizeX, _sizeY * 2];

        if (PlayerPrefs.HasKey("Score"))
        {
            Score = PlayerPrefs.GetInt("Score");
        }
        else
        {
            PlayerPrefs.SetInt("Score", 0);
            Score = 0;
        }
    }
}
