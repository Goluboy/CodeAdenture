using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueFalseButton : MonoBehaviour
{
    [SerializeField] private bool _value;
    [SerializeField] private TrueFalseChooser _chooser;
    private void OnMouseDown()
    {
        _chooser.OnMouse(_value);
    }
}
