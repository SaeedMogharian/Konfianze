using System.Collections.Generic;
using UnityEngine;
using Card;

namespace Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
    
   
        // A list to hold vision cards for later use
        private List<VisionAbilityCardData> _heldVisionCards = new List<VisionAbilityCardData>();

        public void AddVisionCard(VisionAbilityCardData card)
        {
            _heldVisionCards.Add(card);
            Debug.Log($"Added {card.cardName} to hand. You can use it during the Ability Appliance stage.");
        }

        // You would call this method from your UI during the AbilityAppliance stage
        public void UseVisionCard(VisionAbilityCardData cardToUse)
        {
            if (_heldVisionCards.Contains(cardToUse))
            {
                // TODO: Implement the logic for what happens when a vision card is used.
                // For example, enable a UI mode to click on a Place or Guide.
                Debug.Log($"Using vision card: {cardToUse.visionType}");
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
