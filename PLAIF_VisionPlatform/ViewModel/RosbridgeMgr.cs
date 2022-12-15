using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.Work;
using Rosbridge.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static PLAIF_VisionPlatform.Model.RosbridgeModel;

namespace PLAIF_VisionPlatform.ViewModel
{
    public sealed class RosbridgeMgr
    {
        private MessageDispatcher? _md; 
        private MainViewModel? _mainViewModel;
        private RosbridgeModel _rosbrdgModel;

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { 
                _isConnected = value;
                Document.Instance.IsConnected = value;
            }
        }

        List<Subscriber> _subscribers;

        private RosbridgeMgr() 
        {
            _rosbrdgModel = new RosbridgeModel();
            _subscribers = new List<Subscriber>();
            IsConnected = false;
        }
        private static readonly Lazy<RosbridgeMgr> _instance = new Lazy<RosbridgeMgr>(() => new RosbridgeMgr());
        public static RosbridgeMgr Instance => _instance.Value;

        public void SetMainModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public async void Connect(string uri)
        {
            if (IsConnected)
            {
                foreach (var s in _subscribers)
                {
                    s.UnsubscribeAsync().Wait();
                }
                IsConnected = false;
                _subscribers.Clear();

                await _md.StopAsync();
                _md = null;
            }
            else
            {
                try
                {
                    _md = new MessageDispatcher(new Socket(new Uri(uri)), new MessageSerializerV2_0());
                    _md.StartAsync().Wait();

                    foreach (var tuple in _rosbrdgModel.GetSubscribeTopics())
                    {
                        SubscribeMsg(tuple.Item1, tuple.Item2);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                         "Error!! Could not connect to the rosbridge server", MessageBoxButton.OK, MessageBoxImage.Error);
                    _md = null;
                    return;
                }

                IsConnected = true;
            }
            //ToggleConnected();
        }

        public void Capture()
        {
            ServiceCallMsg("/zivid_camera/capture", "[]");
        }

        public void Start_Action()
        {
            string topic = "/camera01_result0a_action/goal";
            string msg_type = "plaif_vision_msgs/VisionActionGoal";
            string msg = "{}";

            PublishMsg(topic, msg_type, msg);
        }

        public async void PublishMsg(string topic, string msg_type, string msg)
        {
            var pb = new Rosbridge.Client.Publisher(topic, msg_type, _md);
            await pb.PublishAsync(JObject.Parse(msg));
        }

        private void _subscriber_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg = e.Message["msg"]!.ToString();

            switch (e.Message["topic"]!.ToString())
            {
                case RosbridgeModel.RosTopics.chatter:
                    Debug.Print("[chatter] : " + msg); // 메시지 크기가 큰 경우 주의할 것
                    break;
                case RosbridgeModel.RosTopics.zvd_point_xyz:
                    _mainViewModel!.CreatePointCloud(e.Message["msg"]!["data"]!.ToString());
                    break;
                case RosbridgeModel.RosTopics.zvd_color_image:
                    _mainViewModel!.Create2DBitMapImage(e.Message["msg"]!["data"]!.ToString());
                    break;
                case RosbridgeModel.RosTopics.zvd_depth_image:
                    _mainViewModel!.Create3DBitMapImage(e.Message["msg"]!["data"]!.ToString());
                    break;
                case RosbridgeModel.RosTopics.vision_result:
                    _mainViewModel!.ParsingVisionResult(e.Message["msg"]!["result"]!);
                    break;
                case RosbridgeModel.RosTopics.vision_result_img:
                    _mainViewModel!.Create2DResultImage(e.Message["msg"]!["data"]!.ToString());
                    break;
            }

        }

        public async void SubscribeMsg(string topic, string msg_type)
        {
            var s = new Subscriber(topic, msg_type, _md);
            s.MessageReceived += _subscriber_MessageReceived;
            await s.SubscribeAsync();
            _subscribers.Add(s);
        }

        public async void ServiceCallMsg(string topic, string msg)
        {
            var sc = new ServiceClient(topic, _md);
            JArray argsList = JArray.Parse(msg);
            var result = await sc.Call(argsList.ToObject<List<dynamic>>());
        }
    }
}
