using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Model
{
    internal class RosbridgeModel
    {
        private List<Tuple<string, string>> _subscribe_topics; // <topic, msg_type>

        public static class RosTopics
        {
            public const string zvd_point_xyz   = "/zivid_camera/normals/xyz";
            public const string zvd_color_image = "/zivid_camera/color/image_color";
            public const string zvd_depth_image = "/zivid_camera/depth/image";
            public const string chatter         = "chatter";

            // 동적으로 연결할 토픽 목록
            public const string chatter2         = "chatter2";
            public const string chatter3         = "chatter3";
        }

        public RosbridgeModel()
        {
            _subscribe_topics = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(RosTopics.zvd_point_xyz  , "sensor_msgs/PointCloud2"),
                new Tuple<string, string>(RosTopics.zvd_color_image, "sensor_msgs/Image"),
                new Tuple<string, string>(RosTopics.zvd_depth_image, "sensor_msgs/Image"),
                new Tuple<string, string>(RosTopics.chatter        , "std_msgs/String"),
            };
        }

        public List<Tuple<string, string>> GetSubscribeTopics()
        {
            return _subscribe_topics;
        }

        public void AddSubscribeTopics(string topic, string msg_type)
        {
            _subscribe_topics.Add(new Tuple<string, string>(topic, msg_type));
        }
    }
}
