using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadmus.Graph.Ef;

public class EfNodeClass
{
    public int Id { get; set; }
    public int NodeId { get; set; }
    public int ClassId { get; set; }
    public int Level { get; set; }

    public EfNode? Node { get; set; }

    public override string ToString()
    {
        return $"{NodeId} a {ClassId} @{Level}";
    }
}
