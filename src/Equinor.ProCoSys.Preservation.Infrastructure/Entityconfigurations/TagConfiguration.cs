﻿using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Description)
                .HasMaxLength(Tag.DescriptionLengthMax);

            builder.Property(x => x.Remark)
                .HasMaxLength(Tag.RemarkLengthMax);

            builder.Property(x => x.StorageArea)
                .HasMaxLength(Tag.StorageAreaLengthMax);

            builder.Property(x => x.TagNo)
                .HasMaxLength(Tag.TagNoLengthMax)
                .IsRequired();

            builder.Property(x => x.TagFunctionCode)
                .HasMaxLength(Tag.TagFunctionCodeLengthMax);

            builder.Property(x => x.AreaCode)
                .HasMaxLength(Tag.AreaCodeLengthMax);

            builder.Property(x => x.AreaDescription)
                .HasMaxLength(Tag.AreaDescriptionLengthMax);

            builder.Property(x => x.DisciplineCode)
                .HasMaxLength(Tag.DisciplineCodeLengthMax);

            builder.Property(x => x.DisciplineDescription)
                .HasMaxLength(Tag.DisciplineDescriptionLengthMax);

            builder.Property(x => x.PurchaseOrderNo)
                .HasMaxLength(Tag.PurchaseOrderNoLengthMax);

            builder.Property(x => x.McPkgNo)
                .HasMaxLength(Tag.McPkgNoLengthMax);

            builder.Property(x => x.CommPkgNo)
                .HasMaxLength(Tag.CommPkgNoLengthMax);

            builder.Property(x => x.Calloff)
                .HasMaxLength(Tag.CalloffLengthMax);

            builder.HasOne<Step>()
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();

            builder
                .HasMany(x => x.Actions)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.Attachments)
                .WithOne()
                .IsRequired();

            builder.Property(f => f.Status)
                .HasDefaultValue(PreservationStatus.NotStarted)
                .IsRequired();

            builder.Property(f => f.TagType)
                .HasConversion<string>()
                .HasDefaultValue(TagType.Standard)
                .IsRequired();
            
            builder.Property(x => x.NextDueTimeUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);

            builder.ToTable(t => t.HasCheckConstraint(
                "constraint_tag_check_valid_statusenum", $"{nameof(Tag.Status)} in ({GetValidStatusEnums()})"));

            builder.ToTable(t => t.HasCheckConstraint(
                "constraint_tag_check_valid_tag_type", $"{nameof(Tag.TagType)} in ({GetValidTagTypes()})"));
            
            builder
                .HasIndex(nameof(Tag.Plant), nameof(Tag.TagNo), "ProjectId") // ProjectId is a shadow property
                .IsUnique();
            
            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_Tags_Plant_ASC")
                .IncludeProperties(x => x.TagNo);

            builder
                .HasIndex(x => x.TagNo)
                .HasDatabaseName("IX_Tags_TagNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.Description,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagType
                }); 
            
            builder
                .HasIndex(x => x.CommPkgNo)
                .HasDatabaseName("IX_Tags_CommPkgNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.Description,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.McPkgNo)
                .HasDatabaseName("IX_Tags_McPkgNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.Description,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.Calloff)
                .HasDatabaseName("IX_Tags_Calloff_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.PurchaseOrderNo)
                .HasDatabaseName("IX_Tags_PurchaseOrderNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.StorageArea)
                .HasDatabaseName("IX_Tags_StorageArea_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

        }

        private string GetValidStatusEnums()
        {
            var values = Enum.GetValues(typeof(PreservationStatus)).Cast<int>();
            return string.Join(',', values);
        }

        private string GetValidTagTypes()
        {
            var names = Enum.GetNames(typeof(TagType)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}
