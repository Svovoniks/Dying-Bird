using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxScript : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject magnet;
    [SerializeField][Range(0f, 100f)] private float coinProbability;
    [SerializeField][Range(0f, 100f)] private float magnetProbability;

    // Start is called before the first frame update
    void Start()
    {
        if (Utils.CheckProbability(coinProbability))
        {
            coin.SetActive(true);
        }
        else if (Utils.CheckProbability(magnetProbability)) 
        {
            magnet.SetActive(true);
        }
    }
}
