using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensioProcuratio.Helper.Models
{
    public class EmailModel
    {
        public EmailModel(string email, string subject, string body)
        {
            Email = email;
            Subject = subject;
            Body = body;
        }

        public string Email { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
