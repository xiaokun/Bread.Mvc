using System.ComponentModel;

namespace Bread.Mvc;

public abstract class Model : INotifyPropertyChanged
{
    public bool IsDataChanged { get; set; } = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string name)
    {
        IsDataChanged = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public virtual void LoadFrom(Config config) { }

    public virtual void SaveTo(Config config) { }

    public virtual void Clear() { }
}
