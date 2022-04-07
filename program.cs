 public class Program {
        public static void Main(string[] args) {
            try {
                CreateHostBuilder(args).Build().Run();
            } catch (Exception ex) {
                writeerrorOnStartup(ex.ToString());
            }
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
        private static void writeerrorOnStartup(string error) {
            File.WriteAllText(@"D:\www\ServerDemo\Services\KironVirtualDemo\Logs\error.log", error);
        }
    }
