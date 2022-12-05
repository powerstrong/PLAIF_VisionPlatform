using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Utilities;
using System;
using System.Collections.Generic;
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

        public void Notify()
        {
            foreach (Observer ob in observers)
            {
                ob.Update();
            }
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

            IsConnected = false;
            IsExistSSHCod = true;

            GetUserInfo();
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

        public Userinfo userinfo { get; set; }
        public JsonUtil jsonUtil { get; set; }
        public Updater updater { get; set; }

        public bool IsExistSSHCod { get; set; }
        public bool IsConnected { get; set; }

        public string ZividSettingFile { get; set; }

    }
}
