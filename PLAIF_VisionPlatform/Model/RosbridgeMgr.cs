using Rosbridge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLAIF_VisionPlatform.Model
{
    class RosbridgeMgr
    {
        private MessageDispatcher _md;
        private bool _isConnected = false;

        public RosbridgeMgr()
        {

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
    }
}
