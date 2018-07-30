using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public abstract class NetworkObject : BaseBehaviour
{
    public int Identity { get; set; }
    public bool IsOwner { get; private set; }
    public long NetworkId { get; private set; }

    protected bool hasDirtyFields = false;
    protected NetDataWriter writer = new NetDataWriter();
    protected abstract NetDataWriter SerializeDirtyFields();

    /// <summary>
    /// Used for when any field event occurs, will pass the target field as a param
    /// </summary>
    /// <typeparam name="T">The acceptable network serializable data type</typeparam>
    /// <param name="field">The target field related to this event</param>
    /// <param name="timestep">The timestep for when this event happens</param>
    public delegate void FieldEvent<T>(T field, ulong timestep);

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Initialize(long createId, bool isOwner = false)
    {
        NetworkId = createId;
        IsOwner = isOwner;     //设置自己的身份
    }

    /// <summary>
    /// 执行心跳
    /// </summary>
    public void HeartBeat(ulong timeStep)
    {
        if (!IsOwner || !hasDirtyFields)
        {
            return;
        }
        var buffer = SerializeDirtyFields();
        networkMgr.SendData(Protocal.SerializeFields, buffer);
        hasDirtyFields = false;
    }

    public virtual void InterpolateUpdate() { }
}
