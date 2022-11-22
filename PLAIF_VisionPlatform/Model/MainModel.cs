﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Model
{
    class MainModel
    {
        private RosbridgeMgr _rosmgr;
        public MainModel()
        {
            _rosmgr = new RosbridgeMgr();
        }

        public bool Connect(string uri)
        {
            _rosmgr.Connect(uri);
            return true;
        }

        public bool IsConnected() { return _rosmgr.IsConnected(); }
    }
}