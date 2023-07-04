using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicMainScript : MonoBehaviour
{
    [SerializeField] private AudioClip mainAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle deathToggle;
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject shopSelectorScreen;
    [SerializeField] private GameObject itemSelectorScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private GameObject playersMoney;
    [SerializeField] private TextMeshProUGUI priceTag;
    [SerializeField] private Image checkmark;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private float pipesScale;
    [SerializeField] private float missilesScale;
    [SerializeField] private float birdScale;

    [Range(0f, 1f)]
    [SerializeField] private float mainAudioVolume;

    private GameObject[] screenArray;
    private Sprite[] itemImageArray;
    private string itemsPath;
    private int currentIdx;
    private string select_key;
    private Dictionary<string, Item> storeItems;
    private string defaultName;

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

        Utils.playAudio(audioSource);

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
        screenArray[num].SetActive(true);
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
        openShop(Utils.DOWN_PATH, Utils.BIRD_KEY, Utils.DEFAULT_BIRD, true, 0, birdScale);
    }

    public void openPipeShop()
    {
        openShop(Utils.PIPES_PATH, Utils.PIPE_KEY,Utils.DEFAULT_PIPE, true, 0, pipesScale);
    }
    public void openMissileShop()
    {
        openShop(Utils.MISSILE_PATH, Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE, true, 90f, missilesScale);
    }
    public void openShop(string path, string key, string dName, bool switchHAndW, float angle, float scale)
    {
        defaultName = dName;
        itemImageArray = Resources.LoadAll<Sprite>(path).Where(c => storeItems.ContainsKey(c.name)).ToArray();
        Array.Sort(itemImageArray, 
            (x, y) => storeItems[x.name].price.CompareTo(storeItems[y.name].price));

        currentIdx = 0;
        select_key = key;

        float height = itemImageArray[0].rect.height;
        float width = itemImageArray[0].rect.width;


        if (switchHAndW)
        {
            itemImage.rectTransform.sizeDelta = new Vector2(width, height);
        }
        else 
        {
            itemImage.rectTransform.sizeDelta = new Vector2(height, width);
        }

        itemImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        itemImage.rectTransform.localScale = new Vector3(scale, scale);

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
                    item.name == defaultName);
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
