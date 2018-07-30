using System;
using System.Collections;
using System.Collections.Generic;
using FirClient.Ctrl;
using FirClient.Manager;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    void Update()
    {
        OnUpdate();
    }

    void LateUpdate()
    {
        OnLateUpdate();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        OnTriggerCollide2D(coll);
    }

    void OnDestroy()
    {
        OnDestroyMe();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnDestroyMe() { }
    protected virtual void OnTriggerCollide2D(Collider2D coll) { }
}
