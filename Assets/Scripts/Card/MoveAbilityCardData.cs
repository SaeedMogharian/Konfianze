using UnityEngine;
using Player;

namespace Card
{
    [CreateAssetMenu(fileName = "New Move Ability Card", menuName = "Cards/Move Ability Card")]
    public class MoveAbilityCardData : CardData
    {
        public enum MoveType { Knight, Double }
    
        public MoveType moveType;
    }
}