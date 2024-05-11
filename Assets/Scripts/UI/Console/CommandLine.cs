using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandLine : DragNDrop
{
    public float CommandLevel { get; set; }
    public Vector3 OffsetDirection { get; set; }
    public BlockType BlockType { get; set; }

    public List<IfCommand> IfList { get; set; }

    public virtual void OnSetup() { }

    public virtual void Execute(Unit unit) { }

    private void OnDestroy()
    {
        Console.CommandLines.Remove(this);
    }
}
