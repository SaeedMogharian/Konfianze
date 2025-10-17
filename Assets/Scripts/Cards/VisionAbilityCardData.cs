using UnityEngine;
using Player;


namespace Cards
{
    public enum VisionType
    {
        ScoutPlace,
        RevealGuideRole
    }
    
    [CreateAssetMenu(fileName = "New Vision Ability Cards", menuName = "Cards/Vision Ability Cards")]
    public class VisionAbilityCardData : CardData
    {
        public VisionType visionType;
    }
}