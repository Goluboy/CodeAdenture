using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionMenu : MonoBehaviour
{
    [SerializeField] CommandLine _commandLine;

    public void OnMouse(Transform pressedButton)
    {
        _commandLine.GetComponent<BoxCollider2D>().enabled = false;

        pressedButton.localPosition.Normalize();

        _commandLine.OffsetDirection = pressedButton.localPosition / Mathf.Max(Math.Abs(pressedButton.localPosition.x), Math.Abs(pressedButton.localPosition.y));

        if (_commandLine is MoveCommand || _commandLine is UseCommand || _commandLine is PickUpCommand || _commandLine is DropCommand)
        {
            _commandLine.GetComponent<BoxCollider2D>().enabled = true;
            DragNDrop.BlockDragging = false;
        }

        if (transform.parent.TryGetComponent(out IfCommand _ifCommand))
        {
            _ifCommand.BlockSelection.SetActive(true);
        }
        
        gameObject.SetActive(false);
    }
}
