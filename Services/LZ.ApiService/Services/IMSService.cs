using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.AccessControl;
using System.Drawing;
using System.IO.Pipes;
using Microsoft.AspNetCore.Http.HttpResults;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LZ.ApiService.Services
{
    public class IMSService(IHttpService httpService,ISettingsService settingsService) : IIMSService, IScopedDependency
    {

        private static String Key = "de40f16924b44ffab4c618108da2559c";
        public async Task<string> SendMessage(Model.IMMessage model)
        {
            try
            {
                var set = await settingsService.GetMasterSettings();
                var body = JsonSerializer.Serialize(model);
                var heads = new Dictionary<string, string>();
                heads.Add("Authorization", "Bearer " + Key);
                return await httpService.PostJsonAsync(set.IMServicesUrl + "/api/web/out/sendmessage", body, heads);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }
        public async Task<string> JoinGroup(int uid, int type, int cid)
        {
            try
            {
                var set = await settingsService.GetMasterSettings();
                var objs = new Dictionary<string, string>();
                objs.Add("cid", cid.ToString());
                objs.Add("uid", uid.ToString());
                objs.Add("type", type.ToString());
                //var body = JsonSerializer.Serialize(objs);
                var heads = new Dictionary<string, string>();
                heads.Add("Authorization", "Bearer " + Key);
                return await httpService.PostFormAsync(set.IMServicesUrl + "/api/web/out/joingroup", objs, heads);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }
        public async Task<string> BatchJoinGroup(int uid, int type)
        {
            try
            {
                var set = await settingsService.GetMasterSettings();
                var objs = new Dictionary<string, string>();
                objs.Add("uid", uid.ToString());
                objs.Add("type", type.ToString());
                var body = JsonSerializer.Serialize(objs);
                var heads = new Dictionary<string, string>();
                heads.Add("Authorization", "Bearer " + Key);
                return await httpService.PostJsonAsync(set.IMServicesUrl + "/api/web/out/batchjoingroup", body, heads);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }
    }
}
