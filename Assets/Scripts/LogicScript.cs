using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject moneyWindow;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePausedScreen;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject startMessage;
    [SerializeField] private GameObject bestScoreNotifier;
    [SerializeField] private GameObject audioCenter;
    [SerializeField] private Image missileCheckmark;
    [SerializeField] private Image missileImage;
    [SerializeField] private float missileTimeout;


    private int score;
    private int money;
    private bool initialWait;
    private float missileTimer;

    public bool canFire { get; private set; }
    
    private void Start()
    {
        initialPause();
        money = PlayerPrefs.GetInt(Utils.MONEY_KEY);
        Utils.setNumber(money, moneyWindow, false);
        
        missileImage.sprite = 
            Resources.Load<Sprite>(Utils.MISSILE_PATH +
            Utils.getSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE));
        missileImage.fillAmount = 1;

        missileTimer = missileTimeout;
        missileCheckmark.gameObject.SetActive(false);
        canFire = true;
    }

    private void Update()
    {
        if (initialWait && Input.GetKeyDown(KeyCode.Space))
        {
           realStart();
        }

        if (!missileCheckmark.IsActive() && !initialWait) 
        {
            updateMissile();
        }
    }

    private void updateMissile()
    {
        
        missileTimer += Time.deltaTime;
        
        if (missileTimer >= missileTimeout) 
        {
            missileTimer = missileTimeout;
            missileCheckmark.gameObject.SetActive(true);
            canFire = true;
        }

        missileImage.fillAmount = missileTimer/missileTimeout;
    }

    public void resetMissile() 
    {
        missileTimer = 0;
        canFire = false;
        missileCheckmark.gameObject.SetActive(false);
    }

    private void initialPause() 
    {
        initialWait = true;
        Time.timeScale = 0;
    }
    private void realStart() 
    {
        initialWait = false;
        startMessage.SetActive(false);
        Time.timeScale = 1;
    }
    public void addScore(int scoreToAdd) 
    {
        score += scoreToAdd;
        Utils.setNumber(score, scoreWindow, true);
    }
    public void addMoney(int moneyToAdd)
    {
        money += moneyToAdd;
        PlayerPrefs.SetInt(Utils.MONEY_KEY, money);
        Utils.setNumber(money, moneyWindow, false);
    }
    public void gameOver()
    {
        int bestScore = PlayerPrefs.HasKey(Utils.BEST_SCORE_KEY) ?
            PlayerPrefs.GetInt(Utils.BEST_SCORE_KEY) : 0;

        if (bestScore < score)
        {
            PlayerPrefs.SetInt(Utils.BEST_SCORE_KEY, score);
            bestScoreNotifier.gameObject.SetActive(true);
        }

        pauseButton.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseButton.SetActive(false);
        gamePausedScreen.SetActive(true);
    }

    public void continueGame()
    {
        pauseButton.SetActive(true);
        AudioListener.pause = false;
        gamePausedScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void exitGame()
    {
        Utils.exitGame();
    }

    public void openMenu() 
    {
        SceneManager.LoadScene(0);
    }
}
