using System.ComponentModel.DataAnnotations.Schema;

namespace JagiCore.Admin.Data
{
    [Table("Clinic")]
    public class Clinic
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
