using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject shopSelectorScreen;
    [SerializeField] private GameObject itemSelectorScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private AudioMixer audioMixer;

    private GameObject[] screenArray;

    private const int MAIN_SCREEN_IDX = 0;
    private const int SHOP_SELECTOR_SCREEN_IDX = 1;
    private const int ITEM_SELECTOR_SCREEN_IDX = 2;
    private const int SETTINGS_SCREEN_IDX = 3;

    public int ShopIDX { get; private set; } = -1;

    public const int BIRD_SHOP_IDX = 0;
    public const int MISSILE_SHOP_IDX = 1;
    public const int PIPE_SHOP_IDX = 2;


    // Start is called before the first frame update
    void Start()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;

        audioMixer.SetFloat("Volume", 
            Utils.VolumeToDecibel(
                Utils.GetPlayerPref(
                    Utils.VOLUME_KEY, Utils.MAX_VOLUME
                )
            )
        );

        screenArray = new GameObject[]
        {
            mainScreen,
            shopSelectorScreen,
            itemSelectorScreen,
            settingsScreen
        };

        OpenMain();
    }

    public void OpenMain()
    {
        Utils.OpenScreen(MAIN_SCREEN_IDX, screenArray);
    }

    public void OpenShopSelector()
    {
        Utils.OpenScreen(SHOP_SELECTOR_SCREEN_IDX, screenArray);
    }
    public void OpenSettings()
    {
        Utils.OpenScreen(SETTINGS_SCREEN_IDX, screenArray);
    }

    public void OpenBirdShop()
    {
        ShopIDX = BIRD_SHOP_IDX;
        Utils.OpenScreen(ITEM_SELECTOR_SCREEN_IDX, screenArray);
    }

    public void OpenPipeShop()
    {
        ShopIDX = PIPE_SHOP_IDX;
        Utils.OpenScreen(ITEM_SELECTOR_SCREEN_IDX, screenArray);
    }
    public void OpenMissileShop()
    {
        ShopIDX = MISSILE_SHOP_IDX;
        Utils.OpenScreen(ITEM_SELECTOR_SCREEN_IDX, screenArray);
    }

    public void StartTheGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Utils.ExitGame();
    }
}
