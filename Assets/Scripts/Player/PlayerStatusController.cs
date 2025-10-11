using System;
using UnityEngine;
using Card;
using System.Collections;

namespace Player
{
    // [RequireComponent(typeof(PlayerMoveController))]
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
        
        // private PlayerMoveController _moveController;
        
        private void Awake()
        {
            food = 3;
            health = 100;
            time = 120;
            
            StartTimer();
            // _moveController = GetComponent<PlayerMoveController>();
        }

        private void Update()
        {
            // Check for Game Over
            if (food < 1) // time , health
            {
                Debug.Log("food is less than 1 -- Game Over");
                StopTimer();
                // Invoke Game over
            }
            
            // Check for win
        }

        public void StartTimer()
        {
            if (!_isTimerRunning)
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
            StopTimer();
            time = newTime;
            PlayerEvents.TriggerTimeChanged(time);
            StartTimer();
        }

        private IEnumerator CountdownTimer()
        {
            while (time > 0 && _isTimerRunning)
            {
                yield return new WaitForSeconds(1f);
                time--;
                PlayerEvents.TriggerTimeChanged(time);
                
                if (time <= 0)
                {
                    TimeRunOut();
                }
            }
        }

        private void TimeRunOut()
        {
            Debug.Log("Time's up! Game over!");
            PlayerEvents.TriggerTimeRunOut();
        }

        public void ApplyMoveConsequence()
        {
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
            if (card is null) return;

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
            PlayerEvents.TriggerMoveAbilityEnabled(moveCard.moveType);
        }
        
        private void ApplyVisionAbilityCard(VisionAbilityCardData visionCard)
        {
            PlayerEvents.TriggerVisionAbilityAdded(visionCard);
        }
        
        private void ApplyDestroyAllAbilitiesCard()
        {
            PlayerEvents.TriggerAllAbilitiesDestroyed();
        }

        private void OnDestroy()
        {
            StopTimer();
        }
    }
}