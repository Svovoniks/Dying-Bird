using Unity.Mathematics;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public static bool isAlive;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float jump = 1;
    [SerializeField] private float burnedTime = 1;
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
    private float lastBurn;
    private bool burning;
    private bool magnetized;


    // Start is called before the first frame update
    void Start()
    {
        spriteName = Utils.GetSpriteName(Utils.BIRD_KEY, Utils.DEFAULT_BIRD);

        LoadFrames(spriteName);

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
            Utils.PlayAudio(flapSource);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Fire();
        }
        Vector3 cornerCoord = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        if ((math.abs(transform.position.x) > cornerCoord.x
            || math.abs(transform.position.y) > cornerCoord.y) && isAlive)
        {
            Utils.PlayAudio(lostSource);

            Die();
            return;
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
            Utils.PlayAudio(hitSource);
        }
        Die();
    }

    private void Die()
    {
        if (isAlive)
        {
            Utils.PlayAudio(deathSource);
        }
        isAlive = false;
        logicScript.GameOver();
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
            Instantiate(missile, missilePosition, new Quaternion());
        }
    }
}
