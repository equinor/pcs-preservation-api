﻿// <auto-generated />
using System;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    [DbContext(typeof(PreservationContext))]
    partial class PreservationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Journey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Journeys");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("JourneyId")
                        .HasColumnType("int");

                    b.Property<int>("ModeId")
                        .HasColumnType("int");

                    b.Property<int>("ResponsibleId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("JourneyId");

                    b.HasIndex("ModeId");

                    b.ToTable("Step");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate.Mode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("RequirementTypes");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate.Responsible", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Responsibles");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAreaTag")
                        .HasColumnType("bit");

                    b.Property<string>("ProjectNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StepId")
                        .HasColumnType("int");

                    b.Property<string>("TagNo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Journey", null)
                        .WithMany("Steps")
                        .HasForeignKey("JourneyId");

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate.Mode", null)
                        .WithMany()
                        .HasForeignKey("ModeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate.Tag", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", null)
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
