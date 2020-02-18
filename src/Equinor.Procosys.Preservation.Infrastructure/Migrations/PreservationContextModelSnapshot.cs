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
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Journey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Journeys");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<int?>("JourneyId")
                        .HasColumnType("int");

                    b.Property<int>("ModeId")
                        .HasColumnType("int");

                    b.Property<int>("ResponsibleId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("JourneyId");

                    b.HasIndex("ModeId");

                    b.HasIndex("ResponsibleId");

                    b.ToTable("Step");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate.Mode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<Guid>("Oid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ClearedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ClearedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasMaxLength(4096);

                    b.Property<DateTime?>("DueTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClearedById");

                    b.HasIndex("TagId");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.FieldValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FieldId")
                        .HasColumnType("int");

                    b.Property<string>("FieldType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PreservationPeriodId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.HasIndex("PreservationPeriodId");

                    b.ToTable("FieldValues");

                    b.HasDiscriminator<string>("FieldType").HasValue("FieldValue");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationPeriod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(2048)")
                        .HasMaxLength(2048);

                    b.Property<DateTime>("DueTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PreservationRecordId")
                        .HasColumnType("int");

                    b.Property<int>("RequirementId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("NeedsUserInput");

                    b.HasKey("Id");

                    b.HasIndex("PreservationRecordId");

                    b.HasIndex("RequirementId");

                    b.ToTable("PreservationPeriods");

                    b.HasCheckConstraint("constraint_period_check_valid_status", "Status in ('NeedsUserInput','ReadyToBePreserved','Preserved','Stopped')");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("BulkPreserved")
                        .HasColumnType("bit");

                    b.Property<DateTime>("PreservedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("PreservedByPersonId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("PreservedByPersonId");

                    b.ToTable("PreservationRecords");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<bool>("IsClosed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IntervalWeeks")
                        .HasColumnType("int");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("NextDueTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("RequirementDefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RequirementDefinitionId");

                    b.HasIndex("TagId");

                    b.ToTable("Requirements");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AreaCode")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Calloff")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommPkgNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("DisciplineCode")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("McPkgNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("PurchaseOrderNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("NotStarted");

                    b.Property<int>("StepId")
                        .HasColumnType("int");

                    b.Property<string>("TagFunctionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TagNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("TagType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Standard");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("StepId");

                    b.ToTable("Tags");

                    b.HasCheckConstraint("constraint_tag_check_valid_status", "Status in ('NotStarted','Active','Completed')");

                    b.HasCheckConstraint("constraint_tag_check_valid_tag_type", "TagType in ('Standard','PreArea','SiteArea','PoArea')");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.Field", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FieldType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Info");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("RequirementDefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<bool?>("ShowPrevious")
                        .HasColumnType("bit");

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("RequirementDefinitionId");

                    b.ToTable("Fields");

                    b.HasCheckConstraint("constraint_field_check_valid_fieldtype", "FieldType in ('Info','Number','CheckBox','Attachment')");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DefaultIntervalWeeks")
                        .HasColumnType("int");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<int>("RequirementTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("RequirementTypeId");

                    b.ToTable("RequirementDefinitions");
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
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

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

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Responsibles");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Attachment", b =>
                {
                    b.HasBaseType("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.FieldValue");

                    b.Property<string>("BlobId")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Attachment");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.CheckBoxChecked", b =>
                {
                    b.HasBaseType("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.FieldValue");

                    b.HasDiscriminator().HasValue("CheckBoxChecked");
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.NumberValue", b =>
                {
                    b.HasBaseType("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.FieldValue");

                    b.Property<double?>("Value")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("NumberValue");
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

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate.Responsible", null)
                        .WithMany()
                        .HasForeignKey("ResponsibleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ClearedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Tag", null)
                        .WithMany("Actions")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.FieldValue", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.Field", null)
                        .WithMany()
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationPeriod", null)
                        .WithMany("FieldValues")
                        .HasForeignKey("PreservationPeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationPeriod", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationRecord", "PreservationRecord")
                        .WithMany()
                        .HasForeignKey("PreservationRecordId");

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement", null)
                        .WithMany("PreservationPeriods")
                        .HasForeignKey("RequirementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.PreservationRecord", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("PreservedByPersonId")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", null)
                        .WithMany()
                        .HasForeignKey("RequirementDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Tag", null)
                        .WithMany("Requirements")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Tag", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Project", null)
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate.Step", null)
                        .WithMany()
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.Field", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", null)
                        .WithMany("Fields")
                        .HasForeignKey("RequirementDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementDefinition", b =>
                {
                    b.HasOne("Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType", null)
                        .WithMany("RequirementDefinitions")
                        .HasForeignKey("RequirementTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
