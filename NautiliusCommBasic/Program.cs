using NautiliusCommBasic.Api.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace NautiliusCommBasic
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }
        static Settings settings = new Settings();
        static string login;
        static readonly string SettingsFileLocation = @".\settings.json";
        static INCommService client;

        static void Main(string[] args)
        {
            Console.WriteLine("App start");
            if (!File.Exists(SettingsFileLocation))
            {
                Console.WriteLine("Creating settings file");

                var settingsString = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);

                using (var fs = new StreamWriter(File.Create(SettingsFileLocation)))
                {
                    fs.Write(settingsString);
                    fs.Flush();
                }

                Console.WriteLine("Settings file created @" + SettingsFileLocation);
            }

            settings = Settings.Load(SettingsFileLocation);

            using (var host = new ServiceHost(typeof(NCommService), settings.HostUrl))
            {
                var metadata = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadata);
                host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
                host.AddServiceEndpoint(typeof(INCommService), new BasicHttpBinding(), settings.HostUrl);
                host.CloseTimeout = TimeSpan.FromSeconds(2);

                Console.WriteLine("Opening service @" + settings.HostUrl);
                host.Open();
                Console.WriteLine("Service running");


                while (true)
                {
                    var read = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(read))
                    {
                        if (read.StartsWith(@"/connect"))
                        {
                            if (client == null)
                            {
                                Console.WriteLine("Connecting @" + settings.ClientUrl);
                                ConnectAsClient(settings.ClientUrl);
                            }
                            else
                            {
                                Console.WriteLine("Client already connected");
                            }
                        }
                        else if (read.StartsWith("/login"))
                        {
                            var startindex = read.IndexOf(" ");
                            startindex++;
                            login = read.Substring(startindex);
                            Console.WriteLine("Login set to: " + login);
                        }
                        else if (read.StartsWith(@"/quit"))
                        {
                            Console.WriteLine("Closing connection");
                            break;
                        }
                        else if (read.StartsWith("/clear"))
                        {
                            Console.Clear();
                        }
                        else if(read.StartsWith("/help"))
                        {
                            Console.Clear();
                            Console.WriteLine(new StringBuilder()
                                .Append("Available commands:")
                                .AppendLine("/connect - connects to server")
                                .AppendLine("/login <name> - Change current login to <name>")
                                .AppendLine("/clear - wipes console content")
                                .AppendLine("/help - show this message")
                                .AppendLine("/quit - close communication platform")
                                .AppendLine("<message> - sends message")
                                .ToString());
                        }
                        else
                        {
                            int currentLineCursor = Console.CursorTop;
                            Console.SetCursorPosition(0, Console.CursorTop-1);
                            Console.Write(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, currentLineCursor);

                            SendMessage(read);
                        }
                    }
                }

                host.Close();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.AppendAllLines("./error.log", new List<string> { DateTime.Now.ToString(), e.ExceptionObject.ToString() });
        }

        private static void SendMessage(string message)
        {
            try
            {
                client.TestConnection();
            }
            catch
            {
                Console.WriteLine("Connection error, try to reconnect");
                client = null;
                return;
            }

            if (client != null)
            {


                var msg = new Api.Model.Message { Text = message, Login = login };
                client.SendMessage(msg);
                Console.WriteLine(string.Format("{0}\tOUT: AS: {1}: {2}",msg.Created, msg.Login, msg.Text));
            }
            else
            {
                Console.WriteLine("No endpoint, message not sent, try to \"/connect\" first");
            }
        }

        private static void ConnectAsClient(Uri clientUrl)
        {
            var tempClient = ChannelFactory<INCommService>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(clientUrl));
            try
            {
                tempClient.TestConnection();
                client = tempClient;
                Console.WriteLine("Connected");
            }
            catch (Exception)
            {
                Console.WriteLine("NotConnected");
            }
        }
    }
}
