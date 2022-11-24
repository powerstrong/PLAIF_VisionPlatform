using Newtonsoft.Json.Linq;
using PLAIF_VisionPlatform.Model;
using Rosbridge.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PLAIF_VisionPlatform.ViewModel
{
    class RosbridgeMgr
    {
        private MessageDispatcher _md;
        private bool _isConnected = false;
        private MainViewModel _mainViewModel;
        private RosbridgeModel _rosbrdgModel;

        List<Subscriber> _subscribers;

        public RosbridgeMgr(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _rosbrdgModel = new RosbridgeModel();

            _subscribers = new List<Subscriber>();
        }

        public async void Connect(string uri)
        {
            if (_isConnected)
            {
                //foreach (var w in _childWindows)
                //{
                //    await w.CleanUp();
                //    (w as Window).Close();
                //}
                //_childWindows.Clear();

                foreach (var s in _subscribers)
                {
                    await s.UnsubscribeAsync();
                }
                _subscribers.Clear();

                await _md.StopAsync();
                _md = null;
                
                _isConnected = false;
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

                _isConnected = true;
            }
            //ToggleConnected();
        }

        public bool IsConnected() { return _isConnected; }
        public async void Capture()
        {
            ServiceCallMsg("/zivid_camera/capture", "[]");
        }

        private async void PublishMsg(string topic, string msg_type, string msg)
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
                    Debug.Print("[chatter] : " + msg);
                    break;
                case RosbridgeModel.RosTopics.zvd_point_xyz:
                    Debug.Print("[zvd_point_xyz] : " + msg);
                    break;
            }

            //Dispatcher.Invoke(() =>
            //{
            //    try
            //    {
            //        _mainViewModel.temp();
            //    }
            //    catch { }
            //});
        }

        private async void SubscribeMsg(string topic, string msg_type)
        {
            var s = new Subscriber(topic, msg_type, _md);
            s.MessageReceived += _subscriber_MessageReceived;
            await s.SubscribeAsync();
            _subscribers.Add(s);
        }

        private async void ServiceCallMsg(string topic, string msg)
        {
            var sc = new ServiceClient(topic, _md);
            JArray argsList = JArray.Parse(msg);
            var result = await sc.Call(argsList.ToObject<List<dynamic>>());

            // UI 쓰레드 접근 시 사용..하나 여기선 밖에서 사용해야 할듯
            //Dispatcher.Invoke(() =>
            //{
            //    try
            //    {
            //        s = result.ToString();
            //    }
            //    catch { }
            //});
        }
    }
}
