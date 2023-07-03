using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    private float destroyAfter;
    private PipeSpawnerScript pipeSpawnerScript;
    private string spritePath;
    // Start is called before the first frame update
    void Start()
    {
        spritePath = Utils.PIPES_PATH + Utils.getSpriteName(Utils.PIPE_KEY, Utils.DEFAULT_PIPE);

        pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();

        destroyAfter = -pipeSpawnerScript.transform.position.x;

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += Vector3.left * pipeSpawnerScript.currentSpeed * Time.deltaTime;
        
        if (transform.position.x < destroyAfter) 
        {
            Destroy(gameObject);
        }
    }
}
