using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        public static void Seed( this ModelBuilder modelBuilder )
        {
            //In case need to seed from csv instead
            //SeedModule(modelBuilder);
        }

        private static void SeedModule(ModelBuilder modelBuilder)
        {
            //CSV to place under "MappingModel" folder. This is to ensure that CSV format is as expected from user 
            /*
            string resourceName = GetResourceName(modelBuilder, typeof(Module).Name );
            var moduleList = new List<Module>();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csvReader.GetRecords<ModuleCSV>().ToList();
                {
                    foreach (var item in records)
                    {
                        var module = new Module(item.ModuleCode, item.ModuleName, item.PortalCode);
                        moduleList.Add(module);
                    }
                }
            }
            modelBuilder.Entity<Module>().HasData(moduleList);
            */
        }

        private static string GetResourceName(ModelBuilder modelBuilder, string entityName)
        {
            string entityNamePlural = entityName + "s";
            string csvName = entityNamePlural + ".csv";

            string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Single(str => str.EndsWith(csvName));

            return resourceName;
        }
    }
}
