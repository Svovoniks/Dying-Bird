using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
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

    public static void OpenScreen(int num, GameObject[] screenArray)
    {
        for (int i = 0; i < screenArray.Length; i++)
        {
            if (i == num)
            {
                continue;
            }
            screenArray[i].gameObject.SetActive(false);
        }
        screenArray[num].gameObject.SetActive(true);
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
    public string description;
    public Item(int id, string name, int price, int bought, string prettyName, string info, string description)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.bought = bought == 1 ? true : false;
        this.prettyName = prettyName;
        this.info = info;
        this.description = description;
    }

    public void UpdateValue() 
    {
        id = 0;
    }

    public override readonly string ToString()
    {
        return id + "," + name + "," + price + "," + (bought ? 1 : 0) + "," + prettyName + "," + info + "\n";
    }
}

public class NewDataBase 
{
    private const string DATABASE_NAME = "DataBase.db";
    private static NewDataBase instance;
    private Dictionary<string, Item> data;

    private NewDataBase() 
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, DATABASE_NAME)))
        {
            InitiateDataBase();
        }

        CheckDataBase();

        data = ReadData(Path.Combine(Application.persistentDataPath, DATABASE_NAME));
    }

    private static Dictionary<string, Item> ReadData(string path)
    {
        Dictionary<string, Item> newData = new();

        string query = "SELECT id, name, price, bought, pretty_name, info, description FROM main;";

        using SqliteConnection connection = new("Data Source=" + path);
        connection.Open();

        SqliteCommand command = new(query, connection);

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Item item = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6)
                );
            newData.Add(item.name, item);
        }

        return newData;
    }

    private static void CheckDataBase() 
    {
        string versionQuery = "PRAGMA user_version;";

        using SqliteConnection connection1 = new("Data Source=" +
            Path.Combine(Application.persistentDataPath, DATABASE_NAME));

        using SqliteConnection connection2 = new("Data Source=" +
        Path.Combine(Application.streamingAssetsPath, DATABASE_NAME));

        connection1.Open();
        connection2.Open();

        SqliteCommand command1 = new(versionQuery, connection1);
        SqliteCommand command2 = new(versionQuery, connection2);

        using SqliteDataReader reader1 = command1.ExecuteReader();
        using SqliteDataReader reader2 = command2.ExecuteReader();

        reader1.Read();

        int v_1 = reader1.GetInt32(0);

        reader2.Read();

        int v_2 = reader2.GetInt32(0);

        if (v_1 != v_2)
        {
            connection1.Close();
            UpdateDataBase();
            Debug.Log("Updated database");
        }
    }

    private static void UpdateDataBase()
    {
        List<int> boughtItems = new();

        using (SqliteConnection connection = new("Data Source=" +
            Path.Combine(Application.persistentDataPath, DATABASE_NAME))) 
        {
            connection.Open();
            string boughtQuery = "SELECT id FROM main WHERE bought = 1;";
            SqliteCommand boughtCommand = new(boughtQuery, connection);

            using SqliteDataReader boughtReader = boughtCommand.ExecuteReader();

            while (boughtReader.Read())
            {
                boughtItems.Add(boughtReader.GetInt32(0));
            }
        }

        InitiateDataBase();

        using SqliteConnection newConnection = new("Data Source=" +
            Path.Combine(Application.persistentDataPath, DATABASE_NAME));

        newConnection.Open();

        foreach (int item in boughtItems) 
        {
            using SqliteCommand buy = new("UPDATE main SET bought = 1 WHERE id = " + item + ";", newConnection);
            buy.ExecuteNonQuery();
        }
    }

    private static void InitiateDataBase()
    {
        File.Copy(Path.Combine(Application.streamingAssetsPath, DATABASE_NAME)
            , Path.Combine(Application.persistentDataPath, DATABASE_NAME), true);
    }

    public static Dictionary<string, Item> GetData()
    {
        instance ??= new NewDataBase();
        return instance.data;
    }

    public static void BuyItem(Item item) 
    {
        string query = "UPDATE main SET bought = 1 WHERE id = " + item.id + ";";

        using (SqliteConnection connection = new("Data Source=" +
            Path.Combine(Application.persistentDataPath, DATABASE_NAME))) 
        {
            connection.Open();
            SqliteCommand command = new(query, connection);
            command.ExecuteNonQuery();
        }

        item.bought = true;
        instance.data[item.name] = item;
    }

}

interface IDamagable
{
    public void TakeDamage(float damage);
}
