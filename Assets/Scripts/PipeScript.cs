using Unity.Mathematics;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField][Range(0f, 100f)] private float moveProbability;
    [SerializeField][Range(0f, 100f)] private float indestructProbability;
    [SerializeField] private GameObject topPipe;
    [SerializeField] private GameObject bottomPipe;
    [SerializeField] private GameObject topShield;
    [SerializeField] private GameObject bottomShield;


    private float destroyAfter;
    private PipeSpawnerScript pipeSpawnerScript;
    private string spritePath;
    // Start is called before the first frame update
    void Start()
    {
        if (!Utils.CheckProbability(moveProbability))
        {
            moveSpeed = 0;
        }
        if (Utils.CheckProbability(indestructProbability))
        {
            topShield.SetActive(true);
            bottomShield.SetActive(true);

            topPipe.layer = 8;
            bottomPipe.layer = 8;
        }

        spritePath = Utils.PIPES_PATH + Utils.GetSpriteName(Utils.PIPE_KEY, Utils.DEFAULT_PIPE);

        pipeSpawnerScript = FindObjectOfType<PipeSpawnerScript>();

        destroyAfter = -pipeSpawnerScript.transform.position.x;


        topPipe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        bottomPipe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
    }

    // Update is called once per frame
    void Update()
    {
        if (math.abs(transform.position.y) >= pipeSpawnerScript.transform.position.y + pipeSpawnerScript.GetOffset())
        {
            moveSpeed = transform.position.y > 0 ? -math.abs(moveSpeed) : math.abs(moveSpeed);
        }

        transform.position += Vector3.left * pipeSpawnerScript.CurrentSpeed * Time.deltaTime;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (transform.position.x < destroyAfter)
        {
            Destroy(gameObject);
        }
    }
}
