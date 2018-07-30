using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MapView : GameBehaviour {
    private Transform trans;
    private RectTransform content;

    void Awake() {
        trans = transform;
        content = trans.Find("Content") as RectTransform;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetUpdate() {
        float y = content.anchoredPosition.y;
        float v = y - 200;
        DOTween.To(OnChange, y, v, 0.3f);
    }

    void OnChange(float v) {
        int index = (int)(Mathf.Abs(v) / 1136) + 1;
        //io.mapMgr.CreateMapItem(index);
        content.anchoredPosition = new Vector2(0, v);
    }
}
