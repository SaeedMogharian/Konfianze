using UnityEngine;
using Card;
using System.Collections.Generic;

namespace GamePlace
{
    public class Place : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;
        
        // Define the color dictionary for categories
        private static readonly Dictionary<PlaceCategory, Color> CategoryColors = new Dictionary<PlaceCategory, Color>
        {
            { PlaceCategory.Empty, Color.gray },
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
        
        [SerializeField] private bool endpoint;
        public bool Endpoint => endpoint;
        
        [SerializeField] private PlaceCategory category;
        public PlaceCategory Category => category;
        
        // Serialized card pool for this place
        [SerializeField] private List<CardData> possibleCards;
        
        
        public CardData DrawCard()
        {
            if (possibleCards == null || possibleCards.Count == 0)
                return null;
                
            int randomIndex = Random.Range(0, possibleCards.Count);
            return possibleCards[randomIndex];
        }
        
        // Coloring 
        private Material _material; // Cache the material

        private void Start()
        {
            // Get the Renderer component and its material
            Renderer colorRenderer = GetComponent<Renderer>();
            if (colorRenderer)
            {
                _material = colorRenderer.material; // Use material to avoid affecting other objects
                SetToDefaultColor(); // Initialize to white
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
                _material.color = CategoryColors[category];
            }
        }

        public void SetToDefaultColor()
        {
            if (_material)
            {
                
            }
        }
    }

    public enum PlaceCategory
    {
        Empty, 
        Resource, 
        Ability, 
        Danger
    }
}