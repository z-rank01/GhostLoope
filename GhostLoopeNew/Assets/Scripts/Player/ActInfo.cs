using UnityEngine;
using UnityEngine.EventSystems;

public class ActInfo : BaseSingleton<ActInfo>
{
    // Player info
    public Vector2 moveDirection { get; set; }
    public float playerSpeed   { get; set; }

    // Bullet info
    public Vector3 fireDirection { get; set; }
    public float bulletSpeed { get; set; }

    public void Clear()
    {
        // Player
        moveDirection = default(Vector2);
        playerSpeed = default(float);


        // Bullet
        fireDirection = default(Vector3);
        bulletSpeed = default(float);
    }

    public void SetActInfo(ActInfo info)
    {
        moveDirection = info.moveDirection;
        playerSpeed = info.playerSpeed;
    }

    public void SetActInfo(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection;
    }

    public void SetActInfo(float speed)
    {
        this.playerSpeed = speed;
    }

}