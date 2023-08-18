# Bread.Mvc
[![NuGet Status](https://img.shields.io/nuget/v/Bread.Mvc.svg)](https://www.nuget.org/packages/Bread.Mvc)

Bread.Mvc 是一款完全支持 Native AOT 的 MVC 框架。

## 1. Ioc 容器

Bread.Mvc 框架重度使用 [ZeroIoC](https://github.com/byme8/ZeroIoC) 作为 IoC 容器。ZeroIoc 是一款摒弃了反射的 IoC 容器，具有极高的性能并且完全兼容AOT。为了支持 .net 7, 我对 ZeroIoc 代码做了零星修改，重新发布在 [Bread.ZeroIoc](https://www.nuget.org/packages/Bread.ZeroIoC/)。

### 1.1 服务注册
由于不能使用反射，ZeroIoc 使用 SourceGenerator 在编译期生成注入代码，这个机制依赖 ZeroIoCContainer 触发。
ZeroIoCContainer 是部分类，并实现了 Bootstrap 方法，用户的注册代码必须放在这个方法中才会被自动生成。
您可以将服务注册类放在项目的不同地方，或者放在不同的项目中。
请参见以下代码实现自己的注册类：

```C#
using Bread.Mvc;
using ZeroIoC;

namespace XDoc.Avalonia;

public partial class SessionContainer : ZeroIoCContainer
{
    protected override void Bootstrap(IZeroIoCContainerBootstrapper builder)
    {
        builder.AddSingleton<IAlertBox, AlertPacker>();
        builder.AddSingleton<IMessageBox, MessagePacker>();
        builder.AddSingleton<IUIDispatcher, MainThreadDispatcher>();

        builder.AddSingleton<Session>();
        builder.AddSingleton<SessionController>();
    }
}
```

### 1.2 IoC 容器初始化

需要使用 IoC.Init 方法初始化 IoC 容器，一般推荐在程序启动之前完成服务注册和 IoC 容器的初始化操作。请参见如下代码：

```C#
using Bread.Mvc;

IoC.Init(new XDocContainer(), new SessionContainer());
```

为了帮助理解，可以查看 IoC.Init 函数的源代码，大致如下所示：

```C#
public static void Init(params ZeroIoCContainer[] containers)
{
    foreach (var container in containers) {
        Resolver.Merge(container);
    }

    Resolver.End();
}
```

## 2. MVC 架构

### 2.1 Command

***声明：***  

用户的输入被抽象为Command，Command 连接用户界面和 Controller。请参见如下代码：

```C#
public static class AppCommands
{
    public static Command Load { get; } = new(nameof(AppCommands), nameof(Load));

    public static Command Save { get; } = new(nameof(AppCommands), nameof(Save));

    public static AsyncCommand<string, string> ImportAsync { get; } = new(nameof(AppCommands), nameof(ImportAsync));

    public static Command Delete { get; } = new(nameof(AppCommands), nameof(Delete));
}
```

有两种类型的 Command， 普通 Command 和 AsyncCommand。如您所见， AsyncCommand 支持异步操作。  

***使用：***

一般我们我在 xaml 或 axaml 的后缀代码文件中使用 Command，表示响应用户的输入。

```C#
private void UiListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (e.AddedItems == null || e.AddedItems.Count == 0) return;
    if (e.AddedItems[0] is not ImageItemViewModel img) return;
    if (img == _session.CurrentImage) return;

    SessionCommands.SwitchImage.Execution(img);
}

private void UiBtnRight_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
{
    SessionCommands.NextImage.Execution();
}

private void UiBtnLeft_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
{
    SessionCommands.PreviousImage.Execution();
}

```

### 2.2 Controller

Controller 是处理业务逻辑的地方。在上面 IoC 注册的例子中，SessionController 就是一个我们自己定义的 Controller 类。
Controller 子类能自动注入已注册过的服务（Model）。

```C#
public class SessionController : Controller, IDisposable
{
    readonly AppModel _app;
    readonly Session _session;
    readonly ProjectModel _prj;

    SerialTaskQueue<Doc?> _loadTask = new();

    public SessionController(AppModel app, Session session, ProjectModel prj)
    {
        _app = app;
        _prj = prj;
        _session = session;

        SessionCommands.SwitchData.Event += SwitchData_Event;
        SessionCommands.SwitchDoc.Event += SwitchDoc_Event;
        SessionCommands.SwitchImage.Event += SwitchImage_Event;

        SessionCommands.NextImage.Event += NextImage_Event;
        SessionCommands.PreviousImage.Event += PreviousImage_Event;

        SessionCommands.SaveDoc.Event += SaveDoc_Event;
        SessionCommands.NextDoc.Event += NextDoc_Event;

        _loadTask.Start();

        _prj.Loaded += _prj_Loaded;
    }
}
```

有以下几点需要特别注意：

- 必须继承自 Controller 类才会被 Ioc 自动实例化（避免没有显式获取时 Command 的 Event 事件不被挂接）；
- 必须使用 AddSingleton 注册，防止事件被多次挂接；
- 构造函数中的参数 Model 类也必须在 ZeroIoCContainer 中注册才会自动注入；
- 相关 Command 的事件处理函数必须写在构造函数中；
- Command 可挂接在不同的 Controller 中，但是不保证执行顺序；
- SessionController 实现了 IDisposable 接口，但是无需我们显式调用 Dispose 方法。请在应用程序结束时调用 IoC.Dispose() 清理。

### 2.3 Model

Model 连结业务逻辑和用户界面。用户输入（鼠标、键盘、触屏动作等）通过 Command 触发 Controller 中的业务流程，
在 Controller 中更新 Model 的属性值，这些修改操作又立即触发用户界面的刷新。
逻辑是闭环的：UI->Command->Controller->Model->UI。   

***定义：***

源代码中对 Model 的定义相当简单，只是声明必须要实现 INotifyPropertyChanged 接口。

```C#
public abstract class Model : INotifyPropertyChanged
{
    public bool IsDataChanged { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string name)
    {
        IsDataChanged = true;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
```

***声明：***  

一般我们将 Model 和相关的 Controller 声明在一个类库中，并用 internal set 修饰以防止不必要的外部修改。建议您也只在对应的 Controller 中修改 Model 的属性。不加限制的修改 Model 对象的属性，只会带来更多的屎山代码。

```C#
public class ProjectModel : Model
{
    public int Volume { get; internal set; } = 3;

    public RangeList<Volume> Volumes { get; } = new();

    public string NewDocFolder { get; internal set; } = string.Empty;

    public RangeList<NewDoc> NewDocs { get; } = new();

    public ProjectModel()
    {
    }
}
```

推荐使用 [PropertyChanged.Fody](https://www.nuget.org/packages/PropertyChanged.Fody) 自动实现 INotifyPropertyChanged 接口。  
事实上因为实现了 INotifyPropertyChanged 接口， 您可以在xaml直接绑定 Model 中的属性。

***使用：***  

我们使用 Watch 函数监听 Model 属性的变化，Watch 和 UnWatch 函数的原型如下：

```C#
public static void Watch(this INotifyPropertyChanged publisher, string propertyName, Action callback);
public static void Watch(this INotifyPropertyChanged publisher, Action callback, params string[] propertyNames);
public static void UnWatch(this INotifyPropertyChanged publisher, string name, Action callback);
public static void UnWatch(this INotifyPropertyChanged publisher, Action callback, params string[] propertyNames);
```

通常我们在 Window 或者 UserControl 的 Load 代码中完成依赖注入和属性监听。
你可以一次监听一个属性，或同时监听多个属性并在一个 Action 中响应这些属性的变化。

请记住，监听的目的是为了响应业务变化以同步更新用户界面。

```C#
private void ImageSlider_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
{
    if (Design.IsDesignMode) return;

    _session = IoC.Get<Session>();  // 从 IoC 容器中取出实例， Session 必须先注册。
    _session.Watch(nameof(Session.CurrentImage), Session_CurrentImage_Changed); // 监听 CurrentImage 属性的变化

    uiListBox.ItemsSource = _session.Images; // UI元素直接绑定 Model 中的属性
    uiListBox.SelectionChanged += UiListBox_SelectionChanged;
}
```

## 3. 其他基础设施

### 3.1 Avalonia

当您的应用平台是 Avalonia 时，[Bread.Mvc.Avalonia](https://www.nuget.org/packages/Bread.Mvc.Avalonia#versions-body-tab) 包含一些非常有用的扩展。

***IUIDispatcher 接口 ：UI线程注入***  

Bread.Mvc.Avalonia.MainThreadDispatcher 实现了 IUIDispatcher 接口。
因为当属性被外部线程修改时，Watch 机制需要使用这个接口检测当前线程是否在主线程中，并将变更 Invoke 给UI线程，所以您必须在Avalonia应用中注册这个服务。

```C#
 builder.AddSingleton<IUIDispatcher, Bread.Mvc.Avalonia.MainThreadDispatcher>();
```

***Reactive***

为了简化 Watch 操作，我们为常见的控件准备了更易用的绑定方法。

```C#

public interface IEnumDescriptioner<T> where T : Enum
{
    string GetDescription(T value);
}

public partial class SettingsPanel : UserControl
{
    SpotModel _spot = null!;

    public EngineSettingsPanel()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;

        _spot = IoC.Get<SpotModel>();

        // combox initted by enum which LanguageHelper implements IEnumDescriptioner
        uiComboxLanguage.InitBy(new LanguageHelper(), Language.Chinese, 
            Language.English, Language.Japanese, Language.Japanese); 

        uiComboxLanguage.BindTo(_spot, m => m.Language); // ComboBox
       
        uiNUDAutoSave.BindTo(_app, x => x.AutoSave); // NumericUpDown
        uiTbRegCode.BindTo(_app, x => x.RegCode); // TextBox
        uiTbFilePath.BindTo(_app, x => x.FilePath); // TextBlock

        uiSlider.BindTo(_app, x => x.Progress); // Slider

        uiSwitchAutoSpot.BindTo(_spot, m => m.IsAutoSpot); // SwitchButton
        uiTbtnChannel.BindTo(_app, x => x.IsLeftChannel); // ToggleButton

        uiCheckSexual.BindTo(_app, x => x.IsMale); // CheckBox
    }
}

```

### 3.2 WPF

### 3.3 日志

Bread.Utility 中提供了一个简单的日志类 Log。

```C#
public static class Log
{
    /// <summary>
    /// 打开日志
    /// </summary>
    /// <param name="path">日志文件名称</param>
    /// <param name="expire">日志文件目录下最多保存天数。0表示不删除多余日志</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Open(string path, int expire = 0);

    /// <summary>
    /// 关闭日志文件
    /// </summary>
    public static void Close();

    public static void Info(string info, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0);

    public static void Warn(string warn, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0);

    public static void Error(string error, string? category = null,
        [CallerFilePath] string? className = null,
        [CallerMemberName] string? methondName = null,
        [CallerLineNumber] int lineNumber = 0);

    public static void Exception(Exception ex);
}
```
### 3.4 配置文件读写

内置 Config 类用于 ini 文件读写。

```C#
public class CustomController : Controller
{
    Config _appConfig;
    readonly AppModel _app;
    readonly ProjectModel _prj;

    public AppController(AppModel app, ProjectModel prj)
    {
        _app = app;
        _prj = prj;
        
        _appConfig = new Config(Path.Combine(app.AppFolder, "app.data"));
    
        AppCommands.Load.Event += Load_Event;
        AppCommands.Save.Event += Save_Event;
    }

    private void Load_Event()
    {
        _appConfig.Load();
        _app.LoadFrom(_appConfig);
        _prj.LoadFrom(_appConfig);
    }

    private void Save_Event()
    {
        _app.SaveTo(_appConfig);
        _prj.SaveTo(_appConfig);
        _appConfig.Save();
    }
}
```

```C#
public class AppModel : Model
{
    public string Recorder { get; internal set; } = string.Empty;

    public ReadOnlyCollection<string> RecentList { get { return _recentList.AsReadOnly(); } }

    List<string> _recentList = new();

    public AppModel()
    {
    }

    public override void LoadFrom(Config config)
    {
        config.Load(nameof(AppModel), nameof(Recorder), (string value) => { Recorder = value; });

        var list = config.LoadList(nameof(RecentList));
        foreach (var item in list) {
            if (File.Exists(item)) {
                _recentList.Add(item);
            }
        }
        OnPropertyChanged(nameof(RecentList));
    }


    public override void SaveTo(Config config)
    {
        base.SaveTo(config);

        config[nameof(AppModel), nameof(Recorder)] = Recorder;
        config.SaveList(nameof(RecentList), _recentList);
    }
}

```
