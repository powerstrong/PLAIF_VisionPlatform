using Newtonsoft.Json.Linq;
using Rosbridge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PLAIF_VisionPlatform.Model
{
    class RosbridgeMgr
    {
        private MessageDispatcher _md;
        private bool _isConnected = false;
        MainModel _mainModel;

        public RosbridgeMgr(MainModel mainModel)
        {
            _mainModel = mainModel;
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

                await _md.StopAsync();
                _md = null;

                _isConnected = false;
            }
            else
            {
                try
                {
                    _md = new MessageDispatcher(new Socket(new Uri(uri)), new MessageSerializerV2_0());
                    await _md.StartAsync();
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
        private async void SubscribeMsg(string topic, string msg_type, string msg)
        {
            // rosbridge_client의 아래 함수 참고할 것..
            //private async void Window_Loaded(object sender, RoutedEventArgs e)
            // _subscriber.MessageReceived += _subscriber_MessageReceived;

            //var ss = new Rosbridge.Client.Subscriber(topic, msg_type, _md);
            //var obj = JObject.Parse(msg);
            //await ss.SubscribeAsync(obj);
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
