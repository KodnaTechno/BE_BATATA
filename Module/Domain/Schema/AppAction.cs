using Module.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCommon.DTOs;

namespace Module.Domain.Schema
{
    public class AppAction: SoftDeleteEntity
    {
        public TranslatableValue Name { get; set; }
        public string Description { get; set; }
        public ActionType Type { get; set; }
        public Guid? ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public Guid? WorkspaceId { get; set; }
        public string ValidationFormula { get; set; } // this works as extra layter to  control the actions to be shown or not
        public virtual Workspace Workspace { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
        public virtual WorkspaceModule WorkspaceModule { get; set; }
    }
    public enum ActionType
    {
        Create,
        Update,
        Delete,
        Read,
        Custom
    }
}
