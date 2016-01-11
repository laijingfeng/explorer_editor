using UnityEngine;
using System.Collections;

/// <summary>
/// 窗体接口
/// </summary>
public interface LikeWindow
{
    /// <summary>
    /// 显示
    /// </summary>
    void Show();

    /// <summary>
    /// 隐藏
    /// </summary>
    void Hide();

    /// <summary>
    /// 反转
    /// </summary>
    void Toggle();

    /// <summary>
    /// 是否可见
    /// </summary>
    bool IsVisible { get; }
}

/// <summary>
/// <para>窗体单例</para>
/// <para>带有UI</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonWindow<T> : SingletonMono<T>, LikeWindow where T : UnityEngine.Component
{
    /// <summary>
    /// 窗体
    /// </summary>
    private GameObject m_goWindow;

    public override void Awake()
    {
        base.Awake();

        Transform t = gameObject.transform.FindChild("Window");
        if (t != null)
        {
            m_goWindow = t.gameObject;
        }
        else
        {
            Debug.LogError(string.Format("{0} Can not find Window in Children!", gameObject.name));
        }

        if (m_goWindow != null)
        {
            m_goWindow.SetActive(false);
        }
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool IsVisible
    {
        get
        {
            return m_goWindow ? m_goWindow.activeSelf : false;
        }
    }

    /// <summary>
    /// 反转
    /// </summary>
    public virtual void Toggle()
    {
        if (m_goWindow == null)
        {
            return;
        }

        if (IsVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    /// <summary>
    /// <para>显示</para>
    /// <para>这个不判状态，每次都通过，可通过ReShow刷新所有数据</para>
    /// </summary>
    public virtual void Show()
    {
        if (m_goWindow == null)
        {
            return;
        }

        m_goWindow.SetActive(true);

        OnShow();
    }

    /// <summary>
    /// 显示完成
    /// </summary>
    public virtual void OnShow()
    {

    }

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Hide()
    {
        if (IsVisible == false)
        {
            return;
        }

        m_goWindow.SetActive(false);

        OnHide();
    }

    /// <summary>
    /// 关闭完成
    /// </summary>
    public virtual void OnHide()
    {

    }
}
