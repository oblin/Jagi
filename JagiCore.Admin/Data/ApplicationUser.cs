using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Admin.Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public int ClinicId { get; set; }
        public int LoginLogId { get; internal set; }
        public virtual string GroupCode { get; set; }
        private string displayName;
        [StringLength(20)]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                    return this.UserName;
                else return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        /// <summary>
        /// For Workflow defined is user can approve
        /// </summary>
        public bool CanApproval { get; set; }

        [NotMapped]
        public string RefreshToken { get; set; }

        [NotMapped]
        public List<Clinic> Clinics { get; set; }
    }
}
