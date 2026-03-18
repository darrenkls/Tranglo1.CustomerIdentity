using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class QuestionInputTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<QuestionInputType>
    {
        protected override void Configure(EntityTypeBuilder<QuestionInputType> builder)
        {
            builder.ToTable("QuestionInputTypes", BusinessProfileDbContext.META_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .HasColumnName("QuestionInputTypeCode");

            builder.Property(o => o.Name)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("QuestionInputTypeDescription");

            builder.HasData(Enumeration.GetAll<QuestionInputType>());
        }
    }
}
