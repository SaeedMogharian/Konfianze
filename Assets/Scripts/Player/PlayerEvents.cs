// PlayerEvents.cs
using System;
using UnityEngine;
using Card;

namespace Player
{
    public static class PlayerEvents
    {
        // Status events
        public static event Action<int> OnFoodChanged;
        public static event Action<int> OnHealthChanged;
        public static event Action<int> OnTimeChanged;
        public static event Action OnTimeRunOut;

        // Ability events
        public static event Action<MoveAbilityCardData.MoveType> OnMoveAbilityEnabled;
        public static event Action<VisionAbilityCardData> OnVisionAbilityAdded;
        public static event Action OnAllAbilitiesDestroyed;
        
        public static void TriggerFoodChanged(int food) => OnFoodChanged?.Invoke(food);
        public static void TriggerHealthChanged(int health) => OnHealthChanged?.Invoke(health);
        public static void TriggerTimeChanged(int time) => OnTimeChanged?.Invoke(time);
        public static void TriggerTimeRunOut() => OnTimeRunOut?.Invoke();
        public static void TriggerMoveAbilityEnabled(MoveAbilityCardData.MoveType moveType) => OnMoveAbilityEnabled?.Invoke(moveType);
        public static void TriggerVisionAbilityAdded(VisionAbilityCardData visionCard) => OnVisionAbilityAdded?.Invoke(visionCard);
        public static void TriggerAllAbilitiesDestroyed() => OnAllAbilitiesDestroyed?.Invoke();
        
    }
}