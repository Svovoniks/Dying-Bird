using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class Utils
{
    public const string BEST_SCORE_KEY = "best_score";
    public const string SOUND_KEY = "sound";
    public const string DEATH_KEY = "death";
    public const string MONEY_KEY = "money";
    public const string BIRD_KEY = "bird";
    public const string PIPE_KEY = "pipe";


    public const string DEFAULT_BIRD = "bird0";
    public const string DEFAULT_PIPE = "pipe-green";
    

    public const string PIPES_PATH = "Images/Pipes/";
    public const string DOWN_PATH = "Images/Birds/down/";
    public const string MIDDLE_PATH = "Images/Birds/middle/";
    public const string UP_PATH = "Images/Birds/up/";
    public const string NUMBERS_PATH = "Images/Numbers/";
    public const string EMPTY_PATH = "Images/empty";
   


    public static bool getBool(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return true;
        }
        if (PlayerPrefs.GetInt(key) == 1)
        {
            return true;
        }
        return false;
    }
    public static void exitGame()
    {
        Application.Quit();
    }

    public static void setNumber(int number, GameObject window, bool fillWithZeroes)
    {
        if (window == null)
        {
            return;
        }
        for (int i = 0; i < window.transform.childCount; i++)
        {
            if (number == 0 && i > 0 && !fillWithZeroes)
            {
                setOneNuber(window, i, EMPTY_PATH);
                continue;
            }
            setOneNuber(window, i, NUMBERS_PATH + number%10);
            number /= 10;
        }
    }

    private static void setOneNuber(GameObject window, int idx, string spritePath) 
    {
        window.transform.GetChild(idx).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>(spritePath);
    }
}

public struct Item 
{
    
    public int id;
    public string name;
    public int price;
    public bool bought;
    public string prettyName;
    public Item(int id, string name, int price, int bought, string prettyName) 
    {
        this.id = id;
        this.name = name;    
        this.price = price;
        this.bought = bought == 1 ? true : false;
        this.prettyName = prettyName;
    }

    public override readonly string ToString()
    {
        return id + "," + name + "," + price + "," + (bought ? 1 : 0) + "," + prettyName + "\n";
    }
}
public class DataBase
{
    private const string ASSESTS_PATH = "db";
    private const string DATABASE_PATH = "flappy bird_data/Resources/Database.db";

    //private const string DATABASE_PATH = "db"; For Unity Editor
    private static void initiateData()
    {
        File.WriteAllText(DATABASE_PATH, Resources.Load<TextAsset>(ASSESTS_PATH).text);
    }
    public static Dictionary<string, Item> getData()
    {
        if (Application.isPlaying)
        {
            Debug.Log("playing");
        }
        Dictionary<string, Item> dict = new();
        if (!File.Exists(DATABASE_PATH))
        {
            initiateData();
        }
        string[] lines = File.ReadAllLines(DATABASE_PATH);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] arr = lines[i].Split(',');
            if (arr.Length != 5)
            {
                continue;
            }
            dict.Add(arr[1], new Item
                (
                int.Parse(arr[0]),
                arr[1],
                int.Parse(arr[2]),
                int.Parse(arr[3]),
                arr[4]
                ));
        }

        return dict;
    }

    public static void storeData(Dictionary<string, Item> dict)
    {
        string lines = "id,name,price,bought,pretty_name\n";
        foreach (Item item in dict.Values)
        {
            lines += item.ToString();
        }

        File.WriteAllText(DATABASE_PATH, lines);
    }
}
