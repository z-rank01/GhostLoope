using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator_GhostLoope : PlayerAnimator
{
    public void Move()
    {
        if (ActInfo.GetInstance().moveDirection.x == 0
            &&
            ActInfo.GetInstance().moveDirection.y == 1)
            animator.SetBool("Forward", true);

        if (ActInfo.GetInstance().moveDirection.x == -1
            &&
            ActInfo.GetInstance().moveDirection.y == 0)
            animator.SetBool("TurnLeft", true);

        if (ActInfo.GetInstance().moveDirection.x == 1
            &&
            ActInfo.GetInstance().moveDirection.y == 0)
            animator.SetBool("TurnLeft", true);
    }

    public void MoveForward()
    {
        if (ActInfo.GetInstance().moveDirection.x == 0 
            && 
            ActInfo.GetInstance().moveDirection.y == 1)
            animator.SetBool("Forward", true);
    }

    public void MoveLeft()
    {
        if (ActInfo.GetInstance().moveDirection.x == -1
            &&
            ActInfo.GetInstance().moveDirection.y == 0)
            animator.SetBool("TurnLeft", true);
    }

    public void MoveRight()
    {
        if (ActInfo.GetInstance().moveDirection.x == 1
            &&
            ActInfo.GetInstance().moveDirection.y == 0)
            animator.SetBool("TurnLeft", true);
    }

    public void Attack()
    {
        animator.SetBool("Attack", true);
    }

    public void Dash()
    {
        animator.SetBool("Dash", true);
    }

    public void Clear()
    {
        animator.SetBool("Forward", false);
        animator.SetBool("TurnLeft", false);
        animator.SetBool("TurnRight", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Dash", false);
    }

    public void ClearForwad()
    {
        animator.SetBool("Forward", false);
    }

    public void ClearLeft()
    {
        animator.SetBool("TurnLeft", false);
    }

    public void ClearRight()
    {
        animator.SetBool("TurnRight", false);
    }

    public void ClearAttack()
    {
        animator.SetBool("Attack", false);
    }

}
