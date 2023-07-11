using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossOverlayScript : GameOverlay
{
    [SerializeField] private Image birdHealth;
    [SerializeField] private Image bossHealth;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        SetMissile();
    }

    public override void RemoveStartMessage()
    {
        Debug.Log("Unexpected call: RemoveStartMessage in BossOverlay");
    }

    public override void SetMagnetCount(int count)
    {
        Debug.Log("Unexpected call: SetMagnetCount in BossOverlay");
    }

    public override void SetMoney(int money)
    {
        Debug.Log("Unexpected call: SetMoney in BossOverlay");
    }

    public override void SetScore(int score)
    {
        Debug.Log("Unexpected call: SetScore in BossOverlay");
    }

    public override void SetBirdHeath(float fill)
    {
        birdHealth.fillAmount = fill;
    }

    public override void SetBossHealth(float fill)
    {
        bossHealth.fillAmount = fill;
    }
}
