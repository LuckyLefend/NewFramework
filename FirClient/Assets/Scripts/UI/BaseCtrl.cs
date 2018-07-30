
public class BaseCtrl : BaseBehaviour
{
    public bool isFrameUpdate = false;
    protected string ResName { get; set; }

    public virtual void OnAwake() { }

    public virtual void OnFixedUpdate(float deltaTime) { }

    public virtual void OnUpdate(float deltaTime) { }

    public virtual void OnLateUpdate() { }

    public virtual void OnDestroy() { }
}
