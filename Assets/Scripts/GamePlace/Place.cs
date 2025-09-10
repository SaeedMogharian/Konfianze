using UnityEngine;

namespace GamePlace
{
    public class Place : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;

        [SerializeField] private int food;
        public int Food => food; 
        [SerializeField] private int health;
        public int Healing => health;
        [SerializeField] private int powerupType;
        
        
        public int PowerupType => powerupType;
        [SerializeField] private bool danger;
        public bool Danger => danger;
        [SerializeField] private bool endpoint;
        public bool Endpoint => endpoint;
        
    
        [SerializeField] private Place leftPlace;
        public Place LeftPlace => leftPlace;
        [SerializeField] private Place rightPlace;
        public Place RightPlace => rightPlace;
        [SerializeField] private Place upPlace;
        public Place UpPlace => upPlace;
        [SerializeField] private Place downPlace;
        public Place DownPlace => downPlace;

        public void LightUp()
        {
            Debug.Log("Changed Color", gameObject);
        }
    }
}
