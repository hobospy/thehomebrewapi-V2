﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using thehomebrewapi.Contexts;

namespace thehomebrewapi.Migrations
{
    [DbContext(typeof(HomeBrewContext))]
    [Migration("20210113104335_SampleData")]
    partial class SampleData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("thehomebrewapi.Entities.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("Ingredients");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 68.0,
                            Name = "Amarillo",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 2,
                            Amount = 5.5,
                            Name = "Pale malt",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 3,
                            Amount = 150.0,
                            Name = "Light crystal malt",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 4,
                            Amount = 300.0,
                            Name = "Dark roast malt",
                            RecipeId = 2
                        },
                        new
                        {
                            Id = 5,
                            Amount = 60.0,
                            Name = "Bittering",
                            RecipeId = 2
                        },
                        new
                        {
                            Id = 6,
                            Amount = 120.0,
                            Name = "Dark crystal",
                            RecipeId = 3
                        },
                        new
                        {
                            Id = 7,
                            Amount = 300.0,
                            Name = "Rice husks",
                            RecipeId = 3
                        },
                        new
                        {
                            Id = 8,
                            Amount = 0.5,
                            Name = "Whirlfloc",
                            RecipeId = 3
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Recipes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Single malt and single hop recipe using Amarillo hops",
                            Name = "Amarillo SMaSH"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Porter based recipe with raspberries added during the second half of fermentation",
                            Name = "Raspberry Porter"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Basic Bock recipe for those cold nights",
                            Name = "Bock"
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Ingredient", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.Recipe", "Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
