using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour, IResettable
{
    [SerializeField] private Sprite _openState;
    [SerializeField] private Sprite _closeState;

    [SerializeField] private bool startState;
    public bool State { get; private set; }
    [SerializeField] private Lever[] _levers;
    private Dictionary<Lever, bool> _leversStates;

    private void Awake()
    {
        State = startState;
        GetComponent<SpriteRenderer>().sprite = startState ? _openState : _closeState;
        _leversStates = new Dictionary<Lever, bool>(_levers.Select(x => new KeyValuePair<Lever, bool>(x, x.IsActive)));
        ChangeDoorState();
    }

    public void ChangeLeverState(Lever lever)
    {
        if (_leversStates.ContainsKey(lever)) _leversStates[lever] = lever.IsActive;
        ChangeDoorState();
    }

    public void ChangeDoorState()
    {
        State = _leversStates.All(x => x.Value);
        CheckStateChange();
    }

    public void CheckStateChange()
    {
        if (State)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = _openState;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<SpriteRenderer>().sprite = _closeState;
        }
    }

    public void OnReset()
    {
        GetComponent<SpriteRenderer>().sprite = startState ? _openState : _closeState;
        State = startState;
        ChangeDoorState();
    }
}
