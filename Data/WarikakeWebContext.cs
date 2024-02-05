using Microsoft.EntityFrameworkCore;
using WarikakeWeb.Entities;

namespace WarikakeWeb.Data
{
    public class WarikakeWebContext : DbContext
    {
        public WarikakeWebContext (DbContextOptions<WarikakeWebContext> options)
            : base(options)
        {
        }

        // テーブル
        public DbSet<MGenre> MGenre { get; set; } = default!;
        public DbSet<MGroup> MGroup { get; set; } = default!;
        public DbSet<MMember> MMember { get; set; } = default!;
        public DbSet<MUser> MUser { get; set; } = default!;
        public DbSet<MSalt> MSalt { get; set; }
        public DbSet<TCost> TCost { get; set; } = default!;
        public DbSet<TPay> TPay { get; set; } = default!;
        public DbSet<TRepay> TRepay { get; set; } = default!;
        public DbSet<TCostSubscribe> TCostSubscribe { get; set; } = default!;
        public DbSet<TPaySubscribe> TPaySubscribe { get; set; } = default!;
        public DbSet<TRepaySubscribe> TRepaySubscribe { get; set; } = default!;
        public DbSet<TDateSubscribe> TDateSubscribe { get; set; }
        public DbSet<TSubscribe> TSubscribe { get; set; }
        public DbSet<WCsvMigration> WCsvMigration { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // シーケンス
            modelBuilder.HasSequence<int>("CostIdSeq").StartsAt(107).IncrementsBy(1);
            modelBuilder.Entity<TCost>().Property(c => c.CostId).HasDefaultValueSql("NEXT VALUE FOR CostIdSeq");

            modelBuilder.HasSequence<int>("SubscribeIdSeq").StartsAt(12).IncrementsBy(1);
            modelBuilder.Entity<TCostSubscribe>().Property(c => c.SubscribeId).HasDefaultValueSql("NEXT VALUE FOR SubscribeIdSeq");

            // インデックス
            modelBuilder.Entity<TCost>().HasIndex(c => new { c.GroupId,  c.status}).HasAnnotation("SqlServer:Clustered", false);
            modelBuilder.Entity<TPay>().HasIndex(p => new { p.CostId, p.status }).HasAnnotation("SqlServer:Clustered", false);
            modelBuilder.Entity<TRepay>().HasIndex(r => new { r.CostId, r.status }).HasAnnotation("SqlServer:Clustered", false);
        }
    }
}
