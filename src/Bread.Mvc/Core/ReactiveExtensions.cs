using System.ComponentModel;

namespace Bread.Mvc;

public static class ReactiveExtensions
{
    /// <summary>
    /// Contains a list of subscriptions Subscriptions[Publisher][PropertyName].List of subscriber-action pairs.
    /// </summary>
    private static readonly Dictionary<INotifyPropertyChanged, SubscriptionSet> Subscriptions
        = new Dictionary<INotifyPropertyChanged, SubscriptionSet>();

    private static readonly object SyncLock = new object();


    public static void UnWatch(this INotifyPropertyChanged publisher, Action callback, params string[] propertyNames)
    {
        lock (SyncLock) {
            if (Subscriptions.ContainsKey(publisher) == false) return;

            var set = Subscriptions[publisher];
            foreach (var propertyName in propertyNames) {
                if (set.ContainsKey(propertyName) == false) continue;
                var callbackList = set[propertyName];
                callbackList.Remove(callback);
            }
        }
    }

    public static void UnWatch(this INotifyPropertyChanged publisher, string name, Action callback)
    {
        lock (SyncLock) {
            if (Subscriptions.ContainsKey(publisher) == false) return;
            var set = Subscriptions[publisher];
            if (set.ContainsKey(name) == false) return;
            var callbackList = set[name];
            callbackList.Remove(callback);
        }
    }


    /// <summary>
    /// Specifies a callback when properties change.
    /// </summary>
    /// <param name="publisher">The publisher.</param>
    /// <param name="callback">The callback.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void Watch(this INotifyPropertyChanged publisher, Action callback, params string[] propertyNames)
    {
        var bindPropertyChanged = false;

        lock (SyncLock) {
            // Create the subscription set for the publisher if it does not exist.
            if (Subscriptions.ContainsKey(publisher) == false) {
                Subscriptions[publisher] = new SubscriptionSet();

                // if it did not exist before, we need to bind to the
                // PropertyChanged event of the publisher.
                bindPropertyChanged = true;
            }

            foreach (var propertyName in propertyNames) {
                // Create the set of callback references for the publisher's property if it does not exist.
                if (Subscriptions[publisher].ContainsKey(propertyName) == false)
                    Subscriptions[publisher][propertyName] = new CallbackList();

                // Add the callback for the publisher's property changed
                Subscriptions[publisher][propertyName].Add(callback);
            }
        }

        try {
            // Make an initial call
            if (IoC.MainThread != null && IoC.MainThread.IsInvokeRequired)
                IoC.MainThread.Invoke(callback);
            else callback();
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }

        if (bindPropertyChanged == false) return;

        // Finally, bind to property changed
        publisher.PropertyChanged += (s, e) => {
            if (string.IsNullOrWhiteSpace(e.PropertyName)) return;

            CallbackList callbacks = new();

            lock (SyncLock) {
                if (Subscriptions.ContainsKey(publisher) == false) return;
                if (Subscriptions[publisher].ContainsKey(e.PropertyName) == false) return;

                // Get the list of alive subscriptions for this property name
                var propertyCallbacks = Subscriptions[publisher][e.PropertyName];
                if (propertyCallbacks.Count == 0) return;
                foreach (var action in propertyCallbacks) {
                    callbacks.Add(action);
                }
            }

            if (callbacks.Count == 0) return;

            // Call the subscription's callbacks
            foreach (var action in callbacks) {
                try {
                    if (IoC.MainThread != null && IoC.MainThread.IsInvokeRequired)
                        IoC.MainThread.Invoke(action);
                    else action();
                }
                catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        };
    }

    /// <summary>
    /// Specifies a callback when properties change.
    /// </summary>
    /// <param name="publisher">The publisher.</param>
    /// <param name="callback">The callback.</param>
    /// <param name="propertyNames">The property names.</param>
    public static void Watch(this INotifyPropertyChanged publisher, string propertyName, Action callback)
    {
        var bindPropertyChanged = false;

        lock (SyncLock) {
            // Create the subscription set for the publisher if it does not exist.
            if (Subscriptions.ContainsKey(publisher) == false) {
                Subscriptions[publisher] = new SubscriptionSet();

                // if it did not exist before, we need to bind to the
                // PropertyChanged event of the publisher.
                bindPropertyChanged = true;
            }

            // Create the set of callback references for the publisher's property if it does not exist.
            if (Subscriptions[publisher].ContainsKey(propertyName) == false)
                Subscriptions[publisher][propertyName] = new CallbackList();

            // Add the callback for the publisher's property changed
            Subscriptions[publisher][propertyName].Add(callback);
        }

        try {
            // Make an initial call
            if (IoC.MainThread != null && IoC.MainThread.IsInvokeRequired)
                IoC.MainThread.Invoke(callback);
            else callback();
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }

        if (bindPropertyChanged == false) return;

        // Finally, bind to property changed
        publisher.PropertyChanged += (s, e) => {
            if (string.IsNullOrWhiteSpace(e.PropertyName)) return;

            CallbackList callbacks = new();

            lock (SyncLock) {
                if (Subscriptions.ContainsKey(publisher) == false) return;
                if (Subscriptions[publisher].ContainsKey(e.PropertyName) == false) return;

                // Get the list of alive subscriptions for this property name
                var propertyCallbacks = Subscriptions[publisher][e.PropertyName];
                if (propertyCallbacks.Count == 0) return;
                foreach (var action in propertyCallbacks) {
                    callbacks.Add(action);
                }
            }

            if (callbacks.Count == 0) return;

            // Call the subscription's callbacks
            foreach (var action in callbacks) {
                try {
                    if (IoC.MainThread != null && IoC.MainThread.IsInvokeRequired)
                        IoC.MainThread.Invoke(action);
                    else action();
                }
                catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        };
    }

    internal sealed class SubscriptionSet : Dictionary<string, CallbackList> { }

    internal sealed class CallbackList : List<Action>
    {
        public CallbackList() : base(32)
        {
            // placeholder
        }
    }
}
