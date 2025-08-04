using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;

    // Number of Obstacles - Modifiable in Stage Manager
    public int obstacleCount = 1;

    // Obstacle generation method
    public void SpawnObstacles()
    {
        obstacleCount = StageManager.Instance.CurrentStageData.obstacleCount;

        List<GameObject> obstacles = new List<GameObject>();

        // Obstacle Positioning
        List<Vector3> positions = SetObjectsPosition(obstacleCount);

        // Obstacle Generation
        for (int i = 0; i < obstacleCount; i++)
        {
            // Get Random Obstacle
            int index = Random.Range(0, obstaclePrefabs.Count());

            GameObject prefab = obstaclePrefabs[index];
            Vector3 pos = positions[i];

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            
            Debug.Log($" obstacle spawn location {i}: {positions[i]}");
            
            obstacles.Add(obj);
        }

        result.dungeonRoot.transform.SetParent(StageManager.Instance.dungeonParent.transform);
        result.obstacles = obstacles;
    }

    // Create a specified number of random locations and return them as list
    private List<Vector3> SetObjectsPosition(int count)
    {
        List<Vector3> positions = new List<Vector3>();

        float minDistance = 1.0f;
        int maxAttempt = 100;
        bool flag = true;

        for (int i = 0; i < count; i++)
        {
            int attempt = 0;
            Vector3 newPos;
            do
            {
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(0f, 2f);
                newPos = new Vector3((x + 20 * ((StageManager.Instance.StageLevel - 1) % 5)), y);
                attempt++;

                // break and force to place obj after try 100 times
                if (attempt > maxAttempt)
                {
                    Debug.LogWarning("오브젝트 배치 실패! 강제 배치합니다.");
                    break;
                }

                flag = (IsOverlapping(newPos, minDistance)
                    || IsInPlayerSpawnArea(newPos)) ? true : false;
            }
            while (flag);

            positions.Add(newPos);
            allOccupiedPositions.Add(newPos);
        }

        return positions;
    }

    // spawn fixed obstacles for boss stage
    private List<GameObject> SpawnFixedObstacles(int level)
    {
        List<GameObject> obstacles = new List<GameObject>();

        float startX = -4.8f;
        float y = 1.5f;
        float gap = 2.4f;

        // base location X in current map
        float offsetX = 20f * level;

        for (int i = 0; i < 5; i++)
        {
            float x = startX + gap * i + offsetX;
            Vector3 pos = new Vector3(x, y, 0);

            // only spawn Box which index is 0
            GameObject obj = Instantiate(obstaclePrefabs[0], pos, Quaternion.identity);

            Debug.Log($" object spawn location {i}: {pos}");

            allOccupiedPositions.Add(pos);

            obstacles.Add(obj);
        }

        result.dungeonRoot.transform.SetParent(StageManager.Instance.dungeonParent.transform);
        return obstacles;
    }
}
