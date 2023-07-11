using JetBrains.Annotations;
using UnityEngine;

public class MiddlePipeScript : MonoBehaviour
{
    private bool used = false;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && !used)
        {
            GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>().Score += 1;
            used = true;
        }
    }
}
