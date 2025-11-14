using System.Collections.Generic;
using __Lamboo_packages.__Konfianze_specific.Scripts.GamePlace;
using UnityEngine;

namespace __Lamboo_packages.__Konfianze_specific.Scripts.Guides
{
    public abstract class Guide : ScriptableObject
    {
        [SerializeField] protected string guideName;
        [SerializeField] protected List<PlaceCategory> visibleCategories;
        
        public string GuideName => guideName;
        public List<PlaceCategory> VisibleCategories => visibleCategories;
        
        public abstract float CalculateScore(List<Place> playerMoves);
        
        public bool CanSeeCategory(PlaceCategory category)
        {
            return visibleCategories.Contains(category);
        }
        
    }

    [CreateAssetMenu(fileName = "MentorGuide", menuName = "Guides/Mentor Guide")]
    public class MentorGuide : Guide
    {
        public override float CalculateScore(List<Place> playerMoves)
        {
            float score = 0f;

            foreach (var place in playerMoves)
            {
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
            }
            
            return Mathf.Clamp(score, -1f, 1f);
        }
    }

    [CreateAssetMenu(fileName = "WitchGuide", menuName = "Guides/Witch Guide")]
    public class WitchGuide : Guide
    {
        public override float CalculateScore(List<Place> playerMoves)
        {
            float score = 0f;

            foreach (var place in playerMoves)
            {
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
            }

            return Mathf.Clamp(score, -1f, 1f);
        }
    }

    [CreateAssetMenu(fileName = "LandlordGuide", menuName = "Guides/Landlord Guide")]
    public class LandlordGuide : Guide
    {
        public override float CalculateScore(List<Place> playerMoves)
        {
            float score = 0f;

            foreach (var place in playerMoves)
            {
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
            }

            return Mathf.Clamp(score, -1f, 1f);
        }
    }
}