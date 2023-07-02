using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public static bool isAlive;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float jump = 1;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private AudioSource flapSource;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioSource lostSource;
    [SerializeField] private AudioClip flapClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip lostClip;
    [SerializeField] private GameObject missile;

    private SpriteRenderer birdRenderer;
    private string spriteName;
    private Sprite[] frames = new Sprite[3];
    private float timer = 0f;
    private int currentFrameIndex = 0;
    private LogicScript logicScript;
    // Start is called before the first frame update
    void Start()
    {
        spriteName = Utils.getSpriteName(Utils.BIRD_KEY, Utils.DEFAULT_BIRD);

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
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            fire();
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

    private void fire() 
    {
        if (isAlive && Time.timeScale == 1 && logicScript.canFire) 
        {
            logicScript.resetMissile();
            Instantiate(missile, transform.position, new Quaternion());
        }
    }
}
