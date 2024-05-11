using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : CommandLine
{
    [SerializeField] GameObject returnCommand;

    public override void OnSetup()
    { 

    }

    public override void Execute(Unit unit)
    {

    }

    private void OnDestroy()
    {
        Console.Instance.Remove(this);
        if (!gameObject.name.Contains("(Clone)"))
        {
            Destroy(returnCommand);
        }
    }
}
