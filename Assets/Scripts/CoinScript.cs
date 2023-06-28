using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinScript : MonoBehaviour
{
    public int probability = 20;

    private LogicScript logicScript;
    private AudioSource coinSource;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(1, 100) > probability)
        {
            transform.gameObject.SetActive(false);
        }

        coinSource = GameObject.FindGameObjectWithTag("CoinSource").GetComponent<AudioSource>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logicScript.addMoney(1);
            transform.gameObject.SetActive(false);
            if (Utils.getBool(Utils.SOUND_KEY))
            {
                coinSource.Play();
            }
        }
    }  
}
