//Generate By @ExportViewCode
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlePanel : UIPanel 
{
    public GameObject objMinimap;
    public Image imgMyposition;
    public Text txtMypos;
    public GameObject prefabMapitem;
    public Button btnBattle;
    public Button btnSkill;

	// Use this for initialization
    void Awake() 
    {
        base.Awake();
        objMinimap = transform.Find("TopBar/Preview/#obj_minimap").gameObject;
        imgMyposition = transform.Find("TopBar/Preview/#img_myposition").GetComponent<Image>();
        txtMypos = transform.Find("TopBar/Preview/#img_myposition/#txt_mypos").GetComponent<Text>();
        prefabMapitem = transform.Find("TopBar/Preview/#prefab_mapitem").gameObject;
        btnBattle = transform.Find("CtrlBar/#btn_Battle").GetComponent<Button>();
        btnSkill = transform.Find("CtrlBar/#btn_Skill").GetComponent<Button>();
	}
	
	void OnDestroy () 
    {
        base.OnDestroy();
        objMinimap = null;
        imgMyposition = null;
        txtMypos = null;
        prefabMapitem = null;
        btnBattle = null;
        btnSkill = null;
	}
}
