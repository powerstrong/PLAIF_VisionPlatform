using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using static PLAIF_VisionPlatform.Utilities.JsonUtil;
using static System.Net.WebRequestMethods;

namespace PLAIF_VisionPlatform.Utilities
{
    public class JsonUtil
    {
        public JObject? jsonVisionSetting = null;

        public enum FileType
        {
            Type_Json,
            Type_Yaml,
        }

        public JsonUtil()
        {
            jsonVisionSetting = new JObject();
        }

        ~JsonUtil()
        {

        }

        public bool Load(in string Path, FileType fileType)
        {
            bool Rtn = false;
            try
            {
                if(fileType == FileType.Type_Yaml)
                {
                    Rtn = LoadYaml(Path);
                }
                else
                {
                    Rtn = LoadJson(Path);
                }
            }
            catch
            {
                return Rtn;
            }

            return Rtn;
        }

        private bool LoadJson(in string Path)
        {
            try
            {
                using (StreamReader file = System.IO.File.OpenText(Path))
                {
                    using (JsonTextReader readder = new JsonTextReader(file))
                    {
                        JObject json = (JObject)JToken.ReadFrom(readder);

                        if (json != null)
                        {
                            jsonVisionSetting = json;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool LoadYaml(in string Path)
        {
            bool Rtn = false;
            try
            {
                using (StreamReader file = System.IO.File.OpenText(Path))
                {
                    Rtn = ConvertFromYamlToJson(file);
                }
            }
            catch
            {
                return Rtn;
            }

            return Rtn;
        }

        public bool Save(in string Path, FileType fileType)
        {
            bool Rtn = false;
            try
            {
                if (fileType == FileType.Type_Yaml)
                {
                    Rtn = SaveYaml(Path);
                }
                else
                {
                    Rtn = SaveJson(Path);
                }
            }
            catch
            {
                return Rtn;
            }

            return Rtn;
        }

        private bool SaveYaml(in string Path)
        {
            bool Rtn = false;
            try
            {
                var w = new StringWriter();
                ConvertFromJsonToYaml(jsonVisionSetting.ToString(), ref w);

                using (StreamWriter sr = new StreamWriter(Path))
                {
                    sr.WriteLine(w);
                    sr.Close();
                }

                w.Close();
                Rtn = true;
            }
            catch
            {
                return Rtn;
            }

            return Rtn;
        }

        private bool SaveJson(in string Path)
        {
            bool Rtn = false;
            try
            {
                using (StreamWriter sr = new StreamWriter(Path))
                {
                    sr.WriteLine(jsonVisionSetting.ToString());
                    sr.Close();
                }
                Rtn = true;
            }
            catch
            {
                return Rtn;
            }

            return true;
        }

        private bool ConvertFromYamlToJson(in StreamReader file)
        {
            // convert string/file to YAML object
            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize(file);

            // now convert the object to JSON. Simple!
            JsonSerializer js = new JsonSerializer();

            using (StringWriter w = new StringWriter())
            {
                js.Serialize(w, yamlObject);

                using (StringReader r = new StringReader(w.ToString()))
                {
                    using (JsonTextReader readder = new JsonTextReader(r))
                    {
                        JObject json = (JObject)JToken.ReadFrom(readder);

                        if (json != null)
                        {
                            jsonVisionSetting = json;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private object ConvertJTokenToObject(JToken token)
        {
            if (token is JValue)
                return ((JValue)token).Value;
            if (token is JArray)
                return token.AsEnumerable().Select(ConvertJTokenToObject).ToList();
            if (token is JObject)
                return token.AsEnumerable().Cast<JProperty>().ToDictionary(x => x.Name, x => ConvertJTokenToObject(x.Value));
            throw new InvalidOperationException("Unexpected token: " + token);
        }

        private bool ConvertFromJsonToYaml(string json, ref StringWriter w)
        {
            var swaggerDocument = ConvertJTokenToObject(JsonConvert.DeserializeObject<JToken>(json));
            var serialize = new Serializer();
            serialize.Serialize(w, swaggerDocument);
        
            return true;
        }

        private void Example()
        {
            #region 객체 생성해서 사용 할 시
            JsonUtil jsonTest = new JsonUtil();
            jsonTest.Load("config_file.yaml", JsonUtil.FileType.Type_Yaml);
            //config_file.yaml 2DImage_json.json

            if (jsonTest.jsonVisionSetting != null)
            {
                jsonTest.Save("Test_json.yaml", JsonUtil.FileType.Type_Yaml);
            }
            #endregion

            #region Document에 속해서 사용할 시
            //var jsonUtil = Document.Instance.jsonUtil;
            //jsonUtil.Load("config_file.yaml", JsonUtil.FileType.Type_Yaml);

            //if (jsonUtil.jsonVisionSetting != null)
            //{
            //    jsonUtil.Save("Test_json.yaml", JsonUtil.FileType.Type_Yaml);
            //}
            #endregion
        }
    }
}
