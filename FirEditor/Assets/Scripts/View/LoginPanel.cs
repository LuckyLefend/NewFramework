//Generate By @ExportViewCode
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginPanel : UIPanel 
{
    public Button btnButton;

	// Use this for initialization
    void Awake() 
    {
        base.Awake();
        btnButton = transform.Find("#btn_Button").GetComponent<Button>();
	}
	
	void OnDestroy () 
    {
        base.OnDestroy();
        btnButton = null;
	}
}
