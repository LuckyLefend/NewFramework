//Generate By @ExportViewCode
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainPanel : UIPanel 
{
    public Button btnBattle;

	// Use this for initialization
    void Awake() 
    {
        base.Awake();
        btnBattle = transform.Find("Center/#btn_Battle").GetComponent<Button>();
	}
	
	void OnDestroy () 
    {
        base.OnDestroy();
        btnBattle = null;
	}
}
