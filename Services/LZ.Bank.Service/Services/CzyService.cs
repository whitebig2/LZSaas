using LZ.ApiService.ContractResolvers;
using LZ.ApiService.Helper;
using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Model.Bank.CZY;
using LZ.Model.Bank.CZY.BodyData;
using LZ.Model.Bank.CZY.BusinessModels;
using LZ.Model.Bank.CZY.ParamSetModels;
using LZ.Model.Bank.CZY.Request;
using LZ.Model.Bank.CZY.Response;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace LZ.Bank.Service.Services
{
    public class CzyService:ICzyService, IScopedDependency
    {
        private readonly CzyOption _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public CzyService(IOptions<CzyOption> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 1.2.1. T0001-交易流水信息查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>

        public async Task<CzyCmscloud<T0001Response>> QueryTransactionHistoryRecords(T0001ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0001",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };
            try
            {

            if (string.IsNullOrWhiteSpace(paramSetItem.AcctNo))
                throw new Exception("【AcctNo】参数缺失");

            if (string.IsNullOrWhiteSpace(paramSetItem.CurrencyCode))
                paramSetItem.CurrencyCode = "CNY";

            if (string.IsNullOrWhiteSpace(paramSetItem.StartTransDate))
                paramSetItem.StartTransDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (string.IsNullOrWhiteSpace(paramSetItem.EndTransDate))
                paramSetItem.EndTransDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (string.IsNullOrWhiteSpace(paramSetItem.NextTag))
                paramSetItem.NextTag = "1";

            
            var body = new CzyCommonBody<T0001ParamSetBodyData>()
            {
                Data = new T0001ParamSetBodyData()
                {
                    ParamSet = new List<T0001ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,
            };

            T0001Request transRequest = new T0001Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0001Request> request = new CzyCmscloud<T0001Request>()
            {
                Cmscloud = transRequest
            };
            
                var res = await ExecuteAsync<CzyCmscloud<T0001Request>, CzyCmscloud<T0001Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0001Response> res = new CzyCmscloud<T0001Response>()
                {
                    Cmscloud = new T0001Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0001ResultSetBodyData>()
                        {
                            Data = new T0001ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }
        public async Task<CzyCmscloud<T0002Response>> TransSingle(T0002ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0002",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0002ParamSetBodyData>()
            {
                Data = new T0002ParamSetBodyData()
                {

                    ParamSet = new List<T0002ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0002Request transRequest = new T0002Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0002Request> request = new CzyCmscloud<T0002Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0002Request>, CzyCmscloud<T0002Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0002Response> res = new CzyCmscloud<T0002Response>()
                {
                    Cmscloud = new T0002Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0002ResultSetBodyData>()
                        {
                            Data = new T0002ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }
        /// <summary>
        /// 工资代发 批量对私
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0003Response>> TransBatchToPerson(T0003ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0003",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = Guid.NewGuid().ToString("N"),
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0003ParamSetBodyData>()
            {
                Data = new T0003ParamSetBodyData()
                {

                    ParamSet = new List<T0003ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0003Request transRequest = new T0003Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0003Request> request = new CzyCmscloud<T0003Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0003Request>, CzyCmscloud<T0003Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0003Response> res = new CzyCmscloud<T0003Response>()
                {
                    Cmscloud = new T0003Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0003ResultSetBodyData>()
                        {
                            Data = new T0003ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 1.2.4. T0004-应付单业务状态查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0004Response>> QueryTransactionDetail(T0004ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0004",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0004ParamSetBodyData>()
            {
                Data = new T0004ParamSetBodyData()
                {

                    ParamSet = new List<T0004ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0004Request transRequest = new T0004Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0004Request> request = new CzyCmscloud<T0004Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0004Request>, CzyCmscloud<T0004Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0004Response> res = new CzyCmscloud<T0004Response>()
                {
                    Cmscloud = new T0004Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0004ResultSetBodyData>()
                        {
                            Data = new T0004ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 1.2.5. T0005-账户信息查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0005Response>> QueryAccountInfo(T0005ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0005",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0005ParamSetBodyData>()
            {
                Data = new T0005ParamSetBodyData()
                {

                    ParamSet = new List<T0005ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0005Request transRequest = new T0005Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0005Request> request = new CzyCmscloud<T0005Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0005Request>, CzyCmscloud<T0005Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0005Response> res = new CzyCmscloud<T0005Response>()
                {
                    Cmscloud = new T0005Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0005ResultSetBodyData>()
                        {
                            Data = new T0005ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 1.2.6. T0006-账户余额查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0006Response>> QueryAccountBalanceInfo(T0006ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0006",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0006ParamSetBodyData>()
            {
                Data = new T0006ParamSetBodyData()
                {

                    ParamSet = new List<T0006ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0006Request transRequest = new T0006Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0006Request> request = new CzyCmscloud<T0006Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0006Request>, CzyCmscloud<T0006Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0006Response> res = new CzyCmscloud<T0006Response>()
                {
                    Cmscloud = new T0006Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0006ResultSetBodyData>()
                        {
                            Data = new T0006ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 金融网点查询
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0014Response>> QueryBankBranchesInfo(T0014ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0014",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0014ParamSetBodyData>()
            {
                Data = new T0014ParamSetBodyData()
                {

                    ParamSet = new List<T0014ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0014Request transRequest = new T0014Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0014Request> request = new CzyCmscloud<T0014Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0014Request>, CzyCmscloud<T0014Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0014Response> res = new CzyCmscloud<T0014Response>()
                {
                    Cmscloud = new T0014Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0014ResultSetBodyData>()
                        {
                            Data = new T0014ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }




        public async Task<CzyCmscloud<T0018Response>> TransBatch(T0018ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0018",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0018ParamSetBodyData>()
            {
                Data = new T0018ParamSetBodyData()
                {

                    ParamSet = new List<T0018ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0018Request transRequest = new T0018Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0018Request> request = new CzyCmscloud<T0018Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0018Request>, CzyCmscloud<T0018Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0018Response> res = new CzyCmscloud<T0018Response>()
                {
                    Cmscloud = new T0018Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0018ResultSetBodyData>()
                        {
                            Data = new T0018ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = $"未能获取到正确的响应信息，异常信息：{ex.Message}"
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 余额对账信息查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0019Response>> QueryAccountBalanceBill(T0019ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0019",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0019ParamSetBodyData>()
            {
                Data = new T0019ParamSetBodyData()
                {

                    ParamSet = new List<T0019ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0019Request transRequest = new T0019Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0019Request> request = new CzyCmscloud<T0019Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0019Request>, CzyCmscloud<T0019Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0019Response> res = new CzyCmscloud<T0019Response>()
                {
                    Cmscloud = new T0019Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0019ResultSetBodyData>()
                        {
                            Data =new T0019ResultSetBodyData() { 
                            
                             ResultCode="",
                              ResultMsg="",
                               ResultSet=null
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }


        /// <summary>
        /// 1.2.20. T0022-通用付款接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0022Response>> TransCommon(T0022ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0022",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0022ParamSetBodyData>()
            {
                Data = new T0022ParamSetBodyData()
                {

                    ParamSet = new List<T0022ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0022Request transRequest = new T0022Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0022Request> request = new CzyCmscloud<T0022Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0022Request>, CzyCmscloud<T0022Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0022Response> res = new CzyCmscloud<T0022Response>()
                {
                    Cmscloud = new T0022Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0022ResultSetBodyData>()
                        {
                            Data = new T0022ResultSetBodyData()
                            {
                                ResultCode = "",
                                ResultMsg = "",
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }

        /// <summary>
        /// 认领结果查询接口
        /// </summary>
        /// <param name="paramSetItem"></param>
        /// <returns></returns>
        public async Task<CzyCmscloud<T0023Response>> QueryClaimResult(T0023ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0023",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0023ParamSetBodyData>()
            {
                Data = new T0023ParamSetBodyData()
                {

                    ParamSet = new List<T0023ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0023Request transRequest = new T0023Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0023Request> request = new CzyCmscloud<T0023Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0023Request>, CzyCmscloud<T0023Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0023Response> res = new CzyCmscloud<T0023Response>()
                {
                    Cmscloud = new T0023Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0023ResultSetBodyData>()
                        {
                            Data = new T0023ResultSetBodyData()
                            {
                                ResultCode = "",
                                ResultMsg = "",
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }
        public async Task<CzyCmscloud<T0028Response>> QueryElectronicReceiptList(T0028ParamSetItem paramSetItem)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            CzyCommonHead head = new CzyCommonHead()
            {
                MesgNo = "T0028",
                TenantId = _options.TenantId,
                SystemName = _options.SystemName,
                SystemNo = _options.SystemNo,
                RequestNo = timestamp,
                Timestamp = timestamp,
            };

            var body = new CzyCommonBody<T0028ParamSetBodyData>()
            {
                Data = new T0028ParamSetBodyData()
                {

                    ParamSet = new List<T0028ParamSetItem>() { paramSetItem }
                },
                Sign = string.Empty,

            };

            T0028Request transRequest = new T0028Request()
            {
                Body = body,
                Head = head
            };
            CzyCmscloud<T0028Request> request = new CzyCmscloud<T0028Request>()
            {
                Cmscloud = transRequest
            };
            try
            {
                var res = await ExecuteAsync<CzyCmscloud<T0028Request>, CzyCmscloud<T0028Response>>(request);
                return res;
            }
            catch (Exception ex)
            {
                CzyCmscloud<T0028Response> res = new CzyCmscloud<T0028Response>()
                {
                    Cmscloud = new T0028Response()
                    {
                        Head = head,
                        Body = new CzyCommonBody<T0028ResultSetBodyData>()
                        {
                            Data = new T0028ResultSetBodyData()
                            {
                                ResultCode = "-1",
                                ResultMsg = ex.Message,
                            },
                            Sign = string.Empty
                        }
                    }
                };
                return res;
            }
        }
        private async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request) 
        {
            try
            {
               var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver()
                });
                Console.WriteLine(jsonRequest);
                var httpClient = _httpClientFactory.CreateClient();
                StringContent stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/dtxg", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var contentStr = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(contentStr);
                }
                else
                {
                    throw new Exception("请求失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
