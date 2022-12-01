using PLAIF_VisionPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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


    public sealed class Document
    {
        public static Document Instance => _instance.Value;
        private static readonly Lazy<Document> _instance = new Lazy<Document>(() => new Document());
        private Document() 
        {
            userinfo = new Userinfo();
            jsonUtil = new JsonUtil();
        }

        public Userinfo userinfo { get; set; }
        public JsonUtil jsonUtil { get; set; }
    }
}
