using UnityEngine;

// 定義輸入策略介面
public interface ISnekInputStrategy
{
    Vector2 GetMoveDirection();
}

// PC 鍵盤輸入
public class PCInputStrategy : ISnekInputStrategy
{
    public Vector2 GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector2(h, v);
    }
}

// 手機 Joystick 輸入
public class MobileInputStrategy : ISnekInputStrategy
{
    private Joystick _joystick;
    public MobileInputStrategy(Joystick joystick)
    {
        _joystick = joystick;
    }

    public Vector2 GetMoveDirection()
    {
        return _joystick != null ? _joystick.Direction : Vector2.zero;
    }
}

public class SnekInputController
{
    private ISnekInputStrategy _strategy;

    public void SetStrategy(ISnekInputStrategy strategy)
    {
        _strategy = strategy;
    }

    public Vector2 GetMoveDirection()
    {
        return _strategy != null ? _strategy.GetMoveDirection() : Vector2.zero;
    }
}
