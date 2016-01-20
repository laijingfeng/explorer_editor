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
    /// 跳跃音效
    /// </summary>
    public AudioClip[] jumpClips;

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
    /// 检测跳跃
    /// </summary>
    private void CheckJump()
    {
        bool grounded = false;

        if (PlayerAttr.Instance.JumpCount <= 0)
        {
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            bool canJump = false;

            if (nowJumpedCount == 0
                || nowJumpedCount >= PlayerAttr.Instance.JumpCount)//第一次需要在地面//不要把nowJumpedCount置为false，空中吃药可以继续跳
            {
                grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
                if (grounded)
                {
                    canJump = true;

                    StopCoroutine("ListenOnSky");
                    StartCoroutine("ListenOnSky");
                }
            }
            else if (rigidbody2D.velocity.y < maxSpeedStartContinueJump)
            {
                canJump = true;
            }

            if (canJump)
            {
                nowJumpedCount++;
                
                anim.SetTrigger("Jump");

                //播放音效
                int i = Random.Range(0, jumpClips.Length);
                AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

                rigidbody2D.AddForce(new Vector2(0f, PlayerAttr.Instance.JumpForce));
            }
        }
    }

    /// <summary>
    /// 监听落地
    /// </summary>
    /// <returns></returns>
    private IEnumerator ListenOnGround()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")) == true)
            {
                nowJumpedCount = 0;
                break;
            }
        }
    }

    /// <summary>
    /// 监听腾空
    /// </summary>
    /// <returns></returns>
    private IEnumerator ListenOnSky()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")) == false)
            {
                break;
            }
        }

        yield return StartCoroutine("ListenOnGround");
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(h));

        //转向或者未达到最大速度
        if (h * rigidbody2D.velocity.x < PlayerAttr.Instance.MaxSpeed)
        {
            rigidbody2D.AddForce(Vector2.right * h * PlayerAttr.Instance.MoveForce);
        }

        //超过最大速度，设为最大速度
        if (Mathf.Abs(rigidbody2D.velocity.x) > PlayerAttr.Instance.MaxSpeed)
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * PlayerAttr.Instance.MaxSpeed, rigidbody2D.velocity.y);
        }

        if (h > 0 && !facingRight)
        {
            Flip();
        }
        else if (h < 0 && facingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// 翻转朝向
    /// </summary>
    void Flip()
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
        if (tauntChance > tauntProbability)
        {
            yield return new WaitForSeconds(tauntDelay);

            if (!audio.isPlaying)
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
