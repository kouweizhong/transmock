﻿/***************************************
//   Copyright 2014 - Svetoslav Vasilev

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
*****************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
//using System.Configuration.Install;
using System.ServiceModel.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TransMock.Deploy.Utils;


namespace TransMock.Deploy.CustomActions.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestTransMockInstaller
    {
        private static System.Reflection.Assembly adapterAssembly;

        public TestTransMockInstaller()
        {            
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void TestSuitSetup(TestContext testContext) 
        {
            //First create a temp copy of the machine.config files
            BackupConfigFiles();
            //Loading the Adapter assembly in memory
            System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();            

            procInfo.EnvironmentVariables["PATH"] = CreatePathVariable();
            procInfo.FileName = @"gacutil.exe";
            //procInfo.WorkingDirectory = @"C:\Program files (x86)\Microsoft Visual Studio 11.0\VC";
            procInfo.Arguments = @"/if ..\..\..\..\Adapter\TransMock.Wcf.Adapter\bin\Debug\TransMock.Wcf.Adapter.dll";
            procInfo.CreateNoWindow = true;
            procInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            procInfo.UseShellExecute = false;

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = procInfo;
                p.Start();
                p.WaitForExit();
            }                      

        }
        
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void TestSuitCleanUp() 
        {
            //Restore config files
            RestoreConfigFiles();

            System.Reflection.Assembly adapterAssembly = 
                System.Reflection.Assembly.LoadFrom(@"..\..\..\..\Adapter\TransMock.Wcf.Adapter\bin\Debug\TransMock.Wcf.Adapter.dll");

            System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();

            procInfo.EnvironmentVariables["PATH"] = CreatePathVariable();
            procInfo.FileName = @"gacutil.exe";
            procInfo.Arguments = @"/u " + adapterAssembly.FullName.Replace(" ", string.Empty);
            procInfo.CreateNoWindow = true;
            procInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            procInfo.UseShellExecute = false;

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = procInfo;
                p.Start();
                p.WaitForExit();
            }
        }

        private static string CreatePathVariable()
        {
            string pathVar = System.Environment.GetEnvironmentVariable("PATH");
            
            if (!pathVar.Contains(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools;"))
            {
                pathVar += @";C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools;";
            }

            if (!pathVar.Contains(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin"))
            {
                pathVar += @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin";
            }

            if (!pathVar.Contains(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools;"))
            {
                pathVar += @"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools;";
            }

            System.Environment.SetEnvironmentVariable("PATH", pathVar);

            return pathVar;
        }

        private static void BackupConfigFiles()
        {
            Configuration config = ConfigurationManager.OpenMachineConfiguration();

            Assert.IsNotNull(config, "Machine.Config returned null");

            config.SaveAs(
                Path.Combine(Path.GetDirectoryName(config.FilePath), 
                "machine.config.test"));

            if (Environment.Is64BitOperatingSystem)
            {
                config = Get64bitMachineConfig();

                Assert.IsNotNull(config, "Machine.Config returned null");

                config.SaveAs(
                    Path.Combine(Path.GetDirectoryName(config.FilePath),
                    "machine.config.test"));
            }
        }

        private static void RestoreConfigFiles()
        {
            Configuration config = ConfigurationManager.OpenMachineConfiguration();

            Assert.IsNotNull(config, "Machine.Config returned null");

            string path = System.IO.Path.GetDirectoryName(config.FilePath);

            if (File.Exists(Path.Combine(path, "machine.config.test")))
            {
                System.IO.File.Copy(Path.Combine(path, "machine.config.test"), config.FilePath, true);
            }

            if (Environment.Is64BitOperatingSystem)
            {
                config = Get64bitMachineConfig();

                Assert.IsNotNull(config, "Machine.Config returned null");

                path = Path.GetDirectoryName(config.FilePath);

                if (File.Exists(Path.Combine(path, "machine.config.test")))
                {
                    File.Copy(Path.Combine(path, "machine.config.test"), config.FilePath, true);
                }
            }            
        }
        
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]        
        public void TestInstall_32Bit()
        {            
            Configuration config = ConfigurationManager.OpenMachineConfiguration();

            Assert.IsNotNull(config, "Machine.Config returned null");

            MachineConfigManager.AddMachineConfigurationInfo(
                @"..\..\..\..\Adapter\TransMock.Wcf.Adapter\bin\Debug",
                config);

            config = ConfigurationManager.OpenMachineConfiguration();

            Assert.IsNotNull(config, "Machine.Config returned null");

            ServiceModelSectionGroup sectionGroup = config.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;
                        
            Assert.IsTrue(sectionGroup.Extensions.BindingElementExtensions.ContainsKey("mockTransport"),
                "The mockBinding element extension is not present in the machine.config");

            Assert.IsTrue(sectionGroup.Extensions.BindingExtensions.ContainsKey("mockBinding"),
                "The mockBinding extention is not present in the machine.config");
        }

        [TestMethod]
        public void TestInstall_64Bit()
        {
            if (!System.Environment.Is64BitOperatingSystem)
            {
                return;
            }

            Configuration config = Get64bitMachineConfig();

            Assert.IsNotNull(config, "Machine.Config returned null");

            MachineConfigManager.AddMachineConfigurationInfo(
                @"..\..\..\..\Adapter\TransMock.Wcf.Adapter\bin\Debug",
                config);

            config = Get64bitMachineConfig();

            ServiceModelSectionGroup sectionGroup = config.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;

            Assert.IsTrue(sectionGroup.Extensions.BindingElementExtensions.ContainsKey("mockTransport"),
                "The mockBinding element extension is not present in the machine.config");

            Assert.IsTrue(sectionGroup.Extensions.BindingExtensions.ContainsKey("mockBinding"),
                "The mockBinding extention is not present in the machine.config");
        }

        [TestMethod]        
        public void TestUninstall_32bit()
        {
            Configuration config = ConfigurationManager.OpenMachineConfiguration();
            Assert.IsNotNull(config, "Machine.Config returned null");

            MachineConfigManager.RemoveMachineConfigurationInfo(config);

            config = ConfigurationManager.OpenMachineConfiguration();
            Assert.IsNotNull(config, "Machine.Config returned null");

            ServiceModelSectionGroup sectionGroup = config.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;
                        
            Assert.IsFalse(sectionGroup.Extensions.BindingElementExtensions.ContainsKey("mockTransport"),
                "The mockBinding element extension is not present in the machine.config");

            Assert.IsFalse(sectionGroup.Extensions.BindingExtensions.ContainsKey("mockBinding"),
                "The mockBinding extention is not present in the machine.config");
        }

        [TestMethod]
        public void TestUninstall_64bit()
        {
            if (!System.Environment.Is64BitOperatingSystem)
            {
                return;
            }

            Configuration config = Get64bitMachineConfig();
            Assert.IsNotNull(config, "Machine.Config returned null");

            MachineConfigManager.RemoveMachineConfigurationInfo(config);

            config = Get64bitMachineConfig();
            Assert.IsNotNull(config, "Machine.Config returned null");

            ServiceModelSectionGroup sectionGroup = config.GetSectionGroup("system.serviceModel") as ServiceModelSectionGroup;

            Assert.IsFalse(sectionGroup.Extensions.BindingElementExtensions.ContainsKey("mockTransport"),
                "The mockBinding element extension is not present in the machine.config");

            Assert.IsFalse(sectionGroup.Extensions.BindingExtensions.ContainsKey("mockBinding"),
                "The mockBinding extention is not present in the machine.config");
        }

        private static Configuration Get64bitMachineConfig()
        {
            string machineConfigPathFor64Bit = System.Runtime.InteropServices.RuntimeEnvironment
                .GetRuntimeDirectory().Replace("Framework", "Framework64");

            ConfigurationFileMap configMap = new ConfigurationFileMap(
                System.IO.Path.Combine(machineConfigPathFor64Bit,
                    "Config", "machine.config"));

            Configuration config = ConfigurationManager.OpenMappedMachineConfiguration(configMap);
            
            Assert.IsNotNull(config, "Machine.Config returned null");
            
            return config;
        }        
    }
}
