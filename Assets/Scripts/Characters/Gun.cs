using UnityEngine;
using System.Collections;

/// <summary>
/// 炮
/// </summary>
public class Gun : MonoBehaviour
{
    /// <summary>
    /// 火箭
    /// </summary>
	public Rigidbody2D rocket;
	
    /// <summary>
    /// 火箭速度
    /// </summary>
    public float speed = 20f;

    /// <summary>
    /// 角色控制器
    /// </summary>
    private PlayerControl playerCtrl;
	
    /// <summary>
    /// 动画控制器
    /// </summary>
    private Animator anim;
    
    void Awake()
	{
		anim = transform.parent.gameObject.GetComponent<Animator>();
		playerCtrl = transform.parent.GetComponent<PlayerControl>();
	}

	void Update ()
	{
		if(Input.GetButtonDown("Shoot"))
		{
			anim.SetTrigger("Shoot");
			audio.Play();

			if(playerCtrl.facingRight)
			{
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
		}
	}
}
