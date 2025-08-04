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
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.Return(poolKey, gameObject);
        }
        else
        {
            Debug.LogWarning("ObjectPoolManager.Instance�� null. Destroy�� ��ü��");
            Destroy(gameObject);
        }
    }
}
