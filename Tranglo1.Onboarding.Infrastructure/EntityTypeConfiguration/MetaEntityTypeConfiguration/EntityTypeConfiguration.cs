using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;

namespace Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration.MetaEntityTypeConfiguration
{
    class EntityTypeConfiguration : BaseEntityTypeConfiguration<TrangloEntity>
    {
        protected override void Configure(EntityTypeBuilder<TrangloEntity> builder)
        {
           
        }
    }


}
