using System;
using System.Collections.Generic;

namespace MSS.Database.AppDbContextModels;

public partial class TblPermission
{
    public string Id { get; set; } = null!;

    public string PermissionName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
