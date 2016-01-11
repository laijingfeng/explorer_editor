using UnityEngine;
using System.Collections;

/// <summary>
/// 背景物体产生器
/// </summary>
public class BackgroundPropSpawner : MonoBehaviour
{
    /// <summary>
    /// 背景物体
    /// </summary>
    public Rigidbody2D backgroundProp;

    /// <summary>
    /// 左边产生点
    /// </summary>
    public float leftSpawnPosX;

    /// <summary>
    /// 右边产生点
    /// </summary>
    public float rightSpawnPosX;

    /// <summary>
    /// 最低的Y
    /// </summary>
    public float minSpawnPosY;

    /// <summary>
    /// 最高的Y
    /// </summary>
    public float maxSpawnPosY;

    /// <summary>
    /// 产生间隔最小值
    /// </summary>
    public float minTimeBetweenSpawns;

    /// <summary>
    /// 产生间隔最大值
    /// </summary>
    public float maxTimeBetweenSpawns;

    /// <summary>
    /// 最低速度
    /// </summary>
    public float minSpeed;

    /// <summary>
    /// 最大速度
    /// </summary>
    public float maxSpeed;

    void Start()
    {
        Random.seed = System.DateTime.Today.Millisecond;
        StartCoroutine("Spawn");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(rightSpawnPosX + leftSpawnPosX, maxSpawnPosY + minSpawnPosY, 0) * 0.5f, new Vector3(rightSpawnPosX - leftSpawnPosX, maxSpawnPosY - minSpawnPosY, 0));
    }

    /// <summary>
    /// 产生
    /// </summary>
    /// <returns></returns>
    private IEnumerator Spawn()
    {
        float waitTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        yield return new WaitForSeconds(waitTime);

        bool facingLeft = Random.Range(0, 2) == 0;

        float posX = facingLeft ? rightSpawnPosX : leftSpawnPosX;

        float posY = Random.Range(minSpawnPosY, maxSpawnPosY);

        Vector3 spawnPos = new Vector3(posX, posY, transform.position.z);

        Rigidbody2D propInstance = Instantiate(backgroundProp, spawnPos, Quaternion.identity) as Rigidbody2D;

        if (!facingLeft)
        {
            Vector3 scale = propInstance.transform.localScale;
            scale.x *= -1;
            propInstance.transform.localScale = scale;
        }

        float speed = Random.Range(minSpeed, maxSpeed);

        speed *= facingLeft ? -1f : 1f;

        propInstance.velocity = new Vector2(speed, 0);

        StartCoroutine(Spawn());

        while (propInstance != null)
        {
            if (facingLeft)
            {
                if (propInstance.transform.position.x < leftSpawnPosX - 0.5f)
                {
                    Destroy(propInstance.gameObject);
                }
            }
            else
            {
                if (propInstance.transform.position.x > rightSpawnPosX + 0.5f)
                {
                    Destroy(propInstance.gameObject);
                }
            }

            yield return null;
        }
    }
}
