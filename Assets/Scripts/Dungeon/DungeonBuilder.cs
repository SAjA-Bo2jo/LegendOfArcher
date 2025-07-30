//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using UnityEngine;

//public class DungeonBuilder : MonoBehaviour
//{
//    [SerializeField] private SpriteRenderer wall;
//    [SerializeField] private SpriteRenderer floorTile;

//    [SerializeField] private int width = 21;
//    [SerializeField] private int height = 13;

//    private WallType wallType;

//    public void BuildRoom(Vector2 startPos)
//    {
//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                Vector2 tilePos = startPos + new Vector2(x, y);

//                // º® ¹èÄ¡
//            //    if (x == 0 && y == height - 1)
//            //        //Instantiate(wallTopLeft, tilePos, Quaternion.identity, transform);
                    
//            //    else if (x == width - 1 && y == height - 1)
//            //        //Instantiate(wallTopRight, tilePos, Quaternion.identity, transform);
//            //    else if (x == 0 && y == 0)
//            //        //Instantiate(wallBottomLeft, tilePos, Quaternion.identity, transform);
//            //    else if (x == width - 1 && y == 0)
//            //        //Instantiate(wallBottomRight, tilePos, Quaternion.identity, transform);
//            //    else if (y == height - 1)
//            //        //Instantiate(wallBack, tilePos, Quaternion.identity, transform);
//            //    else if (y == 0)
//            //        //Instantiate(wallFront, tilePos, Quaternion.identity, transform);
//            //    else if (x == 0 || x == width - 1)
//            //        //Instantiate(wallSide, tilePos, Quaternion.identity, transform);
//            //    else
//            //        //Instantiate(floorTile, tilePos, Quaternion.identity, transform);
//            }
//        }
//    }
//}
