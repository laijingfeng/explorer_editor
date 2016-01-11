using UnityEngine;
using System.Collections;

public class Test1 : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnCollisionEnter2D" + ":" + transform.name + " " + coll.transform.name);
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnCollisionExit2D" + ":" + transform.name + " " + coll.transform.name);
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnCollisionStay2D" + ":" + transform.name + " " + coll.transform.name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnTriggerEnter2D" + ":" + transform.name + " " + other.transform.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnTriggerExit2D" + ":" + transform.name + " " + other.transform.name);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Test")
        {
            UnityEngine.Debug.LogError("OnTriggerStay2D" + ":" + transform.name + " " + other.transform.name);
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D bb = transform.GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(new Vector3(transform.position.x + bb.center.x, transform.position.y + bb.center.y, transform.position.z), bb.size);
    }
}
