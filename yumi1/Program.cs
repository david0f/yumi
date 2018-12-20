using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace yumi1
{
    class RobotWare
    {
        public string Id { get; set; }
        public string Availability { get; set; }
        public string IsVirtual { get; set; }
        public string SystemName { get; set; }
        public string Version { get; set; }
        public string ControllerName { get; set; }
    }
    public class Modules
    {
        public static string RIGHT = "MainModuleRight";
        public static string LEFT = "MainModuleLeft";
        public static string TRAYLEFT = "TrayLeft";
        public static string TRAYRIGHT = "TrayRight";
    }

    public class Tasks
    {
        public static string LEFT = "T_ROB_L";
        public static string RIGHT = "T_ROB_R";
    }

    class Program
    {
        static RapidData rd;
        static RapidDataType rdt;
        static Controller controller;
        NetworkScanner scanner;
        static ABB.Robotics.Controllers.RapidDomain.Task[] tasks;

        static void Main(string[] args)
        {
            Console.Out.WriteLine("Scanning for controllers...");

            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();
            ControllerInfoCollection controllers = scanner.Controllers;

            Console.Out.WriteLine("Number of controllers found: {0}", controllers.Count);

            //connecting to first controller found. usually only one is present.
            ControllerInfo selectedController = controllers.First();

            Console.Out.WriteLine(selectedController.Name + " " + selectedController.HostName + " " + selectedController.IPAddress + " " + selectedController.Availability);
            Console.Read();

            if (selectedController.Availability == Availability.Available)
            {
                try
                {
                    controller = ControllerFactory.CreateFrom(selectedController);
                    controller.Logon(UserInfo.DefaultUser);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not establish a connection to the controller" + Environment.NewLine + e.StackTrace);
                }
                finally { Console.Out.WriteLine("Connected to " + controller.IPAddress); }
            }
            else
            {
                Console.WriteLine("The controller is not available for connections");
            }

            controller.OperatingModeChanged += Controller_ModeChangedHandler;

            Console.Out.WriteLine(controller.OperatingMode);
            tasks = controller.Rapid.GetTasks();
            foreach (var task in tasks)
            {
                Console.Out.WriteLine("Current task on the controller: {0} ", task.Name);
            }

            PrintRapidData(Tasks.LEFT, Modules.LEFT, "codeL");
            PrintRapidData(Tasks.RIGHT, Modules.RIGHT, "codeR");
            Console.ReadKey(false);
        }

        private static StringBuilder ReadRequest(string transferFile)
        {
            StringBuilder request = new StringBuilder();
            try
            {
                using (StreamReader streamReader = new StreamReader(transferFile))
                {
                    string fileContents;
                    fileContents = streamReader.ReadToEnd();
                    fileContents.Trim();
                    // make sure the request's line returns are the correct format
                    fileContents = Regex.Replace(fileContents, @"(\r*\n)+", "\r\n");
                    request.AppendLine(fileContents);
                    request.AppendLine(); // add a blank line at the end
                }
            }
            catch (Exception ex)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine("TestLink Error: Read from transfer file failed");
                errorMessage.AppendLine(ex.ToString());
                throw new Exception(errorMessage.ToString());
            }
            return request;
        }

        private static void PrintRapidData(string taskName, string moduleName, string varName)
        {
            try
            {
                rdt = controller.Rapid.GetRapidDataType(taskName, moduleName, varName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace); Thread.Sleep(5000);
            }
            try
            {
                if (rd != null)
                    rd.Dispose();
                rd = controller.Rapid.GetRapidData(taskName, moduleName, varName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not fetch rapid data");
                Console.WriteLine(e.StackTrace);}

            if (System.String.IsNullOrWhiteSpace(rd.StringValue))
            {
                Console.WriteLine("{0} : {1} ", taskName, rd.StringValue);
            }
            // subscribing to variable change
            rd.ValueChanged += RapidVar_ValueChanged;
        }

        private static void Controller_ModeChangedHandler(object sender, OperatingModeChangeEventArgs e)
        {
            Console.WriteLine("Operator mode has been changed to: {0}", e.NewMode);
        }

        private static void RapidVar_ValueChanged(object sender, DataValueChangedEventArgs e)
        {
            var x = (RapidData)sender;
            if (System.String.IsNullOrWhiteSpace(x.StringValue))
                Console.WriteLine("{0} : {1} modified at {2}", x, x.StringValue, e.Time);
            
        }
    }
}
