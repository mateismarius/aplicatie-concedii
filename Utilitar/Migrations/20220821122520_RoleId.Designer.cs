﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Utilitar.AppContext;

#nullable disable

namespace Utilitar.Migrations
{
    [DbContext(typeof(UtilitiesContext))]
    [Migration("20220821122520_RoleId")]
    partial class RoleId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Utilitar.Models.DayOffType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPaid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("DayOffTypes");
                });

            modelBuilder.Entity("Utilitar.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DataFinal")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataStart")
                        .HasColumnType("datetime2");

                    b.Property<int>("DrepturiCurente")
                        .HasColumnType("int");

                    b.Property<int>("DrepturiRestante")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDelegate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Marca")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermanentLocation")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Utilitar.Models.FreeDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DayOffTypeId")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FinishDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DayOffTypeId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("FreeDay");
                });

            modelBuilder.Entity("Utilitar.Models.LegallDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateOff")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LegallDay");
                });

            modelBuilder.Entity("Utilitar.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("PriorityNo")
                        .HasColumnType("int");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Utilitar.Models.Employee", b =>
                {
                    b.HasOne("Utilitar.Models.Role", "Roles")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Utilitar.Models.FreeDay", b =>
                {
                    b.HasOne("Utilitar.Models.DayOffType", "DayOffType")
                        .WithMany("FreeDays")
                        .HasForeignKey("DayOffTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Utilitar.Models.Employee", "Employee")
                        .WithMany("FreeDays")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DayOffType");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Utilitar.Models.DayOffType", b =>
                {
                    b.Navigation("FreeDays");
                });

            modelBuilder.Entity("Utilitar.Models.Employee", b =>
                {
                    b.Navigation("FreeDays");
                });
#pragma warning restore 612, 618
        }
    }
}
