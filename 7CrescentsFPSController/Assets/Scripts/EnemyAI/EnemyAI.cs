using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    #region STATE PARAMETERS
    [Tooltip("Durum Makinesi")]
    [Header("Durum Makinesi ve Durumlar")]
    public StateMachine stateMachine;

    private IdleState idleState;
    private PatrolState patrolState;
    private ChaseState chaseState;
    private SuspicionState suspicionState;
    private SearchState searchState;
    #endregion

    #region COMPONENT PARAMETERS
    [Header("Components")]
    [HideInInspector] public FieldOfView fieldOfView;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyHealth enemyHealth;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;

    [Header("Düşman Obje")]
    [HideInInspector] public List<Transform> targets;
    #endregion

    #region OTHER PARAMETERS
    [Header("Hareket Hızı")]
    public float moveSpeed;

    [Header("Dönme Hızı")]
    public float rotateSpeed;

    [Header("Alarm Değişkenleri")]
    [Tooltip("Chase' e geçerken alarm süresi")]
    public float chaseAlarmTime;

    [Tooltip("Suspicion' a geçerken alarm süresi")]
    public float suspicionAlarmTime;

    [Tooltip("Search' a geçerken alarm süresi")]
    public float searchAlarmTime;

    [Tooltip("Alarm ibaresi")]
    public GameObject alarmDisplay;

    [Tooltip("Alarm seviyesi göstergesi")]
    public Image alarmImage;

    [Tooltip("Durma anında çalınacak ses")]
    public AudioClip idleClip;

    [Tooltip("Koşma anında çalınacak ses")]
    public AudioClip runClip;

    [Tooltip("Can ve uyarı göstergeleri için yerel Canvas")]
    public GameObject localCanvas;
    #endregion

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        fieldOfView = GetComponent<FieldOfView>();
        enemyHealth = GetComponent<EnemyHealth>();
        targets.Add(fieldOfView.target.transform);
        alarmImage.fillAmount = 0;
        StateGenerator();
        Idle();
    }
    private void Update()
    {
        stateMachine.UpdateStates();
        localCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateStates();
    }

    #region STATE CHANGERS
    public void Idle()
    {
        stateMachine.ChangeStates(idleState);
    }
    public void Patrol()
    {
        stateMachine.ChangeStates(patrolState);
    }
    public void Chase()
    {
        stateMachine.ChangeStates(chaseState);
    }
    public void Suspicion()
    {
        stateMachine.ChangeStates(suspicionState);
    }
    public void Search()
    {
        stateMachine.ChangeStates(searchState);
    }
    #endregion


    #region CALLBACKS
    private void IdleCallback(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                Idle();
                break;
            case "patrol":
                Patrol();
                break;
            case "chase":
                Chase();
                break;
            case "suspicion":
                Suspicion();
                break;
            case "Search":
                Search();
                break;
            default:
                Idle();
                break;
        }
    }

    private void PatrolCallback(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                Idle();
                break;
            case "patrol":
                Patrol();
                break;
            case "chase":
                Chase();
                break;
            case "suspicion":
                Suspicion();
                break;
            case "Search":
                Search();
                break;
            default:
                Idle();
                break;
        }
    }

    private void ChaseCallback(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                Idle();
                break;
            case "patrol":
                Patrol();
                break;
            case "chase":
                Chase();
                break;
            case "suspicion":
                Suspicion();
                break;
            case "Search":
                Search();
                break;
            default:
                Idle();
                break;
        }
    }
    private void SuspicionCallback(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                Idle();
                break;
            case "patrol":
                Patrol();
                break;
            case "chase":
                Chase();
                break;
            case "suspicion":
                Suspicion();
                break;
            case "Search":
                Search();
                break;
            default:
                Idle();
                break;
        }
    }
    private void SearchCallback(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                Idle();
                break;
            case "patrol":
                Patrol();
                break;
            case "chase":
                Chase();
                break;
            case "suspicion":
                Suspicion();
                break;
            case "Search":
                Search();
                break;
            default:
                Idle();
                break;
        }
    }

    #endregion


    #region NEW STATES  
    private void StateGenerator()
    {
        NewIdle();
        NewPatrol();
        NewChase();
        NewSuspicion();
        NewSearch();
    }


    private IdleState NewIdle()
    {
        idleState = new IdleState(IdleCallback, audioSource, animator, idleClip, alarmDisplay,
            fieldOfView, enemyHealth, this);
        return idleState;
    }
    private PatrolState NewPatrol()
    {
        patrolState = new PatrolState(PatrolCallback, audioSource, animator, runClip, alarmDisplay,
            fieldOfView, enemyHealth, this);
        return patrolState;
    }
    private ChaseState NewChase()
    {
        chaseState = new ChaseState(ChaseCallback, audioSource, animator, runClip, moveSpeed,
            fieldOfView, enemyHealth, this);
        return chaseState;
    }
    private SuspicionState NewSuspicion()
    {
        suspicionState = new SuspicionState(SuspicionCallback, audioSource, animator, rotateSpeed,
            alarmDisplay, alarmImage, fieldOfView, enemyHealth,
            this);
        return suspicionState;
    }
    private SearchState NewSearch()
    {
        searchState = new SearchState(SearchCallback, audioSource, animator, rotateSpeed,
            alarmDisplay, alarmImage, fieldOfView, enemyHealth,
            this);
        return searchState;
    }
    #endregion

}
