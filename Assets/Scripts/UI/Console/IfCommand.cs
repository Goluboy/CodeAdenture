using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IfCommand : CommandLine
{
    [SerializeField] private GameObject _directionMenu;
    [SerializeField] public GameObject BlockSelection;
    [SerializeField] public GameObject TrueFalseChooser;

    public bool IsTrue { get; set; }

    private void Awake()
    {
        _directionMenu.SetActive(false);
        BlockSelection.SetActive(false);
        TrueFalseChooser.SetActive(false);
    }

    public override void OnSetup()
    {
        DragNDrop.BlockDragging = true;
        GetComponent<BoxCollider2D>().enabled = false;
        _directionMenu.SetActive(true);
    }

    public override void Execute(Unit unit)
    {
        unit.ExecuteIf(this);

    }
}
