namespace PathAlarm.Engine.Permissions
{
    public interface IPermissionManager
    {
        Task<bool> GetPermission(PermissionType permissionType);
    }
}
