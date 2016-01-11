using UnityEngine;
using System.Collections;

/// <summary>
/// 相机跟随
/// </summary>
public class CameraFollow : SingletonMono<CameraFollow>
{
    /// <summary>
    /// x方向自由范围
    /// </summary>
    public float xMargin = 1f;

    /// <summary>
    /// y方向自由范围
    /// </summary>
    public float yMargin = 1f;

    /// <summary>
    /// 平滑时间
    /// </summary>
    public float smoothTime = 0.07f;

    /// <summary>
    /// 相机中心能到达的最大x和y
    /// </summary>
    private Vector2 maxXAndY;

    /// <summary>
    /// 相机中心能到达的最小x和y
    /// </summary>
    private Vector2 minXAndY;

    /// <summary>
    /// 目标
    /// </summary>
    private Transform target;

    void LateUpdate()
    {
        TrackPlayer();
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    public void SetTarget(Transform tar)
    {
        target = tar;
    }

    /// <summary>
    /// 设置范围
    /// </summary>
    /// <param name="tmaxXAndY">相机中心能到达的最大x和y</param>
    /// <param name="tminXAndY">相机中心能到达的最小x和y</param>
    public void SetRange(Vector2 tmaxXAndY, Vector2 tminXAndY)
    {
        maxXAndY = tmaxXAndY;
        minXAndY = tminXAndY;
    }

    /// <summary>
    /// 检查X轴
    /// </summary>
    /// <returns></returns>
    private bool CheckXMargin()
    {
        return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
    }

    /// <summary>
    /// 检查Y轴
    /// </summary>
    /// <returns></returns>
    private bool CheckYMargin()
    {
        return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
    }

    /// <summary>
    /// 跟随玩家
    /// </summary>
    private void TrackPlayer()
    {
        if (target == null)
        {
            return;
        }

        Vector3 pos = transform.position;
        float velo = 0.0f;

        if (CheckXMargin())
        {
            velo = 0.0f;
            pos.x = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velo, smoothTime);
        }

        if (CheckYMargin())
        {
            velo = 0.0f;
            pos.y = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velo, smoothTime);
        }

        pos.x = Mathf.Clamp(pos.x, minXAndY.x, maxXAndY.x);
        pos.y = Mathf.Clamp(pos.y, minXAndY.y, maxXAndY.y);

        transform.position = pos;
    }

    /// <summary>
    /// <para>相机的半宽</para>
    /// <para>orthographicSize/640=halfWidth/960</para>
    /// </summary>
    private float HalfCameraWidth
    {
        get
        {
            float size = 3f;
            if (Camera.main != null)
            {
                size = Camera.main.orthographicSize;
            }
            return size * 960f / 640f;
        }
    }

    /// <summary>
    /// <para>相机的半高</para>
    /// </summary>
    private float HalfCameraHeight
    {
        get
        {
            float size = 3f;
            if (Camera.main != null)
            {
                size = Camera.main.orthographicSize;
            }
            return size;
        }
    }

    void OnDrawGizmos()
    {
        //绘制相机边框区域
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minXAndY.x - HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0), new Vector3(maxXAndY.x + HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(minXAndY.x - HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0), new Vector3(minXAndY.x - HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(maxXAndY.x + HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0), new Vector3(minXAndY.x - HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(maxXAndY.x + HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0), new Vector3(maxXAndY.x + HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0));
    }
}
