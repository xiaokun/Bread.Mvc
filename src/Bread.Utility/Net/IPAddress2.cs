using System.Net;

namespace Bread.Utility.Net;

public class IPAddress2 : IEquatable<IPAddress2>
{
    public IPAddress? IPAddress { get; private set; } = null;

    public bool IsEmpty { get; private set; } = false;

    public byte[] Data { get; private set; } = new byte[4] { 0, 0, 0, 0 };

    public bool IsValid { get; private set; } = false;

    private string? _ip = null;

    public IPAddress2(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        var ip = NetworkHelper.GetIpAddressFrom(url);
        if (string.IsNullOrEmpty(ip)) return;

        int[] nums = new int[4] { 0, 0, 0, 0 };
        var splits = ip.Split('.');
        if (splits.Length != 4) return;

        if (!int.TryParse(splits[0], out nums[0])) return;
        if (!int.TryParse(splits[1], out nums[1])) return;
        if (!int.TryParse(splits[2], out nums[2])) return;
        if (!int.TryParse(splits[3], out nums[3])) return;

        if (nums[0] < 0 || nums[0] >= 256) return;
        if (nums[1] < 0 || nums[1] >= 256) return;
        if (nums[2] < 0 || nums[2] >= 256) return;
        if (nums[3] < 0 || nums[3] >= 256) return;

        Data[0] = (byte)nums[0];
        Data[1] = (byte)nums[1];
        Data[2] = (byte)nums[2];
        Data[3] = (byte)nums[3];

        IPAddress = new IPAddress(Data);
        _ip = ip;

        if((Data[0] == 0) && (Data[1] == 0) && (Data[2] == 0) && (Data[3] == 0))
        {
            IsEmpty = true;
        }

        IsValid = true;
    }

    public IPAddress2(byte[] bytes)
    {
        Data = bytes;
        IPAddress = new IPAddress(Data);
        _ip = $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";

        if ((Data[0] == 0) && (Data[1] == 0) && (Data[2] == 0) && (Data[3] == 0)) {
            IsEmpty = true;
        }

        IsValid = true;
    }

    public IPAddress2(byte a, byte b, byte c, byte d)
    {
        Data[0] = a;
        Data[1] = b;
        Data[2] = c;
        Data[3] = d;

        IPAddress = new IPAddress(Data);
        _ip = $"{a}.{b}.{c}.{d}";

        if ((Data[0] == 0) && (Data[1] == 0) && (Data[2] == 0) && (Data[3] == 0)) {
            IsEmpty = true;
        }

        IsValid = true;
    }



    public override string? ToString()
    {
        return _ip;
    }

    public static bool IsValidIP(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return false;

        int[] nums = new int[4] { 0, 0, 0, 0 };
        var splits = ip.Split('.');
        if (splits.Length != 4) return false;

        if (!int.TryParse(splits[0], out nums[0])) return false;
        if (!int.TryParse(splits[1], out nums[1])) return false;
        if (!int.TryParse(splits[2], out nums[2])) return false;
        if (!int.TryParse(splits[3], out nums[3])) return false;

        if (nums[0] < 0 || nums[0] >= 256) return false;
        if (nums[1] < 0 || nums[1] >= 256) return false;
        if (nums[2] < 0 || nums[2] >= 256) return false;
        if (nums[3] < 0 || nums[3] >= 256) return false;
        return true;
    }

    public bool Equals(IPAddress2? other)
    {
        if(other == null) return false;
        if (!IsValid) return false;
        if (!other.IsValid) return false;
        if (Data[0] != other.Data[0]) return false;
        if (Data[1] != other.Data[1]) return false;
        if (Data[2] != other.Data[2]) return false;
        if (Data[3] != other.Data[3]) return false;
        return true;
    }
}
