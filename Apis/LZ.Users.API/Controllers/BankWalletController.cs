using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Bank.Wallet;
using LZ.Model.Dtos;
using LZ.Model.Dtos.OpenAccountDtos;
using LZ.Model.Enums;
using LZ.Model.Enums.Bank;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.ICBC.Plkh;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record;
using NPOI.Util;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BankWalletController(IBankWalletService bankWalletService,IEntryService entryService,IHttpService httpService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query, string label)
        {
            var cid = CurrentUser.CompanyID;

            var expable = Expressionable.Create<Model.Live_BankWallet>();
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable.AndIF(!string.IsNullOrEmpty(query.CardID), t => t.CertNo.Contains(query.CardID));
            expable.AndIF(!query.Status.HasValue, t => t.Status == query.Status);
            var exp = expable.ToExpression();
            var models = await bankWalletService.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<Model.Live_BankWallet> Get(Guid? gid)
        {
            if (!gid.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            return await bankWalletService.Get(t=>t.GID== gid);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Model.Live_BankWallet model)
        {
            await bankWalletService.Update(model);
            return Ok();
        }

        //e钱包开户和查询是否已开通
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OpenAcct([FromForm] int? userID)
        {
            if (!userID.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var entry = await entryService.Get(t=>t.UserID==userID && t.Status==2);
            if (entry == null)
            {
                {
                    throw new ErrorCodeException(-1, "入职信息为空");
                }
            }
            if (string.IsNullOrEmpty(entry.BankNumber) || string.IsNullOrEmpty(entry.BankName))
            {
                throw new ErrorCodeException(-1, "未绑定银行卡,或者未识别银行");
            }
            string[] bankArr = new string[] { "工商银行", "农业银行", "中国银行", "建设银行", "交通银行", "储蓄银行" };
            bool isok = bankArr.Any(t => entry.BankName.Contains(t));
            if (!isok)
            {
                throw new ErrorCodeException(-1, "非六大行不能开通");
            }
            var exist = await bankWalletService.Get(t => t.Name == entry.Name && t.CertNo == entry.CardID && t.Status != (int)BankWalletEnumType.开通失败 && t.Status != (int)BankWalletEnumType.已销户);
            if (exist != null)
            {
                if (exist.Status == (int)BankWalletEnumType.开通中)
                {
                    //调用查询接口
                    string json = JsonConvert.SerializeObject(exist);
                    var res = await httpService.PostAsync<BankWalletAcctQuery>(ServiceNames.BankService, "/api/BankWallet/query", json);
                    if (res.Code == 200)
                    {

                        if (res.Data.Code == 0 && !string.IsNullOrEmpty(res.Data.OpenacctStatus))
                        {
                            switch (res.Data.OpenacctStatus)
                            {
                                case "00":
                                    exist.Status = (int)BankWalletEnumType.开通中;
                                    break;
                                case "01":
                                    exist.Status = (int)BankWalletEnumType.开通中;
                                    break;
                                case "02":
                                    exist.Status = (int)BankWalletEnumType.开通成功;
                                    break;
                                case "03":
                                    exist.Status = (int)BankWalletEnumType.开通失败;
                                    break;
                            }
                            exist.Status = Convert.ToInt32(res.Data.OpenacctStatus);
                            exist.MediumId = res.Data.MediumId;
                            if (!string.IsNullOrEmpty(res.Data.OpenAcctErrorCode))
                            {
                                exist.OpenAcctErrorCode = res.Data.OpenAcctErrorCode;
                            }
                            if (!string.IsNullOrEmpty(res.Data.OpenAcctErrorMsg))
                            {
                                exist.OpenAcctErrorMsg = res.Data.OpenAcctErrorMsg;
                            }

                        }
                        else
                        {
                            exist.Code = res.Data.Code.ToString();
                            exist.Msg = res.Data.Msg =="SUCCESS" ? "未开户" : res.Data.Msg;
                        }
                        await bankWalletService.Update(exist);
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, res.Message ?? "e钱包查询账户出错");
                    }
                }
            }
            if(exist !=null && (exist.Status == (int)BankWalletEnumType.开通成功 || exist.Status == (int)BankWalletEnumType.开通中))
            {
                return Ok(new { items = exist });
            }
            Live_BankWallet model = new Live_BankWallet();
            var date = DateTime.Now;
            model.GID = Guid.NewGuid();
            model.Name = entry.Name;
            model.CertType = 0;//0-身份证，目前仅支持身份证类型
            model.CertNo = entry.CardID;
            model.Address = entry.Address;
            model.Mobile = entry.Phone;
            model.CreateTime = date;
            model.BindMedium = entry.BankNumber;
            model.Occupation = 3;
            model.Status = (int)BankWalletEnumType.开通中;
            if (!string.IsNullOrEmpty(entry.CardStartDate))
            {
                DateTime sdate = DateTime.ParseExact(entry.CardStartDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                string sformattedDate = sdate.ToString("yyyy-MM-dd");
                model.SignDate = sformattedDate;
            }
            if (!string.IsNullOrEmpty(entry.CardEndDate))
            {
                DateTime edate = DateTime.ParseExact(entry.CardEndDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                string eformattedDate = edate.ToString("yyyy-MM-dd");
                model.ValidityPeriod = eformattedDate;

            }
            
            var id = await bankWalletService.Add(model);
            model.ID = id;
            model.OutUserID = string.Format("LB{0}{1}", date.ToString("yyyyMMddHHmm"), model.ID);
            await bankWalletService.Update(model);
            return Ok(new { items = model });
        }

        //e钱包销户发送验证码
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CloseAcctSendSms([FromForm] int? userID)
        {
            if (!userID.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var entry = await entryService.Get(t => t.UserID == userID && t.Status == 2);
            if (entry == null)
            {
                {
                    throw new ErrorCodeException(-1, "入职信息为空");
                }
            }
            if (string.IsNullOrEmpty(entry.BankNumber))
            {
                throw new ErrorCodeException(-1, "未绑定银行卡");
            }
            var exist = await bankWalletService.Get(t => t.Name == entry.Name && t.CertNo == entry.CardID && t.Status ==(int)BankWalletEnumType.开通成功);
            if(exist == null)
            {
                throw new ErrorCodeException(-1, "未开通e钱包");
            }

            string json = JsonConvert.SerializeObject(exist);
            var res = await httpService.PostAsync<BankWalletAcctClose>(ServiceNames.BankService, "/api/BankWallet/sendSmsBankWalletLogout", json);
            if (res.Code == 200)
            {

                if (res.Data.Code == 0)
                {
                    exist.CloseCorpSerno = res.Data.CorpSerno;
                    exist.SmsSendNo = res.Data.SmsSendNo;

                }
                else
                {
                    exist.Code = res.Data.Code.ToString();
                    exist.Msg = res.Data.Msg;
                }
                await bankWalletService.Update(exist);
                return Ok(new { items = exist });
            }
            else
            {
                throw new ErrorCodeException(-1, res.Message ?? "e钱包销户发验证码失败");
            }



        }


        //e钱包销户校对验证码
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CloseAcctSmsCheck([FromBody] Model.Live_BankWallet model)
        {
            
            
            string json = JsonConvert.SerializeObject(model);
            var res = await httpService.PostAsync<BankWalletAcctCloseMsgCheck>(ServiceNames.BankService, "/api/BankWallet/sendSmsBankWalletLogoutCheck", json);
            if (res.Code == 200)
            {

                if (res.Data.Code == 0)
                {
                    model.Status = (int)BankWalletEnumType.已销户;
                    await bankWalletService.Update(model);
                    return Ok(new { items = model });

                }
                else
                {
                    throw new ErrorCodeException(-1, res.Data.Msg);
                }
                
            }
            else
            {
                throw new ErrorCodeException(-1, res.Message ?? "e钱包销户校对验证码失败");
            }



        }
    }
}
