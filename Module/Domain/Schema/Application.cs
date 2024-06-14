using Module.Domain.Base;
using System.Reflection;

namespace Module.Domain.Schema
{
    public class Application : BaseEntity
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public ICollection<Workspace> Workspaces { get; set; }
        public ICollection<Module> Modules { get; set; }
    }
}
