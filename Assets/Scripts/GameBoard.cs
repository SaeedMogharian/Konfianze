using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GamePlace;
using UnityEngine.InputSystem;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get => _instance; }
    private static GameBoard _instance;
    
    [SerializeField] private List<Place> playerMoves;

    [SerializeField] private RoundState state;
    public RoundState State => state;
    public static Action<RoundState> OnStateChange;

    public void ChangeRoundState()
    {
        var nextState = ((int)state + 1) % Enum.GetValues(typeof(RoundState)).Length;
        state = (RoundState)nextState;
        OnStateChange?.Invoke(state);
    }
    
    public void AddPlayerMove(Place newPlace)
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

        // var boardPosition = transform.position;

    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ChangeRoundState();
        }

        if (state is RoundState.Consequences or RoundState.Guidance)
        {
            foreach (var place in playerMoves.Where(place => place))
            {
                place.ShowCategoryColor();
            }
        }
        
    }
}

public enum RoundState
{
    Guidance, 
    // AbilityAppliance,
    Choose,
    Consequences,
}