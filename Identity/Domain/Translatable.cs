using System.ComponentModel.DataAnnotations.Schema;

namespace AppIdentity.Domain;

[ComplexType]
public class Translatable
{
    public string Ar { get; set; }
    public string En { get; set; }

}
