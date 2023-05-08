using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensioProcuratio.Models
{
    public class ProjectAssociatesModel
    {
        public ProjectAssociatesModel(string userId, string projectId)
        {
            UserId = userId;
            ProjectId = projectId;
        }

        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
    }
}
