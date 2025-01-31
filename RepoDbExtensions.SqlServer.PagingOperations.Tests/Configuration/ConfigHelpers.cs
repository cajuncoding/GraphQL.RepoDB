using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RepoDb.SqlServer.PagingOperations.Tests
{
    public class ConfigHelpers
    {
        /// <summary>
        /// BBernard
        /// Settings Adaptation to load local.settings.json file into Environment so that Azure Functions Tests can
        /// then be used directly with valid settings reading via Environment.GetEnvironmentVariable
        /// Original Source inspired by Stack Overflow answer here:
        ///     https://stackoverflow.com/a/50223191/7293142
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void InitEnvironmentFromLocalSettingsJson(string jsonFileName = null)
        {
            var settingsFileName = jsonFileName ?? "local.settings.json";
            var basePath = Directory.GetCurrentDirectory();
            var localSettingsJsonText = File.ReadAllText(Path.Combine(basePath, settingsFileName));
            var localSettingsJson = JObject.Parse(localSettingsJsonText);

            var valuesJson = (JObject)(localSettingsJson["Values"] ?? throw new Exception($"'Values' node cannot be found in file [{settingsFileName}]."));
            foreach (var setting in valuesJson.Properties())
            {
                Environment.SetEnvironmentVariable(setting.Name, setting.Value.ToString());
            }
        }
    }
}
