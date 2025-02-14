using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Service
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly ModuleDbContext _context;

        public WorkspaceService(ModuleDbContext context)
        {
            _context = context;
        }

        public async Task<Workspace> GetWorkspaceByIdAsync(Guid id)
        {
            return await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Workspace> CreateWorkspaceAsync(Workspace workspace, Guid UserId)
        {
            workspace.CreatedAt = DateTime.UtcNow;
            workspace.UpdatedAt = DateTime.UtcNow;
            workspace.UpdatedBy = UserId;
            workspace.CreatedBy = UserId;

            await _context.Workspaces.AddAsync(workspace);
            await _context.SaveChangesAsync();

            return workspace;
        }

        public async Task<Workspace> UpdateWorkspaceAsync(Workspace workspace, Guid UserId)
        {
            var existingWorkspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == workspace.Id);

            if (existingWorkspace == null)
                throw new KeyNotFoundException($"Workspace with ID {workspace.Id} not found");

            // Update basic properties
            existingWorkspace.UpdatedAt = DateTime.UtcNow;
            existingWorkspace.UpdatedBy = UserId;

            await _context.SaveChangesAsync();
            return existingWorkspace;
        }

        public async Task<bool> DeleteWorkspaceAsync(Guid id, Guid UserId)
        {
            var workspace = await _context.Workspaces
                .Include(x => x.Actions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workspace == null)
                return false;

            workspace.DeletedAt = DateTime.UtcNow;
            workspace.DeletedBy = UserId;
            workspace.IsDeleted = true;

            // Delete associated actions
            foreach (var action in workspace.Actions)
            {
                action.DeletedAt = DateTime.UtcNow;
                action.DeletedBy = UserId;
                action.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
