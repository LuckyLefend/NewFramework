using UnityEngine;

public interface IGameInput
{

    bool IsClickDown { get; }

    bool IsClickUp { get; }
    bool IsClicking { get; }

    bool HasTouch { get; }

    Vector3 MousePosition { get; }
    int TouchCount { get; }
}

public class DesktopGameInput : IGameInput
{
    public bool IsClickDown
    {
        get
        {
            return Input.GetMouseButtonDown(0);
        }
    }

    public bool IsClickUp
    {
        get
        {
            return Input.GetMouseButtonUp(0);
        }
    }

    public bool IsClicking
    {
        get
        {
            return Input.GetMouseButtonDown(0);
        }
    }


    public Vector3 MousePosition
    {
        get { return Input.mousePosition; }
    }

    public bool HasTouch { get { return true; } }
    public int TouchCount { get { return 1; } }
}

public class SingleTouchGameInput : IGameInput
{
    public bool IsClickDown
    {
        get
        {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }

    public bool IsClickUp
    {
        get
        {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
        }
    }

    public bool IsClicking
    {
        get
        {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary;
        }
    }

    public Vector3 MousePosition
    {
        get
        {
            if (Input.touchCount == 1)
            {
                return Input.GetTouch(0).position;
            }
            else
            {
                return Input.mousePosition;
            }
        }
    }

    public bool HasTouch
    {
        get
        {
            return Input.touchCount > 0;
        }
    }
    public int TouchCount
    {
        get
        {
            return Input.touchCount;
        }
    }
}
/// <summary>
/// 手势和触摸的判断【游戏控制】
/// </summary>
public static class GameControl
{
    public static IGameInput Input
    {
        get
        {
            if (_gameInput == null)
            {
                Initialize();
            }
            return _gameInput;
        }
    }

    private static void Initialize()
    {
        if (PlatformUtil.IsTouchDevice)
        {
            _gameInput = new SingleTouchGameInput();
        }
        else
        {
            _gameInput = new DesktopGameInput();
        }
    }

    private static IGameInput _gameInput;
}