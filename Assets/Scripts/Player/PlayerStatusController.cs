using System;
using GamePlace;
using UnityEngine;
using Card;

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
        
        private void Awake()
        {
            food = 3;
            health = 100;
            time = 120;
            
            _moveController = GetComponent<PlayerMoveController>();
            _abilityController = GetComponent<PlayerAbilityController>();
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
                // OnFoodChanged?.Invoke(food);
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