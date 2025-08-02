using UnityEngine;

namespace GamePlace
{
    public class Place : MonoBehaviour
    {

        [SerializeField] private int id;
        public int Id => id;
    
        [SerializeField] private Place leftPlace;
        public Place LeftPlace => leftPlace;
        [SerializeField] private Place rightPlace;
        public Place RightPlace => rightPlace;
        [SerializeField] private Place upPlace;
        public Place UpPlace => upPlace;
        [SerializeField] private Place downPlace;
        public Place DownPlace => downPlace;

        public void LighUp()
        {
            Debug.Log("Changed Color", gameObject);
        }
    
    }
}
