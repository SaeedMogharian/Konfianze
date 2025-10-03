using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Destroy All Abilities Card", menuName = "Cards/Destroy Abilities")]
    public class DestroyAllAbilitiesCardData : CardData
    {
        // This card is a "flag" type. Its existence is its effect.
        // No additional fields are needed.
    }
}