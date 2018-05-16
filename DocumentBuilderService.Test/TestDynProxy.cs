namespace DocumentBuilderService.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using DocumentBuilderservice;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    [TestClass]
    public class TestDynProxy
    {
        private FileService _fileService;
        private string _currentDir;
        private string _badFilesDir;
        private string _outDir;
        private string _inDir;

        private string _outFile1;

        [TestInitialize]
        public void Initialize()
        {
            _inDir = @"C:\MP.ServicesTest";
            _currentDir = Directory.GetCurrentDirectory();
            _outDir = _currentDir;
            _badFilesDir = _currentDir;
            DependencyResolver.Initialize();
            var builder = DependencyResolver.For<IPdfDocumentBuilder>();
            _fileService = new FileService(_inDir, _outDir, _badFilesDir, builder);

            using (var client = new WebClient())
            {
                client.DownloadFile("http://1.bp.blogspot.com/-MD44WcoiWJs/T-u-uhS5vhI/AAAAAAAAEqM/U_137198yeg/s1600/Tiger+3D+Wallpapers+1.jpg", Path.Combine(_inDir, "Image_001.jpg"));
                client.DownloadFile("http://3.bp.blogspot.com/-xELHFxk2K1Y/U04vzF7sjVI/AAAAAAAAcIY/nsYLKp2Y3uw/s1600/Toshiba+Wallpapers+(1).jpg", Path.Combine(_inDir, "Image_002.jpg"));

                client.DownloadFile("http://2.bp.blogspot.com/-W2uGp9ckgT4/U1YV39k_III/AAAAAAAAcOM/o2GEy-Krb0A/s1600/Lake+Powell+Wallpapers+%25281%2529.jpg", Path.Combine(_inDir, "Image_005.jpg"));

                _outFile1 = Path.Combine(_outDir, "Document0.pdf");
                DeleteOutFiles();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_inDir))
            {
                Directory.Delete(_inDir, true);
            }

            DeleteOutFiles();
        }

        [TestMethod]
        public void TestLogFileCreated()
        {
            _fileService.Start();
            Thread.Sleep(10);
            _fileService.Stop();
            Assert.IsTrue(File.Exists("DynamicProxyMethodLog.txt"));
        }

        [TestMethod]
        public void TestLogFileConsistent()
        {
            _fileService.Start();
            Thread.Sleep(10);
            _fileService.Stop();
            var jsons = File.ReadAllText(@"DynamicProxyMethodLog.txt").TrimEnd(Convert.ToChar(";")).Split(Convert.ToChar(";"));
            foreach (var json in jsons)
            {
                var obj = JsonConvert.DeserializeObject(json);
                Assert.IsTrue(obj != null);
            }
        }

        private void DeleteOutFiles()
        {
            if (File.Exists(_outFile1))
            {
                File.Delete(_outFile1);
            }

            if (File.Exists("DynamicProxyMethodLog.txt"))
            {
                File.Delete("DynamicProxyMethodLog.txt");
            }
        }
    }
}
