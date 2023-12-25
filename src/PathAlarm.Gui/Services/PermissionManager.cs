using PathAlarm.Engine.Permissions;

namespace PathAlarm.Gui.Services;

internal class PermissionManager : IPermissionManager
{
    public async Task<bool> GetPermission(PermissionType permissionType)
    {
        var s1 = await PermissionHelper.CheckAndRequestPermissionAsync<Permissions.LocationWhenInUse>();
        if (s1 != PermissionStatus.Granted)
        {
            return false;
        }
        var status = await PermissionHelper.CheckAndRequestPermissionAsync<Permissions.LocationAlways>();
        return status == PermissionStatus.Granted;
    }
}