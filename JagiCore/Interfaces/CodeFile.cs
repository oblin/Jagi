using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Interfaces
{
    public class CodeFile : IEntity
    {
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string ParentType { get; set; }
        public string ParentCode { get; set; }
        public string Remark { get; set; }

        public virtual ICollection<CodeDetail> CodeDetails { get; set; }
    }
}
