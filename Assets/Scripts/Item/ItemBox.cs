using UnityEngine;
using System.Collections;

/// <summary>
/// 盒子
/// </summary>
public class ItemBox : MonoBehaviour
{
    /// <summary>
    /// 预设
    /// </summary>
    public GameObject m_Prefab;

    /// <summary>
    /// 金币出生点
    /// </summary>
    public Transform m_BornPoint;

    /// <summary>
    /// 死亡换图
    /// </summary>
    public Sprite m_ChangeSprite;

    /// <summary>
    /// 生命
    /// </summary>
    public float m_fLife = 1f;

    /// <summary>
    /// 计数
    /// </summary>
    public int m_iCount = 1;

    /// <summary>
    /// 精灵图渲染器
    /// </summary>
    private SpriteRenderer m_Ren;

    /// <summary>
    /// 工作中
    /// </summary>
    private bool m_bWorking = false;

    /// <summary>
    /// 是否死亡
    /// </summary>
    private bool m_bDead = false;

    /// <summary>
    /// 生命模式
    /// </summary>
    public LifeMode m_LifeMode = LifeMode.Count;

    /// <summary>
    /// 死亡结果
    /// </summary>
    public DeadResult m_DeadResult = DeadResult.Remove;

    /// <summary>
    /// 生产金币
    /// </summary>
    public bool m_bMakeCoin = false;

    /// <summary>
    /// 触发方向
    /// </summary>
    public CollisionDir m_TriggerDir = CollisionDir.Bottom;

    void Start()
    {
        m_Ren = this.transform.GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (m_bDead)
        {
            return;
        }

        if (coll.gameObject.tag == "Player")
        {
            if (m_TriggerDir != CollisionDir.All
                && JudgeCollisionDir(coll.gameObject) != m_TriggerDir)
            {
                return;
            }

            switch (m_LifeMode)
            {
                case LifeMode.Count:
                    {
                        m_iCount--;
                        if (m_iCount <= 0)
                        {
                            SetDead();
                        }
                    }
                    break;
                case LifeMode.Time:
                    {
                        if (m_bWorking == false)
                        {
                            m_bWorking = true;
                            StopCoroutine("Work");
                            StartCoroutine("Work");
                        }
                    }
                    break;
            }

            if (m_bMakeCoin)
            {
                MakeCoin();
            }
        }
    }

    /// <summary>
    /// 设置死亡
    /// </summary>
    private void SetDead()
    {
        m_bDead = true;
        switch (m_DeadResult)
        {
            case DeadResult.ChangeSprite:
                {
                    if (m_ChangeSprite != null)
                    {
                        m_Ren.sprite = m_ChangeSprite;
                    }
                }
                break;
            case DeadResult.Remove:
                {
                    StopCoroutine("DelayDestroy");
                    StartCoroutine("DelayDestroy");
                }
                break;
        }
    }

    /// <summary>
    /// 延时删除
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.Object.Destroy(this.gameObject);

        TriggerBase trigger = this.transform.GetComponentInParent<TriggerBase>();
        if(trigger != null)
        {
            trigger.OnFinish();
        }
    }

    /// <summary>
    /// 工作
    /// </summary>
    /// <returns></returns>
    private IEnumerator Work()
    {
        yield return new WaitForSeconds(m_fLife);
        SetDead();
        yield return null;
    }

    /// <summary>
    /// 产生一个金币
    /// </summary>
    private void MakeCoin()
    {
        GameObject go = GameObject.Instantiate(m_Prefab, m_BornPoint.position, Quaternion.Euler(Vector3.zero)) as GameObject;
        go.rigidbody2D.velocity = new UnityEngine.Vector2(Random.Range(-5, 6), Random.Range(10, 20));
    }

    /// <summary>
    /// 判断碰撞方向
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private CollisionDir JudgeCollisionDir(GameObject go)
    {
        return go.transform.position.y > this.transform.position.y ? CollisionDir.Top : CollisionDir.Bottom;
    }

    /// <summary>
    /// 碰撞方向
    /// </summary>
    public enum CollisionDir
    {
        /// <summary>
        /// 上
        /// </summary>
        Top = 0,

        /// <summary>
        /// 下
        /// </summary>
        Bottom,

        /// <summary>
        /// 所有
        /// </summary>
        All,
    }

    /// <summary>
    /// 生命模式
    /// </summary>
    public enum LifeMode
    {
        /// <summary>
        /// 计时
        /// </summary>
        Time = 0,

        /// <summary>
        /// 计数
        /// </summary>
        Count,
    }

    /// <summary>
    /// 死亡结果
    /// </summary>
    public enum DeadResult
    {
        /// <summary>
        /// 换图
        /// </summary>
        ChangeSprite = 0,

        /// <summary>
        /// 移除
        /// </summary>
        Remove,
    }
}
