using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField][Range(0f, 100f)] private float probability;

    private float destroyAfter;
    private PipeSpawnerScript pipeSpawnerScript;
    private string spritePath;
    // Start is called before the first frame update
    void Start()
    {
        if (UnityEngine.Random.Range(0f, 100f) > probability)
        {
            moveSpeed = 0;
        }

        spritePath = Utils.PIPES_PATH + Utils.getSpriteName(Utils.PIPE_KEY, Utils.DEFAULT_PIPE);

        pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();

        destroyAfter = -pipeSpawnerScript.transform.position.x;

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
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
