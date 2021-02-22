﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using thehomebrewapi.Contexts;

namespace thehomebrewapi.Migrations
{
    [DbContext(typeof(HomeBrewContext))]
    [Migration("20210215005958_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("thehomebrewapi.Entities.Brew", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("ABV")
                        .HasColumnType("float");

                    b.Property<DateTime>("BrewDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("BrewedState")
                        .HasColumnType("int");

                    b.Property<string>("BrewingNotes")
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("Brews");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ABV = 4.2999999999999998,
                            BrewDate = new DateTime(2020, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BrewedState = 2,
                            BrewingNotes = "The yeast in this one didn't settle",
                            Name = "First Brew",
                            Rating = 3.0,
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 2,
                            ABV = 4.0999999999999996,
                            BrewDate = new DateTime(2021, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BrewedState = 0,
                            Name = "Gonna be better brew",
                            Rating = 0.0,
                            RecipeId = 1
                        });
                });

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

                    b.Property<int>("RecipeStepId")
                        .HasColumnType("int");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipeStepId");

                    b.ToTable("Ingredients");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 68.0,
                            Name = "Amarillo",
                            RecipeStepId = 2,
                            Unit = 1
                        },
                        new
                        {
                            Id = 2,
                            Amount = 5.5,
                            Name = "Pale malt",
                            RecipeStepId = 2,
                            Unit = 0
                        },
                        new
                        {
                            Id = 3,
                            Amount = 150.0,
                            Name = "Light crystal malt",
                            RecipeStepId = 2,
                            Unit = 1
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

                    b.Property<double>("ExpectedABV")
                        .HasColumnType("float");

                    b.Property<bool>("Favourite")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<int>("WaterProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WaterProfileId");

                    b.ToTable("Recipes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Single malt and single hop recipe using Amarillo hops",
                            ExpectedABV = 3.5,
                            Favourite = true,
                            Name = "Amarillo SMaSH",
                            Type = (short)7,
                            WaterProfileId = 1
                        },
                        new
                        {
                            Id = 2,
                            Description = "Porter based recipe with raspberries added during the second half of fermentation",
                            ExpectedABV = 5.2000000000000002,
                            Favourite = false,
                            Name = "Raspberry Porter",
                            Type = (short)10,
                            WaterProfileId = 2
                        },
                        new
                        {
                            Id = 3,
                            Description = "Basic Bock recipe for those cold nights",
                            ExpectedABV = 6.7999999999999998,
                            Favourite = false,
                            Name = "Bock",
                            Type = (short)16,
                            WaterProfileId = 2
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.RecipeStep", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeSteps");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Get the water up to mash in temp",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 2,
                            Description = "Mash in the grains",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 3,
                            Description = "Mash out",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 4,
                            Description = "Raise temp to a rolling boil",
                            RecipeId = 1
                        },
                        new
                        {
                            Id = 5,
                            Description = "Add bittering hops",
                            RecipeId = 1
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.TastingNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrewID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("BrewID");

                    b.ToTable("TastingNotes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BrewID = 1,
                            Date = new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Note = "Oooooh so tasty"
                        },
                        new
                        {
                            Id = 2,
                            BrewID = 1,
                            Date = new DateTime(2021, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Note = "Oh my god, what happened, has someone poisoned this!!"
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Timer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Duration")
                        .HasColumnType("bigint");

                    b.Property<int>("RecipeStepId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipeStepId")
                        .IsUnique();

                    b.ToTable("Timers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Duration = 3600L,
                            RecipeStepId = 2,
                            Type = 1
                        },
                        new
                        {
                            Id = 2,
                            Duration = 600L,
                            RecipeStepId = 5,
                            Type = 2
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.WaterProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("WaterProfiles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Light beer water profile",
                            Name = "Pilsner"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Dark beer water profile",
                            Name = "Stout"
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.WaterProfileAddition", b =>
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

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.Property<int>("WaterProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WaterProfileId");

                    b.ToTable("WaterProfileAdditions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 300.0,
                            Name = "Light crystal malt",
                            Unit = 1,
                            WaterProfileId = 1
                        });
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Brew", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.Recipe", "Recipe")
                        .WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Ingredient", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.RecipeStep", "RecipeStep")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeStepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Recipe", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.WaterProfile", "WaterProfile")
                        .WithMany()
                        .HasForeignKey("WaterProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.RecipeStep", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.Recipe", "Recipe")
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.TastingNote", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.Brew", "Brew")
                        .WithMany("TastingNotes")
                        .HasForeignKey("BrewID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.Timer", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.RecipeStep", "RecipeStep")
                        .WithOne("Timer")
                        .HasForeignKey("thehomebrewapi.Entities.Timer", "RecipeStepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("thehomebrewapi.Entities.WaterProfileAddition", b =>
                {
                    b.HasOne("thehomebrewapi.Entities.WaterProfile", "WaterProfile")
                        .WithMany("Additions")
                        .HasForeignKey("WaterProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}