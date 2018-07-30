using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ProtoBuf;

[ProtoBuf.ProtoContract]
public class ObjectPoolTable
{
    [ProtoBuf.ProtoMember(1)]
    public string name { get; set; }

    private List<ObjectPoolTableItem> _items = new List<ObjectPoolTableItem>();
    [ProtoBuf.ProtoMember(2)]
    public List<ObjectPoolTableItem> items { get { return _items; } }
}

[ProtoBuf.ProtoContract]
public class ObjectPoolTableItem
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; } 

    [ProtoBuf.ProtoMember(2)]
    public string name { get; set; } 

    [ProtoBuf.ProtoMember(3)]
    public string path { get; set; } 

    [ProtoBuf.ProtoMember(4)]
    public int count { get; set; } 

    [ProtoBuf.ProtoMember(5)]
    public int fixedsize { get; set; }
}
        