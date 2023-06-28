using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PipeSpawnerScript : MonoBehaviour
{

    public GameObject pipe;
    public float interval = 1;
    public float pipeOffset = 1;
    public float speed = 1;

    public static float currentSpeed;

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update() {

        if (!BirdScript.isAlive)
        {
            currentSpeed = 0;
            return;
        }

        currentSpeed += Time.deltaTime / 1000;

        if (interval > timer * currentSpeed)
        {
            timer += Time.deltaTime;
        }
        else 
        {
            spawn();
            timer = 0;
        }
    }

    void spawn() 
    {
        float high = transform.position.y + pipeOffset;
        float low = transform.position.y - pipeOffset;

        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(high, low), transform.position.z), transform.rotation);
    }
}