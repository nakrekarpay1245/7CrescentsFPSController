using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{
    private IState currentState;

    //[SerializeField]
    //private Text stateText;
    public void ChangeStates(IState iState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = iState;
        currentState.OnStateEnter();
    }

    public void UpdateStates()
    {
        //stateText.text = currentState.ToString();
        currentState.OnStateUpdate();
    }

    public void FixedUpdateStates()
    {
        currentState.OnStateFixedUpdate();
    }
}
