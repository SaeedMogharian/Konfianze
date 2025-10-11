using System;
using UnityEngine;
using Card;
using System.Collections;

namespace Player
{
    public class PlayerStatusController : MonoBehaviour
    {
        [SerializeField] private int food;
        [SerializeField] private int health;
        [SerializeField] private int time;
        
        public int Food => food;
        public int Health => health;
        public int Time => time;
        
        private Coroutine _countdownCoroutine;
        private bool _isTimerRunning = false;
        private bool _isGameOver = false;
        public bool IsGameOver() => _isGameOver;
        private bool _isGameWon = false; // NEW: Track win state
        
        private void Awake()
        {
            food = 3;
            health = 100;
            time = 120;
            
            StartTimer();
        }

        private void Update()
        {
            // Don't process anything if game is over OR won
            if (_isGameOver || _isGameWon) return;

            // Check for Game Over conditions
            if (food <= 0 || health <= 0 || time <= 0) 
            {
                TriggerGameOver("Game Over - " + (food <= 0 ? "Starved to death!" : health <= 0 ? "Health depleted!" : "Run out of time!"));
            }
        }

        // NEW: Handle game win event
        private void HandleGameWin()
        {
            if (_isGameOver || _isGameWon) return;
            
            _isGameWon = true;
            Debug.Log("Victory! Game Won!");
            StopTimer();
        }

        private void TriggerGameOver(string reason)
        {
            if (_isGameOver || _isGameWon) return;
            
            _isGameOver = true;
            Debug.Log(reason);
            
            StopTimer();
            PlayerEvents.TriggerGameOver();
        }

        public void StartTimer()
        {
            if (!_isTimerRunning && !_isGameOver && !_isGameWon)
            {
                _isTimerRunning = true;
                _countdownCoroutine = StartCoroutine(CountdownTimer());
            }
        }

        public void StopTimer()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
            _isTimerRunning = false;
        }

        public void ResetTimer(int newTime = 120)
        {
            if (_isGameOver || _isGameWon) return;
            
            StopTimer();
            time = newTime;
            PlayerEvents.TriggerTimeChanged(time);
            StartTimer();
        }

        private IEnumerator CountdownTimer()
        {
            while (time > 0 && _isTimerRunning && !_isGameOver && !_isGameWon)
            {
                yield return new WaitForSeconds(1f);
                time--;
                PlayerEvents.TriggerTimeChanged(time);
                
                if (time <= 0)
                {
                    TriggerGameOver("Game Over - " + "Run out of time!");
                }
            }
        }

        // private void TimeRunOut()
        // {
        //     if (_isGameOver) return; // NEW: Don't trigger if already game over
        //     
        //     Debug.Log("Time's up! Game over!");
        //     PlayerEvents.TriggerTimeRunOut();
        //     TriggerGameOver("Time's up!");
        // }

        public void ApplyMoveConsequence()
        {
            if (_isGameOver || _isGameWon) return;
            
            if (food > 0)
            {
                food -= 1;
                PlayerEvents.TriggerFoodChanged(food);
                Debug.Log($"Consumed 1 food for movement. Remaining food: {food}");
            }
            else
            {
                Debug.LogWarning("Tried to consume food for movement but player has no food!");
                PlayerEvents.TriggerFoodChanged(food);
            }
        }
        
        public void ProcessReceivedCard(CardData card)
        {
            if (card is null || _isGameOver || _isGameWon) return;

            Debug.Log($"Processing received card: {card.cardName}");

            switch (card)
            {
                case ResourceCardData resourceCard:
                    ApplyResourceCard(resourceCard);
                    break;
                case MoveAbilityCardData moveCard:
                    ApplyMoveAbilityCard(moveCard);
                    break;
                case VisionAbilityCardData visionCard:
                    ApplyVisionAbilityCard(visionCard);
                    break;
                case DestroyAllAbilitiesCardData:
                    ApplyDestroyAllAbilitiesCard();
                    break;
                default:
                    Debug.LogWarning($"Unknown card type: {card.GetType()}");
                    break;
            }
        }
        
        private void ApplyResourceCard(ResourceCardData resourceCard)
        {
            if (_isGameOver || _isGameWon) return;
            
            if (resourceCard.healthChange != 0)
            {
                health += resourceCard.healthChange;
                PlayerEvents.TriggerHealthChanged(health);
            }
            
            if (resourceCard.foodChange != 0)
            {
                food += resourceCard.foodChange;
                PlayerEvents.TriggerFoodChanged(food);
            }
            
            if (resourceCard.timeChange != 0)
            {
                time += resourceCard.timeChange;
                PlayerEvents.TriggerTimeChanged(time);
            }
        }
        
        private void ApplyMoveAbilityCard(MoveAbilityCardData moveCard)
        {
            if (_isGameOver || _isGameWon) return;
            PlayerEvents.TriggerMoveAbilityEnabled(moveCard.moveType);
        }
        
        private void ApplyVisionAbilityCard(VisionAbilityCardData visionCard)
        {
            if (_isGameOver || _isGameWon) return;
            PlayerEvents.TriggerVisionAbilityAdded(visionCard);
        }
        
        private void ApplyDestroyAllAbilitiesCard()
        {
            if (_isGameOver || _isGameWon) return;
            PlayerEvents.TriggerAllAbilitiesDestroyed();
        }

        // NEW: Subscribe to events
        private void OnEnable()
        {
            PlayerEvents.OnGameWin += HandleGameWin;
        }

        private void OnDisable()
        {
            PlayerEvents.OnGameWin -= HandleGameWin;
        }

        private void OnDestroy()
        {
            StopTimer();
        }
    }
}