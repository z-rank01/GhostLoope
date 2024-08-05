using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator_GhostLoope : PlayerAnimator
{
    private float moveFrame = 0;
    
    public void Move()
    {
        moveFrame += Time.deltaTime;// * GlobalSetting.GetInstance().playerSpeed / 10;
        if (moveFrame > 1) moveFrame = 1;
        SetFloat("Move", moveFrame);
    }

    public void Attack()
    {
        SetBool("Attack", true);
    }

    public void Dash()
    {
        SetBool("Dash", true);
    }

    public void TakeDamage()
    {
        SetBool("TakeDamage", true);
    }

    public void Die()
    {
        SetTrigger("Die");
    }


    public void Idle()
    {
        moveFrame -= Time.deltaTime;// * GlobalSetting.GetInstance().playerSpeed / 10;
        if (moveFrame < 0) moveFrame = 0;
        SetFloat("Move", moveFrame);
    }

    public void ClearAttack()
    {
        SetBool("Attack", false);
    }

    public void ClearDash()
    {
        SetBool("Dash", false);
    }

    public void ClearTakeDamage()
    {
        SetBool("TakeDamage", false);
    }

}
