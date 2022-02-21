using System;
using UnityEngine;

public class ChaseState : IState
{
    public Action<string> Callback;

    public AudioSource audioSource;
    public Animator animator;

    public AudioClip audioClip;

    public float speed;

    public FieldOfView fieldOfView;
    public EnemyHealth enemyHealth;
    public EnemyAI enemyAI;
    public ChaseState(Action<string> Callback, AudioSource audioSource, Animator animator,
        AudioClip audioClip, float speed, FieldOfView fieldOfView, EnemyHealth enemyHealth,
        EnemyAI enemyAI)
    {
        this.Callback = Callback;
        this.audioSource = audioSource;
        this.animator = animator;
        this.audioClip = audioClip;
        this.speed = speed;
        this.fieldOfView = fieldOfView;
        this.enemyHealth = enemyHealth;
        this.enemyAI = enemyAI;
    }
    public void OnStateEnter()
    {
        //Debug.Log("Game Over");

        animator.SetBool("isSuspicion", false);
        animator.SetBool("isIdle", false);
        animator.SetBool("isRun", true);

        audioSource.clip = audioClip;

        fieldOfView.viewMeshFilter.gameObject.SetActive(false);
    }

    public void OnStateExit()
    {
        //  Debug.Log("Chase Exit");
    }

    public void OnStateFixedUpdate()
    {
        // Debug.Log("Chase FixedUpdate");
    }

    public void OnStateUpdate()
    {
        //  Debug.Log("Chase Update");
        //look and follow the target
        LookToTarget();
        FollowTarget();
    }

    private void LookToTarget()
    {
        //  Debug.Log("Look To Target in Chase");
        Quaternion rotationTarget = Quaternion.LookRotation(fieldOfView.targetPosition -
            enemyAI.transform.position);
        enemyAI.transform.rotation = Quaternion.RotateTowards(enemyAI.transform.rotation,
            rotationTarget, Time.deltaTime * speed);
    }
    private void FollowTarget()
    {
        //  Debug.Log("Follow To Target in Chase");
        Vector3 followPosition = Vector3.Lerp(enemyAI.transform.position,
            fieldOfView.targetPosition, Time.deltaTime * speed * 0.25f);
        enemyAI.transform.position = followPosition;
    }
}
