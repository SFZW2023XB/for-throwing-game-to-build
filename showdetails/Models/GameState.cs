using System;
using ReactiveUI;

public class GameState : ReactiveObject
{
    private int _itemCount;
    private double _targetPositionX;
    private double _targetPositionY;
    private double _targetPositionZ;
    private bool _isTargetInRange;
    private double _minRange;
    private double _maxRange;

    public int ItemCount
    {
        get => _itemCount;
        set => this.RaiseAndSetIfChanged(ref _itemCount, value);
    }

    public double TargetPositionX
    {
        get => _targetPositionX;
        set
        {
            this.RaiseAndSetIfChanged(ref _targetPositionX, value);
            UpdateTargetStatus();
        }
    }

    public double TargetPositionY
    {
        get => _targetPositionY;
        set
        {
            this.RaiseAndSetIfChanged(ref _targetPositionY, value);
            UpdateTargetStatus();
        }
    }

    public double TargetPositionZ
    {
        get => _targetPositionZ;
        set
        {
            this.RaiseAndSetIfChanged(ref _targetPositionZ, value);
            UpdateTargetStatus();
        }
    }

    public bool IsTargetInRange
    {
        get => _isTargetInRange;
        private set => this.RaiseAndSetIfChanged(ref _isTargetInRange, value);
    }

    public double MinRange
    {
        get => _minRange;
        set => this.RaiseAndSetIfChanged(ref _minRange, value);
    }

    public double MaxRange
    {
        get => _maxRange;
        set => this.RaiseAndSetIfChanged(ref _maxRange, value);
    }

    private void UpdateTargetStatus()
    {
        var distance = Math.Sqrt(
            _targetPositionX * _targetPositionX +
            _targetPositionY * _targetPositionY +
            _targetPositionZ * _targetPositionZ);
        IsTargetInRange = distance >= _minRange && distance <= _maxRange;
    }

    public double PlayerCenterX => 35; // 玩家圆心X坐标
    public double PlayerCenterY => 100; // 玩家圆心Y坐标

    public double Distance => Math.Sqrt(
        _targetPositionX * _targetPositionX +
        _targetPositionY * _targetPositionY +
        _targetPositionZ * _targetPositionZ);

    private Ball _ball;
    private Stone _stone;
    private int _stoneCount;

    public Ball Ball
    {
        get => _ball;
        set => this.RaiseAndSetIfChanged(ref _ball, value);
    }

    public Stone Stone
    {
        get => _stone;
        set => this.RaiseAndSetIfChanged(ref _stone, value);
    }

    public int StoneCount
    {
        get => _stoneCount;
        set => this.RaiseAndSetIfChanged(ref _stoneCount, value);
    }
}