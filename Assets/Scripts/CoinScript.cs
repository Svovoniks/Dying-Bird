using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private AudioSource coinSource;
    [SerializeField] private float coinSpeed;

    
    private LogicScript logicScript;
    private BirdScript bird;
    private Vector3 cornerCoord;
    private Vector3 initialLocalPosition;
    private float homeDistance;
    private void Start()
    {

        initialLocalPosition = transform.localPosition;
        cornerCoord = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        bird = FindObjectOfType<BirdScript>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void Update()
    {
        if (logicScript.UsingMagnet)
        {
            ComeALittleBitCloser();
            
        }
        else if (transform.localPosition != initialLocalPosition) 
        {
            GoHome();
        }
    }

    private void ComeALittleBitCloser()
    {  
        if (math.abs(transform.position.x) > cornerCoord.x)
        {
            return;
        }
        Vector3 moveVector = bird.transform.position - transform.position;
        transform.position += coinSpeed * Time.deltaTime * moveVector.normalized;
    }

    private void GoHome() 
    {
        homeDistance = Vector3.Distance(initialLocalPosition, transform.localPosition);
        
        Vector3 moveVector = initialLocalPosition - transform.localPosition;
        transform.position += moveVector.normalized * coinSpeed * Time.deltaTime;

        if (homeDistance <= Vector3.Distance(initialLocalPosition, transform.localPosition)) 
        {
            transform.localPosition = initialLocalPosition;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logicScript.Money += 1;
            coinSource.Play();
            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<Collider2D>().enabled = false;
        }
    }
}
