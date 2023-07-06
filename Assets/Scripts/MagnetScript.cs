using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    [SerializeField] private AudioSource magnetSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>().Magnets += 1;
            Utils.PlayAudio(magnetSource);
            transform.gameObject.SetActive(false);
        }
    }
}
