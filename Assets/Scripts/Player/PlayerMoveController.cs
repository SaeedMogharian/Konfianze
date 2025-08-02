using System.Collections.Generic;
using System.Linq;
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
            {
                possibleMoves.Add(_currentPlace.DownPlace);
            }
            if (_currentPlace.UpPlace is not null)
            {
                possibleMoves.Add(_currentPlace.UpPlace);
            }
            if (_currentPlace.RightPlace is not null)
            {
                possibleMoves.Add(_currentPlace.RightPlace);
            }
            if (_currentPlace.LeftPlace is not null)
            {
                possibleMoves.Add(_currentPlace.LeftPlace);
            }
        
            // Diagonals: infer from combinations of two directions
            // ↖️ Up-Left
            if (_currentPlace.UpPlace?.LeftPlace is not null)
                possibleMoves.Add(_currentPlace.UpPlace.LeftPlace);

            // ↗️ Up-Right
            if (_currentPlace.UpPlace?.RightPlace is not null)
                possibleMoves.Add(_currentPlace.UpPlace.RightPlace);

            // ↙️ Down-Left
            if (_currentPlace.DownPlace?.LeftPlace is not null)
                possibleMoves.Add(_currentPlace.DownPlace.LeftPlace);

            // ↘️ Down-Right
            if (_currentPlace.DownPlace?.RightPlace is not null)
                possibleMoves.Add(_currentPlace.DownPlace.RightPlace);

            foreach (var possibleMove in possibleMoves)
            {
                possibleMove.LighUp();
            }
            
            return possibleMoves;
        }

        private void Update()
        {
            if (GameBoard.Instance.State != RoundState.Choose)
                return;
        
            // Only respond to mouse clicks
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;
            // Check if the clicked object is a valid "Place"
            Place clickedPlace = hit.transform.GetComponent<Place>();
            if (clickedPlace is null)
                return;

            var possibleMoves = GetPossibleMoves();

            foreach (var place in possibleMoves.Where(place => place.Id == clickedPlace.Id))
            {
                Debug.Log("Clicked Place id: " + place.Id);
                GameBoard.Instance.AddPlayerMove(place);
                transform.position = place.transform.position;
                transform.position -= Vector3.forward * 2;
                _statusController.ConsumeFood();
            
                GameBoard.Instance.ChangeRoundState();
                return;
            }

        }
    }
}
