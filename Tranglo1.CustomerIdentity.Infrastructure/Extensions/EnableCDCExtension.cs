using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Infrastructure.Extensions
{
    public static class EnableCDCExtension
    {
        public static string CDCQuery(string schema, string sourceName)
        {
            var query = $@"
                IF NOT EXISTS (select name, is_cdc_enabled
                from sys.databases
                where is_cdc_enabled = 1 AND name = '{DatabaseName}')
                BEGIN
                USE {DatabaseName} 
                EXEC sys.sp_cdc_enable_db  
                END    
                EXEC sys.sp_cdc_enable_table
                @source_schema = '{schema}', @source_name = '{sourceName}', @role_name = NULL, @supports_net_changes = 0;";

            return query;
        }

        public static string DatabaseName = "Tranglo1_CustomerIdentityP2";
    }
}
