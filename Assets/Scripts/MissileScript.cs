using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MissileScript : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroyAfter;
    [SerializeField] private GameObject engine;
    [SerializeField] private ParticleSystem explosion;

    
    private int hitsLeft;
    private BirdScript bird;

    // Start is called before the first frame update
    void Start()
    {
        string spriteName = Utils.getSpriteName(Utils.MISSILE_KEY, Utils.DEFAULT_MISSILE);
        transform.GetComponent<SpriteRenderer>().sprite = 
            Resources.Load<Sprite>(Utils.MISSILE_PATH + spriteName);

        bird = FindObjectOfType<BirdScript>();
       // transform.position = bird.transform.position;

        hitsLeft = int.Parse(DataBase.getData()[spriteName].info);

        if (!Utils.getBool(Utils.SOUND_KEY)) 
        {
            transform.GetComponentsInChildren<AudioSource>()[1].gameObject.SetActive(false);
            transform.GetComponentsInChildren<AudioSource>()[0].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > destroyAfter)
        {
            if (hitsLeft > 0 && Utils.getBool(Utils.SOUND_KEY))
            {
                hitsLeft = 0;
                transform.GetComponentsInChildren<AudioSource>()[1].Pause();
                transform.GetComponentsInChildren<AudioSource>()[0].Play();
            }
            if (Utils.getBool(Utils.SOUND_KEY))
            {
                Destroy(gameObject, transform.GetComponentsInChildren<AudioSource>()[0].clip.length);
            }
            else 
            {
                Destroy(gameObject);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && hitsLeft > 0)
        {
            hitsLeft--;

            collision.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            if (Utils.getBool(Utils.SOUND_KEY))
            {

                transform.GetComponentsInChildren<AudioSource>()[0].Play();
            }

            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

        }
        else if (collision.gameObject.layer == 8 && hitsLeft > 0) 
        {
            hitsLeft = 0;
            transform.GetComponentsInChildren<AudioSource>()[0].Play();
            explosion.Play();

            Destroy(gameObject, explosion.main.duration);
            PipeSpawnerScript pipeSpawnerScript = FindFirstObjectByType<PipeSpawnerScript>();
            speed = -pipeSpawnerScript.currentSpeed;
        }
        if (hitsLeft == 0) 
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<Collider2D>().enabled = false;
            engine.SetActive(false);
            if (Utils.getBool(Utils.SOUND_KEY)) 
            {
                transform.GetComponentsInChildren<AudioSource>()[1].Pause();
            }
            
        }
}
}
