using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Service;

public class ModuleService : IModuleService
{
    private readonly ModuleDbContext _context;

    public ModuleService(ModuleDbContext context)
    {
        _context = context;
    }


    public async Task<Domain.Schema.Module> GetModuleByIdAsync(Guid id)
    {
        return await _context.Modules
            .Include(m => m.Properties)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Domain.Schema.Module> CreateModuleAsync(Domain.Schema.Module module,Guid UserId)
    {
        module.CreatedAt = DateTime.UtcNow;
        module.UpdatedAt = DateTime.UtcNow;
        module.UpdatedBy= UserId;
        module.CreatedBy = UserId;
        module.IsActive = true;

        module.Actions = new List<AppAction>()
        {
            new AppAction
            {
                Id = Guid.NewGuid(),
                Name =new(){Ar = "اضافة",En = "Create"},
                Description = "Create module",
                Type = ActionType.Create,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AppAction
            {
                Id = Guid.NewGuid(),
                Name =new(){Ar = "تعديل",En = "Update"},
                Description = "Update module",
                Type = ActionType.Update,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new AppAction
            {
                Id = Guid.NewGuid(),
                Name =new(){Ar = "حذف",En = "Delete"},
                Description = "Delete module",
                CreatedAt = DateTime.UtcNow,
                Type = ActionType.Delete,
                UpdatedAt = DateTime.UtcNow
            },
            new AppAction
            {
                Id = Guid.NewGuid(),
                Name =new(){Ar = "معاينة",En = "Read"},
                Description = "Read module",
                Type = ActionType.Read,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
        };
        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        return module;
    }

    public async Task<Domain.Schema.Module> UpdateModuleAsync(Domain.Schema.Module module, Guid UserId)
    {
        var existingModule = await _context.Modules
            .Include(m => m.Properties)
            .FirstOrDefaultAsync(m => m.Id == module.Id);

        if (existingModule == null)
            throw new KeyNotFoundException($"Module with ID {module.Id} not found");

        // Update basic properties
        existingModule.Name = module.Name;
        existingModule.IsActive = module.IsActive;
        existingModule.UpdatedAt = DateTime.UtcNow;
        existingModule.UpdatedBy = UserId;

        await _context.SaveChangesAsync();
        return existingModule;
    }

    public async Task<bool> DeleteModuleAsync(Guid id,Guid UserId)
    {
        var module = await _context.Modules.Include(x=>x.Actions).FirstOrDefaultAsync(x=>x.Id==id);
        if (module == null)
            return false;

        module.DeletedAt = DateTime.UtcNow;
        module.DeletedBy = UserId;
        module.IsDeleted = true;
        module.IsActive = false;
        foreach (var action in module.Actions)
        {
            action.DeletedAt = DateTime.UtcNow;
            action.DeletedBy = UserId;
            action.IsDeleted = true;

        }
        await _context.SaveChangesAsync();
        return true;
    }
}