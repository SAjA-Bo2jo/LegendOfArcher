using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Front,
    Back,
    Side,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}


public class WallSpriteSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer wallRenderer;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite topLeftSprite;
    [SerializeField] private Sprite topRightSprite;
    [SerializeField] private Sprite bottomLeftSprite;
    [SerializeField] private Sprite bottomRightSprite;

    public void SetType(WallType type)
    {
        switch (type)
        {
            case WallType.Front: wallRenderer.sprite = frontSprite; break;
            case WallType.Back: wallRenderer.sprite = backSprite; break;
            case WallType.Side: wallRenderer.sprite = sideSprite; break;
            case WallType.TopLeft: wallRenderer.sprite = topLeftSprite; break;
            case WallType.TopRight: wallRenderer.sprite = topRightSprite; break;
            case WallType.BottomLeft: wallRenderer.sprite = bottomLeftSprite; break;
            case WallType.BottomRight: wallRenderer.sprite = bottomRightSprite; break;
        }
    }
}
