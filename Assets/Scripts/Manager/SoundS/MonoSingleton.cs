using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this as T;
        }

        if (transform.parent != null && transform.root != null)
            DontDestroyOnLoad(this.transform.root.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);

        // 씬 로드 이벤트 등록은 Awake에서 호출해 강제 등록
        RegisterSceneLoadedEvent();
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            UnregisterSceneLoadedEvent();
            instance = null;
        }
    }

    // 자식클래스가 오버라이드하여 씬로딩 이벤트 처리
    protected virtual void RegisterSceneLoadedEvent() { }
    protected virtual void UnregisterSceneLoadedEvent() { }
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
}
