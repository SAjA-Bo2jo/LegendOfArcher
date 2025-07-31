using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각각의 매니저들이 싱글톤을 상속하여 따로 싱글톤을 구현하지 않아도 싱글톤이 되도록 구현
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject go;
                go = GameObject.Find(typeof(T).Name);

                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
                else
                {
                    instance = go.GetComponent<T>();
                }
            }

            return instance;
        }
    }
}
