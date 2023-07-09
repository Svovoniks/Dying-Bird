using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlayScript : MonoBehaviour
{
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject magnetWindow;
    [SerializeField] private GameObject moneyWindow;
    [SerializeField] private Image missileImage;
    [SerializeField] private Image missileCkeckMarkImage;
    [SerializeField] private Image magmetTimerImage;

    private int score;
    private int money;
    private float missileTimer;
    private float magnetTimer;
    private bool initialWait;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
