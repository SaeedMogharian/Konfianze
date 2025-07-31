using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Place initializedPlace;
    private Place _currentPlace;

    private List<string> _abilityPack = new();

    private void Awake()
    {
        _currentPlace = initializedPlace;
    }

    void Update()
    {
        if (GameBoard.Instance.State != RoundState.Choose)
            return;
        
        // Only respond to mouse clicks
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // Check if the clicked object is a valid "Place"
                Place clickedPlace = hit.transform.GetComponent<Place>();
                if (clickedPlace is null)
                    return;

                var possibleMoves = GameBoard.Instance.GetPossibleMoves();

                foreach (var place in possibleMoves)
                {
                    if (place.Id == clickedPlace.Id)
                    {
                        Debug.Log("Clicked Place id: " + place.Id);
                        GameBoard.Instance.MovePlayer(place);
                        transform.position = place.transform.position;
                        GameBoard.Instance.ChangeRoundState();
                        return;
                    }
                }
            }
        }
            
    }
}
