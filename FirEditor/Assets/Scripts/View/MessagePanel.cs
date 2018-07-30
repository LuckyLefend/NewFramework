//Generate By @ExportViewCode
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessagePanel : UIPanel 
{
    public GameObject objSubPanel;
    public Image imgSprite;
    public Text txtLabel;
    public Button btnButton;

	// Use this for initialization
    void Awake() 
    {
        base.Awake();
        objSubPanel = transform.Find("#obj_SubPanel").gameObject;
        imgSprite = transform.Find("#obj_SubPanel/#img_Sprite").GetComponent<Image>();
        txtLabel = transform.Find("#obj_SubPanel/#txt_Label").GetComponent<Text>();
        btnButton = transform.Find("#obj_SubPanel/#btn_Button").GetComponent<Button>();
	}
	
	void OnDestroy () 
    {
        base.OnDestroy();
        objSubPanel = null;
        imgSprite = null;
        txtLabel = null;
        btnButton = null;
	}
}
