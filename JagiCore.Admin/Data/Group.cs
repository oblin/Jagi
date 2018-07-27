using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Admin.Data
{
    [Table("Group")]
    public class Group
    {
        [Key, Required, StringLength(10)]
        public string Code { get; set; }
        public string Name { get; set; }
        [Column("parent_code")]
        public string ParentCode { get; set; }
        [Column("clinic_code")]
        public string ClinicCode { get; set; }
        [ForeignKey("ParentCode")]
        public List<Group> Groups { get; set; }
    }
}
