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
    public class InjuryDocumentService(IHttpService httpService) : IInjuryDocumentService, IScopedDependency
    {
        private static String Service = "http://localhost:8868";
        private static String Key = "de40f16924b44ffab4c618108da2559c";
        public async Task<string> GenerationTemplate(Model.Live_V_InjuryInfo model)
        {
            try
            {
                var objs = new Dictionary<string, string>();
                objs.Add("cid", model.UserName);
                var body = JsonSerializer.Serialize(objs);
                var heads = new Dictionary<string, string>();
                heads.Add("Authorization", "Bearer " + Key);
                return await httpService.PostJsonAsync(Service + "/api/web/out/joingroup", body, heads);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }
    }
}
