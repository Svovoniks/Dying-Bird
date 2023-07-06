using UnityEngine;

public class MiddlePipeScript : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>().Score += 1;
        }
    }
}
