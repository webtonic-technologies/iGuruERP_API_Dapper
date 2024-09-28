namespace Employee_API.DTOs
{
    public class GetAllEmployeeListRequest
    {
        public int InstituteId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
    public class EmployeeLoginRequest
    {
        public int InstituteId { set; get; }
        public int DepartmentId { set; get; }
        public int DesignationId { set; get; }
        public string SearchText { set; get; } = string.Empty;
        public int PageNumber { set; get; }
        public int PageSize { set; get; }
    }
    public class UserLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsAppUser { get; set; } // Added IsAppUser
        public string Brand { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Fingerprint { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string VersionSdkInt { get; set; } = string.Empty;
        public string VersionSecurityPatch { get; set; } = string.Empty;
        public int? BuildId { get; set; }
        public bool IsPhysicalDevice { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string SystemVersion { get; set; } = string.Empty;
        public string UtsnameVersion { get; set; } = string.Empty;
        public string OperSysName { get; set; } = string.Empty;
        public string BrowserName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public string DeviceMemory { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string KernelVersion { get; set; } = string.Empty;
        public string ComputerName { get; set; } = string.Empty;
        public string SystemGUID { get; set; } = Guid.NewGuid().ToString(); // Default GUID
    }

    public class DownloadExcelRequest
    {
        public int InstituteId { set; get; }
        public int DepartmentId { set; get; }
        public int DesignationId { set; get; }
    }
    public class ExcelDownloadRequest
    {
        public int InstituteId { set; get; }
        public int DesignationId { set; get; }
        public int DepartmetnId { set; get; }
    }
    public class UserSwitchOverRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public int InstituteId { get; set; }
    }
    public class ForgotPassword
    {
        public string UserEmailOrPhoneOrUsername { get; set; } = string.Empty;
    }
    public class ResetPassword
    {
        public int UserId { set; get; }
        public string UserName { get; set; } = string.Empty;
        public string Usertype { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
