using Module.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Service
{
    public interface IModuleService
    {
        Task<Domain.Schema.Module> GetModuleByIdAsync(Guid id);
        Task<Domain.Schema.Module> CreateModuleAsync(Domain.Schema.Module module, Guid UserId);
        Task<Domain.Schema.Module> UpdateModuleAsync(Domain.Schema.Module module, Guid UserId);
        Task<bool> DeleteModuleAsync(Guid id,Guid UserId);
    }
}
