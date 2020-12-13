// ReSharper disable VirtualMemberCallInConstructor
namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Relationships = new HashSet<Relationship>();
            this.Followers = new HashSet<ApplicationUser>();
            this.Following = new HashSet<ApplicationUser>();
            this.Collections = new List<Collection>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string ProfitPicturePath { get; set; }

        public string BackgroundPicturePath { get; set; }

        public string Status { get; set; }

        public virtual ICollection<Relationship> Relationships { get; set; }

        public virtual ICollection<ApplicationUser> Followers { get; set; }

        public virtual ICollection<ApplicationUser> Following { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
