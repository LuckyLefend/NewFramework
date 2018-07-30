
/// <summary>
/// 网络协议文件
/// </summary>
public enum Protocal : ushort
{
    Default = 1000,                 //缺省消息
    Disconnect = 1002,              //异常掉线
    SerializeFields = 1003,         //序列化字段

    ReqUserInfo = 1004,             //请求用户信息
    ReqPlayers = 1005,              //连接服务器
    ReqMapInfo = 1006,              //请求地图信息
	ReqNpcInfo = 1007,              //请求NPC信息
    ReqAttackNpc = 1008,            //请求杀掉怪物
    RetNewPlayer = 1009,            //服务器创建新角色
    ReqOpenFire = 1010,             //请求开火
    ReqHitTarget = 1011,            //请求命中目标
}

public enum NpcType : ushort
{
    Player = 0,
    Monster = 1,
}