using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameOverlay : MonoBehaviour
{
    [SerializeField] private Image missileImage;
    [SerializeField] private GameObject missileCheckMark;

    protected LogicScript logicScript;

    protected void Init()
    {
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        missileImage.sprite = Resources.Load<Sprite>(Utils.MISSILE_PATH +
            Utils.GetPlayerPref(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE));

    }

    protected void SetMissile() 
    {
        if (logicScript.CanFire)
        {
            if (!missileCheckMark.activeSelf) 
            {
                missileImage.fillAmount = 1;
                missileCheckMark.SetActive(true);
            }
            return;
        }

        missileImage.fillAmount = logicScript.MissileFill;

        if (missileCheckMark.activeSelf) 
        {
            missileCheckMark.SetActive(false);
        }
    }

    public abstract void SetScore(int score);
    public abstract void SetMoney(int money);
    public abstract void SetMagnetCount(int count);
    public abstract void RemoveStartMessage();
    public abstract void SetBirdHeath(float fill);
    public abstract void SetBossHealth(float fill);
}
