using AppCommon.DTOs;
using Module.Domain.Base;
using System.Reflection;

namespace Module.Domain.Schema
{
    public class Application : BaseProperty
    {
        public TranslatableValue Title { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public ICollection<Workspace> Workspaces { get; set; }
        public ICollection<Module> Modules { get; set; }
    }
}
