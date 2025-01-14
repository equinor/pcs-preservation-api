﻿using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(f => f.Label)
                .HasMaxLength(Field.LabelLengthMax)
                .IsRequired();

            builder.Property(f => f.Unit)
                .HasMaxLength(Field.UnitLengthMax);

            builder.Property(f => f.FieldType)
                .HasConversion<string>()
                .HasDefaultValue(FieldType.Info)
                .IsRequired();

            builder.HasMany<FieldValue>()
                .WithOne()
                .HasForeignKey(pr => pr.FieldId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.ToTable(t => t.HasCheckConstraint(
                "constraint_field_check_valid_fieldtype", $"{nameof(Field.FieldType)} in ({GetValidFieldTypes()})"));
        }

        private string GetValidFieldTypes()
        {
            var fieldTypes = Enum.GetNames(typeof(FieldType)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}
