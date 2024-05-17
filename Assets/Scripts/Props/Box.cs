using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour, IResettable
{
    [SerializeField] public int Value;
    [SerializeField] public GameObject ValueObj;
    private Transform _parent;

    public bool IsCarrying;

    private Vector3 _startPos;
    private int _startSortingOrder; 

    private void Awake()
    {
        _parent = transform.parent;
        _startPos = transform.position;
        _startSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    public void OnReset()
    {
        gameObject.SetActive(true);
        IsCarrying = false;
        transform.SetParent(_parent);
        transform.position = _startPos;
        GetComponent<SpriteRenderer>().sortingOrder = _startSortingOrder;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
