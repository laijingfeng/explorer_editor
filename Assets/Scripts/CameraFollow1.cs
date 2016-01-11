using UnityEngine;
using System.Collections;

/// <summary>
/// 相机跟随
/// </summary>
public class CameraFollow1 : MonoBehaviour 
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
    /// x方向平滑参数
    /// </summary>
    public float xSmooth = 8f;
	
    /// <summary>
    /// y方向平滑参数
    /// </summary>
    public float ySmooth = 8f;
	
    /// <summary>
    /// 相机中心能到达的最大x和y
    /// </summary>
    public Vector2 maxXAndY;
	
    /// <summary>
    /// 相机中心能到达的最小x和y
    /// </summary>
    public Vector2 minXAndY;

    /// <summary>
    /// 角色
    /// </summary>
	private Transform player;

	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}

	bool CheckYMargin()
	{
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}

	void FixedUpdate ()
	{
		TrackPlayer();
	}
	
	void TrackPlayer ()
	{
		float targetX = transform.position.x;
		float targetY = transform.position.y;

        if (CheckXMargin())
        {
            targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);
        }

        if (CheckYMargin())
        {
            targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);
        }

		targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		transform.position = new Vector3(targetX, targetY, transform.position.z);
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
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minXAndY.x - HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0), new Vector3(maxXAndY.x + HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(minXAndY.x - HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0), new Vector3(minXAndY.x - HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(maxXAndY.x + HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0), new Vector3(minXAndY.x - HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0));
        Gizmos.DrawLine(new Vector3(maxXAndY.x + HalfCameraWidth, maxXAndY.y + HalfCameraHeight, 0), new Vector3(maxXAndY.x + HalfCameraWidth, minXAndY.y - HalfCameraHeight, 0));
    }
}
