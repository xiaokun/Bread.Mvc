namespace Bread.Mvc;

public class RangDics<T> : ObservableConcurrentDictionary<T>
    where T : class, new()
{

}
