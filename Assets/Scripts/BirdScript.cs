using System;
using Unity.Mathematics;
using UnityEngine;

public class BirdScript : MonoBehaviour, IDamagable
{
    public static bool isAlive;
    public event EventHandler JustDied;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float jump = 1;
    [SerializeField] private float burnedTime = 1;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private float homingSpeed = 1;
    [SerializeField] private AudioSource flapSource;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioSource lostSource;
    [SerializeField] private GameObject missile;
    [SerializeField] private ParticleSystem explosion;

    private SpriteRenderer birdRenderer;
    private string spriteName;
    private Sprite[] frames = new Sprite[3];
    private float timer = 0f;
    private int currentFrameIndex = 0;
    private LogicScript logicScript;
    private float lastBurn;
    private bool burning;
    private bool magnetized;
    private bool comingHome;
    private float startPosition;
    private float homeDistance;


    public float Health { get; private set; }
    public float HealthLeft { get; private set; }
    public bool BossReady { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        burning = false;
        magnetized = false;
        comingHome = false;
        BossReady = false;
        homeDistance = 0;


        startPosition = transform.position.x;

        spriteName = Utils.GetPlayerPref(Utils.BIRD_KEY, Utils.DEFAULT_BIRD);

        Health = float.Parse(NewDataBase.GetData()[spriteName].info);
        HealthLeft = Health;

        LoadFrames(spriteName);

        birdRenderer = transform.GetComponent<SpriteRenderer>();
        birdRenderer.sprite = frames[0];
        isAlive = true;
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(JustDied.GetInvocationList().Length);
        if (Input.GetKeyDown(KeyCode.Space) && isAlive)
        {
            body.velocity = Vector2.up * jump;
            flapSource.Play();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Fire();
        }

        Vector3 cornerCoord = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if ((math.abs(transform.position.x) > cornerCoord.x
            || math.abs(transform.position.y) > cornerCoord.y) && isAlive)
        {
            lostSource.Play(); 

            Die();
            return;
        }

        if (comingHome && isAlive) 
        {
            homeDistance = math.abs(transform.position.x - startPosition);
            transform.position += homingSpeed *
                (startPosition - transform.position.x > 0 ? 1 : -1) *
                Time.deltaTime *
                Vector3.right;

            if (homeDistance < math.abs(transform.position.x - startPosition))
            {
                transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
                BossReady = true;
                comingHome = false;
            }
        }

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate && isAlive)
        {
            timer = 0f;
            currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
            birdRenderer.sprite = frames[currentFrameIndex];
        }

        if (burning && lastBurn + burnedTime <= Time.time)
        {
            if (magnetized)
            {
                LoadFrames(Utils.MAGNET_BIRD);
            }
            else
            {
                LoadFrames(spriteName);
            }
            burning = false;
        }

        if (logicScript.UsingMagnet)
        {
            if (!magnetized && !burning)
            {
                LoadFrames(Utils.MAGNET_BIRD);
                magnetized = true;
            }
        }
        else if (magnetized)
        {
            magnetized = false;
            if (!burning) 
            {
                LoadFrames(spriteName);
            }
        }
    }

    private void LoadFrames(string name)
    {
        frames[0] = Resources.Load<Sprite>(Utils.DOWN_PATH + name);
        frames[1] = Resources.Load<Sprite>(Utils.MIDDLE_PATH + name);
        frames[2] = Resources.Load<Sprite>(Utils.UP_PATH + name);
    }

    private void OnParticleCollision(GameObject other)
    {
        LoadFrames(Utils.BURNED_BIRD);
        burning = true;
        lastBurn = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PlayerPrefs.HasKey(Utils.DEATH_KEY) && PlayerPrefs.GetInt(Utils.DEATH_KEY) == 0 &&
            (collision.gameObject.layer == 7 || collision.gameObject.layer == 8))
        {
            return;
        }
        if (isAlive)
        {
            hitSource.Play();
        }
        Die();
    }

    private void Die()
    {
        if (isAlive)
        {
            deathSource.Play();
        }
        isAlive = false;
        OnDeath();
    }

    private void Fire()
    {
        if (isAlive && Time.timeScale == 1 && logicScript.CanFire)
        {
            logicScript.ResetMissile();
            Vector3 missilePosition = new Vector3
                (
                transform.position.x,
                transform.position.y - birdRenderer.size.y,
                transform.position.z
                );
            MissileScript missileScript = 
                Instantiate(missile, missilePosition, new Quaternion()).GetComponent<MissileScript>();
            JustDied += missileScript.OnHostDeath;
            missileScript.GotDestroyed += UnsubscribeMissile;
        }
    }


    public void GoHome() 
    {
        BossReady = false;
        comingHome = true;
    }

    public void TakeDamage(float damage) 
    {
        HealthLeft -= damage;
        logicScript.GameOverlay.SetBirdHeath(math.clamp(HealthLeft / Health, 0, 1));
        if (HealthLeft <= 0) 
        {
            explosion.Play();
            Die();
        }
    }

    private void OnDeath() 
    {
        JustDied?.Invoke(this, EventArgs.Empty);
    }

    public void UnsubscribeMissile(object sender, EventArgs e)
    {
        JustDied -= (sender as MissileScript).OnHostDeath;
    }
}
