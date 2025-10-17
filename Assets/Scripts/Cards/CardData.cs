using UnityEngine;

// This enum helps categorize cards in the editor and in code.
public enum CardType { Danger, Ability, Resources }

namespace Cards
{
    public abstract class CardData : ScriptableObject
    {
        public string cardName;
        public CardType cardType;
    }
}
