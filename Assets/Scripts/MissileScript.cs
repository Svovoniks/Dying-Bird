using System;
using Unity.Mathematics;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    public event EventHandler GotDestroyed;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroyAfter;
    [SerializeField] private bool evil = false;
    [SerializeField] private float damagePerHit = 1;
    [SerializeField] private GameObject engine;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioSource explosionSource;
    [SerializeField] private AudioSource missileSource;


    private int hitsLeft;
    private bool blownUp;

    // Start is called before the first frame update
    void Start()
    {
        blownUp = false;
        if (!evil)
        {
            string spriteName = Utils.GetSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE);
            transform.GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>(Utils.MISSILE_PATH + spriteName);

            hitsLeft = int.Parse(DataBase.GetData()[spriteName].info);
        }
        else 
        {
            hitsLeft = 1;
        }
        

        missileSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.right;

        if (math.abs(transform.position.x) > destroyAfter)
        {
            BlowUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer == 7 && hitsLeft > 0)
        {
            hitsLeft--;

            collision.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            explosionSource.Play();

            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

        }
        else if (collision.gameObject.layer == 8 && hitsLeft > 0)
        {
            BlowUp();

            PipeSpawnerScript pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();
            speed = -pipeSpawnerScript.CurrentSpeed;
        }
        else if (collision.gameObject.layer == 3 && evil || collision.gameObject.layer == 10 && !evil) 
        {
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(hitsLeft * damagePerHit);

            BlowUp();
            speed = 0;
        }
        else if(collision.gameObject.layer == 6 && hitsLeft > 0) 
        {
            BlowUp();
            speed = 0;
        }
        if (hitsLeft == 0)
        {
            BlowUp(false);
        }
    }

    private void BlowUp(bool fire = true) 
    {
        if (blownUp) 
        {
            return;
        }
        

        hitsLeft = 0;
        blownUp = true;

        if (fire)
        {
            explosionSource.Play();
            explosion.Play();
        }
        
        missileSource.Pause();

        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = false;
        engine.SetActive(false);

        OnDestruction();
        
        Destroy(gameObject, explosionSource.clip.length);
    }

    public void OnHostDeath(object sender, EventArgs e) 
    {
        BlowUp();
    }

    private void OnDestruction()
    {
        GotDestroyed?.Invoke(this, EventArgs.Empty);
    }
}
