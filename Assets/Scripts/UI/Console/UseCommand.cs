using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCommand : CommandLine
{
    [SerializeField] private GameObject _directionMenu;

    private void Awake()
    {
        _directionMenu.SetActive(false);
    }

    public override void OnSetup()
    {
        DragNDrop.BlockDragging = true;
        GetComponent<BoxCollider2D>().enabled = false;
        _directionMenu.SetActive(true);
    }

    public override void Execute(Unit unit)
    {
        unit.ExecuteUse(this);
    }
}
