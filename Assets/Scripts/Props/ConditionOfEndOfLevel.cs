using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ConditionOfEndOfLevel : MonoBehaviour, IResettable
{
    [SerializeField] private bool _isTrigger;
    [SerializeField] private int _requiredBox;
    public bool IsTrue { get; set; } = false;

    public void CalculateStatement()
    {
        if (_requiredBox != 0)
        {
            var result = Physics2D.RaycastAll(transform.position, Vector2.up, 0.01f)
                .Select(x => x.collider.gameObject.GetComponent<Box>())
                .FirstOrDefault();
            IsTrue = result != null ? result.Value == _requiredBox : false;
            return;
        }

        if (TryGetComponent(out Stack stack))
        {
            IsTrue = stack.CheckStack();
            return;
        }

        if (_isTrigger)
        {
            var result = Physics2D.RaycastAll(transform.position, Vector2.up,0.01f);
            IsTrue = result.Count() > 0 && result.Any(x => x.collider.TryGetComponent(out Unit unit));
            return;
        }

        if (TryGetComponent(out Lever lever))
        {
            IsTrue = lever.IsActive;
            return;
        }

        if (TryGetComponent(out Door door))
        {
            IsTrue = door.State;
            return;
        }

        if (TryGetComponent(out BoxCollider2D box1))
        {
            Collider2D[] result = new Collider2D[64];
            box1.OverlapCollider(new ContactFilter2D(), result);
            IsTrue = result.Length > 0 ? result.Any(x => x.TryGetComponent(out Unit unit)) : false;
            return;
        }
        IsTrue = false;
        Debug.Log(IsTrue);
    }

    public void OnReset()
    {
        IsTrue = false;
    }
}
