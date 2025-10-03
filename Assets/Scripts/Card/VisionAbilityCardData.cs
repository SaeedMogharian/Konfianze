using UnityEngine;
using Player;


namespace Card
{
    public enum VisionType
    {
        ScoutPlace,
        RevealGuideRole
    }
    
    [CreateAssetMenu(fileName = "New Vision Ability Card", menuName = "Cards/Vision Ability Card")]
    public class VisionAbilityCardData : CardData
    {
        public VisionType visionType;
    }
}