using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolObject : MonoBehaviour
{
    [Header("Pool Key Value")] [SerializeField]
    private string poolKey = "Enemy";
    public string PoolKey => poolKey;

    public void ReturnToPool()
    {
        EnemyController enemyCtrl = GetComponent<EnemyController>();

        if (enemyCtrl != null)
            enemyCtrl.OnReturnToPool(); // obj reset

        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.Return(poolKey, gameObject);
        }
        else
        {
            Debug.LogWarning("ObjectPoolManager.Instance가 null. Destroy로 대체함");
            Destroy(gameObject);
        }
    }
}
