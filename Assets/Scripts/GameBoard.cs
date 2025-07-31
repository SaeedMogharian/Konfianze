using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get => _instance; }
    private static GameBoard _instance;

    private List<List<Place>> Board = new();
    [SerializeField] private List<Place> playerMoves;

    private RoundState _state;
    public RoundState State { get => _state; }
    public static Action<RoundState> OnStateChange;
    
    public static Action<List<Place>> OnGetPossiblePlace;
    

    public void ChangeRoundState()
    {
        var nextState = ((int)_state + 1) % Enum.GetValues(typeof(RoundState)).Length;
        _state = (RoundState)nextState;

        OnStateChange?.Invoke(_state);
    }
    
    public void MovePlayer(Place newPlace)
    {
        playerMoves.Add(newPlace);
    }

    private void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        var boardPosition = transform.position;

    }

    public List<Place> GetPossibleMoves()
    {
        var currentPlace = playerMoves[^1];
        List<Place> possibleMoves = new();
        
        // Cardinal directions
        if (currentPlace.DownPlace is not null)
        {
            possibleMoves.Add(currentPlace.DownPlace);
        }
        if (currentPlace.UpPlace is not null)
        {
            possibleMoves.Add(currentPlace.UpPlace);
        }
        if (currentPlace.RightPlace is not null)
        {
            possibleMoves.Add(currentPlace.RightPlace);
        }
        if (currentPlace.LeftPlace is not null)
        {
            possibleMoves.Add(currentPlace.LeftPlace);
        }
        
        // Diagonals: infer from combinations of two directions
        // ↖️ Up-Left
        if (currentPlace.UpPlace?.LeftPlace is not null)
            possibleMoves.Add(currentPlace.UpPlace.LeftPlace);

        // ↗️ Up-Right
        if (currentPlace.UpPlace?.RightPlace is not null)
            possibleMoves.Add(currentPlace.UpPlace.RightPlace);

        // ↙️ Down-Left
        if (currentPlace.DownPlace?.LeftPlace is not null)
            possibleMoves.Add(currentPlace.DownPlace.LeftPlace);

        // ↘️ Down-Right
        if (currentPlace.DownPlace?.RightPlace is not null)
            possibleMoves.Add(currentPlace.DownPlace.RightPlace);
        

        return possibleMoves;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ChangeRoundState();
        }
    }
}

public enum RoundState
{
    Guidance, 
    Choose,
    Consequences,
}