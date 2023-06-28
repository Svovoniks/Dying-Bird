using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public float destroyAfter = 1;

    private string spritePath;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(Utils.PIPE_KEY))
        {
            spritePath = Utils.PIPES_PATH + PlayerPrefs.GetString(Utils.PIPE_KEY);
        }
        else
        {
            spritePath = Utils.PIPES_PATH + Utils.DEFAULT_PIPE;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += Vector3.left * PipeSpawnerScript.currentSpeed * Time.deltaTime;
        
        if (transform.position.x < destroyAfter) 
        {
            Destroy(gameObject);
        }
    }
}
