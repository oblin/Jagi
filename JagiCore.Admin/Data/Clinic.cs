using System.ComponentModel.DataAnnotations.Schema;

namespace JagiCore.Admin.Data
{
    [Table("Clinic")]
    public class Clinic
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string Database { get; set; }

        /// <summary>
        /// 資料庫登入帳號
        /// </summary>
        public string DatabaseUser { get; set; }

        [Column("DatabasePassword")]
        public byte[] EncryptDatabasePassword { get; set; }

        /// <summary>
        /// 資料庫登入帳號的密碼（對應 DatabaseUser）
        /// </summary>
        [NotMapped]
        public string DatabasePassword { get; set; }
    }
}
