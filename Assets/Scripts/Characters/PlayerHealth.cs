using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家健康
/// </summary>
public class PlayerHealth : MonoBehaviour
{	
	public float health = 100f;					// The player's health.
	
    /// <summary>
    /// 玩家的受击频率
    /// </summary>
    public float repeatDamagePeriod = 2f;		// How frequently the player can be damaged.
	
    public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	public float hurtForce = 10f;				// The force with which the player is pushed when hurt.
	public float damageAmount = 10f;			// The amount of damage to take when enemies touch the player

	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	private float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).
	private PlayerControl playerControl;		// Reference to the PlayerControl script.
	
    /// <summary>
    /// 动画控制器
    /// </summary>
    private Animator anim;

	void Awake ()
	{
		playerControl = GetComponent<PlayerControl>();
		anim = GetComponent<Animator>();

		// Getting the intial scale of the healthbar (whilst the player has full health).
		//healthScale = healthBar.transform.localScale;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if(col.gameObject.tag == "Enemy"
            || col.gameObject.tag.StartsWith("Boss"))
		{
			if (Time.time > lastHitTime + repeatDamagePeriod) 
			{
				if(PlayerAttr.Instance.Blood > 1)
				{
					TakeDamage(col.transform); 
					lastHitTime = Time.time; 
				}
				else
				{
					Collider2D[] cols = GetComponents<Collider2D>();
					foreach(Collider2D c in cols)
					{
						c.isTrigger = true;
					}

					// Move all sprite parts of the player to the front
					SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
					foreach(SpriteRenderer s in spr)
					{
						s.sortingLayerName = "UI";
					}

					GetComponent<PlayerControl>().enabled = false;

                    if (GetComponentInChildren<Gun>())
                    {
                        GetComponentInChildren<Gun>().enabled = false;
                    }
					
					anim.SetTrigger("Die");
				}
			}
		}
	}

	void TakeDamage (Transform enemy)
	{
        PlayerAttr.Instance.Blood--;

		// Make sure the player can't jump.
		//playerControl.jump = false;

		// Create a vector that's from the enemy to the player with an upwards boost.
		Vector3 hurtVector = transform.position - enemy.position + Vector3.up * 5f;

		// Add a force to the player in the direction of the vector and multiply by the hurtForce.
		rigidbody2D.AddForce(hurtVector * hurtForce);

		// Update what the health bar looks like.
		UpdateHealthBar();

		// Play a random clip of the player getting hurt.
		int i = Random.Range (0, ouchClips.Length);
		AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);
	}

	public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		//healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);

		// Set the scale of the health bar to be proportional to the player's health.
		//healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
	}
}
