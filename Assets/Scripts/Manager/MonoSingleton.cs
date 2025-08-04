using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// MonoBehaviour를 상속받는 제네릭 싱글톤 클래스
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스 변수
    private static T instance;

    // 외부에서 접근 가능한 싱글톤 인스턴스 프로퍼티
    public static T Instance
    {
        get
        {
            // 만약 인스턴스가 아직 없으면 씬에서 해당 타입의 객체를 찾음
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                // 씬에서 찾지 못하면 새 게임 오브젝트를 생성하고 컴포넌트 추가
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
        set { instance = value; }
    }

    //private void Awake()
    //{
    //    if (transform.parent != null && transform.root != null) // 해당 오브젝트가 자식 오브젝트라면
    //    {
    //        DontDestroyOnLoad(this.transform.root.gameObject); // 부모 오브젝트를 DontDestroyOnLoad 처리
    //    }
    //    else
    //    {
    //        DontDestroyOnLoad(this.gameObject); // 해당 오브젝트가 최 상위 오브젝트라면 자신을 DontDestroyOnLoad 처리
    //    }
    //}

    // MonoBehaviour의 Awake 메서드, 초기화 역할 수행
    protected virtual void Awake()
    {
        // 이미 인스턴스가 존재하고 그것이 지금 이 오브젝트가 아니면 중복 방지 위해 삭제
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스가 할당되지 않은 경우 현재 오브젝트를 할당
        if (instance == null)
        {
            instance = this as T;
        }

        // 부모가 있으면 부모 루트 게임오브젝트를 파괴하지 않도록 설정
        // if (transform.parent != null && transform.root != null)
        //     DontDestroyOnLoad(this.transform.root.gameObject);
        // else
        //     // 부모가 없으면 이 게임오브젝트를 파괴하지 않도록 설정
        //     DontDestroyOnLoad(this.gameObject);

        // 씬이 로드될 때 호출될 이벤트 강제 등록
        RegisterSceneLoadedEvent();
    }

    // 오브젝트가 파괴될 때 호출 (리소스 정리용)
    protected virtual void OnDestroy()
    {
        // 지금 파괴되는 오브젝트가 싱글톤 인스턴스라면
        if (instance == this)
        {
            // 씬 로드 이벤트 등록 해제
            UnregisterSceneLoadedEvent();

            // 인스턴스 변수를 초기화해서 다시 생성 가능하게 함
            instance = null;
        }
    }

    // 자식 클래스에서 씬 로드 이벤트 등록 방법 재정의 가능
    protected virtual void RegisterSceneLoadedEvent() { }

    // 자식 클래스에서 씬 로드 이벤트 등록 해제 방법 재정의 가능
    protected virtual void UnregisterSceneLoadedEvent() { }

    // 씬이 로드될 때 호출되는 콜백, 자식 클래스가 재정의해 사용
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
}
