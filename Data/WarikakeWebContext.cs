using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Models;

namespace WarikakeWeb.Data
{
    public class WarikakeWebContext : DbContext
    {
        public WarikakeWebContext (DbContextOptions<WarikakeWebContext> options)
            : base(options)
        {
        }

        public DbSet<WarikakeWeb.Models.MGenre> MGenre { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.MGroup> MGroup { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.MMember> MMember { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.MUser> MUser { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TCost> TCost { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TPay> TPay { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TRepay> TRepay { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TCostSubscribe> TCostSubscribe { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TPaySubscribe> TPaySubscribe { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TRepaySubscribe> TRepaySubscribe { get; set; } = default!;
        public DbSet<WarikakeWeb.Models.TDateSubscribe> TDateSubscribe { get; set; }
        public DbSet<WarikakeWeb.Models.TSubscribe> TSubscribe { get; set; }
        public DbSet<WarikakeWeb.Models.CsvMigration> CsvMigration { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("CostIdSeq").StartsAt(107).IncrementsBy(1);
            modelBuilder.Entity<TCost>().Property(c => c.CostId).HasDefaultValueSql("NEXT VALUE FOR CostIdSeq");

            modelBuilder.HasSequence<int>("SubscribeIdSeq").StartsAt(12).IncrementsBy(1);
            modelBuilder.Entity<TCostSubscribe>().Property(c => c.SubscribeId).HasDefaultValueSql("NEXT VALUE FOR SubscribeIdSeq");

            modelBuilder.Entity<TCost>().HasIndex(c => new { c.GroupId,  c.status}).HasAnnotation("SqlServer:Clustered", false);
            modelBuilder.Entity<TPay>().HasIndex(p => new { p.CostId, p.status }).HasAnnotation("SqlServer:Clustered", false);
            modelBuilder.Entity<TRepay>().HasIndex(r => new { r.CostId, r.status }).HasAnnotation("SqlServer:Clustered", false);
        }
    }
}
