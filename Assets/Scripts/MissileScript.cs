using UnityEngine;

public class MissileScript : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroyAfter;
    [SerializeField] private GameObject engine;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioSource explosionSource;
    [SerializeField] private AudioSource missileSource;


    private int hitsLeft;

    // Start is called before the first frame update
    void Start()
    {
        string spriteName = Utils.GetSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE);
        transform.GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>(Utils.MISSILE_PATH + spriteName);

        hitsLeft = int.Parse(DataBase.GetData()[spriteName].info);

        missileSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > destroyAfter)
        {
            if (hitsLeft > 0 && Utils.GetBool(Utils.SOUND_KEY))
            {
                hitsLeft = 0;
                missileSource.Play();
                explosionSource.Play();
            }

            Destroy(gameObject, explosionSource.clip.length);
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
            hitsLeft = 0;


            explosionSource.Play();

            explosion.Play();

            Destroy(gameObject, explosion.main.duration);
            PipeSpawnerScript pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();
            speed = -pipeSpawnerScript.CurrentSpeed;
        }
        if (hitsLeft == 0)
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<Collider2D>().enabled = false;
            engine.SetActive(false);
            missileSource.Pause();

        }
    }
}
