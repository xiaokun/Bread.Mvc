namespace Bread.Utility.IO;

public class ApplicationInstance : IDisposable
{
    public static ApplicationInstance Instance { get; private set; } = new ApplicationInstance();

    protected Mutex? _mutex = null;
    private bool disposedValue = false;

    private ApplicationInstance()
    {
    }

    private bool CheckRunning(Guid id)
    {
        if (_mutex != null) {
            return false;
        }

        try {
            var mutextId = $"Global\\{{{id.ToString()}}}";
            var mutex = new Mutex(true, mutextId, out bool canCreate);
            //Log.Info($"create app instance mutex : {mutextId}:{canCreate}");
            Log.Flush();

            if (canCreate) {
                _mutex = mutex;
            }
            else {
                mutex?.Dispose();
            }

            return !canCreate;
        }
        catch (Exception) {
            return true;
        }
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue) {
            if (disposing) {
            }

            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            disposedValue = true;
        }
    }

    ~ApplicationInstance()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
