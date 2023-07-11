using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreScript : MonoBehaviour
{
    [SerializeField] private float birdScale;
    [SerializeField] private float pipeScale;
    [SerializeField] private float missileScale;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image checkmark;
    [SerializeField] private GameObject moneyView;
    [SerializeField] private GameObject descriptionView;
    [SerializeField] private TextMeshProUGUI priceTag;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private int currentIdx;
    private string defaultName;
    private string select_key;
    private Sprite[] itemImageArray;
    private LogicMenuScript logicMainScript;
    private Dictionary<string, Item> storeItems;

    // Start is called before the first frame update
    private void OnEnable()
    { 
        logicMainScript = FindObjectOfType<LogicMenuScript>();

        storeItems = DataBase.GetData();

        Utils.SetNumber(Utils.GetPlayerPref(Utils.MONEY_KEY, 0),
            moneyView, false);
        
        switch (logicMainScript.ShopIDX)
        {
            case LogicMenuScript.BIRD_SHOP_IDX:
                SetUpShop(Utils.DOWN_PATH, Utils.BIRD_KEY, Utils.DEFAULT_BIRD, true, 0, birdScale);
                break;

            case LogicMenuScript.MISSILE_SHOP_IDX:
                SetUpShop(Utils.MISSILE_PATH, Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE, true, 90f, missileScale);
                break;

            case LogicMenuScript.PIPE_SHOP_IDX:
                SetUpShop(Utils.PIPES_PATH, Utils.PIPE_KEY, Utils.DEFAULT_PIPE, true, 0, pipeScale);
                break;
        }

    }

    public void SetUpShop(string path, string key, string dName, bool switchHAndW, float angle, float scale)
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
        bool SBInteractable = true;
        if (item.bought)
        {
            selectButtonText.text = "Select";
            bool selected;
            if (PlayerPrefs.HasKey(select_key))
            {
                selected = PlayerPrefs.GetString(select_key) == item.name;
                checkmark.gameObject.SetActive(selected);
                
            }
            else
            {
                selected = item.name == defaultName;
                checkmark.gameObject.SetActive(selected);
            }
            SBInteractable = !selected;
            priceTag.gameObject.SetActive(false);
        }
        else
        {
            selectButtonText.text = "Buy";
            SBInteractable = Utils.GetPlayerPref(Utils.MONEY_KEY, 0) >= item.price;
            checkmark.gameObject.SetActive(false);
            priceTag.gameObject.SetActive(true);
            priceTag.text = "Price: " + item.price;
        }
        selectButton.interactable = SBInteractable;
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
            Utils.SetNumber(money, moneyView, false);
            PlayerPrefs.SetInt(Utils.MONEY_KEY, money);

            priceTag.gameObject.SetActive(false);
            selectButton.interactable = true;
            selectButtonText.text = "Select";
            item.bought = true;
            storeItems[itemImageArray[currentIdx].name] = item;
            DataBase.StoreData(storeItems);
        }
        selectButton.interactable = false;
    }

    public void ShowDescription() 
    {
        descriptionView.SetActive(true);
    }

    public void HideDescription() 
    {
        descriptionView.SetActive(false);
    }
}
