using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensioProcuratio.Helper.Models
{
    public class SmtpSettings
    {
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string Pass { get; set; } = null!;
        public int Door { get; set; }
    }
}
