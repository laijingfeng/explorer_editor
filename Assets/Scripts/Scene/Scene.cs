using UnityEngine;
using System.Collections;

/// <summary>
/// 场景
/// </summary>
[ExecuteInEditMode]
public class Scene : MonoBehaviour
{
    /// <summary>
    /// 相机中心能到达的最大x和y
    /// </summary>
    public Vector2 maxXAndY;

    /// <summary>
    /// 相机中心能到达的最小x和y
    /// </summary>
    public Vector2 minXAndY;

    /// <summary>
    /// 单例
    /// </summary>
    private static Scene m_instance;

    /// <summary>
    /// 相机跟随
    /// </summary>
    private CameraFollow m_CameraFollow;

    /// <summary>
    /// 单例
    /// </summary>
    public static Scene Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject instGo = GameObject.FindGameObjectWithTag("Scene");
                if (instGo != null)
                {
                    m_instance = instGo.GetComponent<Scene>();
                }
            }
            return m_instance;
        }
    }

    void Start()
    {
        SetCameraRange();
    }

    /// <summary>
    /// 设置像机区域
    /// </summary>
    public void SetCameraRange()
    {
        if (m_CameraFollow == null
                && Camera.main)
        {
            m_CameraFollow = Camera.main.GetComponent<CameraFollow>();
        }

        if (m_CameraFollow != null)
        {
            m_CameraFollow.SetRange(maxXAndY, minXAndY);
        }
    }

    void Update()
    {
        if (Application.isPlaying == false)
        {
            SetCameraRange();
        }
    }
}
