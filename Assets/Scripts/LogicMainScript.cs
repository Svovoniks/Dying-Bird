using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicMainScript : MonoBehaviour
{
    public AudioClip mainAudio;
    public AudioSource audioSource;
    public Toggle soundToggle;
    public Toggle deathToggle;
    public GameObject mainScreen;
    public GameObject shopSelectorScreen;
    public GameObject itemSelectorScreen;
    public GameObject settingsScreen;
    public Image birdImage;
    public Image pipeImage;
    public Text itemName;
    public GameObject playersMoney;
    public Text priceTag;
    public Image checkmark;
    public Button selectButton;
    public Text selectButtonText;
    public Text bestScoreText;

    [Range(0f, 1f)]
    public float mainAudioVolume;

    private GameObject[] screenArray;
    private Sprite[] itemImageArray;
    private string itemsPath;
    private int currentIdx;
    private Image itemImage;
    private string select_key;
    private Dictionary<string, Item> storeItems;

    private const int MAIN_SCREEN_IDX = 0;
    private const int SHOP_SELECTOR_SCREEN_IDX = 1;
    private const int ITEM_SELECTOR_SCREEN_IDX = 2;
    private const int SETTINGS_SCREEN_IDX = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        screenArray = new GameObject[]
        {
            mainScreen, 
            shopSelectorScreen,
            itemSelectorScreen,
            settingsScreen
        };

        storeItems = DataBase.getData();

        audioSource.clip = mainAudio;
        audioSource.loop = true;
        audioSource.volume = mainAudioVolume;
        if (Utils.getBool(Utils.SOUND_KEY)) 
        {
            audioSource.Play();
        }
        soundToggle.isOn = Utils.getBool(Utils.SOUND_KEY);
        deathToggle.isOn = Utils.getBool(Utils.DEATH_KEY);

        if (PlayerPrefs.HasKey(Utils.MONEY_KEY))
        {
            Utils.setNumber(PlayerPrefs.GetInt(Utils.MONEY_KEY), playersMoney, false);
        }
        else 
        {
            Utils.setNumber(0, playersMoney, false);
        }

        if (PlayerPrefs.HasKey(Utils.BEST_SCORE_KEY))
        {
            bestScoreText.gameObject.SetActive(true);
            bestScoreText.text = "Best Score: " + PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY);
        }
        else 
        {
            bestScoreText.gameObject.SetActive(false);
        }
    }

    private void openScreen(int num)
    {
        for (int i = 0; i < screenArray.Length; i++) 
        {
            if (i == num)
            {
                continue;
            }
            screenArray[i].SetActive(false);
        }
        screenArray[num] .SetActive(true);
    }

    public void openMain() 
    {
        openScreen(MAIN_SCREEN_IDX);
    }

    public void openShopSelector() 
    {
        openScreen(SHOP_SELECTOR_SCREEN_IDX);
    }
    public void openSettings()
    {
        openScreen(SETTINGS_SCREEN_IDX);
    }

    public void startTheGame()
    {
        SceneManager.LoadScene(1);
    }

    public void openBirdShop() 
    {
        itemImageArray = Resources.LoadAll<Sprite>(Utils.DOWN_PATH);
        currentIdx = 0;
        itemImage = birdImage;
        select_key = Utils.BIRD_KEY;

        birdImage.gameObject.SetActive(true);
        pipeImage.gameObject.SetActive(false);

        openScreen(ITEM_SELECTOR_SCREEN_IDX);
        setItem(currentIdx);
    }

    public void openPipeShop()
    {
        itemImageArray = Resources.LoadAll<Sprite>(Utils.PIPES_PATH);
        currentIdx = 0;
        itemImage = pipeImage;
        select_key = Utils.PIPE_KEY;
        
        birdImage.gameObject.SetActive(false);
        pipeImage.gameObject.SetActive(true);
        
        openScreen(ITEM_SELECTOR_SCREEN_IDX);
        setItem(currentIdx);
    }

    public void cycleRight()
    {
        cycleIndex(1);
        setItem(currentIdx);
    }
    public void cycleLeft()
    {
        cycleIndex(-1);
        setItem(currentIdx);
    }

    private void cycleIndex(int step)
    {
        currentIdx += step;
        if (currentIdx >= 0) 
        {
            currentIdx %= itemImageArray.Length;
        }
        else 
        {
            currentIdx += itemImageArray.Length; 
        }
    }

    private void setItem(int idx)
    {
        itemImage.sprite = itemImageArray[idx];

        Item item = storeItems[itemImageArray[idx].name];

        if (item.bought)
        {
            selectButtonText.text = "Select";
            selectButton.interactable = true;
            if (PlayerPrefs.HasKey(select_key))
            {
                checkmark.gameObject.SetActive(
                    PlayerPrefs.GetString(select_key) ==
                    item.name);
            }
            else 
            {
                checkmark.gameObject.SetActive(
                    (item.name == Utils.DEFAULT_BIRD &&
                    select_key == Utils.BIRD_KEY) 
                    ||
                    (item.name == Utils.DEFAULT_PIPE && 
                    select_key == Utils.PIPE_KEY));
            }
            priceTag.gameObject.SetActive(false);
        }
        else 
        {
            selectButtonText.text = "Buy";
            selectButton.interactable = 
                PlayerPrefs.GetInt(Utils.MONEY_KEY) >= item.price;
            checkmark.gameObject.SetActive(false);
            priceTag.gameObject.SetActive(true);
            priceTag.text = "Price: " + item.price;
        }
        itemName.text = item.prettyName;
    }

    public void seletItem() 
    {
        Item item = storeItems[itemImageArray[currentIdx].name];

        if (item.bought)
        {
            PlayerPrefs.SetString(select_key, item.name);
            checkmark.gameObject.SetActive(true);
        }
        else
        {
            int money = PlayerPrefs.GetInt(Utils.MONEY_KEY);
            money -= item.price;
            Utils.setNumber(money, playersMoney, false);
            PlayerPrefs.SetInt(Utils.MONEY_KEY, money);

            priceTag.gameObject.SetActive(false);
            selectButton.interactable = true;
            selectButtonText.text = "Select";
            item.bought = true;
            storeItems[itemImageArray[currentIdx].name] = item;
            DataBase.storeData(storeItems);
        }
    }

    public void exit()
    {
        Utils.exitGame();
    }

    public void setSound(bool sound) 
    {
        PlayerPrefs.SetInt(Utils.SOUND_KEY, sound ? 1 : 0);
        if (sound)
        {
            audioSource.Play();
        }
        else 
        {
            audioSource.Stop();
        }
    }

    public void setDeathMode(bool mode) 
    {
        PlayerPrefs.SetInt(Utils.DEATH_KEY, mode ? 1 : 0);
    }
}
