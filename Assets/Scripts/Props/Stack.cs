using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stack : MonoBehaviour, IResettable
{
    [SerializeField] private int[] requiredStack;
    public Transform LastValue { get; set; }

    static public readonly Stack<Box> BoxesStack = new();

    public static void Add(Box box)
    {
        BoxesStack.Push(box);
        
        foreach (var stack in FindObjectsByType(typeof(Stack), FindObjectsSortMode.None).Select(x => x as Stack).ToArray())
        {
            if (stack.LastValue != null)
                Destroy(stack.LastValue.gameObject);
            stack.LastValue = Instantiate(box.ValueObj.transform);
            stack.LastValue.SetParent(stack.transform);
            stack.LastValue.transform.localPosition = new Vector3(0, .135f, 0);
        }

    }
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
