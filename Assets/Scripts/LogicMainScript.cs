using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Range(0f, 1f)][SerializeField] private float mainAudioVolume;

    private GameObject[] screenArray;
    private Sprite[] itemImageArray;
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

        AudioListener.pause = false;

        Time.timeScale = 1;

        storeItems = DataBase.GetData();

        audioSource.clip = mainAudio;
        audioSource.loop = true;
        audioSource.volume = mainAudioVolume;

        Utils.PlayAudio(audioSource);

        soundToggle.isOn = Utils.GetBool(Utils.SOUND_KEY);
        deathToggle.isOn = Utils.GetBool(Utils.DEATH_KEY);

        if (PlayerPrefs.HasKey(Utils.MONEY_KEY))
        {
            Utils.SetNumber(PlayerPrefs.GetInt(Utils.MONEY_KEY), playersMoney, false);
        }
        else
        {
            Utils.SetNumber(0, playersMoney, false);
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

    private void OpenScreen(int num)
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

    public void OpenMain()
    {
        OpenScreen(MAIN_SCREEN_IDX);
    }

    public void OpenShopSelector()
    {
        OpenScreen(SHOP_SELECTOR_SCREEN_IDX);
    }
    public void OpenSettings()
    {
        OpenScreen(SETTINGS_SCREEN_IDX);
    }

    public void StartTheGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenBirdShop()
    {
        OpenShop(Utils.DOWN_PATH, Utils.BIRD_KEY, Utils.DEFAULT_BIRD, true, 0, birdScale);
    }

    public void OpenPipeShop()
    {
        OpenShop(Utils.PIPES_PATH, Utils.PIPE_KEY, Utils.DEFAULT_PIPE, true, 0, pipesScale);
    }
    public void OpenMissileShop()
    {
        OpenShop(Utils.MISSILE_PATH, Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE, true, 90f, missilesScale);
    }
    public void OpenShop(string path, string key, string dName, bool switchHAndW, float angle, float scale)
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

        OpenScreen(ITEM_SELECTOR_SCREEN_IDX);
        SetItem(currentIdx);
    }

    public void CycleRight()
    {
        CycleIndex(1);
        SetItem(currentIdx);
    }
    public void CycleLeft()
    {
        CycleIndex(-1);
        SetItem(currentIdx);
    }

    private void CycleIndex(int step)
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

    private void SetItem(int idx)
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

    public void SeletItem()
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
            Utils.SetNumber(money, playersMoney, false);
            PlayerPrefs.SetInt(Utils.MONEY_KEY, money);

            priceTag.gameObject.SetActive(false);
            selectButton.interactable = true;
            selectButtonText.text = "Select";
            item.bought = true;
            storeItems[itemImageArray[currentIdx].name] = item;
            DataBase.StoreData(storeItems);
        }
    }

    public void Exit()
    {
        Utils.ExitGame();
    }

    public void SetSound(bool sound)
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

    public void SetDeathMode(bool mode)
    {
        PlayerPrefs.SetInt(Utils.DEATH_KEY, mode ? 1 : 0);
    }
}
