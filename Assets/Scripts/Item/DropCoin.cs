using UnityEngine;
using System.Collections;

public class DropCoin : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
        else if (coll.gameObject.tag == "Bullet")//no use
        {
            coll.gameObject.GetComponent<Rocket>().OnExplode();

            UnityEngine.Object.Destroy(coll.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);       
        }
    }
}
