using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // 싱글톤 - 생성자 제한
    public static ObjectPoolManager Instance { get; private set; }

    // 유니티 에디터에서 보기 위한 직렬화
    [Serializable]
    public class PoolEntry
    {
        public string key;
        public GameObject prefab;
        public int size;
    }
    [SerializeField] private List<PoolEntry> entries;

    // 키값으로 호출되는 오브젝트 큐의 딕셔너리
    private Dictionary<string, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        // 싱글톤 패턴
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
            // 큐 배열 초기화
            Queue<GameObject> queue = new Queue<GameObject>();

            // 풀의 크기만큼 반복
            for (int i = 0; i < entry.size; i++)
            {
                GameObject obj = Instantiate(entry.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            // 풀 키값에 큐 저장
            poolDict[entry.key] = queue;
        }
    }

    // 객체 활성화 메서드 - ObjectPoolManager.Instance.Get(키값 이름);
    public GameObject Get(string key)
    {
        // key 호출이 잘못됬을 경우 오류 처리
        if (!poolDict.ContainsKey(key))
        {
            Debug.LogError($"[ObjectPool] 키 '{key}'에 해당하는 오브젝트가 없습니다.");
            return null;
        }
        
        // 풀 크기보다 많은 객체를 소환하는 경우
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

        // 풀에서 객체 소환
        GameObject obj = poolDict[key].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    // 객체 비활성화 메서드 - ObjectPoolManager.Instance.Return(키값, this.gameObject);
    public void Return(string key, GameObject obj)
    {
        obj.SetActive(false);
        poolDict[key].Enqueue(obj);
    }
}
