using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DungeonBuilder : MonoBehaviour
{
    // set boss stage obstacles
    private List<GameObject> SpawnFixedObstacles()
    {
        List<GameObject> obstacles = new List<GameObject>();

        float startX = -4.8f;
        float y = 1.5f;
        float gap = 2.4f;

        // base location X in current map
        float offsetX = 20f * ((StageManager.Instance.StageLevel - 1) % 5);

        for (int i = 0; i < 5; i++)
        {
            float x = startX + gap * i + offsetX;
            Vector3 pos = new Vector3(x, y, 0);

            // only spawn Box which index is 0
            GameObject obj = Instantiate(obstaclePrefabs[0], pos, Quaternion.identity);

            obstacles.Add(obj);
        }

        return obstacles;
    }

    /*
    // spawn boss
    private void SpawnBoss()
    {
        // base location X in current map
        float offsetX = 20f * ((StageManager.Instance.StageLevel - 1) % 5);

        Vector2 bossPos = new Vector2(6.5f + offsetX, 1f);
        GameObject boss = ObjectPoolManager.Instance.Get("Boss");

        boss.transform.SetParent(StageManager.Instance.enemyParent.transform);
        boss.transform.position = bossPos;

        result.enemies = new List<GameObject> { boss };
        StageManager.Instance.AddMonsterToList(boss);
    }
    */

}
