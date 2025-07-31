using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TempStageManager : MonoBehaviour
{
    [SerializeField] private GameObject dungeonBuilderObj;

    private void Awake()
    {
        dungeonBuilderObj = Instantiate(dungeonBuilderObj);      
    }

    private void Start()
    {
        DungeonBuilder dungeonBuilder = dungeonBuilderObj.GetComponent<DungeonBuilder>();

        if (dungeonBuilderObj == null)
        {
            Debug.LogError("DungeonBuilder ������Ʈ�� ã�� �� �����ϴ�.");
        }

        dungeonBuilder.Build();
    }


}
