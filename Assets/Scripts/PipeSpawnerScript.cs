using UnityEngine;

public class PipeSpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject pipe;
    [SerializeField] private float interval = 1;
    [SerializeField] private float pipeOffset = 1;
    [SerializeField] private float speed = 1;

    public float CurrentSpeed { get; private set; }

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        CurrentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {

        if (!BirdScript.isAlive)
        {
            CurrentSpeed = 0;
            return;
        }

        CurrentSpeed += Time.deltaTime / 1000;

        if (interval > timer * CurrentSpeed)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Spawn();
            timer = 0;
        }
    }

    public float GetOffset()
    {
        return pipeOffset;
    }

    void Spawn()
    {
        float high = transform.position.y + pipeOffset;
        float low = transform.position.y - pipeOffset;

        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(high, low), transform.position.z), transform.rotation);
    }
}