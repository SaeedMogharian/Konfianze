using System.Collections.Generic;
using GamePlace;
using UnityEngine;

namespace Guides
{
    public abstract class Guide : ScriptableObject
    {
        [SerializeField] protected string guideName;
        [SerializeField] protected List<PlaceCategory> visibleCategories;
        
        public string GuideName => guideName;
        public List<PlaceCategory> VisibleCategories => visibleCategories;
        
        public abstract float CalculateScore(Place place, Player.PlayerStatusController playerStatus, List<Place> playerMoves);
        
        public bool CanSeeCategory(PlaceCategory category)
        {
            return visibleCategories.Contains(category);
        }
        
        // public string GetComment(Place place, Player.PlayerStatusController playerStatus, List<Place> playerMoves)
        // {
        //     float score = CalculateScore(place, playerStatus, playerMoves);
        //     
        //     if (score > 0.7f) return "Highly recommended! This place looks very promising.";
        //     if (score > 0.4f) return "This could be a good choice for your journey.";
        //     if (score > 0.1f) return "Might be worth considering, but be cautious.";
        //     if (score > -0.1f) return "Not the best option, but not terrible either.";
        //     if (score > -0.4f) return "I'd advise against this path.";
        //     
        //     return "Avoid this place at all costs!";
        // }
    }

    [CreateAssetMenu(fileName = "MentorGuide", menuName = "Guides/Mentor Guide")]
    public class MentorGuide : Guide
    {
        public override float CalculateScore(Place place, Player.PlayerStatusController playerStatus, List<Place> playerMoves)
        {
            float score = 0f;
            
            // Mentor wants to get player closer to the End
            if (place.Category == PlaceCategory.End)
            {
                score += 1.0f;
            }
            else if (place.Category == PlaceCategory.Ability)
            {
                score += 0.6f; // Abilities help reach the end
            }
            else if (place.Category == PlaceCategory.Danger)
            {
                score -= 0.8f; // Dangers hinder progress
            }
            else if (place.Category == PlaceCategory.Resource)
            {
                score += 0.3f; // Resources are somewhat helpful
            }
            
            return Mathf.Clamp(score, -1f, 1f);
        }
    }

    [CreateAssetMenu(fileName = "WitchGuide", menuName = "Guides/Witch Guide")]
    public class WitchGuide : Guide
    {
        public override float CalculateScore(Place place, Player.PlayerStatusController playerStatus, List<Place> playerMoves)
        {
            float score = 0f;
            
            // Witch wants player to encounter dangers
            if (place.Category == PlaceCategory.Danger)
            {
                score += 1.0f;
            }
            else if (place.Category == PlaceCategory.End)
            {
                score -= 0.5f; // Don't want player to reach end too quickly
            }
            else if (place.Category == PlaceCategory.Resource)
            {
                score += 0.2f; // Resources might lead to more dangerous paths
            }
            else if (place.Category == PlaceCategory.Ability)
            {
                score -= 0.7f; // Abilities help avoid dangers
            }
            
            return Mathf.Clamp(score, -1f, 1f);
        }
    }

    [CreateAssetMenu(fileName = "LandlordGuide", menuName = "Guides/Landlord Guide")]
    public class LandlordGuide : Guide
    {
        public override float CalculateScore(Place place, Player.PlayerStatusController playerStatus, List<Place> playerMoves)
        {
            float score = 0f;
            
            // Landlord wants player to collect abilities
            if (place.Category == PlaceCategory.Ability)
            {
                score += 1.0f;
            }
            else if (place.Category == PlaceCategory.Danger)
            {
                score -= 0.9f; // Dangers might destroy abilities
            }
            else if (place.Category == PlaceCategory.Resource)
            {
                score += 0.1f; // Neutral about resources
            }
            else if (place.Category == PlaceCategory.End)
            {
                score -= 0.3f; // Ending stops ability collection
            }
            
            return Mathf.Clamp(score, -1f, 1f);
        }
    }
}