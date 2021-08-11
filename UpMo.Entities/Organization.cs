using System;
using System.Collections.Generic;

namespace UpMo.Entities
{
    public class Organization : BaseEntity<Guid>
    {
        public Organization()
        {
            Monitors = new HashSet<Monitor>();
            Managers = new HashSet<OrganizationManager>();
        }
        
        public string Name { get; set; }
        public uint CreatorUserID { get; set; }
        
        public virtual ICollection<Monitor> Monitors { get; set; }
        public virtual ICollection<OrganizationManager> Managers { get; set; }
    }
}