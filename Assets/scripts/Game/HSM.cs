using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HSM : MonoBehaviour {
    public enum State
    {
        Start,
        Init,
        Play,
        PlayerReachCell,
        Transaction,
        CompleteTransaction,
        Evaluate,
        EndGame        
    };

    Dictionary<KeyValuePair<State, State>, Action> TransitionMap = new Dictionary<KeyValuePair<State, State>, Action>();
    State currentState = State.Start;

    public void AddTransition(KeyValuePair<State, State> key, Action action)
    {
        TransitionMap[key] = action;
    }
    IEnumerator transit(KeyValuePair<State, State> key, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        TransitionMap[key]();
    }
    public void Go(State nextState, float delay = 0.01f)
    {
        KeyValuePair<State, State> key = new KeyValuePair<State, State>(currentState, nextState);
        if (TransitionMap.ContainsKey(key))
        {
            //Debug.Log(currentState + "-->" + nextState);
            currentState = nextState;
            StartCoroutine(transit(key, delay));
        }
        else
        {
            Debug.Log("Failed To Transit From " + currentState.ToString() +  " to " + nextState.ToString());
        }
    }

    public State GetCurrentState() { return currentState; }	
}
