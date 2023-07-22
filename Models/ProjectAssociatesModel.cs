using ExtensioProcuratio.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensioProcuratio.Models
{
    public class ProjectAssociatesModel
    {
        public ProjectAssociatesModel(string userId, ProjectId projectId)
        {
            UserId = userId;
            ProjectId = projectId;
        }

        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public ProjectId ProjectId { get; set; }

        public ProjectAssociatesModel() { }
    }
}
