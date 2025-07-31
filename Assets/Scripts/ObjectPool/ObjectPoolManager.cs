using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // �̱��� - ������ ����
    public static ObjectPoolManager Instance { get; private set; }

    // ����Ƽ �����Ϳ��� ���� ���� ����ȭ
    [Serializable]
    public class PoolEntry
    {
        public string key;
        public GameObject prefab;
        public int size;
    }
    [SerializeField] private List<PoolEntry> entries;

    // Ű������ ȣ��Ǵ� ������Ʈ ť�� ��ųʸ�
    private Dictionary<string, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        foreach (var entry in entries)
        {
            // ť �迭 �ʱ�ȭ
            Queue<GameObject> queue = new Queue<GameObject>();

            // Ǯ�� ũ�⸸ŭ �ݺ�
            for (int i = 0; i < entry.size; i++)
            {
                GameObject obj = Instantiate(entry.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            // Ǯ Ű���� ť ����
            poolDict[entry.key] = queue;
        }
    }

    // ��ü Ȱ��ȭ �޼��� - ObjectPoolManager.Instance.Get(Ű�� �̸�);
    public GameObject Get(string key)
    {
        // key ȣ���� �߸����� ��� ���� ó��
        if (!poolDict.ContainsKey(key))
        {
            Debug.LogError($"[ObjectPool] Ű '{key}'�� �ش��ϴ� ������Ʈ�� �����ϴ�.");
            return null;
        }
        
        // Ǯ ũ�⺸�� ���� ��ü�� ��ȯ�ϴ� ���
        if (poolDict[key].Count <= 0)
        {
            PoolEntry poolEntry = new PoolEntry();
            foreach (var entry in entries)
            {
                if (entry.key == key)
                {
                    poolEntry = entry;
                    break;
                }
            }
            GameObject prefab = poolEntry.prefab;
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(true);
            return newObj;
        }

        // Ǯ���� ��ü ��ȯ
        GameObject obj = poolDict[key].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    // ��ü ��Ȱ��ȭ �޼��� - ObjectPoolManager.Instance.Return(Ű��, this.gameObject);
    public void Return(string key, GameObject obj)
    {
        obj.SetActive(false);
        poolDict[key].Enqueue(obj);
    }
}
