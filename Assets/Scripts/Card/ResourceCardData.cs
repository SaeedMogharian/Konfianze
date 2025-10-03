// ResourceCardData.cs
using Player;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Resource Card", menuName = "Cards/Resource Card")]
    public class ResourceCardData : CardData
    {
        public int healthChange;
        public int foodChange;
        public int timeChange;
    }
}