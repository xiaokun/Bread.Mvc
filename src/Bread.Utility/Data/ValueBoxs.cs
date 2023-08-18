using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bread.Utility;

/// <summary>
///     装箱后的值类型（用于提高效率）
/// </summary>
public static class ValueBoxes
{
    public static object True = true;

    public static object False = false;

    public static object Double0 = .0;

    public static object Double01 = .1;

    public static object Double1 = 1.0;

    public static object Double10 = 10.0;

    public static object Double20 = 20.0;

    public static object Double100 = 100.0;

    public static object Double200 = 200.0;

    public static object Double300 = 300.0;

    public static object DoubleNeg1 = -1.0;

    public static object Int0 = 0;

    public static object Int1 = 1;

    public static object Int2 = 2;

    public static object Int5 = 5;

    public static object Int6 = 6;

    public static object Int7 = 7;

    public static object Int8 = 8;

    public static object Int9 = 9;

    public static object Int99 = 99;

    public static object BooleanBox(bool value) => value ? True : False;
}
