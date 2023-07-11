using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour, IDamagable
{
    [SerializeField] private float missileTimeout;
    [SerializeField] private int HP;
    [SerializeField] private float screenOffset;
    [SerializeField] private float arrivalSpeed;
    [SerializeField] private GameObject missile;
    [SerializeField] private Transform missileSpawnPosition;
    [SerializeField] private ParticleSystem explosion;

    private bool arrived;
    private bool isAlive;
    private float healthLeft;
    private float startingPosition;
    private LogicScript logicScript;
    private float lastMissile;

    
    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        arrived = false;
        startingPosition = transform.position.x;
        logicScript = GameObject.FindGameObjectWithTag("Logic").
            GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!logicScript.Playing)
        {
            return;
        }
        if (!arrived) 
        {
            transform.position += arrivalSpeed * Time.deltaTime * Vector3.left;
            if (math.abs(transform.position.x - startingPosition) >= screenOffset) 
            {
                arrived = true;
                lastMissile = Time.time;
            }
            Debug.Log(transform.position);
            return;
        }

        if (lastMissile + missileTimeout < Time.time) 
        {
            lastMissile = Time.time;
            Fire();
        }

    }

    private void Fire()
    {
        if (isAlive) 
        {
            Instantiate(missile, missileSpawnPosition.position, new Quaternion());
        }
    }

    public void TakeDamage(float damage)
    {
        healthLeft -= damage;
        logicScript.GameOverlay.SetBossHealth(healthLeft);
        if (healthLeft <= 0) 
        {
            explosion.Play();
            Destroy(gameObject, explosion.main.duration);
            isAlive = false;
        }

    }

}
