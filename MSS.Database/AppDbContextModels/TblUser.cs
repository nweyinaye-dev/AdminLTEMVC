using System;
using System.Collections.Generic;

namespace MSS.Database.AppDbContextModels;

public partial class TblUser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RoleId { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
