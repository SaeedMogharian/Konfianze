using System.Collections.Generic;
using __Lamboo_packages.__Konfianze_specific.Scripts.Guides;
using __Lamboo_packages.__Konfianze_specific.Scripts.Cards;
using UnityEngine;

namespace __Lamboo_packages.__Konfianze_specific.Scripts.GamePlace
{
    public class Place : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;

        // Coloring 
        private Material _material;
        private bool _shown = false;

        // Define the color dictionary for categories
        private static readonly Dictionary<PlaceCategory, Color> CategoryColors = new Dictionary<PlaceCategory, Color>
        {
            { PlaceCategory.End, Color.black },
            { PlaceCategory.Empty, Color.lightGray },
            { PlaceCategory.Resource, new Color(0.5647f, 0.9333f, 0.5647f) }, // light-green
            { PlaceCategory.Ability, new Color(0.6784f, 0.8471f, 0.9020f) }, // light-blue
            { PlaceCategory.Danger, new Color(1f, 0.6471f, 0f) } // orange
        };

        // Place alignment
        [SerializeField] private Place leftPlace;
        public Place LeftPlace => leftPlace;
        [SerializeField] private Place rightPlace;
        public Place RightPlace => rightPlace;
        [SerializeField] private Place upPlace;
        public Place UpPlace => upPlace;
        [SerializeField] private Place downPlace;
        public Place DownPlace => downPlace;

        [SerializeField] private PlaceCategory category;
        public PlaceCategory Category => category;

        // Serialized card pool for this place
        [SerializeField] private List<CardData> possibleCards;

        // Guide system
        private bool _isRevealedToPlayer = false;
        public bool IsRevealedToPlayer => _isRevealedToPlayer;

        public CardData DrawCard()
        {
            if (possibleCards == null || possibleCards.Count == 0)
                return null;

            int randomIndex = Random.Range(0, possibleCards.Count);
            return possibleCards[randomIndex];
        }

        private void Start()
        {
            // Get the Renderer component and its material
            Renderer colorRenderer = GetComponent<Renderer>();
            if (colorRenderer)
            {
                _material = colorRenderer.material;
            }
            else
            {
                Debug.LogError("Renderer component not found on Place object!", this);
            }
        }

        public void ShowCategoryColor()
        {
            if (_material)
            {
                _shown = true;
                _material.color = CategoryColors[category];
                _isRevealedToPlayer = true;
            }
        }

        public void ShowCategoryColorForGuide(Guide guide)
        {
            if (_material && guide != null)
            {
                if (_isRevealedToPlayer || guide.CanSeeCategory(category))
                {
                    _material.color = CategoryColors[category];
                }
                else
                {
                    SetToDefaultColor();
                }
            }
        }

        public void SetToDefaultColor()
        {
            if (_material)
            {
                _material.color = Color.white;
            }
        }

        public void LightUp()
        {
            if (!_material) return;

            Color flashColor = new Color(0.95f, 1f, 0.47f);
            _material.color = flashColor;
        }

        public void LightOff()
        {
            if (!_material) return;
            
            if (_shown)
            {
                ShowCategoryColor();
            }
            else
            {
                SetToDefaultColor();
            }
        }

        // Method to check if a guide can see this place's category
        // public bool IsVisibleToGuide(Guide guide)
        // {
        //     return _isRevealedToPlayer || (guide != null && guide.CanSeeCategory(category));
        // }
        //
        // // Method to get the category for a specific guide (returns Unknown if not visible)
        // public PlaceCategory GetCategoryForGuide(Guide guide)
        // {
        //     if (_isRevealedToPlayer || (guide != null && guide.CanSeeCategory(category)))
        //     {
        //         return category;
        //     }
        //     return PlaceCategory.Empty; // Return Empty as default "unknown" category
        // }
    }
    
    public enum PlaceCategory
    {
        Empty,
        Resource,
        Ability,
        Danger,
        End
    }
}