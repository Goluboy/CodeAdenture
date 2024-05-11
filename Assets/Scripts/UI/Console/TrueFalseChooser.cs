using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueFalseChooser : MonoBehaviour
{
    [SerializeField] private IfCommand _IfLine;
    public void OnMouse(bool value)
    {
        _IfLine.IsTrue = value;

        _IfLine.GetComponent<BoxCollider2D>().enabled = true;
        
        gameObject.SetActive(false);
    }
}
