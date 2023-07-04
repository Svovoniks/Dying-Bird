using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.Examples;
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
        if (UnityEngine.Random.Range(0f, 100f) > moveProbability)
        {
            moveSpeed = 0;
        }
        if (UnityEngine.Random.Range(0f, 100f) <= indestructProbability) 
        {
            topShield.SetActive(true);
            bottomShield.SetActive(true);

            topPipe.layer = 8;
            bottomPipe.layer = 8;
        }

        spritePath = Utils.PIPES_PATH + Utils.getSpriteName(Utils.PIPE_KEY, Utils.DEFAULT_PIPE);

        pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();

        destroyAfter = -pipeSpawnerScript.transform.position.x;
        

        topPipe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        bottomPipe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
    }

    // Update is called once per frame
    void Update()
    {
        if (math.abs(transform.position.y) >= pipeSpawnerScript.transform.position.y + pipeSpawnerScript.getOffset())
        {
            moveSpeed = transform.position.y > 0 ? -math.abs(moveSpeed) : math.abs(moveSpeed);
        }

        transform.position += Vector3.left * pipeSpawnerScript.currentSpeed * Time.deltaTime;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (transform.position.x < destroyAfter) 
        {
            Destroy(gameObject);
        }
    }
}
