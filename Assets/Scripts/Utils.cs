using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class Utils
{
    public const string BEST_SCORE_KEY = "best_score";
    public const string SOUND_KEY = "sound";
    public const string VOLUME_KEY = "volume";
    public const string DEATH_KEY = "death";
    public const string MONEY_KEY = "money";
    public const string BIRD_KEY = "bird";
    public const string PIPE_KEY = "pipe";
    public const string MISSILE_KEY = "missile";


    public const string DEFAULT_BIRD = "bird0";
    public const string DEFAULT_PIPE = "pipe-green";
    public const string DEFAULT_MISSILE = "grey-missile";
    public const string BURNED_BIRD = "BurnedBird";
    public const string MAGNET_BIRD = "MagneticBird";


    public const float MAX_VOLUME = 1f;
    public const float MIN_VOLUME = 0.00000001f;


    public const string MISSILE_PATH = "Images/Missiles/";
    public const string PIPES_PATH = "Images/Pipes/";
    public const string DOWN_PATH = "Images/Birds/down/";
    public const string MIDDLE_PATH = "Images/Birds/middle/";
    public const string UP_PATH = "Images/Birds/up/";
    public const string NUMBERS_PATH = "Images/Numbers/";
    public const string EMPTY_PATH = "Images/empty";



    public static bool GetBool(string key)
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

    public static float DecibelToVolume(float decibel)
    {
        return math.exp10(decibel / 20);
    }

    public static float VolumeToDecibel(float volume)
    {
        return math.log10(volume) * 20;
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    public static void SetNumber(int number, GameObject window, bool fillWithZeroes)
    {
        if (window == null)
        {
            return;
        }
        for (int i = 0; i < window.transform.childCount; i++)
        {
            if (number == 0 && i > 0 && !fillWithZeroes)
            {
                SetOneNuber(window, i, EMPTY_PATH);
                continue;
            }
            SetOneNuber(window, i, NUMBERS_PATH + number % 10);
            number /= 10;
        }
    }

    public static bool CheckProbability(float probability)
    {
        return UnityEngine.Random.Range(1, 100) <= probability;
    }

    private static void SetOneNuber(GameObject window, int idx, string spritePath)
    {
        window.transform.GetChild(idx).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>(spritePath);
    }

    public static T GetPlayerPref<T>(string key, T defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) 
        {
            return defaultValue;
        }

        if (typeof(T) == typeof(float)) 
        {
            return PlayerPrefs.GetFloat(key).ConvertTo<T>();
        }

        if (typeof(T) == typeof(int))
        {
            return PlayerPrefs.GetInt(key).ConvertTo<T>();
        }

        if (typeof(T) == typeof(string))
        {
            return PlayerPrefs.GetString(key).ConvertTo<T>();
        }

        return defaultValue;
    }

    public static string GetSpriteName(string key, string defaultPath)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        return defaultPath;
    }
}

public struct Item
{

    public int id;
    public string name;
    public int price;
    public bool bought;
    public string prettyName;
    public string info;
    public Item(int id, string name, int price, int bought, string prettyName, string info)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.bought = bought == 1 ? true : false;
        this.prettyName = prettyName;
        this.info = info;
    }

    public override readonly string ToString()
    {
        return id + "," + name + "," + price + "," + (bought ? 1 : 0) + "," + prettyName + "," + info + "\n";
    }
}
public class DataBase
{
    private const string ASSESTS_PATH = "db";
    //private const string DATABASE_PATH = "flappy bird_data/Resources/Database.db";

    private const string DATABASE_PATH = "db";// For Unity Editor
    private static void InitiateData()
    {
        File.WriteAllText(DATABASE_PATH, Resources.Load<TextAsset>(ASSESTS_PATH).text);
        Dictionary<string, Item> data = GetData();

        string[] arr =
        {
            Utils.GetSpriteName(Utils.BIRD_KEY, Utils.DEFAULT_BIRD),
            Utils.GetSpriteName(Utils.PIPE_KEY, Utils.DEFAULT_PIPE),
            Utils.GetSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE)
        };

        foreach (string i in arr)
        {
            data[i] = new Item(data[i].id, data[i].name, data[i].price, 1, data[i].prettyName, data[i].info);
        }
        StoreData(data);
    }
    public static Dictionary<string, Item> GetData()
    {
        Dictionary<string, Item> dict = new();
        if (!File.Exists(DATABASE_PATH))
        {
            InitiateData();
        }
        string[] lines = File.ReadAllLines(DATABASE_PATH);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] arr = lines[i].Split(',');
            if (arr.Length != 6)
            {
                continue;
            }
            dict.Add(arr[1], new Item
                (
                int.Parse(arr[0]),
                arr[1],
                int.Parse(arr[2]),
                int.Parse(arr[3]),
                arr[4],
                arr[5]
                ));
        }

        return dict;
    }

    public static void StoreData(Dictionary<string, Item> dict)
    {
        string lines = "id,name,price,bought,pretty_name,info\n";
        foreach (Item item in dict.Values)
        {
            lines += item.ToString();
        }

        File.WriteAllText(DATABASE_PATH, lines);
    }
}
