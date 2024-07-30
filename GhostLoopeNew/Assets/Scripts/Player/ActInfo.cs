using UnityEngine;
using UnityEngine.EventSystems;

public class ActInfo : BaseSingleton<ActInfo>
{
    public Vector2 moveDirection { get; set; }
    public float speed   { get; set; }

    public void Clear()
    {
        moveDirection = default(Vector2);
        speed = default(float);
    }

    public void SetActInfo(ActInfo info)
    {
        moveDirection = info.moveDirection;
        speed = info.speed;
    }

    public void SetActInfo(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    public void SetActInfo(float speed)
    {
        this.speed = speed;
    }

}