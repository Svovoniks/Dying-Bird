using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreenScipt : MonoBehaviour
{
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject bestScore;

    // Start is called before the first frame update
    void Start()
    {
        LogicScript logicScript = FindObjectOfType<LogicScript>();
        Utils.SetNumber(logicScript.Score, scoreWindow, true);

        if (logicScript.Score > Utils.GetPlayerPref(Utils.BEST_SCORE_KEY, 0))
        {
            bestScore.SetActive(true);
        }
    }

}
