using UnityEngine;

public interface IObjectView
{
    void OnAwake();
    void OnStart();
    void OnUpdate();
    void OnDispose();
    void OnDestroy();
    void OnTriggerEnter2D(Collider2D coll);
}
