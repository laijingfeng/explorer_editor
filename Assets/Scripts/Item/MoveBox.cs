using UnityEngine;
using System.Collections;

public class MoveBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            other.gameObject.GetComponent<Rocket>().OnExplode();

            UnityEngine.Object.Destroy(other.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);       
        }
    }
}
