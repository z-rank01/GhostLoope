using UnityEngine;

public class BombOfBossPoison : MonoBehaviour
{
    public float bombRadius;
    public float bombDamage;
    public float flyingTime;

    private Rigidbody rb;
    private float currflyingDeltaTime;
    private int currflyingSeconds;



    protected void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void Update()
    {
        if (CheckReachTime()) Bomb();
    }


    // interface
    public void SetOrigin(Vector3 origin)
    {
        transform.position = origin;
    }


    public void throwOut()
    {
        float distance = GetPlayerDistance();
        rb.velocity = calculateBestThrowSpeed(transform.position, 
                                            Player.GetInstance().transform.position, 
                                            flyingTime);
    }


    // private function
    private void Bomb()
    {
        // music
        MusicManager.GetInstance().PlayFireSound("’®µØ±¨’®“Ù–ß");

        // exert effect
        if (GetPlayerDistance() < bombRadius)
            Player.GetInstance().PlayerReceiveDamage(bombDamage);

        // return pool
        PoolManager.GetInstance().ReturnObj(E_PoolType.BossPoisonBomb, this.gameObject);
    }
    private bool CheckReachTime()
    {
        if (currflyingSeconds < flyingTime)
        {
            currflyingDeltaTime += Time.deltaTime;
            currflyingSeconds = (int)currflyingDeltaTime % 60;
            return false;
        }
        else return true;
    }
    private float GetPlayerDistance()
    {
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        // calculate vectors
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // calculate xz and y
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
        // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
        // so xz = v0xz * t => v0xz = xz / t
        // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
        float t = timeToTarget;
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;

        // create result vector for calculated starting speeds
        Vector3 result = toTargetXZ.normalized;     // get direction of xz but with magnitude 1
        result *= v0xz;                             // set magnitude of xz to v0xz (starting speed in xz plane)
        result.y = v0y;                             // set y to v0y (starting speed of y plane)

        return result;
    }
}