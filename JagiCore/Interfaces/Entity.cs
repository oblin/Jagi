using System;
using System.ComponentModel.DataAnnotations;

namespace JagiCore.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Entity : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "建立日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "建立使用者")]
        public string CreatedUser { get; set; }

        [Display(Name = "修改日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "修改使用者")]
        public string ModifiedUser { get; set; }
    }
}
