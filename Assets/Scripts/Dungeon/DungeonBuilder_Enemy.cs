//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public partial class DungeonBuilder : MonoBehaviour
//{
//    [SerializeField] private GameObject[] enemyPrefabs;

//    // Number of enemies - Modifiable in Stage Manager
//    public int enemyCount = 1;

//    // Enemy generation method
//    public List<GameObject> SpawnEnemies()
//    {
//        List<GameObject> enemies = new List<GameObject>();

//        // Enemy Positioning
//        List<Vector3> positions = SetEnemiesPosition(enemyCount);

//        // Enemy Generation
//        for (int i = 0; i < enemyCount; i++)
//        {
//            // Get Random Enemy
//            int index = Random.Range(0, enemyPrefabs.Count());

//            GameObject prefab = enemyPrefabs[index];
//            Vector3 pos = positions[i];

//            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
//            enemies.Add(obj);

//            // send to Stageanager
//            StageManager.Instance.AddMonsterToList(prefab);
//        }

//        return enemies;
//    }

//    // Create a specified number of random locations and return them as list
//    private List<Vector3> SetEnemiesPosition(int count)
//    {
//        List<Vector3> positions = new List<Vector3>();

//        float minDistance = 1.0f;
//        int maxAttempt = 100;
//        bool flag = true;

//        for (int i = 0; i < count; i++)
//        {
//            int attempt = 0;
//            Vector3 newPos;
//            do
//            {
//                float x = Random.Range(-8.3f, 8.3f);
//                float y = Random.Range(-0.5f, 3.3f);
//                newPos = new Vector3(x, y);
//                attempt++;

//                // break and force to place obj after try 100 times
//                if (attempt > maxAttempt)
//                    break;

//                flag = (IsOverlapping(newPos, minDistance)
//                    || IsInPlayerSpawnArea(newPos)) ? true : false;
//            }
//            while (flag);

//            positions.Add(newPos);
//            allOccupiedPositions.Add(newPos);
//        }

//        return positions;
//    }
//}
