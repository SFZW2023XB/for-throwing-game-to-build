using ReactiveUI;

public class StatusLight : ReactiveObject
{
    private bool _isActive;
    private string _text = string.Empty;
    private string _description = string.Empty;

    public bool IsActive
    {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }
}