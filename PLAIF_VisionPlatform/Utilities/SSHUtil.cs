using OpenCvSharp;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PLAIF_VisionPlatform.Utilities
{
    static public class SSHUtil
    {
        static public bool ConnectSSH(string IP, string username, string password)
        {
            try
            {
                using (var client = new SshClient(IP, username, password))
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(5);
                    client.Connect();

                    if (client.IsConnected == false) { return false; }

                    //SshCommand cmd = client.CreateCommand("ls -al");
                    //cmd.Execute();
                    //Console.WriteLine(cmd.Result);
                    client.Disconnect();
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }

        static public bool DownloadFile(string host, string user, string pwd, string host_file, string pc_dir)
        {
            //string host = "192.168.1.75";
            //int port = 9090;
            //string user = "yhpark";
            //string pwd = "vmffkdlv777";
            //string host_dir = @"/home/yhpark/catkin_ws/config/config_file";
            //string host_file = @"config_file.yaml";
            //string client_file = @"e:\tt.txt";
            //string pc_dir = @"C:\Users\jwpar";

            try
            {
                string host_dir = @"/home/" + user + @"/catkin_ws/config/config_file"; 

                using (var client = new SftpClient(host, user, pwd))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                    client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
                    client.OperationTimeout = TimeSpan.FromMinutes(180);
                    client.Connect();
                    if (client.IsConnected == false) { return false; }

                    ////-현재 디렉코리
                    //foreach (SftpFile f in client.ListDirectory(host_dir))
                    //{
                    //    Console.WriteLine(f.Name);
                    //}

                    var file = client.ListDirectory(host_dir).FirstOrDefault(f => f.Name == host_file);
                    if (file != null)
                    {
                        using (Stream fs = File.OpenWrite(Path.Combine(pc_dir, file.Name)))
                        {
                            client.DownloadFile(file.FullName, fs);
                        }
                    }
                    client.Disconnect();

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }  
        }
        static public bool UploadFile(string host, string user, string pwd, string host_file, string client_file)
        {
            //string host = "192.168.1.75";
            //int port = 9090;
            //string user = "yhpark";
            //string pwd = "vmffkdlv777";
            //string host_dir = @"/home/yhpark/catkin_ws/config/config_file";
            //string host_file = @"config_file.yaml";
            //string client_file = @"C:\Users\jwpar\config_file.yaml";
            //string pc_dir = @"C:\Users\jwpar";

            try
            {
                string host_dir = @"/home/" + user + @"/catkin_ws/config/config_file";

                using (var client = new SftpClient(host, user, pwd))
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                    client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
                    client.OperationTimeout = TimeSpan.FromMinutes(180);
                    client.Connect();
                    if (client.IsConnected == false) { return false; }
                    client.ChangeDirectory(host_dir);
                    //var listDir = client.ListDirectory(host_dir);
                    //foreach(var fi in listDir) {
                    //    Console.WriteLine(" - " + fi.Name);
                    //}
                    using (var fs = new FileStream(client_file, FileMode.Open))
                    {
                        //client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, Path.GetFileName(client_file));
                    }
                    client.Disconnect();

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //public void DownloadFiles()
        //{
        //    string host = "192.168.0.*";
        //    int port = 9090;
        //    string user = "pi";
        //    string pwd = "****";
        //    string host_dir = @"/home/pi/";
        //    string host_file = "t.txt";
        //    string client_file = @"e:\tt.txt";
        //    string pc_dir = @"E:\tmp";

        //    using (var client = new SftpClient(host, port, user, pwd))
        //    {
        //        client.KeepAliveInterval = TimeSpan.FromSeconds(60);
        //        client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
        //        client.OperationTimeout = TimeSpan.FromMinutes(180);
        //        client.Connect();
        //        if (client.IsConnected == false) { return; }
        //        //-현재 디렉코리
        //        //foreach(SftpFile f in client.ListDirectory(".")){
        //        //    Console.WriteLine(f.Name);
        //        //}
        //        //-download
        //        using (var outfile = File.Create(host_file))
        //        {
        //            client.DownloadFile(host_file, outfile);
        //        }
        //        client.Disconnect();
        //    }
        //}

        /// <summary>
        /// linux의 경로는 usr의 접근 귄한이 잇어야 한다. 
        /// </summary>
        //public void UploadFiles()
        //{
        //    string host = "192.168.1.75";
        //    int port = 9090;
        //    string user = "yhpark";
        //    string pwd = "vmffkdlv777";
        //    string host_dir = @"/home/yhpark/catkin_ws/config/config_file";
        //    string host_file = @"config_file.yaml";
        //    string client_file = @"e:\tt.txt";
        //    string pc_dir = @"C:\Users\jwpar";

        //    using (var client = new SftpClient(host, port, user, pwd))
        //    {
        //        client.KeepAliveInterval = TimeSpan.FromSeconds(60);
        //        client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(10);
        //        client.OperationTimeout = TimeSpan.FromMinutes(180);
        //        client.Connect();
        //        if (client.IsConnected == false) { return; }
        //        client.ChangeDirectory(host_dir);

        //        using (var infile = File.Open(client_file, FileMode.Open))
        //        {
        //            client.UploadFile(infile, "tt.txt");
        //        }
        //        client.Disconnect();
        //    }
        //}

    }
}
