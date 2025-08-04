using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// MonoBehaviour 기반의 제네릭 싱글톤 베이스 클래스
// 상속받는 클래스가 별도 싱글톤 구현 없이 인스턴스 단 하나만 존재하도록 보장
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // 싱글톤 인스턴스 접근 프로퍼티
    public static T Instance
    {
        get
        {
            // 인스턴스 없으면 씬에서 찾거나 새로 생성
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

    // Awake에서 인스턴스 중복 체크 및 씬 전환 시 파괴 방지 설정
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 중복된 다른 인스턴스라면 자신 파괴
            return;
        }

        if (instance == null)
        {
            instance = this as T; // 인스턴스가 비어있으면 자신이 담당
        }

        // 부모가 있으면 루트 오브젝트를 파괴 금지 처리, 없으면 자기 자신 처리
        if (transform.parent != null && transform.root != null)
            DontDestroyOnLoad(this.transform.root.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

    // OnDestroy에서 인스턴스 참조 초기화
    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // 하위클래스에서 씬 로드 이벤트 구독용으로 오버라이드 가능
    protected virtual void RegisterSceneLoadedEvent() { }
    protected virtual void UnregisterSceneLoadedEvent() { }
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
}