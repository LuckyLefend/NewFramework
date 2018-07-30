using UnityEngine;
using System.Collections;

public enum HUDType {
    HealthBar,
    NameHint,
}

public class HUDObject : GameBehaviour
{
    private static Camera uiCamera;

    public HUDType currType;
    private float offsetY;
    public Transform target;

    Camera UICamera
    {
        get
        {
            if (uiCamera == null)
            {
                uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
            }
            return uiCamera;
        }
    }

    // Use this for initialization

    public void InitHUD(HUDType type, Transform target)
    {
        this.currType = type;
        this.target = target;
        switch (currType)
        {
            case HUDType.NameHint:
                offsetY = 1.5f;
            break;
            case HUDType.HealthBar:
                offsetY = 1f;
            break;
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (target == null || Camera.main == null)
        {
            return;
        }
        var pos = new Vector3(target.position.x, target.position.y + offsetY, target.position.z);
        Vector3 newPos = Camera.main.WorldToScreenPoint(pos);
        transform.position = UICamera.ScreenToWorldPoint(newPos);
    }
}
