using AppCommon.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Module.Service
{
    public class PropertyKeyGenerator
    {
        private readonly ModuleDbContext _moduleDbContext;
        private static readonly Regex _alphanumericOnly = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);
        private static readonly Regex _multipleUnderscores = new("_+", RegexOptions.Compiled);

        public PropertyKeyGenerator(ModuleDbContext moduleDbContext)
        {
            _moduleDbContext = moduleDbContext;
        }

        /// <summary>
        /// Generates a unique key and normalized key for a property based on its TranslatableValue title
        /// </summary>
        public async Task<(string Key, string NormalizedKey)> GeneratePropertyKey(
            TranslatableValue title,
            Guid? moduleId = null,
            Guid? workspaceId = null,
            Guid? workspaceModuleId = null,
            Guid? applicationId = null)
        {
            if (title == null || (string.IsNullOrWhiteSpace(title.En) && string.IsNullOrWhiteSpace(title.Ar)))
            {
                throw new ArgumentException("Title is required to generate a property key");
            }

            // Generate prefix based on entity type
            string prefix = await GetEntityPrefix(moduleId, workspaceId, workspaceModuleId, applicationId);

            // Get a title string to normalize - prioritize English, then Arabic
            string titleText = !string.IsNullOrWhiteSpace(title.En) ? title.En : title.Ar;

            // Normalize the title to create a base key
            string normalizedTitle = NormalizeTitle(titleText);
            string normalizedKey = normalizedTitle.ToUpper();
            string baseKey = $"{prefix}_{normalizedTitle}".ToLower();

            // Check if the key is unique
            var isUnique = await IsKeyUnique(normalizedKey, moduleId, workspaceId, workspaceModuleId, applicationId);

            // If the key is not unique, add a numeric suffix
            if (!isUnique)
            {
                int suffix = 1;
                string suffixedNormalizedKey;

                do
                {
                    suffixedNormalizedKey = $"{normalizedKey}_{suffix}";
                    isUnique = await IsKeyUnique(suffixedNormalizedKey, moduleId, workspaceId, workspaceModuleId, applicationId);
                    suffix++;
                } while (!isUnique);

                normalizedKey = suffixedNormalizedKey;
                baseKey = $"{prefix}_{normalizedTitle}_{suffix - 1}".ToLower();
            }

            return (baseKey, normalizedKey);
        }

        /// <summary>
        /// Checks if a normalized key is unique within the given context
        /// </summary>
        public async Task<bool> IsKeyUnique(
            string normalizedKey,
            Guid? moduleId = null,
            Guid? workspaceId = null,
            Guid? workspaceModuleId = null,
            Guid? applicationId = null)
        {
            var query = _moduleDbContext.Properties
                .Where(p => p.NormalizedKey == normalizedKey && !p.IsDeleted);

            if (moduleId.HasValue)
            {
                query = query.Where(p => p.ModuleId == moduleId);
            }
            else if (workspaceId.HasValue)
            {
                query = query.Where(p => p.WorkspaceId == workspaceId);
            }
            else if (workspaceModuleId.HasValue)
            {
                query = query.Where(p => p.WorkspaceModuleId == workspaceModuleId);
            }
            else if (applicationId.HasValue)
            {
                query = query.Where(p => p.ApplicationId == applicationId);
            }

            return !await query.AnyAsync();
        }

        /// <summary>
        /// Normalizes a title to create a key compatible format
        /// </summary>
        private string NormalizeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "property";

            // Replace spaces with underscores and remove any non-alphanumeric characters
            string normalizedTitle = title.Trim().Replace(' ', '_');
            normalizedTitle = _alphanumericOnly.Replace(normalizedTitle, "");

            // Replace multiple underscores with a single one
            normalizedTitle = _multipleUnderscores.Replace(normalizedTitle, "_");

            // Make sure it doesn't start with a number
            if (normalizedTitle.Length > 0 && char.IsDigit(normalizedTitle[0]))
            {
                normalizedTitle = "prop_" + normalizedTitle;
            }

            // Limit length
            if (normalizedTitle.Length > 50)
            {
                normalizedTitle = normalizedTitle.Substring(0, 50);
            }

            // Ensure it doesn't end with an underscore
            normalizedTitle = normalizedTitle.TrimEnd('_');

            // If after all this we have an empty string, use a default
            if (string.IsNullOrWhiteSpace(normalizedTitle))
            {
                normalizedTitle = "property";
            }

            return normalizedTitle;
        }

        /// <summary>
        /// Gets a prefix for the key based on the entity type
        /// </summary>
        private async Task<string> GetEntityPrefix(
            Guid? moduleId,
            Guid? workspaceId,
            Guid? workspaceModuleId,
            Guid? applicationId)
        {
            if (moduleId.HasValue)
            {
                var module = await _moduleDbContext.Modules
                    .Where(m => m.Id == moduleId)
                    .Select(m => m.Key)
                    .FirstOrDefaultAsync();

                return module ?? "mod";
            }
            else if (workspaceId.HasValue)
            {
                var workspace = await _moduleDbContext.Workspaces
                    .Where(w => w.Id == workspaceId)
                    .Select(w => w.Key)
                    .FirstOrDefaultAsync();

                return workspace ?? "ws";
            }
            else if (workspaceModuleId.HasValue)
            {
                var wsm = await _moduleDbContext.WorkspaceModules
                    .Include(wm => wm.Workspace)
                    .Where(wm => wm.Id == workspaceModuleId)
                    .Select(wm => wm.Workspace.Key)
                    .FirstOrDefaultAsync();

                return wsm ?? "wsm";
            }
            else if (applicationId.HasValue)
            {
                var app = await _moduleDbContext.Applications
                    .Where(a => a.Id == applicationId)
                    .Select(a => a.Key)
                    .FirstOrDefaultAsync();

                return app ?? "app";
            }

            return "prop";
        }
    }
}
