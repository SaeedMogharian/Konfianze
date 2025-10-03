using UnityEngine;
using Card;
using System.Collections.Generic;

namespace GamePlace
{
    public class Place : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;
        
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
    }
    
    public enum PlaceCategory { Empty, Resource, Ability, Danger }
}