using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace yumi1
{
    public class Modules
    {
        public static string RIGHT = "MainModuleRight";
        public static string LEFT = "MainModuleLeft";
        public static string TRAYLEFT = "TrayLeft";
        public static string TRAYRIGHT = "TrayRight";
    }

    public class CodeModel
    {
        private string code;
        public CodeModel(string xyz)
        {
            Code = xyz;
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
    }

    public class Tasks
    {
        public static string LEFT = "T_ROB_L";
        public static string RIGHT = "T_ROB_R";
    }

    public class Requests
    {
        public static string SerialNumber = "SERIAL_NUMBER=";
        public static string LoginUser = "USER_ID=";
        public static string LoginPassword = "PASSWORD=";
    }

    public enum Status
    {
        Disconnected, NotAvailable, Connecting, Connected,
    }

    public class YuMi : INotifyPropertyChanged
    {
        public Status status;
        RapidData rd;
        RapidDataType rdt;
        Controller controller;
        ControllerInfo selectedController;
        Task[] tasks;

        private string _name;
        private string _codeL;
        private string _codeR;
        private BindingList<CodeModel> _codesListL;
        private BindingList<CodeModel> _codesListR;

        public BindingList<CodeModel> CodeListL
        {
            get { return _codesListL; }
        }
        public BindingList<CodeModel> CodeListR
        {
            get { return _codesListR; }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    return null;
                else
                    return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string CodeL
        {
            get
            {
                if (System.String.IsNullOrWhiteSpace(_codeR))
                    return null;
                else
                    return CodeL;
            }
            set
            {
                _codeL = value;
                OnPropertyChanged("CodeL");
            }
        }

        public string CodeR
        {
            get
            {
                if (System.String.IsNullOrWhiteSpace(_codeR))
                    return null;
                else
                    return _codeR;
            }
            set
            {
                _codeR = value;
                OnPropertyChanged("CodeR");
            }
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void CodeListL_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("CodeListL");
        }
        private void CodeListR_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("CodeListR");
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public YuMi()
        {
            _codesListL = new BindingList<CodeModel>();
            _codesListL.ListChanged += CodeListL_ListChanged;
            _codesListR = new BindingList<CodeModel>();
            _codesListR.ListChanged += CodeListR_ListChanged;
        }

        public void ConnectAndDisplayData()
        {

            Console.Out.WriteLine("Scanning for controllers...");
            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();
            ControllerInfoCollection controllers = scanner.Controllers;
            Console.Out.WriteLine("Number of controllers found: {0}", controllers.Count);
            //connecting to first controller found. usually only one is present.
            selectedController = controllers.First();
            Console.Out.WriteLine(selectedController.Name + " " + selectedController.HostName + " " + selectedController.IPAddress + " " + selectedController.Availability);
            Name = selectedController.Name + "\n" + selectedController.IPAddress;

            if (selectedController.Availability == Availability.Available)
            {
                try
                {
                    status = Status.Connecting;
                    controller = ControllerFactory.CreateFrom(selectedController);
                    controller.Logon(UserInfo.DefaultUser);
                    status = Status.Connected;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not establish a connection to the controller" + Environment.NewLine + e.StackTrace);
                    status = Status.Disconnected;
                }
                finally { Console.Out.WriteLine("Connected to " + controller.IPAddress); }
            }
            else
            {
                Console.WriteLine("The controller is not available for connections");
                status = Status.NotAvailable;
                status = Status.Disconnected;
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
        }

        private void PrintRapidData(string taskName, string moduleName, string varName)
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
                Console.WriteLine(e.StackTrace);
            }

            if (!string.IsNullOrWhiteSpace(rd.StringValue))
            {
                Console.WriteLine("{0} : {1} ", taskName, rd.StringValue);
            }
            // subscribing to variable change
            rd.ValueChanged += RapidVar_ValueChanged;
        }

        private void Controller_ModeChangedHandler(object sender, OperatingModeChangeEventArgs e)
        {
            Console.WriteLine("Operator mode has been changed to: {0}", e.NewMode);
        }

        private void RapidVar_ValueChanged(object sender, DataValueChangedEventArgs e)
        {
            var var = (RapidData)sender;
            string extracted = var.StringValue.Replace("\"", string.Empty);
            switch (var.Name)
            {
                case "codeL":
                    CodeListL.Add(new CodeModel(extracted));
                    break;
                case "codeR":
                    CodeListR.Add(new CodeModel(extracted));
                    break;
            }
            if (!string.IsNullOrWhiteSpace(var.StringValue))
            {
                Console.WriteLine("{0} : {1} modified at {2}", var, extracted, e.Time);
                PfsHandler.ReadReplaceRequest("transfer_file.txt", extracted, Requests.SerialNumber);
                //Pfs4.ClassLibrary.SendRequest("127.0.0.1", ReadReplaceRequest("transfer_file.txt", x.StringValue));
            }
        }
    }
}
