using UnityEngine;
using System.Collections;

/// <summary>
/// 移除器
/// </summary>
public class Remover : MonoBehaviour
{
	public GameObject splash;

	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
			// .. stop the camera tracking the player
			//GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().enabled = false;

			// .. stop the Health Bar following the player
			//if(GameObject.FindGameObjectWithTag("HealthBar").activeSelf)
			//{
			//	GameObject.FindGameObjectWithTag("HealthBar").SetActive(false);
			//}

			// ... instantiate the splash where the player falls in.
			Instantiate(splash, col.transform.position, transform.rotation);
			// ... destroy the player.
			Destroy (col.gameObject);
			// ... reload the level.
			StartCoroutine("ReloadGame");

            //if (GameStateManager.Instance.CurState == CopyState.Instance)
            //{
            //    CopyState.Instance.Finish(false);
            //}
		}
		else
		{
			// ... instantiate the splash where the enemy falls in.
			Instantiate(splash, col.transform.position, transform.rotation);

			// Destroy the enemy.
			Destroy (col.gameObject);	
		}
	}

	IEnumerator ReloadGame()
	{			
		yield return new WaitForSeconds(2);
		//Application.LoadLevel(Application.loadedLevel);
	}
}
