 public class Program {
        public static void Main(string[] args) {
               CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// Start Method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureAppConfiguration((hostingContext, configBuilder) => {
                var config = configBuilder.Build();
                var configSource = new EFConfigurationSource(opts =>
                    opts.UseSqlServer(config.GetConnectionString("YourConnectionString")));
                configBuilder.Add(configSource);
            });
    }
