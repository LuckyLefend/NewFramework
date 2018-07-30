
using UnityEngine;
using System.Collections;

public class TableManager : MonoBehaviour {
    
    public ObjectPoolTableItem[] GetObjectPoolTableItems() {
        return ObjectPoolTableData.Instance.GetObjectPoolTableItems();
    }

}

