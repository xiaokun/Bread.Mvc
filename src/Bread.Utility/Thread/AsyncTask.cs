namespace Bread.Utility.Threading;


public class AsyncTask
{
    /// <summary>
    /// 内部使用Sleep延迟任务的执行
    /// </summary>
    /// <param name="milliseconds">毫秒</param>
    /// <param name="action"></param>
    public static void Delay(int milliseconds, Action action)
    {
        ThreadPool.QueueUserWorkItem(new WaitCallback((object? O) => {
            if (milliseconds > 0) {
                Thread.Sleep(milliseconds);
            }
            action();
        }));
    }

    /// <summary>
    /// 异步动作，内部会异步invoke到control所在的界面线程
    /// </summary>
    /// <typeparam name="T">界面控件类型</typeparam>
    /// <param name="milliseconds">延迟时间</param>
    /// <param name="obj">参数</param>
    /// <param name="action">执行动作</param>
    public static void Delay<T>(int milliseconds, T param, Action<T> action)
    {
        ThreadPool.QueueUserWorkItem(new WaitCallback((object? O) => {
            if (milliseconds > 0) {
                Thread.Sleep(milliseconds);
            }
            action(param);
        }));
    }
}
