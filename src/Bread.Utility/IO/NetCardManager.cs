using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Bread.Utility.IO;

public class Netcard
{
    public static readonly Netcard Empty = new Netcard() { Name = "请选择网卡" };

    public string Name { get; set; } = string.Empty;

    public string Mac { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}

public static class NetCardManager
{
    /// <summary>
    /// 获取物理网卡列表
    /// </summary>
    /// <returns></returns>
    public static List<Netcard> GetNetcards()
    {
        List<Netcard> netcards = new List<Netcard>();
        netcards.Add(Netcard.Empty);
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
            if (!nic.Supports(NetworkInterfaceComponent.IPv4)) continue;
            if (nic.GetPhysicalAddress() == null || nic.GetPhysicalAddress().ToString() == string.Empty) continue;

            Netcard card = new Netcard();
            card.Name = nic.Description;
            string macAddr = nic.GetPhysicalAddress().ToString();
            for (int i = 2; i < macAddr.Length; i += 3) {
                macAddr = macAddr.Insert(i, ":");
            }
            card.Mac = macAddr;
            netcards.Add(card);
        }
        return netcards;
    }

    /// <summary>
    /// 获取第一个物理网卡mac地址
    /// </summary>
    /// <returns></returns>
    public static string? GetMacAddress()
    {
        var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                       where nic.Supports(NetworkInterfaceComponent.IPv4)
                       select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        if (string.IsNullOrEmpty(macAddr)) return null;

        for (int i = 2; i < macAddr.Length; i += 3) {
            macAddr = macAddr.Insert(i, ":");
        }
        return macAddr;
    }

    public static async Task<string?> GetIPAddress()
    {
        try {
            using (var client = new HttpClient()) {
                client.Timeout = TimeSpan.FromSeconds(3);
                var ip = string.Empty;
                var response = await client.GetAsync("http://ip.chinaz.com/getip.aspx");
                if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
                if (response.Content == null) return null;
                var content = await response.Content.ReadAsStringAsync();
                var regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                var matches = regex.Matches(content);
                if (matches == null || matches.Count == 0) return null;
                return matches[0].ToString();
            }
        }
        catch (Exception) {
            return null;
        }
    }

}
