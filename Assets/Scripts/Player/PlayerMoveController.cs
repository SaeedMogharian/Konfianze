using System.Collections.Generic;
using GamePlace;
using UnityEngine;
using UnityEngine.InputSystem;
using Card;

namespace Player
{
    [RequireComponent(typeof(PlayerStatusController))]
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField] private Place initializedPlace;
        
        private Place _currentPlace;
        private PlayerStatusController _statusController;
        
        private List<Place> _possibleMoves;
        private NormalMove _normalMove;
        private KnightMove _knightMove;
        private DoubleMove _doubleMove;
        private bool _isGameOver = false; // NEW: Track game over state

        private void Awake()
        {
            _currentPlace = initializedPlace;
            GameBoard.Instance.AddPlayerMove(initializedPlace);
            _statusController = GetComponent<PlayerStatusController>();
            
            // Initialize move types
            _normalMove = new NormalMove { IsEnabled = true };
            _knightMove = new KnightMove();
            _doubleMove = new DoubleMove();
            
            _possibleMoves = new List<Place>();
            
            // Subscribe to events
            PlayerEvents.OnMoveAbilityEnabled += HandleMoveAbilityEnabled;
            PlayerEvents.OnAllAbilitiesDestroyed += DisableAllSpecialMoves;
            PlayerEvents.OnGameOver += HandleGameOver; // NEW: Subscribe to game over
        }
        
        // NEW: Handle game over event
        private void HandleGameOver()
        {
            _isGameOver = true;
            
            // Light off all possible moves
            foreach (var possiblePlace in _possibleMoves)
            {
                possiblePlace.LightOff();
            }
            _possibleMoves.Clear();
            
            Debug.Log("Player movement disabled due to game over.");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            PlayerEvents.OnMoveAbilityEnabled -= HandleMoveAbilityEnabled;
            PlayerEvents.OnAllAbilitiesDestroyed -= DisableAllSpecialMoves;
            PlayerEvents.OnGameOver -= HandleGameOver; // NEW: Unsubscribe
        }

        private void HandleMoveAbilityEnabled(MoveAbilityCardData.MoveType moveType)
        {
            if (_isGameOver) return; // NEW: Don't process if game over
            
            switch (moveType)
            {
                case MoveAbilityCardData.MoveType.Knight:
                    _knightMove.IsEnabled = true;
                    break;
                case MoveAbilityCardData.MoveType.Double:
                    _doubleMove.IsEnabled = true;
                    break;
                default:
                    Debug.LogWarning($"Unknown move type: {moveType}");
                    break;
            }
        }
        
        private void DisableAllSpecialMoves()
        {
            if (_isGameOver) return; // NEW: Don't process if game over
            
            _knightMove.IsEnabled = false;
            _doubleMove.IsEnabled = false;
            Debug.Log("All special movement abilities have been destroyed!");
        }

        private void Update()
        {
            // NEW: Don't process anything if game is over
            if (_isGameOver) return;
            
            // Check if the state is good for move
            if (GameBoard.Instance.State != RoundState.Choose) return;
            
            // Calculate and show possible moves
            CalculatePossibleMoves();
            
            // Light Up possible places
            foreach (var possiblePlace in _possibleMoves)
            {
                possiblePlace.LightUp();
            }
            
            // Get Mouse Click
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 100f)) return;
            
            var clickedPlace = hit.transform.GetComponent<Place>();
            if (clickedPlace is null)
                return;
            
            // Check if clicked place is in possible moves
            if (!_possibleMoves.Exists(place => place.Id == clickedPlace.Id) || !HasEnoughFoodToMove())
                return;
            
            MoveToPlace(clickedPlace);
        }
        
        private bool HasEnoughFoodToMove()
        {
            if (_statusController.Food >= 1)
            {
                return true;
            }
            else {
                Debug.Log("Not enough food to move! You need at least 1 food to travel.");
                return false;
            }
        }
        
        private void MoveToPlace(Place targetPlace)
        {
            if (_isGameOver) return; // NEW: Don't process if game over
            
            Debug.Log($"Moving to place: {targetPlace.Id}");
            
            // Move the player visually
            transform.position = targetPlace.transform.position - Vector3.forward * 2;
            _currentPlace = targetPlace;
            
            //Light Off possible places
            foreach (var possiblePlace in _possibleMoves)
            {
                possiblePlace.LightOff();
            }
            // Move consequence
            _statusController.ApplyMoveConsequence();
            // Draw and process a card from the place
            CardData receivedCard = _currentPlace.DrawCard();
            if (receivedCard)
            {
                _statusController.ProcessReceivedCard(receivedCard);
            }
            
            // Update game state
            GameBoard.Instance.AddPlayerMove(targetPlace);
            GameBoard.Instance.ChangeRoundState();
        }
        
        private void CalculatePossibleMoves()
        {
            if (_isGameOver) return; // NEW: Don't process if game over
            
            _possibleMoves.Clear();
            
            _possibleMoves.AddRange(_normalMove.GetPossibleMoves(_currentPlace));
            
            if (_knightMove.IsEnabled)
                _possibleMoves.AddRange(_knightMove.GetPossibleMoves(_currentPlace));
                
            if (_doubleMove.IsEnabled)
                _possibleMoves.AddRange(_doubleMove.GetPossibleMoves(_currentPlace));
        }
    }
    
    public abstract class Move
    {
        public bool IsEnabled { get; set; }
        
        public List<Place> GetPossibleMoves(Place currentPlace)
        {
            if (!IsEnabled) return new List<Place>();
            return GetMoveSpecificPlaces(currentPlace);
        }
        
        protected abstract List<Place> GetMoveSpecificPlaces(Place currentPlace);
    }
    
    public class NormalMove : Move
    {
        protected override List<Place> GetMoveSpecificPlaces(Place currentPlace)
        {
            List<Place> possibleMoves = new List<Place>();
        
            // Cardinal directions
            if (currentPlace.DownPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace);
            if (currentPlace.UpPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace);
            if (currentPlace.RightPlace is not null)
                possibleMoves.Add(currentPlace.RightPlace);
            if (currentPlace.LeftPlace is not null)
                possibleMoves.Add(currentPlace.LeftPlace);
        
            // Diagonals
            if (currentPlace.UpPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.LeftPlace);
            if (currentPlace.UpPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.RightPlace);
            if (currentPlace.DownPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.LeftPlace);
            if (currentPlace.DownPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.RightPlace);

            return possibleMoves;
        }
    }

    public class KnightMove : Move
    {
        protected override List<Place> GetMoveSpecificPlaces(Place currentPlace)
        {
            List<Place> possibleMoves = new List<Place>();
            
            // All 8 possible L-shaped moves (like a knight in chess)
            // Two up, one left/right
            if (currentPlace.UpPlace?.UpPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.UpPlace.LeftPlace);
            if (currentPlace.UpPlace?.UpPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.UpPlace.RightPlace);
                
            // Two down, one left/right
            if (currentPlace.DownPlace?.DownPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.DownPlace.LeftPlace);
            if (currentPlace.DownPlace?.DownPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.DownPlace.RightPlace);
                
            // Two left, one up/down
            if (currentPlace.LeftPlace?.LeftPlace?.UpPlace is not null)
                possibleMoves.Add(currentPlace.LeftPlace.LeftPlace.UpPlace);
            if (currentPlace.LeftPlace?.LeftPlace?.DownPlace is not null)
                possibleMoves.Add(currentPlace.LeftPlace.LeftPlace.DownPlace);
                
            // Two right, one up/down
            if (currentPlace.RightPlace?.RightPlace?.UpPlace is not null)
                possibleMoves.Add(currentPlace.RightPlace.RightPlace.UpPlace);
            if (currentPlace.RightPlace?.RightPlace?.DownPlace is not null)
                possibleMoves.Add(currentPlace.RightPlace.RightPlace.DownPlace);

            return possibleMoves;
        }
    }

    public class DoubleMove : Move
    {
        protected override List<Place> GetMoveSpecificPlaces(Place currentPlace)
        {
            List<Place> possibleMoves = new List<Place>();
            
            // Double cardinal moves
            if (currentPlace.DownPlace?.DownPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.DownPlace);
            if (currentPlace.UpPlace?.UpPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.UpPlace);
            if (currentPlace.RightPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.RightPlace.RightPlace);
            if (currentPlace.LeftPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.LeftPlace.LeftPlace);
            
            // Double diagonal moves
            if (currentPlace.UpPlace?.LeftPlace?.UpPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.LeftPlace.UpPlace.LeftPlace);
            if (currentPlace.UpPlace?.RightPlace?.UpPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.UpPlace.RightPlace.UpPlace.RightPlace);
            if (currentPlace.DownPlace?.LeftPlace?.DownPlace?.LeftPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.LeftPlace.DownPlace.LeftPlace);
            if (currentPlace.DownPlace?.RightPlace?.DownPlace?.RightPlace is not null)
                possibleMoves.Add(currentPlace.DownPlace.RightPlace.DownPlace.RightPlace);

            return possibleMoves;
        }
    }
}