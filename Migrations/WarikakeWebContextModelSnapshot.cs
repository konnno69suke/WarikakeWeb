﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarikakeWeb.Data;

#nullable disable

namespace WarikakeWeb.Migrations
{
    [DbContext(typeof(WarikakeWebContext))]
    partial class WarikakeWebContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence<int>("CostIdSeq")
                .StartsAt(107L);

            modelBuilder.HasSequence<int>("SubscribeIdSeq")
                .StartsAt(12L);

            modelBuilder.Entity("WarikakeWeb.Entities.CsvMigration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("buyAmount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("buyDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("buyStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("importId")
                        .HasColumnType("int");

                    b.Property<string>("inputDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("kindName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pa1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pa2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pa3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pf1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pf2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pf3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pr1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pr2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pr3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CsvMigration");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.MGenre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("GenreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MGenre");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.MGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MGroup");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.MMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MMember");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.MSalt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<byte[]>("salt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MSalt");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.MUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MUser");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TCost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CostAmount")
                        .HasColumnType("int");

                    b.Property<DateTime>("CostDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR CostIdSeq");

                    b.Property<int>("CostStatus")
                        .HasColumnType("int");

                    b.Property<string>("CostTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("GenreName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("ProvisionFlg")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId", "status");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("GroupId", "status"), false);

                    b.ToTable("TCost");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TCostSubscribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CostAmount")
                        .HasColumnType("int");

                    b.Property<int>("CostStatus")
                        .HasColumnType("int");

                    b.Property<string>("CostTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("GenreName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("ProvisionFlg")
                        .HasColumnType("int");

                    b.Property<int>("SubscribeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR SubscribeIdSeq");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TCostSubscribe");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TDateSubscribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SubscribeId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("d1")
                        .HasColumnType("bit");

                    b.Property<bool>("d10")
                        .HasColumnType("bit");

                    b.Property<bool>("d11")
                        .HasColumnType("bit");

                    b.Property<bool>("d12")
                        .HasColumnType("bit");

                    b.Property<bool>("d13")
                        .HasColumnType("bit");

                    b.Property<bool>("d14")
                        .HasColumnType("bit");

                    b.Property<bool>("d15")
                        .HasColumnType("bit");

                    b.Property<bool>("d16")
                        .HasColumnType("bit");

                    b.Property<bool>("d17")
                        .HasColumnType("bit");

                    b.Property<bool>("d18")
                        .HasColumnType("bit");

                    b.Property<bool>("d19")
                        .HasColumnType("bit");

                    b.Property<bool>("d2")
                        .HasColumnType("bit");

                    b.Property<bool>("d20")
                        .HasColumnType("bit");

                    b.Property<bool>("d21")
                        .HasColumnType("bit");

                    b.Property<bool>("d22")
                        .HasColumnType("bit");

                    b.Property<bool>("d23")
                        .HasColumnType("bit");

                    b.Property<bool>("d24")
                        .HasColumnType("bit");

                    b.Property<bool>("d25")
                        .HasColumnType("bit");

                    b.Property<bool>("d26")
                        .HasColumnType("bit");

                    b.Property<bool>("d27")
                        .HasColumnType("bit");

                    b.Property<bool>("d28")
                        .HasColumnType("bit");

                    b.Property<bool>("d29")
                        .HasColumnType("bit");

                    b.Property<bool>("d3")
                        .HasColumnType("bit");

                    b.Property<bool>("d30")
                        .HasColumnType("bit");

                    b.Property<bool>("d31")
                        .HasColumnType("bit");

                    b.Property<bool>("d4")
                        .HasColumnType("bit");

                    b.Property<bool>("d5")
                        .HasColumnType("bit");

                    b.Property<bool>("d6")
                        .HasColumnType("bit");

                    b.Property<bool>("d7")
                        .HasColumnType("bit");

                    b.Property<bool>("d8")
                        .HasColumnType("bit");

                    b.Property<bool>("d9")
                        .HasColumnType("bit");

                    b.Property<bool>("m1")
                        .HasColumnType("bit");

                    b.Property<bool>("m10")
                        .HasColumnType("bit");

                    b.Property<bool>("m11")
                        .HasColumnType("bit");

                    b.Property<bool>("m12")
                        .HasColumnType("bit");

                    b.Property<bool>("m2")
                        .HasColumnType("bit");

                    b.Property<bool>("m3")
                        .HasColumnType("bit");

                    b.Property<bool>("m4")
                        .HasColumnType("bit");

                    b.Property<bool>("m5")
                        .HasColumnType("bit");

                    b.Property<bool>("m6")
                        .HasColumnType("bit");

                    b.Property<bool>("m7")
                        .HasColumnType("bit");

                    b.Property<bool>("m8")
                        .HasColumnType("bit");

                    b.Property<bool>("m9")
                        .HasColumnType("bit");

                    b.Property<bool>("r1")
                        .HasColumnType("bit");

                    b.Property<bool>("r2")
                        .HasColumnType("bit");

                    b.Property<bool>("r3")
                        .HasColumnType("bit");

                    b.Property<bool>("r4")
                        .HasColumnType("bit");

                    b.Property<bool>("r5")
                        .HasColumnType("bit");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.Property<bool>("w1")
                        .HasColumnType("bit");

                    b.Property<bool>("w2")
                        .HasColumnType("bit");

                    b.Property<bool>("w3")
                        .HasColumnType("bit");

                    b.Property<bool>("w4")
                        .HasColumnType("bit");

                    b.Property<bool>("w5")
                        .HasColumnType("bit");

                    b.Property<bool>("w6")
                        .HasColumnType("bit");

                    b.Property<bool>("w7")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("TDateSubscribe");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TPay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CostId")
                        .HasColumnType("int");

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PayAmount")
                        .HasColumnType("int");

                    b.Property<int>("PayId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CostId", "status");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("CostId", "status"), false);

                    b.ToTable("TPay");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TPaySubscribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PayAmount")
                        .HasColumnType("int");

                    b.Property<int>("PayId")
                        .HasColumnType("int");

                    b.Property<int>("SubscribeId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TPaySubscribe");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TRepay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CostId")
                        .HasColumnType("int");

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RepayAmount")
                        .HasColumnType("int");

                    b.Property<int>("RepayId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CostId", "status");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("CostId", "status"), false);

                    b.ToTable("TRepay");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TRepaySubscribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RepayAmount")
                        .HasColumnType("int");

                    b.Property<int>("RepayId")
                        .HasColumnType("int");

                    b.Property<int>("SubscribeId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TRepaySubscribe");
                });

            modelBuilder.Entity("WarikakeWeb.Entities.TSubscribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CostId")
                        .HasColumnType("int");

                    b.Property<string>("CreatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SubscribeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubscribedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatePg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TSubscribe");
                });
#pragma warning restore 612, 618
        }
    }
}
