using ReactiveUI;

public class Ball : ReactiveObject
{
    private double _positionX;
    private double _positionY;
    private double _positionZ;
    private bool _isInRange;

    public double PositionX
    {
        get => _positionX;
        set
        {
            this.RaiseAndSetIfChanged(ref _positionX, value);
        }
    }

    public double PositionY
    {
        get => _positionY;
        set
        {
            this.RaiseAndSetIfChanged(ref _positionY, value);
        }
    }

    public double PositionZ
    {
        get => _positionZ;
        set
        {
            this.RaiseAndSetIfChanged(ref _positionZ, value);
        }
    }

    public bool IsInRange
    {
        get => _isInRange;
        set => this.RaiseAndSetIfChanged(ref _isInRange, value);
    }
}