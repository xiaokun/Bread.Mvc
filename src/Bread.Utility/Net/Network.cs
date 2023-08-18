using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Bread.Utility.IO;

namespace Bread.Utility.Net;

public static class Network
{
    public static async Task<bool> PingAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        try {
            var ip = string.Empty;
            if (url.Contains(":") == false && IPAddress2.IsValidIP(url)) {
                ip = url;
            }
            else {
                var host = (new Uri(url)).Host;
                var dns = await Dns.GetHostAddressesAsync(host);
                if (dns.Length == 0 || dns[0].ToString().Length <= 6) {
                    Log.Error($"get dns from {url} fail");
                    return false;
                }
                ip = dns[0].ToString();
            }

            if (string.IsNullOrEmpty(ip))
                return false;

            using (var sender = new Ping()) {
                var options = new PingOptions();
                options.DontFragment = true;
                var data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                var buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 500;

                for (int i = 0; i < 4; i++) {
                    var reply = await sender.SendPingAsync(ip, timeout, buffer);
                    if (reply.Status == IPStatus.Success)
                        return true;
                }

                Log.Error($"ping {url} {ip} fail");
                return false;
            }
        }
        catch (Exception ex) {
            Log.Error($"ping url fail. {url}, {ex.Message}");
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
            var list = new List<IPAddress2>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                if (nic.OperationalStatus != OperationalStatus.Up) continue;

                foreach (var unicast in nic.GetIPProperties().UnicastAddresses) {
                    if (unicast.Address.AddressFamily != AddressFamily.InterNetwork) continue;  //从IP地址列表中筛选出IPv4类型的IP地址

                    var ip = new IPAddress2(unicast.Address.GetAddressBytes());
                    if (!ip.IsValid) continue;
                    list.Add(ip);
                }
            }
            return list;
        }
        catch (Exception) {
            return new List<IPAddress2>();
        }
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
    /// 判断两个ip地址是否在同一个网段内
    /// </summary>
    /// <param name="ip1"></param>
    /// <param name="ip2"></param>
    /// <returns></returns>
    public static bool IsInSameNetSapce(string ip1, string ip2)
    {
        if (string.IsNullOrEmpty(ip1)) return false;
        if (string.IsNullOrEmpty(ip2)) return false;

        var ips1 = ip1.Split('.');
        if (ips1 == null || ips1.Length != 4) return false;
        var ips2 = ip2.Split('.');
        if (ips2 == null || ips2.Length != 4) return false;

        if (ips1[0] != ips2[0]) return false;
        if (ips1[1] != ips2[1]) return false;
        //if (ips1[2] != ips2[2]) return false;

        return true;
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


    public static IPAddress GetSubnetMask(IPAddress address)
    {
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            if (nic.OperationalStatus != OperationalStatus.Up) continue;

            foreach (var unicast in nic.GetIPProperties().UnicastAddresses) {
                if (unicast.Address.AddressFamily == AddressFamily.InterNetwork) {
                    if (address.Equals(unicast.Address)) {
                        return unicast.IPv4Mask;
                    }
                }
            }
        }
        throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
    }

    public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++) {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
        }
        return new IPAddress(broadcastAddress);
    }

    public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
    {
        IPAddress network1 = address.GetNetworkAddress(subnetMask);
        IPAddress network2 = address2.GetNetworkAddress(subnetMask);

        return network1.Equals(network2);
    }

    public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++) {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }

    public static IPAddress2? GetNeareastAddress(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        var ip2 = new IPAddress2(url);
        if (ip2.IsValid == false) return null;

        var ip = new IPAddress(ip2.Data);

        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()) {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            if (nic.OperationalStatus != OperationalStatus.Up) continue;

            foreach (var unicast in nic.GetIPProperties().UnicastAddresses) {
                if (unicast.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                try {
                    if (IsInSameSubnet(unicast.Address, ip, unicast.IPv4Mask)) {
                        return new IPAddress2(unicast.Address.GetAddressBytes());
                    }
                }
                catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        }
        return null;
    }

}
