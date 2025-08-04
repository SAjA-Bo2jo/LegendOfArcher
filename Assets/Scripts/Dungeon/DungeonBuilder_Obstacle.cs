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
    public List<GameObject> SpawnObstacles()
    {
        obstacleCount = StageManager.Instance.CurrentStageData.obstacleCount;
        
        List<GameObject> obstacles = new List<GameObject>();

        // Obstacle Positioning
        List<Vector3> positions = SetObstaclesPosition(obstacleCount);

        // Obstacle Generation
        for (int i = 0; i < obstacleCount; i++)
        {
            // Get Random Obstacle
            int index = Random.Range(0, obstaclePrefabs.Count());
            
            GameObject prefab = obstaclePrefabs[index];
            Vector3 pos = positions[i];
            
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            obstacles.Add(obj);
        }
        
        return obstacles;
    }

    // Create a specified number of random locations and return them as list
    private List<Vector3> SetObstaclesPosition(int count)
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
                float x = Random.Range(-8.3f, 8.3f);
                float y = Random.Range(-0.5f, 3.3f);
                newPos = new Vector3((x + 20 * ((StageManager.Instance.StageLevel - 1) % 5)), y);
                attempt++;

                // break and force to place obj after try 100 times
                if (attempt > maxAttempt)
                    break;

                flag = (IsOverlapping(newPos, minDistance)
                    || IsInPlayerSpawnArea(newPos)) ? true : false;
            }
            while (flag);

            positions.Add(newPos);
            allOccupiedPositions.Add(newPos);
        }

        return positions;
    }
}
