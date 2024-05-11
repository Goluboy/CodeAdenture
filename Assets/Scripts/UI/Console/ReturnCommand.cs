using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCommand : CommandLine
{
    [SerializeField] public GameObject Waypoint;

    private void Awake()
    {
        Waypoint.SetActive(false);
    }
    private void OnDestroy()
    {
        Console.Instance.Remove(this);
        Destroy(gameObject);
    }

    public override void Execute(Unit unit)
    {
        unit.ExecuteReturn(this);
    }
    public override void OnSetup()
    {
        Waypoint.SetActive(true);
        Console.Instance.Add(Waypoint.GetComponent<CommandLine>());
    }
}
