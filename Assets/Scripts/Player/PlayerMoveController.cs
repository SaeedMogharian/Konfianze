using System.Collections.Generic;
using GamePlace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerStatusController))]
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField] private Place initializedPlace;
        private Place _currentPlace;
        private PlayerStatusController _statusController;

        private void Awake()
        {
            _currentPlace = initializedPlace;
            _statusController = GetComponent<PlayerStatusController>();
        }

        private List<Place> GetPossibleMoves()
        {
            List<Place> possibleMoves = new();
        
            // Cardinal directions
            if (_currentPlace.DownPlace is not null)
                possibleMoves.Add(_currentPlace.DownPlace);
            if (_currentPlace.UpPlace is not null)
                possibleMoves.Add(_currentPlace.UpPlace);
            if (_currentPlace.RightPlace is not null)
                possibleMoves.Add(_currentPlace.RightPlace);
            if (_currentPlace.LeftPlace is not null)
                possibleMoves.Add(_currentPlace.LeftPlace);
        
            // Diagonals
            if (_currentPlace.UpPlace?.LeftPlace is not null)
                possibleMoves.Add(_currentPlace.UpPlace.LeftPlace);
            if (_currentPlace.UpPlace?.RightPlace is not null)
                possibleMoves.Add(_currentPlace.UpPlace.RightPlace);
            if (_currentPlace.DownPlace?.LeftPlace is not null)
                possibleMoves.Add(_currentPlace.DownPlace.LeftPlace);
            if (_currentPlace.DownPlace?.RightPlace is not null)
                possibleMoves.Add(_currentPlace.DownPlace.RightPlace);

            foreach (var possibleMove in possibleMoves)
                possibleMove.LightUp();
            
            return possibleMoves;
        }

        private void Update()
        {
            // Check if the state is good for move
            if (GameBoard.Instance.State != RoundState.Choose) return;
            
            // Get Mouse Click
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 100f)) return;
            var clickedPlace = hit.transform.GetComponent<Place>();
            if (clickedPlace is null)
                return;
            
            // Get possible moves and find the clicked place in them
            var possibleMoves = GetPossibleMoves();
            var place = possibleMoves.Find(place => place.Id == clickedPlace.Id);
            
            // Transform player
            Debug.Log("Clicked Place id: " + place.Id);
            GameBoard.Instance.AddPlayerMove(place);
            transform.position = place.transform.position;
            transform.position -= Vector3.forward * 2;
            _currentPlace = place;
            
            // Change game state if move is successful
            GameBoard.Instance.ChangeRoundState();
            
            // Handle status consequences
            _statusController.Consequence(_currentPlace);

        }
    }
}
