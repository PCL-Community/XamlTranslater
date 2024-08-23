using AlibabaCloud.SDK.Alimt20181012.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace XamlTranslater
{
    public class Translater
    {
        public class Ali
        {
            private AlibabaCloud.SDK.Alimt20181012.Client Client;

            public Ali(string KeyID,string KeySecret)
            {
                AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
                {
                    // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_ID。
                    AccessKeyId = Environment.GetEnvironmentVariable(KeyID),
                    // 必填，请确保代码运行环境设置了环境变量 ALIBABA_CLOUD_ACCESS_KEY_SECRET。
                    AccessKeySecret = Environment.GetEnvironmentVariable(KeySecret),
                };
                // Endpoint 请参考 https://api.aliyun.com/product/alimt
                config.Endpoint = "mt.cn-hangzhou.aliyuncs.com";
                Client = new AlibabaCloud.SDK.Alimt20181012.Client(config);
            }

            public async Task<string> GetTranslation(string Content,string Target)
            {
                AlibabaCloud.SDK.Alimt20181012.Models.TranslateGeneralRequest translateGeneralRequest = new AlibabaCloud.SDK.Alimt20181012.Models.TranslateGeneralRequest
                {
                    TargetLanguage = Target,
                    SourceText = Content,
                    Scene = "general",
                };
                AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
                try
                {
                    var res = await Client.TranslateGeneralWithOptionsAsync(translateGeneralRequest, runtime);
                    return res.Body.Data.Translated;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(),ex.Message);
                }
                return Content;
            }
        }
    }
}
