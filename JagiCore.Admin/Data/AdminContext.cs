using JagiCore.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Admin.Data
{
    public class AdminContext : IdentityDbContext<ApplicationUser>
    {
        public AdminContext(DbContextOptions<AdminContext> options): base(options) { }

        public DbSet<Group> Groups { get; set; }
        public DbSet<CodeFile> CodeFiles { get; set; }
        public DbSet<CodeDetail> CodeDetails { get; set; }
        public DbSet<Clinic> Clinics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CodeFile>().ToTable("CodeFiles");
            builder.Entity<CodeFile>().Property(a => a.Id).HasColumnName("id");
            builder.Entity<CodeFile>().Property(a => a.ItemType).HasColumnName("item_type");
            builder.Entity<CodeFile>().Property(a => a.ParentType).HasColumnName("parent_type");
            builder.Entity<CodeFile>().Property(a => a.ParentCode).HasColumnName("parent_code");
            builder.Entity<CodeFile>().Property(a => a.Remark).HasColumnName("c_remark");
            builder.Entity<CodeFile>().Property(a => a.TypeName).HasColumnName("type_name");
            builder.Entity<CodeFile>().Property(a => a.Description).HasColumnName("desc_1");
            builder.Entity<CodeFile>().Ignore("ModifyFlag");
            builder.Entity<CodeFile>().HasMany(s => s.CodeDetails).WithOne().HasForeignKey(s => s.CodeFileId);

            builder.Entity<CodeDetail>().ToTable("CodeDetails");
            builder.Entity<CodeDetail>().Property(a => a.Id).HasColumnName("id");
            builder.Entity<CodeDetail>().Property(a => a.ItemCode).HasColumnName("item_code");
            builder.Entity<CodeDetail>().Property(a => a.IsBanned).HasColumnName("is_banned");
            builder.Entity<CodeDetail>().Property(a => a.Description).HasColumnName("desc");
            builder.Entity<CodeDetail>().Ignore("IsBanned");

            base.OnModelCreating(builder);
        }
    }
}
