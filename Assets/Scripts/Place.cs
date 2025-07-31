using System;
using UnityEngine;

public abstract class Place : MonoBehaviour
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

    private void OnEnable()
    {
        GameBoard.OnStateChange += HandleNewState;
    }
    
    private void OnDisable()
    {
        GameBoard.OnStateChange -= HandleNewState;
    }

    private void HandleNewState(RoundState newState)
    {
        if (newState == RoundState.Choose)
        {
            var possibleMoves = GameBoard.Instance.GetPossibleMoves();
            if (possibleMoves.Contains(this))
            {
                Debug.Log("Changed Color", gameObject);
            }
        }
    }
}
