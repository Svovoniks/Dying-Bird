using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class CoinCloudScript : MonoBehaviour
{
    public event EventHandler InTheMiddle;

    private PipeSpawnerScript pipeSpawnerScript;
    private float startPosition;
    private bool notified;
    // Start is called before the first frame update
    void Start()
    {
        notified = false;
        pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();
        startPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -startPosition) 
        {
            Destroy(gameObject);
            return;
        }
        else if (transform.position.x <= 0 && !notified)
        {
            OnMiddle();
            notified = true;
            Debug.Log("ttt");
        }
        transform.position += pipeSpawnerScript.CurrentSpeed * Time.deltaTime * Vector3.left;
    }

    private void OnMiddle()
    {
        InTheMiddle?.Invoke(this, EventArgs.Empty);
    }
}
