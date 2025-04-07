namespace PatsyLibrary.Entities;

public class GenericState
{
    public bool IsActive { get; private set; }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}