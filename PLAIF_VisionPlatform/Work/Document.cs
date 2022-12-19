using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Work
{
    public class Userinfo
    {
        public string ip_address="";
        public string username="";
        public string password="";
    }

    public class PcdViewParam
    {
        public double pt_size = 1.0;
        public double pt_show_percentage = 100.0;
        public Color pt_color = Color.Black;
    }

    public class Updater : IObservable
    {
        private List<Observer> observers = new List<Observer>();
        public void Add(Observer ob)
        {
            observers.Add(ob);
        }

        public void Remove(Observer ob)
        {
            observers.Remove(ob);
        }

        public void Notify(Observer.Cmd cmd)
        {
            foreach (Observer ob in observers)
                ob.Update(cmd);
        }
    }

    public sealed class Document
    {
        public static Document Instance => _instance.Value;
        private static readonly Lazy<Document> _instance = new Lazy<Document>(() => new Document());

        private Document() 
        {
            userinfo = new Userinfo();
            jsonUtil = new JsonUtil();
            updater = new Updater();
            pickPoses = new List<Pickpose>();
            visionResult = new List<Vision_Result>();

            IsConnected = false;
            CanConnectedSSH = false;
            IsImported = false;

            xy_2d_color_img = new();
            xyz_3d_depth_img = new();
            xyz_pcd_list = new();
            mainPcdViewParam = new();
            pickposePcdViewParam = new();

            GetUserInfo();
        }

        public void CheckConnection()
        {
            try
            {
                if (userinfo.ip_address != "" && userinfo.username != "" && userinfo.password != "")
                {
                    Document.Instance.CanConnectedSSH = SSHUtil.ConnectSSH(userinfo.ip_address, userinfo.username, userinfo.password);
                }
                else
                {

                }
            }
            catch
            {

            }
        }

        private void GetUserInfo()
        {
            // local/appdata에 ip, username 저장
            string path_appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision";
            Directory.CreateDirectory(path_appdata);
            string path_userinfo = path_appdata + "\\userinfo.ini";
            if(File.Exists(path_userinfo))
            {
                string[] info = File.ReadAllText(path_userinfo).Split();
                userinfo.ip_address = info[0].Replace(",", "");
                if (info.Length > 1) userinfo.username = info[1].Replace(",", "");
                if (info.Length > 2) userinfo.password = info[2];
            }
        }

        public List<Pickpose> pickPoses { get; set; }

        public List<Vision_Result> visionResult { get; set; }
        public Userinfo userinfo { get; set; }
        public JsonUtil jsonUtil { get; set; }
        public Updater updater { get; set; }

        public List<(int x, int y, System.Windows.Media.Color color)> xy_2d_color_img { get; set; } // Color color = Color.FromRgb(255, 0, 0);
        public List<(int x, int y, float z)> xyz_3d_depth_img { get; set; }
        public List<(float x, float y, float z)> xyz_pcd_list { get; set; }

        public bool CanConnectedSSH { get; set; }
        public bool IsConnected { get; set; }
        public bool IsImported { get; set; }

        public string ZividSettingFile { get; set; }
        public PcdViewParam mainPcdViewParam { get; set; }
        public PcdViewParam pickposePcdViewParam { get; set; }
    }
}
