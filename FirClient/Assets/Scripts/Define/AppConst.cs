
public class AppConst {
    public static bool DebugMode = false;                       //调试模式-用于内部测试
    public static bool UpdateMode = false;                     //调试模式
    public static bool LogMode = true;                         //日志模式

    public static float SyncRate = 10;
    public static int TimerInterval = 1;
    public static int GameFrameRate = 30;                       //游戏帧频

    public static string UserId = string.Empty;                 //用户ID
    public static string AppName = "TankSky";                   //应用程序名称
    public static string AppPrefix = AppName + "_";             //应用程序前缀
    public static string ExtName = ".unity3d";                  //素材扩展名
    public static string AssetDirname = "StreamingAssets";      //素材目录 

    public static string WebUrl = "http://localhost/res/";      //测试更新地址
    public static ushort SocketPort = 15940;                        //Socket服务器端口
    public static string SocketAddress = "127.0.0.1";       //Socket服务器地址

    public const string TerrainLayer = "Terrain";
    public const string GameplayLayer = "Gameplay";
    public const string FoodLayer = "FoodLayer";

    public const string FoodTag = "FoodObject";
    public const string TankAtlasPath = "Atlas/Objects/Tanks/Tanks";
    public const string AnimalAtlasPath = "Atlas/Objects/Animals/Animals";

    public const int NetMessagePoolMax = 100;
}