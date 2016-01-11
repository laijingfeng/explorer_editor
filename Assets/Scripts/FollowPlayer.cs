using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FollowPlayer : MonoBehaviour
{
	public Vector3 offset;
	
	private Transform player;

	void Awake ()
	{
		//player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update ()
	{
        if (player != null)
        {
            transform.position = player.position + offset;
        }
	}
}
