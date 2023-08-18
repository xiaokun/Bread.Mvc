using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Bread.Utility.Net;

public class NetworkHelper
{
    /// <summary>  
    /// 检测IP  
    /// </summary>  
    /// <param name="ip"></param>  
    /// <returns></returns>  
    public static bool Ping(string ip)
    {
        try {
            Ping psender = new Ping();
            PingReply prep = psender.Send(ip, 500, Encoding.Default.GetBytes("afda9tu"));
            if (prep.Status != IPStatus.Success) return false;
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    /// <summary>
    /// 获取本机局域网IP地址
    /// </summary>
    /// <returns></returns>
    public static List<IPAddress2> GetLocalIPAddress()
    {
        try {
            List<IPAddress2> list = new List<IPAddress2>();
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());

            for (int i = 0; i < ipEntry.AddressList.Length; i++) {
                if (ipEntry.AddressList[i].AddressFamily != AddressFamily.InterNetwork) continue;   //从IP地址列表中筛选出IPv4类型的IP地址

                var addr = ipEntry.AddressList[i].ToString();
                if (string.IsNullOrEmpty(addr)) continue;

                var ip = new IPAddress2(addr);
                if (!ip.IsValid) continue;

                if (ip.Data[0] == 10) { list.Add(ip); continue; }  //A类私有地址
                if (ip.Data[0] == 172) { list.Add(ip); continue; } //B类私有地址
                if (ip.Data[0] == 192 && ip.Data[1] == 168) { list.Add(ip); continue; } //C类私有地址

                list.Add(ip);
            }

            return list;
        }
        catch (Exception) {
            return new List<IPAddress2>();
        }
    }


    public static string? GetBestIp(List<IPAddress2> localips)
    {
        if (localips == null || localips.Count == 0) return "";

        foreach (var ip in localips) {
            if (ip.Data[0] == 192 && ip.Data[1] == 168) return ip.ToString();
        }

        foreach (var ip in localips) {
            if (ip.Data[0] == 172) return ip.ToString();
        }

        foreach (var ip in localips) {
            if (ip.Data[0] == 10) return ip.ToString();
        }
        return localips[0].ToString();
    }


    public static bool IsLocalIPAddress(string ip)
    {
        var ip2 = new IPAddress2(ip);
        if (ip2.IsValid == false) return false;

        if (ip2.Data[0] == 192 && ip2.Data[1] == 168) { return true; } //C类私有地址
        if (ip2.Data[0] == 172 && ip2.Data[1] >= 16 && ip2.Data[1] <= 31) { return true; } //B类私有地址
        if (ip2.Data[0] == 10) { return true; }  //A类私有地址

        return false;
    }


    /// <summary>
    /// 从本地的局域网ip地址中获取同网段的最接近的地址
    /// </summary>
    /// <param name="url">url地址，如rtsp://192.168.3.58:554/path1/stream</param>
    /// <returns></returns>
    public static IPAddress2? GetNeareastAddress(string url)
    {
        var ips = GetLocalIPAddress();
        if (ips.Count == 0) return null;
        if (string.IsNullOrEmpty(url)) return ips[0];
        IPAddress2 target = new IPAddress2(url);
        if (!target.IsValid) return ips[0];

        foreach (var ip in ips) {
            if (ip.Equals(target)) {
                return ip;
            }
        }

        //分级查找
        List<IPAddress2> lastLevel = new List<IPAddress2>(ips);
        for (int i = 0; i < 4; i++) {
            List<IPAddress2> level = new List<IPAddress2>();
            foreach (var ip in lastLevel) {
                if (ip.Data[i] == target.Data[i])
                    level.Add(ip);
            }
            if (level.Count > 1) {
                lastLevel = level;
                continue;
            }
            return lastLevel[0];
        }

        return lastLevel[0];
    }


    /// <summary>
    /// 从url中解析出ip地址
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string? GetIpAddressFrom(string url)
    {
        int index = url.IndexOf("://");
        string leftComponents = "";
        if (index > 0) leftComponents = url.Substring(index + 3);
        else leftComponents = url;
        var splits = leftComponents.Split('/');
        if (splits.Length == 0) return null;
        var splits2 = splits[0].Split(':');
        if (splits2.Length == 0) return null;
        return splits2[0];
    }

}
