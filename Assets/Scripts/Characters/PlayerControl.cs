using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// 是否面朝右
    /// </summary>
	[HideInInspector]
	public bool facingRight = true;
	
    /// <summary>
    /// 是否需要跳
    /// </summary>
    [HideInInspector]
	public bool jump = false;

    /// <summary>
    /// 移动力
    /// </summary>
	public float moveForce = 365f;
	
    /// <summary>
    /// 移动最大速度
    /// </summary>
    public float maxSpeed = 5f;
	
    /// <summary>
    /// 跳跃音效
    /// </summary>
    public AudioClip[] jumpClips;
	
    /// <summary>
    /// 跳跃力
    /// </summary>
    public float jumpForce = 1000f;

    /// <summary>
    /// 嘲讽音效
    /// </summary>
	public AudioClip[] taunts;

    /// <summary>
    /// 嘲讽概率
    /// </summary>
	public float tauntProbability = 50f;
	
    /// <summary>
    /// 嘲讽延时
    /// </summary>
    public float tauntDelay = 1f;

    /// <summary>
    /// 当前嘲讽
    /// </summary>
	private int tauntIndex;
	
    /// <summary>
    /// 地面检测
    /// </summary>
    private Transform groundCheck;
	
    /// <summary>
    /// 角色的状态机
    /// </summary>
	private Animator anim;

    /// <summary>
    /// 可连跳次数
    /// </summary>
    private int continueJumpCount = 1;

    /// <summary>
    /// 当前已连跳的次数
    /// </summary>
    private int nowJumpedCount = 0;

    /// <summary>
    /// <para>开始连跳的最大速度</para>
    /// <para>防止快速连跳附加的启动速度过大</para>
    /// </summary>
    private float maxSpeedStartContinueJump = 6f;

	void Awake()
	{
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
        nowJumpedCount = 0;
        continueJumpCount = 1;
	}

    void Start()
    {
        CameraFollow.Instance.SetTarget(this.transform);
    }

	void Update()
	{
        CheckJump();
	}

    /// <summary>
    /// 连跳次数
    /// </summary>
    public int ContinueJumpCount
    {
        set
        {
            continueJumpCount = value;
        }
    }

    /// <summary>
    /// 检测跳跃
    /// </summary>
    private void CheckJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (nowJumpedCount == 0)
            {
                bool grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
                if (grounded)
                {
                    jump = true;
                }
            }
            else if(rigidbody2D.velocity.y < maxSpeedStartContinueJump)
            {
                jump = true;
            }
        }
    }

	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");

		anim.SetFloat("Speed", Mathf.Abs(h));

        //转向或者未达到最大速度
        if (h * rigidbody2D.velocity.x < maxSpeed)
        {
            rigidbody2D.AddForce(Vector2.right * h * moveForce);
        }

        //超过最大速度，设为最大速度
        if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
        }

		if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {   
            Flip();
        }

		if(jump)
		{
            nowJumpedCount = (nowJumpedCount + 1) % continueJumpCount;

			anim.SetTrigger("Jump");

            //播放音效
			int i = Random.Range(0, jumpClips.Length);
			AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            
            jump = false;
		}
	}
	
	/// <summary>
	/// 翻转朝向
	/// </summary>
	void Flip ()
	{
		facingRight = !facingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    /// <summary>
    /// <para>嘲讽</para>
    /// <para>加分的时候</para>
    /// </summary>
    /// <returns></returns>
	public IEnumerator Taunt()
	{
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			yield return new WaitForSeconds(tauntDelay);

			if(!audio.isPlaying)
			{
				tauntIndex = TauntRandom();

				audio.clip = taunts[tauntIndex];
				audio.Play();
			}
		}
	}

    /// <summary>
    /// 随机嘲讽
    /// </summary>
    /// <returns></returns>
	private int TauntRandom()
	{
		int i = Random.Range(0, taunts.Length);

        if (i == tauntIndex)
        {
            return TauntRandom();
        }
        else
        {
            return i;
        }
	}
}
