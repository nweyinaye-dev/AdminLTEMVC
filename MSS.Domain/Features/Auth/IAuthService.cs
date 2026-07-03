using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.Domain.Features.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(AuthRequest request);
    }
}
