using UnityEngine;
using System.Collections;

/// <summary>
/// 敌人
/// </summary>
public class Boss_001 : MonoBehaviour
{
    /// <summary>
    /// 移动速度
    /// </summary>
	public float moveSpeed = 2f;

    /// <summary>
    /// 血量，可受击次数
    /// </summary>
	public int HP = 2;

    /// <summary>
    /// 死亡精灵图
    /// </summary>
	public Sprite deadEnemy;
	
    /// <summary>
    /// 受伤精灵图，可选
    /// </summary>
    public Sprite damagedEnemy;

	public AudioClip[] deathClips;		// An array of audioclips that can play when the enemy dies.

    /// <summary>
    /// 死亡加分UI
    /// </summary>
	public GameObject hundredPointsUI;
	
    public float deathSpinMin = -100f;			// A value to give the minimum amount of Torque when dying
	
    public float deathSpinMax = 100f;			// A value to give the maximum amount of Torque when dying

    /// <summary>
    /// Trigger
    /// </summary>
    private TriggerBoss m_TriggerBoss;

    /// <summary>
    /// body图
    /// </summary>
	private SpriteRenderer ren;

    /// <summary>
    /// 前方探路点
    /// </summary>
	private Transform frontCheck;
	
    /// <summary>
    /// 是否死亡
    /// </summary>
    private bool dead = false;
	
	void Awake()
	{
		ren = transform.Find("body").GetComponent<SpriteRenderer>();
		frontCheck = transform.Find("frontCheck").transform;
	}

    void Start()
    {
        m_TriggerBoss = this.GetComponentInParent<TriggerBoss>();
    }

	void FixedUpdate ()
	{
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

		// Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
			if(c.tag == "Obstacle")
			{
				Flip ();
				break;
			}
		}

		// Set the enemy's velocity to moveSpeed in the x direction.
		rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);	

		// If the enemy has one hit point left and has a damagedEnemy sprite...
        if (HP == 1 && damagedEnemy != null)
        {
            ren.sprite = damagedEnemy;
        }

        if (HP <= 0 && !dead)
        {
            Death();
        }
	}
	
    /// <summary>
    /// 受伤
    /// </summary>
	public void Hurt()
	{
		HP--;
	}
	
    /// <summary>
    /// 死亡
    /// </summary>
	void Death()
	{
		SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

		foreach(SpriteRenderer s in otherRenderers)
		{
			s.enabled = false;
		}

		ren.enabled = true;
		ren.sprite = deadEnemy;
        
        dead = true;

		// Allow the enemy to rotate and spin it by adding a torque.
		rigidbody2D.fixedAngle = false;
        rigidbody2D.AddTorque(Random.Range(deathSpinMin, deathSpinMax));

		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider2D[] cols = GetComponents<Collider2D>();
		foreach(Collider2D c in cols)
		{
			c.isTrigger = true;
		}

		// Play a random audioclip from the deathClips array.
		int i = Random.Range(0, deathClips.Length);
		AudioSource.PlayClipAtPoint(deathClips[i], transform.position);

		// Create a vector that is just above the enemy.
		Vector3 scorePos;
		scorePos = transform.position;
		scorePos.y += 1.5f;

		// Instantiate the 100 points prefab at this point.
		Instantiate(hundredPointsUI, scorePos, Quaternion.identity);

        StartCoroutine("DelayNoticeTrigger");
	}

    void OnDestroy()
    {
        if (m_TriggerBoss != null)
        {
            m_TriggerBoss.OnFinish();
        }
    }

    /// <summary>
    /// 延时通知Trigger
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayNoticeTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        if (m_TriggerBoss != null)
        {
            m_TriggerBoss.OnFinish();
        }

        yield return null;
    }

    /// <summary>
    /// 翻转
    /// </summary>
	public void Flip()
	{
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}
}
