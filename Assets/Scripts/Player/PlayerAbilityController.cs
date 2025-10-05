using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
    
   
        // A list to hold vision cards for later use
        private List<VisionAbilityCardData> _heldVisionCards = new List<VisionAbilityCardData>();
        public List<VisionAbilityCardData> HeldVisionCards => new List<VisionAbilityCardData>(_heldVisionCards);

        public void AddVisionCard(VisionAbilityCardData card)
        {
            _heldVisionCards.Add(card);
            Debug.Log($"Added {card.cardName} to hand. You can use it during the Ability Appliance stage.");
        }

        // A public method to use a card by its index in the list
        public void UseVisionCard(int cardIndex)
        {
            if (cardIndex >= 0 && cardIndex < _heldVisionCards.Count)
            {
                VisionAbilityCardData cardToUse = _heldVisionCards[cardIndex];
                Debug.Log($"Using vision card: {cardToUse.visionType}");
                // TODO: Implement the card's specific game effect
                _heldVisionCards.RemoveAt(cardIndex);
            }
        }

        // Optional: A method to use a specific card object
        public void UseVisionCard(VisionAbilityCardData cardToUse)
        {
            if (_heldVisionCards.Contains(cardToUse))
            {
                Debug.Log($"Using vision card: {cardToUse.visionType}");
                // TODO: Implement the card's specific game effect
                _heldVisionCards.Remove(cardToUse);
            }
        }
        
        public void DestroyAllVisionAbilities()
        {
            int destroyedCount = _heldVisionCards.Count;
            _heldVisionCards.Clear();
            Debug.Log($"Destroyed all vision abilities! Removed {destroyedCount} vision cards.");
        }
    }
}
