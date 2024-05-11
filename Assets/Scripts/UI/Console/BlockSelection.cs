using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelection : MonoBehaviour
{
    [SerializeField] private CommandLine _commandLine;

    public void OnMouse(BlockType blockType)
    {
        _commandLine.BlockType = blockType;
        
        if (transform.parent.TryGetComponent(out IfCommand _ifCommand))
        {
            _ifCommand.TrueFalseChooser.SetActive(true);
        }
        else
        {
            _commandLine.GetComponent<BoxCollider2D>().enabled = true;
        }

        gameObject.SetActive(false);
    }
}
