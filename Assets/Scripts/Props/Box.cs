using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour, IResettable
{
    [SerializeField] public int Value;
    [SerializeField] public GameObject ValueObj;

    public bool IsCarrying;

    private Vector3 _startPos;
    private int _startSortingOrder; 

    private void Awake()
    {
        _startPos = transform.position;
        _startSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    public void OnReset()
    {
        transform.SetParent(null);
        gameObject.SetActive(true);
        transform.position = _startPos;
        GetComponent<SpriteRenderer>().sortingOrder = _startSortingOrder;
    }
}
