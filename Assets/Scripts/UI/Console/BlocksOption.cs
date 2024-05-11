using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksOption : MonoBehaviour
{
    [SerializeField] BlockSelection _blockSelection;
    [SerializeField] BlockType _type;

    private void OnMouseDown()
    {
        _blockSelection.OnMouse(_type);
        DragNDrop.BlockDragging = false;
    }
}
