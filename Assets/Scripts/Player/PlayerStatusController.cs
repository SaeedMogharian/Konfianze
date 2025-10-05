using System;
using UnityEngine;
using Card;
using System.Collections;

namespace Player
{
    [RequireComponent(typeof(PlayerMoveController), typeof(PlayerAbilityController))]
    public class PlayerStatusController : MonoBehaviour
    {
        [SerializeField] private int food;
        [SerializeField] private int health;
        [SerializeField] private int time;
        
        public int Food => food;
        public int Health => health;
        public int Time => time;
        
        public event Action<int> OnFoodChanged;
        public event Action<int> OnHealthChanged;
        public event Action<int> OnTimeChanged;
        
        private PlayerMoveController _moveController;
        private PlayerAbilityController _abilityController;
        
        private Coroutine _countdownCoroutine;
        private bool _isTimerRunning = false;
        
        private void Awake()
        {
            food = 3;
            health = 100;
            time = 120;
            
            StartTimer();
            
            _moveController = GetComponent<PlayerMoveController>();
            _abilityController = GetComponent<PlayerAbilityController>();
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
            OnTimeChanged?.Invoke(time);
            StartTimer();
        }

        private IEnumerator CountdownTimer()
        {
            while (time > 0 && _isTimerRunning)
            {
                yield return new WaitForSeconds(1f);
                time--;
                OnTimeChanged?.Invoke(time);
                
                // Optional: Add game over logic when time runs out
                if (time <= 0)
                {
                    TimeRunOut();
                }
            }
        }

        private void TimeRunOut()
        {
            Debug.Log("Time's up! Game over!");
            // Add your game over logic here
            // For example: GameBoard.Instance.ChangeRoundState() to a GameOver state
        }

        public void ApplyMoveConsequence()
        {
            if (food > 0)
            {
                food -= 1;
                OnFoodChanged?.Invoke(food);
                Debug.Log($"Consumed 1 food for movement. Remaining food: {food}");
            }
            else
            {
                Debug.LogWarning("Tried to consume food for movement but player has no food!");
                OnFoodChanged?.Invoke(food);
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
                case DestroyAllAbilitiesCardData destroyCard:
                    ApplyDestroyAllAbilitiesCard(destroyCard);
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
                OnHealthChanged?.Invoke(health);
            }
            
            if (resourceCard.foodChange != 0)
            {
                food += resourceCard.foodChange;
                OnFoodChanged?.Invoke(food);
            }
            
            if (resourceCard.timeChange != 0)
            {
                time += resourceCard.timeChange;
                OnTimeChanged?.Invoke(time);
            }
        }
        
        private void ApplyMoveAbilityCard(MoveAbilityCardData moveCard)
        {
            switch (moveCard.moveType)
            {
                case MoveAbilityCardData.MoveType.Knight:
                    _moveController.EnableKnightMove(true);
                    break;
                case MoveAbilityCardData.MoveType.Double:
                    _moveController.EnableDoubleMove(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ApplyVisionAbilityCard(VisionAbilityCardData visionCard)
        {
            _abilityController.AddVisionCard(visionCard);
        }
        
        private void ApplyDestroyAllAbilitiesCard(DestroyAllAbilitiesCardData destroyCard)
        {
            _moveController.DisableAllSpecialMoves();
            _abilityController.DestroyAllVisionAbilities();
        }
    }
}