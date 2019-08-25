using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI
{
    public static class AppSettingsJson
    {
        public static string ApplicationExeDirectory()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appRoot = Path.GetDirectoryName(location);

            return appRoot;
        }

        public static IConfigurationRoot GetAppSettings()
        {
            string applicationExeDirectory = ApplicationExeDirectory();
            var builder = new ConfigurationBuilder()
            .SetBasePath(applicationExeDirectory)
            .AddJsonFile("appsettings.json");

            //IHostingEnvironment env;
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath) // the path where the json file should be loaded from
            //    .AddEnvironmentVariables();
            //.SetBasePath(applicationExeDirectory)
            //.AddJsonFile("appsettings.json");

            return builder.Build();
        }


    }
}
