using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Bread.Mvc;

public class ObservableConcurrentDictionary<TValue> : ConcurrentDictionary<string, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        where TValue : class, new()
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs changeAction)
    {
        var eh = CollectionChanged;
        if (eh == null) return;

        eh(this, changeAction);

        OnPropertyChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged()
    {
        var eh = PropertyChanged;
        if (eh == null) return;

        // All properties : Keys, Values, Count, IsEmpty
        eh(this, new PropertyChangedEventArgs(null));
    }

    #region Ctors
    public ObservableConcurrentDictionary()
        : base()
    {

    }

    public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<string, TValue>> collection)
        : base(collection)
    {

    }

    public ObservableConcurrentDictionary(IEqualityComparer<string> comparer)
        : base(comparer)
    {

    }

    public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
        : base(concurrencyLevel, capacity)
    {

    }

    public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<string, TValue>> collection, IEqualityComparer<string> comparer)
        : base(collection, comparer)
    {

    }

    public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<string> comparer)
        : base(concurrencyLevel, capacity, comparer)
    {

    }

    public ObservableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<string, TValue>> collection, IEqualityComparer<string> comparer)
        : base(concurrencyLevel, collection, comparer)
    {

    }
    #endregion

    public new void Clear()
    {
        // Clear dictionary
        base.Clear();

        // Raise event
        OnCollectionChanged(changeAction: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public new TValue AddOrUpdate(string key, Func<string, TValue> addValueFactory,
        Func<string, TValue, TValue> updateValueFactory)
    {
        bool isUpdated = false;
        TValue? oldValue = default(TValue);

        TValue value = base.AddOrUpdate(key, addValueFactory, (k, v) => {
            isUpdated = true;
            oldValue = v;
            return updateValueFactory(k, v);
        });

        if (isUpdated) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        return value;
    }

    public new TValue AddOrUpdate(string key, TValue addValue, Func<string, TValue, TValue> updateValueFactory)
    {
        bool isUpdated = false;
        TValue? oldValue = default(TValue);

        TValue value = base.AddOrUpdate(key, addValue, (k, v) => {
            isUpdated = true;
            oldValue = v;
            return updateValueFactory(k, v);
        });

        if (isUpdated) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        return value;
    }

    public new TValue GetOrAdd(string key, Func<string, TValue> addValueFactory)
    {
        bool isAdded = false;

        TValue value = base.GetOrAdd(key, k => {
            isAdded = true;
            return addValueFactory(k);
        });

        if (isAdded) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

        return value;
    }

    public new TValue GetOrAdd(string key, TValue value)
    {
        return GetOrAdd(key, k => value);
    }

    public new bool TryAdd(string key, TValue value)
    {
        bool tryAdd = base.TryAdd(key, value);

        if (tryAdd) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));

        return tryAdd;
    }

    public new bool TryRemove(string key, out TValue? value)
    {
        // Stores tryRemove
        bool tryRemove = base.TryRemove(key, out value);

        // If removed raise event
        if (tryRemove) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));

        return tryRemove;
    }

    public new bool TryUpdate(string key, TValue newValue, TValue comparisonValue)
    {
        // Stores tryUpdate
        bool tryUpdate = base.TryUpdate(key, newValue, comparisonValue);

        if (tryUpdate) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, comparisonValue));

        return tryUpdate;
    }

}