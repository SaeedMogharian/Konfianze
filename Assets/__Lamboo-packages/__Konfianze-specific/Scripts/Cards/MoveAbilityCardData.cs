using UnityEngine;

namespace __Lamboo_packages.__Konfianze_specific.Scripts.Cards
{
    [CreateAssetMenu(fileName = "New Move Ability Cards", menuName = "Cards/Move Ability Cards")]
    public class MoveAbilityCardData : CardData
    {
        public enum MoveType { Knight, Double }
    
        public MoveType moveType;
    }
}