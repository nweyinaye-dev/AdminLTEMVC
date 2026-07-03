using System;
using System.Collections.Generic;

namespace MSS.Database.AppDbContextModels;

public partial class TblLoginUser
{
    public string Id { get; set; } = null!;

    public string LoginName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? BranchId { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
