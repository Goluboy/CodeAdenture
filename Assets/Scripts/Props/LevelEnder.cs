using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEnder : MonoBehaviour, IResettable
{
    [SerializeField] public GameObject Panel;

    private ConditionOfEndOfLevel[] _conditions;
    public void Awake()
    {
        _conditions = FindObjectsByType(typeof(ConditionOfEndOfLevel), FindObjectsSortMode.None)
                                                        .Select(x => x as ConditionOfEndOfLevel).ToArray();
    }

    public bool IsLevelEnded()
    {
        Array.ForEach(_conditions, x => x.CalculateStatement());
        if (_conditions.All(x => x.IsTrue))
        {
            Panel.SetActive(true);
            DragNDrop.BlockDragging = true;
            return true;
        }
        return false;
    }
}
