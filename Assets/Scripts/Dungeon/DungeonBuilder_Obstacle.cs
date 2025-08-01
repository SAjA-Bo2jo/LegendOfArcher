using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;

    // Number of Obstacles - Modifiable in Stage Manager
    public int obstacleCount = 1;

    // Obstacle generation method
    public List<GameObject> SpawnObstacles()
    {
        List<GameObject> obstacles = new List<GameObject>();

        // Obstacle Positioning
        List<Vector3> positions = SetObstaclesPosition(obstacleCount);

        // Obstacle Generation
        foreach (Vector3 pos in positions)
        {
            GameObject obj = Instantiate(obstacle, pos, Quaternion.identity);
            obstacles.Add(obj);
        }
        
        return obstacles;
    }

    // Create a specified number of random locations and return them as list
    private List<Vector3> SetObstaclesPosition(int count)
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-8.3f, 8.3f);
            float y = Random.Range(-0.5f, 3.3f);
            positions.Add(new Vector3(x, y));
        }

        return positions;
    }
}
