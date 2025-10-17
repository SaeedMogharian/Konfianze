using System.Collections.Generic;
using UnityEngine;
using GamePlace;
using Player;

namespace Guides
{
    public class GuideManager : MonoBehaviour
    {
        public static GuideManager Instance { get; private set; }
        
        [SerializeField] private List<Guide> availableGuides;
        [SerializeField] private Guide currentGuide;
        
        private List<Place> _playerMoves;
        private PlayerStatusController _playerStatus;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Get references
            if (GameBoard.Instance != null)
            {
                // You might need to adjust how you get player moves based on your implementation
            }
            
            _playerStatus = FindFirstObjectByType<PlayerStatusController>();
            
            // Set default guide if none is set
            if (currentGuide == null && availableGuides.Count > 0)
            {
                currentGuide = availableGuides[0];
            }
        }
        
        public void SetCurrentGuide(Guide guide)
        {
            currentGuide = guide;
            UpdateGuideVisibility();
        }
        
        public Guide GetCurrentGuide()
        {
            return currentGuide;
        }
        
        public List<Guide> GetAvailableGuides()
        {
            return new List<Guide>(availableGuides);
        }
        
        public void UpdateGuideVisibility()
        {
            // Update all places based on current guide's visibility
            Place[] allPlaces = FindObjectsOfType<Place>();
            foreach (Place place in allPlaces)
            {
                place.ShowCategoryColorForGuide(currentGuide);
            }
        }
        
        public float GetPlaceScore(Place place)
        {
            if (currentGuide == null || _playerStatus == null) return 0f;
            
            // Get player moves from GameBoard
            List<Place> playerMoves = new List<Place>();
            if (GameBoard.Instance != null)
            {
                // You'll need to expose playerMoves from GameBoard or get them another way
            }
            
            return currentGuide.CalculateScore(place, _playerStatus, playerMoves);
        }
        
        // public string GetGuideComment(Place place)
        // {
        //     if (currentGuide == null || _playerStatus == null) return "";
        //     
        //     List<Place> playerMoves = new List<Place>();
        //     if (GameBoard.Instance != null)
        //     {
        //         // You'll need to expose playerMoves from GameBoard or get them another way
        //     }
        //     
        //     return currentGuide.GetComment(place, _playerStatus, playerMoves);
        // }
        
        // Call this when the player moves to update revealed places
        public void OnPlayerMove(Place newPlace)
        {
            UpdateGuideVisibility();
        }
    }
}