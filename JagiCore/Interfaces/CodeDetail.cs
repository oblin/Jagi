using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Interfaces
{
    public class CodeDetail : IEntity
    {
        public int Id { get; set; }

        public int CodeFileId { get; set; }

        public string ItemCode { get; set; }

        public string Description { get; set; }

        public bool IsBanned { get; set; }
    }
}
