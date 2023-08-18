using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Bread.Mvc;

public class RangeList<T> : ObservableCollection<T>
{
    public RangeList() : base() { }

    public RangeList(IEnumerable<T> collection) : base(collection) { }

    public RangeList(List<T> list) : base(list) { }

    public void AddRange(IEnumerable<T> range)
    {
        if (range == null) return;
        if (range.Count() == 0) return;

        CheckReentrancy();

        foreach (var item in range) {
            Items.Add(item);
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    }

    public void Reset(IEnumerable<T> range)
    {
        if (range == null) return;

        CheckReentrancy();

        Items.Clear();

        if(range.Count() > 0) {
            foreach (var item in range) {
                Items.Add(item);
            }
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void ForEach(Action<T> action)
    {
        foreach (var item in this) {
            action(item);
        }
    }

    public List<T> Map(Func<T, bool> predict)
    {
        var list = new List<T>();
        foreach (var item in this) {
            if (predict(item)) {
                list.Add(item);
            }
        }
        return list;
    }
}