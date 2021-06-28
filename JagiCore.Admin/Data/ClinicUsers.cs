using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Admin.Data
{
    public class ClinicUsers
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string UserId { get; set; }
        public bool IsManageable { get; set; }
    }
}
