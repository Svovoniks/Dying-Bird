using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BirdScript : MonoBehaviour
{

    public Rigidbody2D body;
    public float jump = 1;
    public LogicScript logicScript;
    public static bool isAlive;
    public float frameRate = 12f;
    public AudioSource flapSource;
    public AudioSource deathSource;
    public AudioSource hitSource;
    public AudioSource lostSource;
    public AudioClip flapClip;
    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip lostClip;


    private SpriteRenderer birdRenderer;
    private string spriteName;
    private Sprite[] frames = new Sprite[3];
    private float timer = 0f;
    private int currentFrameIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(Utils.BIRD_KEY))
        {
            spriteName = PlayerPrefs.GetString(Utils.BIRD_KEY);
        }
        else
        {
            spriteName = Utils.DEFAULT_BIRD;
        }

        frames[0] = Resources.Load<Sprite>(Utils.DOWN_PATH + spriteName);
        frames[1] = Resources.Load<Sprite>(Utils.MIDDLE_PATH + spriteName);
        frames[2] = Resources.Load<Sprite>(Utils.UP_PATH + spriteName);
        
        birdRenderer = transform.GetComponent<SpriteRenderer>();
        birdRenderer.sprite = frames[0];
        isAlive = true;
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isAlive) 
        {
            body.velocity = Vector2.up * jump;
            if (Utils.getBool(Utils.SOUND_KEY)) 
            {
                flapSource.Play();
            }
        }

        if ((math.abs(transform.position.y) > 1.5 || math.abs(transform.position.x) > 2.6) && isAlive) 
        {
            if (Utils.getBool(Utils.SOUND_KEY) && isAlive) 
            {
                lostSource.Play();
            }
            
            die();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate && isAlive)
        {
            timer = 0f;
            currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
            birdRenderer.sprite = frames[currentFrameIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PlayerPrefs.HasKey(Utils.DEATH_KEY) && PlayerPrefs.GetInt(Utils.DEATH_KEY) == 0)
        {
            return;
        }
        if (isAlive && Utils.getBool(Utils.SOUND_KEY)) 
        {
            hitSource.Play();
        }
        die();
    }

    private void die() 
    {     
        if (isAlive && Utils.getBool(Utils.SOUND_KEY))
        {
            deathSource.Play();
        }
        isAlive = false;
        logicScript.gameOver();
    }
}
