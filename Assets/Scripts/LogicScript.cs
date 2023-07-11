using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScoreWindow;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePausedScreen;
    [SerializeField] private GameObject gameOverlayScreen;
    [SerializeField] private GameObject bossGameOverlayScreen;
    [SerializeField] private GameObject bestScoreNotifier;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameOverlay gameOverlayScript;
    [SerializeField] private GameOverlay bossOverlayScript;
    [SerializeField] private Transform bossPosition;
    [SerializeField] private float missileTimeout;
    [SerializeField] private float magnetTime;
    [SerializeField][Range(0, 9)] private int magnetLimit;
    

    private bool initialWait;
    private bool preparingForBoss;
    private bool waitingForBird;

    private float missileTimer;
    private float lastMagnetTime;

    private GameObject[] screenArray;
    private PipeSpawnerScript pipeSpawner;
    private BirdScript bird;
    

    private const int GAME_OVER_SCREEN_IDX = 0;
    private const int GAME_PAUSED_SCREEN_IDX = 1;
    private const int GAME_OVERLAY_SCREEN_IDX = 2;
    private const int BOSS_GAME_OVERLAY_SCREEN_IDX = 3;

    public GameOverlay GameOverlay { get; private set; }

    public float MagnetFill { get; private set; }
    public float MissileFill { get; private set; }
    public bool Playing { get; private set; }

    private int _score;
    public int Score
    {
        get 
        {
            return _score;
        }
        set 
        {
            _score = value;
            if (_score != 0 && _score % pipeSpawner.GetBossInterval() == 0) 
            {
                preparingForBoss = true;
            }
            GameOverlay.SetScore(_score);
        }
    }

    private int _money;
    public int Money
    {
        get 
        {
            return _money;
        }
        set
        {
            _money = value;
            PlayerPrefs.SetInt(Utils.MONEY_KEY, _money);
            GameOverlay.SetMoney(_money);
        }
    }

    private int _magnets;
    public int Magnets
    {
        get
        {
            return _magnets;
        }
        set
        {
            if (value > magnetLimit) 
            {
                return;
            }
            _magnets = value;
            GameOverlay.SetMagnetCount(_magnets);
        }
    }
    

    public bool CanFire { get; private set; }

    public bool UsingMagnet { get; private set; }

    private void Start()
    { 
        InitialPause();

        GameOverlay = gameOverlayScript;

        AudioListener.pause = false;

        MissileFill = 1;

        CanFire = false;

        MagnetFill = 0;

        UsingMagnet = false;

        lastMagnetTime = -magnetTime;

        Score = 0;

        Magnets = 0;

        screenArray = new GameObject[]
        {
            gameOverScreen,
            gamePausedScreen,
            gameOverlayScreen,
            bossGameOverlayScreen
        };

        pipeSpawner = FindObjectOfType<PipeSpawnerScript>();
        bird = FindObjectOfType<BirdScript>();

        bird.JustDied += OnBirdDeath;

        Money = PlayerPrefs.GetInt(Utils.MONEY_KEY);

        missileTimer = missileTimeout;
    }

    private void Update()
    {
        if (initialWait && Input.GetKeyDown(KeyCode.Space))
        {
            RealStart();
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            UseMagnet();
        }

        if (!CanFire && !initialWait)
        {
            UpdateMissile();
        }

        if (UsingMagnet)
        {
            if (lastMagnetTime + magnetTime < Time.time || !Playing)
            {
                UsingMagnet = false;
            }

            MagnetFill = (lastMagnetTime + magnetTime - Time.time) / magnetTime;
        }

        BossCheck();
    }

    private void InitialPause()
    {
        initialWait = true;
        Time.timeScale = 0;
    }

    private void RealStart()
    {
        initialWait = false;
        Playing = true;
        GameOverlay.RemoveStartMessage();
        Time.timeScale = 1;
        CanFire = true;
    }

    private void UseMagnet()
    {
        if (lastMagnetTime + magnetTime < Time.time && Magnets > 0)
        {
            Magnets--;
            lastMagnetTime = Time.time;
            UsingMagnet = true;
        }
    }

    private void UpdateMissile()
    {
        missileTimer += Time.deltaTime;

        if (missileTimer >= missileTimeout)
        {
            missileTimer = missileTimeout;
            CanFire = true;
        }

        MissileFill = missileTimer / missileTimeout;
    }

    public void ResetMissile()
    {
        missileTimer = 0;
        CanFire = false;
    }

    private void BossCheck()
    {
        if (preparingForBoss)
        {
            PrepareForBoss();
        }
        else if (waitingForBird && bird.BossReady)
        {
            Utils.OpenScreen(BOSS_GAME_OVERLAY_SCREEN_IDX, screenArray);
            Instantiate(boss, bossPosition.position, new Quaternion());
            GameOverlay = bossOverlayScript;

            waitingForBird = false;
        }
    }

    private void PrepareForBoss()
    {
        if (!(FindObjectsByType<PipeScript>(FindObjectsSortMode.None).Length == 0))
        {
            return;
        }

        bird.GoHome();

        preparingForBoss = false;
        waitingForBird = true;
    }
    public void ExitBoss() 
    {
        Utils.OpenScreen(GAME_OVERLAY_SCREEN_IDX, screenArray);
        GameOverlay = gameOverlayScript;
        pipeSpawner.Activate();
    }

    public void GameOver()
    {
        Playing = false;
        Utils.OpenScreen(GAME_OVER_SCREEN_IDX, screenArray);

        if (Score > Utils.GetPlayerPref(Utils.BEST_SCORE_KEY, 0)) 
        {
            PlayerPrefs.SetInt(Utils.BEST_SCORE_KEY, Score);
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        Playing = false;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Utils.OpenScreen(GAME_PAUSED_SCREEN_IDX, screenArray);
    }

    public void ContinueGame()
    {
        Utils.OpenScreen(GAME_OVERLAY_SCREEN_IDX, screenArray);
        Playing = true;
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Utils.ExitGame();
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnBirdDeath(object sender, EventArgs e) 
    {
        GameOver();
    }
}
