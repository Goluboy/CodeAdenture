using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IResettable
{
    [SerializeField] private Sprite _activeState;
    [SerializeField] private Sprite _inactiveState;
    [SerializeField] private bool startState;
    [SerializeField] private Door[] _doors;
    public bool IsActive { get; set; }

    public void Awake()
    {
        IsActive = startState;
        GetComponent<SpriteRenderer>().sprite = IsActive ? _activeState : _inactiveState;
    }
    public void Switch()
    {
        IsActive = !IsActive;
        GetComponent<SpriteRenderer>().sprite = IsActive ? _activeState : _inactiveState;
        ChangeDoorsStates();
    }

    public void OnReset()
    {
        IsActive = startState;
        GetComponent<SpriteRenderer>().sprite = IsActive ? _activeState : _inactiveState;
        ChangeDoorsStates();
    }

    public void ChangeDoorsStates()
    {
        foreach (Door door in _doors)
        {
            door.ChangeLeverState(this);
        }
    }
}
