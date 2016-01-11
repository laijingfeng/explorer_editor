using UnityEngine;
using System.Collections;

/// <summary>
/// 场景速度
/// </summary>
public class SceneSpeed : MonoBehaviour
{
    /// <summary>
    /// 速度
    /// </summary>
    public float speed = 0.1f;

    void LateUpdate()
    {
        if (Camera.main == null)
        {
            return ;
        }

        Vector3 vt = transform.position;
        vt.x = Camera.main.transform.position.x * speed;
        transform.position = vt;
    }
}
