using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable] public class MehEvent : UnityEvent<int> { }

public class ReferenceTest : MonoBehaviour
{
    public int intTest = 1;
    public GameObject[] goReference;
    public Collider colliderReference;
    public GameObject prefab;
    public MehEvent MehEvent;
}
