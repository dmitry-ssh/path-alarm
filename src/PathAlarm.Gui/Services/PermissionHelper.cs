using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PathAlarm.Gui.Services;

public static class PermissionHelper
{
    public static async Task<PermissionStatus> CheckAndRequestPermissionAsync<TPermission>()
        where TPermission : BasePermission, new()
    {
        return await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            TPermission permission = new TPermission();
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        });
    }
}