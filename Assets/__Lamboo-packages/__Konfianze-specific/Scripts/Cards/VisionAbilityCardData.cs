using UnityEngine;

namespace __Lamboo_packages.__Konfianze_specific.Scripts.Cards
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