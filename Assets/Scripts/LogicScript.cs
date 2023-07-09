using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject gameOverScoreWindow;
    [SerializeField] private GameObject moneyWindow;
    [SerializeField] private GameObject magnetWindow;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePausedScreen;
    [SerializeField] private GameObject gameOverlayScreen;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject startMessage;
    [SerializeField] private GameObject bestScoreNotifier;
    [SerializeField] private GameObject audioCenter;
    [SerializeField] private Image missileCheckmark;
    [SerializeField] private Image missileImage;
    [SerializeField] private Image magnetBar;
    [SerializeField] private float missileTimeout;
    [SerializeField] private float magnetTime;
    [SerializeField][Range(0, 9)] private int magnetLimit;
    [SerializeField] private float coinSpeed;


    private bool initialWait;
    private bool playing;
    private float missileTimer;
    private float lastMagnetTime;
    private GameObject[] screenArray;
    private int GAME_OVER_SCREEN_IDX = 0;
    private int GAME_PAUSED_SCREEN_IDX = 1;
    private int GAME_OVERLAY_SCREEN_IDX = 2;

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
            Utils.SetNumber(_score, scoreWindow, true);
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
            Utils.SetNumber(_money, moneyWindow, false);
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
            Utils.SetNumber(_magnets, magnetWindow, false);
        }
    }
    

    public bool CanFire { get; private set; }
    public bool UsingMagnet { get; private set; }

    private void Start()
    {
        InitialPause();

        Score = 0;
        Magnets = 0;

        screenArray = new GameObject[]
        {
            gameOverScreen,
            gamePausedScreen,
            gameOverlayScreen
        };

        UsingMagnet = false;

        lastMagnetTime = -magnetTime;

        AudioListener.pause = false;
        

        Money = PlayerPrefs.GetInt(Utils.MONEY_KEY);

        missileImage.sprite =
            Resources.Load<Sprite>(Utils.MISSILE_PATH +
            Utils.GetSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE));
        missileImage.fillAmount = 1;

        missileTimer = missileTimeout;
        missileCheckmark.gameObject.SetActive(false);
        CanFire = true;
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

        if (UsingMagnet)
        {
            if (lastMagnetTime + magnetTime < Time.time || !playing)
            {
                UsingMagnet = false;
                magnetBar.gameObject.SetActive(false);
            }

            magnetBar.fillAmount = (lastMagnetTime + magnetTime - Time.time) / magnetTime;
        }

        if (!missileCheckmark.IsActive() && !initialWait)
        {
            UpdateMissile();
        }
    }

    
    private void UpdateMissile()
    {

        missileTimer += Time.deltaTime;

        if (missileTimer >= missileTimeout)
        {
            missileTimer = missileTimeout;
            missileCheckmark.gameObject.SetActive(true);
            CanFire = true;
        }

        missileImage.fillAmount = missileTimer / missileTimeout;
    }

    public void ResetMissile()
    {
        missileTimer = 0;
        CanFire = false;
        missileCheckmark.gameObject.SetActive(false);
    }

    private void InitialPause()
    {
        initialWait = true;
        Time.timeScale = 0;
    }
    private void RealStart()
    {
        initialWait = false;
        playing = true;
        startMessage.SetActive(false);
        Time.timeScale = 1;
    }
    private void UseMagnet()
    {
        if (lastMagnetTime + magnetTime < Time.time && Magnets > 0) 
        {
            Magnets--;
            lastMagnetTime = Time.time;
            UsingMagnet = true;
            magnetBar.gameObject.SetActive(true);
        }
    }
    public void GameOver()
    {
        playing = false;
        int bestScore = PlayerPrefs.HasKey(Utils.BEST_SCORE_KEY) ?
            PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY) : 0;

        if (bestScore < Score)
        {
            PlayerPrefs.SetInt(Utils.BEST_SCORE_KEY, Score);
            bestScoreNotifier.SetActive(true);
        }
        Utils.SetNumber(Score, gameOverScoreWindow, true);
        Utils.OpenScreen(GAME_OVER_SCREEN_IDX, screenArray);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        playing = false;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Utils.OpenScreen(GAME_PAUSED_SCREEN_IDX, screenArray);
    }

    public void ContinueGame()
    {
        Utils.OpenScreen(GAME_OVERLAY_SCREEN_IDX, screenArray);
        playing = true;
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
}
