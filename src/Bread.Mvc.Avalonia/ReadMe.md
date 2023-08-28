# Bread.Mvc.Avalonia
[![NuGet Status](https://img.shields.io/nuget/v/Bread.Mvc.Avalonia.svg)](https://www.nuget.org/packages/Bread.Mvc.Avalonia)

[Bread.Mvc.Avalonia](https://www.nuget.org/packages/Bread.Mvc.Avalonia#versions-body-tab) 包含一些非常有用的扩展。

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
