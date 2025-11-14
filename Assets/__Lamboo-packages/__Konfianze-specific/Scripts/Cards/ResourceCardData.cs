// ResourceCardData.cs

using UnityEngine;

namespace __Lamboo_packages.__Konfianze_specific.Scripts.Cards
{
    [CreateAssetMenu(fileName = "New Resource Cards", menuName = "Cards/Resource Cards")]
    public class ResourceCardData : CardData
    {
        public int healthChange;
        public int foodChange;
        public int timeChange;
    }
}