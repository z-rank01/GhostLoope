using UnityEngine;
using UnityEngine.EventSystems;

public enum E_ActInfo
{
    moveDirection,
    playerSpeed,

    fireDirection, 
    bulletSpeed
}

public class ActInfo : BaseSingleton<ActInfo>
{
    // Player info
    public Vector2 moveDirection { get; set; }

    // Bullet info
    public Vector3 fireDirection { get; set; }
    public float bulletSpeed { get; set; }

    public void Clear()
    {
        // Player
        moveDirection = default(Vector2);


        // Bullet
        fireDirection = default(Vector3);
        bulletSpeed = default(float);
    }


    public void SetActInfo(E_ActInfo eActInfo, object info)
    {
        switch (eActInfo)
        {
            case E_ActInfo.moveDirection:
                moveDirection = (Vector2)info; break;
            case E_ActInfo.fireDirection:
                fireDirection = (Vector2)info; break;
            case E_ActInfo.bulletSpeed:
                bulletSpeed = (float)info; break;
        }
    }

}