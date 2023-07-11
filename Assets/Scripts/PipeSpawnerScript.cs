using UnityEngine;

public class PipeSpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject pipe;
    [SerializeField] private float interval = 1;
    [SerializeField] private float pipeOffset = 1;
    [SerializeField] private float speed = 1;
    [SerializeField] private int bossInterval = 50;

    public float CurrentSpeed { get; private set; }

    private int pipeCounter;
    private float timer;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        CurrentSpeed = speed;

        pipeCounter = 0;
        timer = 0;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!BirdScript.isAlive)
        {
            CurrentSpeed = 0;
            return;
        }

        if (!active) 
        {
            return;
        }

        if (pipeCounter == bossInterval) 
        {
            active = false;
        }

        CurrentSpeed += Time.deltaTime / 1000;

        if (interval > timer * CurrentSpeed)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Spawn();
            pipeCounter++;
            timer = 0;
        }
    }

    public float GetOffset()
    {
        return pipeOffset;
    }

    public int GetBossInterval() 
    {
        return bossInterval;
    }

    void Spawn()
    {
        float high = transform.position.y + pipeOffset;
        float low = transform.position.y - pipeOffset;

        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(high, low), transform.position.z), transform.rotation);
    }

    public void Activate()
    {
        active = true;
        pipeCounter = 0;
        timer = 0;
    }
}