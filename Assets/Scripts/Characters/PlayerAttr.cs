using UnityEngine;
using System.Collections;

/// <summary>
/// <para>玩家属性</para>
/// <para>为了方便编辑器调试，变量都设置Public</para>
/// </summary>
public class PlayerAttr : SingletonMono<PlayerAttr>
{
    /// <summary>
    /// 血量
    /// </summary>
    public int m_iBlood = 1;

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
    public int m_iJumpCount = 2;

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
    public float m_fJumpForce = 1000f;

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
    public float m_fMaxSpeed = 5f;

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
    public float m_fMoveForce = 365f;

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

    /// <summary>
    /// 属性变化
    /// </summary>
    public delegate void OnAttrChange();

    /// <summary>
    /// 属性变化
    /// </summary>
    public OnAttrChange onAttrChange;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="iBlood">血量</param>
    /// <param name="iJumpCount">连跳次数</param>
    /// <param name="attrChange">属性变化通知</param>
    public void Init(int iBlood, int iJumpCount, OnAttrChange attrChange)
    {
        m_iBlood = iBlood;
        m_iJumpCount = iJumpCount;

        onAttrChange -= attrChange;
        onAttrChange += attrChange;

        PostChange();
    }

    /// <summary>
    /// 通知变化
    /// </summary>
    private void PostChange()
    {
        if (onAttrChange != null)
        {
            onAttrChange();
        }
    }
}
