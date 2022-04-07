using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KironVirtual.DataAccessLayer.model {
    public class ConfigurationEntity {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class ConfigurationDbContext : DbContext {
        public ConfigurationDbContext(DbContextOptions options)
            : base(options) {
        }

        public DbSet<ConfigurationEntity> ConfigurationEntities { get; set; }
    }
    public class EFConfigurationProvider : ConfigurationProvider {
        public EFConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction) {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        public override void Load() {
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();

            OptionsAction(builder);

            using (var dbContext = new ConfigurationDbContext(builder.Options)) {
                dbContext.Database.EnsureCreated();

                Data = !dbContext.ConfigurationEntities.Any()
                    ? CreateAndSaveDefaultValues(dbContext)
                    : dbContext.ConfigurationEntities.ToDictionary(c => c.Key, c => c.Value);
            }
        }

        private static IDictionary<string, string> CreateAndSaveDefaultValues(ConfigurationDbContext dbContext) {
            var configValues =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            dbContext.ConfigurationEntities.AddRange(configValues
                .Select(kvp => new ConfigurationEntity {
                    Key = kvp.Key,
                    Value = kvp.Value
                })
                .ToArray());

            dbContext.SaveChanges();

            return configValues;
        }
    }
    public class EFConfigurationSource : IConfigurationSource {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public EFConfigurationSource(Action<DbContextOptionsBuilder> optionsAction) {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new EFConfigurationProvider(_optionsAction);
        }
    }
}
