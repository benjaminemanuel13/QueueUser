using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public static class Control
    {
        public static Dictionary<string, Server> Servers { get; set; } = new Dictionary<string, Server>();
    }
}
