namespace Bread.Utility;

/// <summary>
///     文本类型枚举
/// </summary>
public enum TextType
{
    Common,
    Phone,
    Mail,
    Url,
    Chinese,
    Number,
    Digits,
    PInt,
    NInt,
    Int,
    NnInt,
    NpInt,
    PDouble,
    NDouble,
    Double,
    NnDouble,
    NpDouble,
}

/// <summary>
/// IP类型枚举
/// </summary>
public enum IpType
{
    /// <summary>
    /// A类IP地址
    /// </summary>
    A = 0,
    /// <summary>
    /// B类IP地址
    /// </summary>
    B,
    /// <summary>
    /// C类IP地址
    /// </summary>
    C,
    /// <summary>
    /// D类IP地址
    /// </summary>
    D,
    /// <summary>
    /// E类IP地址
    /// </summary>
    E
}


/// <summary>
///     表示一个操作的返回结果类型
/// </summary>
public enum ResultType
{
    /// <summary>
    ///     成功
    /// </summary>
    Success,

    /// <summary>
    ///     失败
    /// </summary>
    Failed,

    /// <summary>
    ///     无数据
    /// </summary>
    None
}

public enum InfoType
{
    Success = 0,
    Info,
    Warning,
    Error,
    Fatal,
    Ask
}

public enum Role : int
{
    Unknow = 0x00,
    Teacher = 0x01,// 老师
    Assist = 0x02,//助教
    Student = 0x04,// 学生
};
