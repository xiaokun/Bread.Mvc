namespace Bread.Utility.IO;

public static class RemovableDevcie
{
    public static List<DriveInfo> GetList()
    {
        List<DriveInfo> list = new List<DriveInfo>();
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (DriveInfo drive in drives) {
            try {
                if ((drive.DriveType == DriveType.Removable) && !drive.Name.Substring(0, 1).Equals("A")) {
                    list.Add(drive);
                }
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }
        }
        return list;
    }
}
