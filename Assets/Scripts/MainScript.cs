using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestScore;

    private void Start()
    {
        if (PlayerPrefs.HasKey(Utils.BEST_SCORE_KEY))
        {
            bestScore.gameObject.SetActive(true);
            bestScore.text = "Best score: " + PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY);
        }
        else
        {
            bestScore.gameObject.SetActive(false);
        }
    }
}