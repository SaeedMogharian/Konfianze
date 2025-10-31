using System.Collections.Generic;
using UnityEngine;
using Cards;
using GamePlace;
using UnityEngine.InputSystem;
using System.Collections;


namespace Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
        // A list to hold vision cards for later use
        private List<VisionAbilityCardData> _heldVisionCards = new List<VisionAbilityCardData>();
        public List<VisionAbilityCardData> HeldVisionCards => new List<VisionAbilityCardData>(_heldVisionCards);

        // State variables for ability appliance
        private bool _isWaitingForPlaceSelection = false;
        private VisionAbilityCardData _currentVisionCard = null;
        private bool _isGameOver = false;
        private bool _isGameWon = false;

        private void Awake()
        {
            PlayerEvents.OnGameOver += HandleGameOver;
            PlayerEvents.OnGameWin += HandleGameWin;
            GameBoard.OnStateChange += HandleStateChange;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            PlayerEvents.OnGameOver -= HandleGameOver;
            PlayerEvents.OnGameWin -= HandleGameWin;
            GameBoard.OnStateChange -= HandleStateChange;
        }

        private void HandleGameOver()
        {
            _isGameOver = true;
            _isWaitingForPlaceSelection = false;
            _currentVisionCard = null;
        }

        private void HandleGameWin()
        {
            _isGameWon = true;
            _isWaitingForPlaceSelection = false;
            _currentVisionCard = null;
        }

        private void HandleStateChange(RoundState newState)
        {
            // Reset waiting state when leaving AbilityAppliance
            if (newState != RoundState.AbilityAppliance)
            {
                _isWaitingForPlaceSelection = false;
                _currentVisionCard = null;
            }
        }

        private void Update()
        {
            // Don't process anything if game is over or won
            if (_isGameOver || _isGameWon) return;

            // Only process during Ability Appliance stage
            if (GameBoard.Instance.State != RoundState.AbilityAppliance) return;

            // If we're waiting for place selection, handle mouse clicks
            if (_isWaitingForPlaceSelection)
            {
                HandlePlaceSelection();
            }
            else
            {
                // Show available vision cards and let player choose one
                HandleVisionCardSelection();
            }
        }

        private Coroutine _visionSelectionCoroutine;

        public void HandleVisionCardSelection()
        {
            if (_visionSelectionCoroutine != null)
            {
                StopCoroutine(_visionSelectionCoroutine);
            }
            _visionSelectionCoroutine = StartCoroutine(VisionCardSelectionRoutine());
        }

        private IEnumerator VisionCardSelectionRoutine()
        {
            if (_heldVisionCards.Count == 0)
            {
                Debug.Log("Ability Appliance Stage: No vision cards available.");
                GameBoard.Instance.ChangeRoundState();
                yield break;
            }

            Debug.Log("Ability Appliance Stage: Press 1-" + _heldVisionCards.Count + " to use a vision card, or press ESC to skip.");

            bool selectionMade = false;

            while (!selectionMade)
            {
                // Check if game state changed — break if needed
                if (GameBoard.Instance.State != RoundState.AbilityAppliance) // adjust to your real state name
                {
                    Debug.Log("State changed, exiting vision selection.");
                    yield break;
                }

                for (int i = 0; i < _heldVisionCards.Count; i++)
                {
                    Key key = Key.Digit1 + i; // Key enum has Digit1, Digit2, ...
                    if (Keyboard.current[key].wasPressedThisFrame)
                    {
                        StartUsingVisionCard(i);
                        selectionMade = true;
                        break;
                    }
                }

                // if (Keyboard.current.escapeKey.wasPressedThisFrame)
                // {
                //     Debug.Log("Skipped vision card usage.");
                //     selectionMade = true;
                //     GameBoard.Instance.ChangeRoundState();
                // }

                yield return null; // wait one frame before checking again
            }
        }

        private void HandlePlaceSelection()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 100f)) return;

            var clickedPlace = hit.transform.GetComponent<Place>();
            if (clickedPlace is null) return;

            // Apply the vision card effect to the selected place
            ApplyVisionCardEffect(_currentVisionCard, clickedPlace);
            
            // Reset state
            _isWaitingForPlaceSelection = false;
            _currentVisionCard = null;

            // Automatically move to next stage after using ability
            GameBoard.Instance.ChangeRoundState();
        }

        public void AddVisionCard(VisionAbilityCardData card)
        {
            _heldVisionCards.Add(card);
            Debug.Log($"Added {card.cardName} to hand. You can use it during the Ability Appliance stage.");
        }

        public void StartUsingVisionCard(int cardIndex)
        {
            if (cardIndex >= 0 && cardIndex < _heldVisionCards.Count)
            {
                VisionAbilityCardData cardToUse = _heldVisionCards[cardIndex];
                
                if (cardToUse.visionType == VisionType.ScoutPlace)
                {
                    _currentVisionCard = cardToUse;
                    _isWaitingForPlaceSelection = true;
                    Debug.Log($"Using {cardToUse.cardName}. Click on any place to reveal its category.");
                }
                else if (cardToUse.visionType == VisionType.RevealGuideRole)
                {
                    ApplyVisionCardEffect(cardToUse, null);
                    // Move to next stage after using non-place ability
                    // GameBoard.Instance.ChangeRoundState();
                }
            }
        }

        public void UseVisionCard(VisionAbilityCardData cardToUse)
        {
            if (_heldVisionCards.Contains(cardToUse))
            {
                if (cardToUse.visionType == VisionType.ScoutPlace)
                {
                    _currentVisionCard = cardToUse;
                    _isWaitingForPlaceSelection = true;
                    Debug.Log($"Using {cardToUse.cardName}. Click on any place to reveal its category.");
                }
                else if (cardToUse.visionType == VisionType.RevealGuideRole)
                {
                    ApplyVisionCardEffect(cardToUse, null);
                    GameBoard.Instance.ChangeRoundState();
                }
            }
        }

        private void ApplyVisionCardEffect(VisionAbilityCardData card, Place targetPlace)
        {
            switch (card.visionType)
            {
                case VisionType.ScoutPlace:
                    if (targetPlace != null)
                    {
                        targetPlace.ShowCategoryColor();
                        Debug.Log($"Scouted {targetPlace.name}: Revealed category {targetPlace.Category}");
                        _heldVisionCards.Remove(card);
                    }
                    break;
                    
                case VisionType.RevealGuideRole:
                    // TODO: Implement RevealGuideRole logic
                    Debug.Log("Revealing guide role - functionality to be implemented");
                    _heldVisionCards.Remove(card);
                    break;
                    
                default:
                    Debug.LogWarning($"Unknown vision type: {card.visionType}");
                    break;
            }
        }

        public void DestroyAllVisionAbilities()
        {
            int destroyedCount = _heldVisionCards.Count;
            _heldVisionCards.Clear();
            _isWaitingForPlaceSelection = false;
            _currentVisionCard = null;
            Debug.Log($"Destroyed all vision abilities! Removed {destroyedCount} vision cards.");
        }

        // Helper method to check if we're currently waiting for place selection
        public bool IsWaitingForPlaceSelection() => _isWaitingForPlaceSelection;

        // Helper method to get the current vision card being used
        public VisionAbilityCardData GetCurrentVisionCard() => _currentVisionCard;
    }
}