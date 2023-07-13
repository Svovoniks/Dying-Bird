using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour, IDamagable
{

    public event EventHandler JustDied;

    [SerializeField] private float missileTimeout;
    [SerializeField] private float missileTimeoutMultiplier;
    [SerializeField] private float HP;
    [SerializeField] private float HPIncrement;
    [SerializeField] private float screenOffset;
    [SerializeField] private float arrivalSpeed;
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject mainBody;
    [SerializeField] private Transform missileSpawnPosition;
    [SerializeField] private ParticleSystem explosion;

    private bool arrived;
    private bool isAlive;
    private float healthLeft;
    private float startingPosition;
    private LogicScript logicScript;
    private BirdScript bird;
    private float lastMissile;

    
    // Start is called before the first frame update
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("Logic").
            GetComponent<LogicScript>();

        HP += HPIncrement * logicScript.BossesKilled;
        missileTimeout *= math.pow(missileTimeoutMultiplier, logicScript.BossesKilled);

        Debug.Log(HP);

        isAlive = true;
        arrived = false;
        healthLeft = HP;
        startingPosition = mainBody.transform.position.x;

        logicScript.GameOverlay.SetBossHealth(1);
        bird = FindObjectOfType<BirdScript>();
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
            mainBody.transform.position += arrivalSpeed * Time.deltaTime * Vector3.left;
            if (math.abs(mainBody.transform.position.x - startingPosition) >= screenOffset) 
            {
                arrived = true;
                lastMissile = Time.time;
            }
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
            MissileScript missileScript = 
                Instantiate(missile, missileSpawnPosition.position, new Quaternion()).GetComponent<MissileScript>();

            JustDied += missileScript.OnHostDeath;
            missileScript.GotDestroyed += UnsubscribeMissile;

            bird.JustDied += missileScript.OnHostDeath;
            missileScript.GotDestroyed += bird.UnsubscribeMissile;
        }
    }

    public void TakeDamage(float damage)
    {
        healthLeft -= damage;
        logicScript.GameOverlay.SetBossHealth(math.clamp(healthLeft / HP, 0, 1));
        if (healthLeft <= 0) 
        {
            explosion.Play();
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(mainBody, explosion.main.duration);
            isAlive = false;
            OnDeath();
        }
    }

    private void OnDeath()
    {
        JustDied?.Invoke(this, EventArgs.Empty);
    }

    public void UnsubscribeMissile(object sender, EventArgs e)
    {
        JustDied -= (sender as MissileScript).OnHostDeath;
    }

}
