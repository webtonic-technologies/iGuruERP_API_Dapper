﻿namespace UserRoleManagement_API.DTOs.Requests
{
    public class GetRolesPermissionsRequest
    {
        public int RoleID { get; set; }
        public int InstituteID { get; set; }
    }
}