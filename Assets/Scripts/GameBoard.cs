using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get => instance; }
    private static GameBoard instance;

    private List<List<Place>> Board = new();
    private List<Vector2D> PlayerMoves = new();

    public void MovePlayer(Vector newPosition)
    {
        PlayerMoves.Add(newPosition);
    }

    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
