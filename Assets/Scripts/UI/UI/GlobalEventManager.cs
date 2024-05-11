using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEventManager : MonoBehaviour
{
    public static UnityEvent<bool> OnLeverSwitched = new UnityEvent<bool>();

    public static void LeverSwitched(bool state)
    {
        OnLeverSwitched.Invoke(state);
    }
}
