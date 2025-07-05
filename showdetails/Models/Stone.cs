using ReactiveUI;

public class Stone : ReactiveObject
{
    private double _positionX;
    private double _positionY;
    private double _positionZ;

    public double PositionX
    {
        get => _positionX;
        set => this.RaiseAndSetIfChanged(ref _positionX, value);
    }

    public double PositionY
    {
        get => _positionY;
        set => this.RaiseAndSetIfChanged(ref _positionY, value);
    }

    public double PositionZ
    {
        get => _positionZ;
        set => this.RaiseAndSetIfChanged(ref _positionZ, value);
    }
}