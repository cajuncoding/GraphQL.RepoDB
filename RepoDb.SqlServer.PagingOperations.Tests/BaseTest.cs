using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqlServer.PagingOperations.Tests
{
    public abstract class BaseTest
    {
        protected BaseTest()
        {
            CurrentDirectory = Directory.GetCurrentDirectory();
        }

        public string CurrentDirectory { get; }

        public TestContext TestContext { get; set; } = null;

        public string LoadTestData(string fileName)
        {
            var filePath = Path.Combine(CurrentDirectory, $@"TestData\{fileName}");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The Test Data file [{fileName}] could not be found at: [{filePath}].", fileName);

            return File.ReadAllText(Path.Combine(CurrentDirectory, filePath));
        }
    }
}
