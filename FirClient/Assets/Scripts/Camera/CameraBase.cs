using UnityEngine;
using System.Collections;

public class CameraBase : GameBehaviour {

	// Use this for initialization
	protected override void OnAwake()
    {
        var camera = transform.Find("Camera");
        if (camera != null)
        {
            camera.GetComponent<Camera>().eventMask = 0;
        }
        base.OnAwake();
	}
}
