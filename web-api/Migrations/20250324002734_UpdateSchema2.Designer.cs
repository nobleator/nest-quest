﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace NestQuestApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250324002734_UpdateSchema2")]
    partial class UpdateSchema2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("CacheEntry", b =>
                {
                    b.Property<string>("Parameters")
                        .HasColumnType("TEXT");

                    b.Property<string>("Response")
                        .HasColumnType("TEXT");

                    b.HasKey("Parameters");

                    b.ToTable("CacheEntries");
                });

            modelBuilder.Entity("Criterion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Direction")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tolerance")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Unit")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Criteria");
                });
#pragma warning restore 612, 618
        }
    }
}
