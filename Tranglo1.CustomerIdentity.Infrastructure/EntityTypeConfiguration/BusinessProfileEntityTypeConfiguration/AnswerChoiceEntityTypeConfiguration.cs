using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration
{
    class AnswerChoiceEntityTypeConfiguration : BaseEntityTypeConfiguration<AnswerChoice>
    {
        protected override void Configure(EntityTypeBuilder<AnswerChoice> builder)
        {
            builder.ToTable("AnswerChoices", BusinessProfileDbContext.DEFAULT_SCHEMA);

            builder.HasTemporalTable(config =>
            {
                config.HistorySchema(BusinessProfileDbContext.HISTORY_SCHEMA);
                config.HistoryTable("AnswerChoices");
            });

            //Primary Key
            builder.Property(kyc => kyc.Id)
                    .HasColumnName("AnswerChoiceCode");
            builder.HasKey(kyc => kyc.Id);

            builder.Property(kyc => kyc.Description)
                    .HasColumnName("AnswerChoiceDescription");

            builder.HasOne(o => o.Question)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey("QuestionCode");

        }
    }
}
