
using System;
using System.IO;
using ProtoBuf;
using UnityEngine;
using FirClient.Manager;

public class ObjectPoolTableData {
    private static ObjectPoolTableData instance;
    private ObjectPoolTable tableData;
    private ResourceManager resMgr;

    public static ObjectPoolTableData Instance {
        get {
            if (instance == null)
                instance = new ObjectPoolTableData();
            return instance;
        }
    }

	public ObjectPoolTableData() {
		this.LoadData("Tables/ObjectPoolTable.bytes");
	}

	public void LoadData(string path) {
		var data = resMgr.LoadAsset<TextAsset>(path);
		if (data != null) {
            using (MemoryStream ms = new MemoryStream(data.bytes)) {
		        tableData = Serializer.Deserialize<ObjectPoolTable>(ms);
            }
		}
	}

	public ObjectPoolTableItem[] GetObjectPoolTableItems() {
		return tableData.items.ToArray();
	}
}
