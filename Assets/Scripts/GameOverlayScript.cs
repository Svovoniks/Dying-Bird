using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlayScript : GameOverlay
{
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject magnetWindow;
    [SerializeField] private GameObject moneyWindow;
    [SerializeField] private GameObject startMessage;
    [SerializeField] private Image magmetTimerImage;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        SetMagnetBar();
        SetMissile();
    }

    private void SetMagnetBar()
    {
        if (logicScript.UsingMagnet)
        {
            if (!magmetTimerImage.IsActive())
            {
                magmetTimerImage.gameObject.SetActive(true);
            }
            magmetTimerImage.fillAmount = logicScript.MagnetFill;
        }
        else if (magmetTimerImage.IsActive())
        {
            magmetTimerImage.gameObject.SetActive(false);
        }
    }

    public override void SetMagnetCount(int count)
    {
        Utils.SetNumber(count, magnetWindow, false);
    }

    public override void SetMoney(int money)
    {
        Utils.SetNumber(money, moneyWindow, false);
    }

    public override void SetScore(int score)
    {
        Utils.SetNumber(score, scoreWindow, true);
    }

    public override void RemoveStartMessage()
    {
        startMessage.SetActive(false);
    }

    public override void SetBirdHeath(float fill)
    {
        Debug.Log("Unexpected call: SetBirdHeath in GameOverlay");
    }

    public override void SetBossHealth(float fill)
    {
        Debug.Log("Unexpected call: SetBossHealth in GameOverlay");
    }
}
