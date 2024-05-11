using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stack : MonoBehaviour, IResettable
{
    [SerializeField] private int[] requiredStack;
    public Transform LastValue { get; set; }

    public readonly Stack<Box> BoxesStack = new();

    public bool CheckStack()
    {
        Box[] boxesArray = BoxesStack.ToArray();
        if (requiredStack.Length != BoxesStack.Count)
        {
            return false;
        }

        for (int i = 0; i < requiredStack.Length; i++)
        {
            if (requiredStack[i] != boxesArray[i].Value)
            {
                return false;
            }
        }
        return true;

    }

    public void OnReset()
    {
        if (LastValue != null) 
            Destroy(LastValue.gameObject);
        LastValue = null;
        BoxesStack.Clear();
    }
}
