// ResourceCardData.cs
using Player;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "New Resource Cards", menuName = "Cards/Resource Cards")]
    public class ResourceCardData : CardData
    {
        public int healthChange;
        public int foodChange;
        public int timeChange;
    }
}