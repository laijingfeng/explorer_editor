using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家属性
/// </summary>
public class PlayerAttr : SingletonMono<PlayerAttr>
{
    /// <summary>
    /// 血量
    /// </summary>
    private int m_iBlood = 1;

    /// <summary>
    /// 血量
    /// </summary>
    public int Blood
    {
        get
        {
            return m_iBlood;
        }

        set
        {
            m_iBlood = value;
        }
    }

    /// <summary>
    /// 连跳次数
    /// </summary>
    private int m_iJumpCount = 2;

    /// <summary>
    /// 连跳次数
    /// </summary>
    public int JumpCount
    {
        get
        {
            return m_iJumpCount;
        }
    }

    /// <summary>
    /// 起跳力
    /// </summary>
    private float m_fJumpForce = 1000f;

    /// <summary>
    /// 跳跃力
    /// </summary>
    public float JumpForce
    {
        get
        {
            return m_fJumpForce;
        }
    }

    /// <summary>
    /// 移动最大速度
    /// </summary>
    private float m_fMaxSpeed = 5f;

    /// <summary>
    /// 移动最大速度
    /// </summary>
    public float MaxSpeed
    {
        get
        {
            return m_fMaxSpeed;
        }
    }

    /// <summary>
    /// 移动力
    /// </summary>
    private float m_fMoveForce = 365f;

    /// <summary>
    /// 移动力
    /// </summary>
    public float MoveForce
    {
        get
        {
            return m_fMoveForce;
        }
    }

    public void Init()
    {

    }
}
