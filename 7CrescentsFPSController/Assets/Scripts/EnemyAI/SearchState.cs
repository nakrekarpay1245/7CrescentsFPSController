using System;
using UnityEngine;
using UnityEngine.UI;

public class SearchState : IState
{
    public Action<string> Callback;

    public AudioSource audioSource;
    public Animator animator;

    public float speed;

    public GameObject alarmDisplay;
    public Image alarmImage;

    public float suspicionAlarmTimer;

    public FieldOfView fieldOfView;
    public EnemyHealth enemyHealth;
    public EnemyAI enemyAI;

    public SearchState(Action<string> Callback, AudioSource audioSource, Animator animator,
        float speed, GameObject alarmDisplay, Image alarmImage,
        FieldOfView fieldOfView, EnemyHealth enemyHealth, EnemyAI enemyAI)
    {
        this.Callback = Callback;
        this.audioSource = audioSource;
        this.animator = animator;
        this.speed = speed;
        this.alarmDisplay = alarmDisplay;
        this.alarmImage = alarmImage;
        this.fieldOfView = fieldOfView;
        this.enemyHealth = enemyHealth;
        this.enemyAI = enemyAI;
    }
    public void OnStateEnter()
    {
        //Debug.Log("Search Enter");
        animator.SetBool("isSuspicion", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isRun", false);

        alarmDisplay.SetActive(false);
        alarmImage.fillAmount = 0;

        fieldOfView.viewMeshFilter.gameObject.SetActive(true);
    }

    public void OnStateExit()
    {
       // Debug.Log("Search Exit");
    }

    public void OnStateFixedUpdate()
    {
       // Debug.Log("Search FixedUpdate");
    }

    public void OnStateUpdate()
    {
        suspicionAlarmTimer = Mathf.Clamp(suspicionAlarmTimer, 0, Mathf.Infinity);

      //  Debug.Log("Search Update");

        if (fieldOfView.targetIsDetected || enemyHealth.impact)
        {
            if (SearchToSuspicionAlarmControl())
            {
                Callback("suspicion");
            }
        }
        else
        {
            suspicionAlarmTimer -= Time.deltaTime;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            RotateAround();
        }
    }

    private void RotateAround()
    {
        enemyAI.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
    private bool SearchToSuspicionAlarmControl()
    {
        //Debug.Log("Search to Chase suspicion : " + suspicionAlarmTimer);

        suspicionAlarmTimer += Time.deltaTime;
        if (suspicionAlarmTimer >= enemyAI.suspicionAlarmTime)
        {
          //  Debug.Log("Search to Suspicion");
            return true;
        }
        else
            return false;
    }
}
