using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Model.ApiModel;
using LZ.Model.Enums;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LZ.ApiService.Services
{
    public class HupunERPService(ISettingsService settingsService,IHttpService httpService) : IHupunERPService, IScopedDependency
    {
        private string serviceUrl = "https://open-api.hupun.com/api/erp/b2c";
        private string appKey = "3068922137";
        private string secret = "309ba6f9ad4c2286e67943aca9d222e8";
        private string shopName = "幸福职生活商城";
        public async Task<HupunResultModel?> Categories(List<Model.Shop_Categories> items, int status=1)
        {
            if (items.Count == 0)
            {
                return null;
            }
            var url = serviceUrl + "/categories/open";
            var settings = await settingsService.GetMasterSettings();
            if (!settings.IsProduction)
            {
                return null;
            }
            var json = new StringBuilder();
            json.Append("[");
            foreach (var category in items)
            {
                json.Append("{");
                json.AppendFormat("\"category_id\":\"{0}\",", category.CateID);
                json.AppendFormat("\"name\":\"{0}\",", category.Name);
                json.AppendFormat("\"parent_id\":\"{0}\",", category.ParentID);
                json.AppendFormat("\"shop_nick\":\"{0}\",", shopName);
                json.AppendFormat("\"status\":{0}", status);
                json.Append("},");
            }
            
            json.Length--;
            json.Append("]");
            var paras = new Dictionary<string, string>();
            paras.Add("categories", json.ToString());
            var data = SignParameters(paras);
            var res = await httpService.PostFormAsync(url, data);
            Common.StaticLog.Debug(res);
            return JsonSerializer.Deserialize<Model.ApiModel.HupunResultModel>(res);
        }
        public async Task<HupunResultModel?> Product(List<Model.Shop_Product> models, List<Model.Shop_SKUs> skuModels = null, List<Model.Shop_V_SKUItems> skuItems = null, int status = 1)
        {
            var settings = await settingsService.GetMasterSettings();
            if (!settings.IsProduction)
            {
                return null;
            }
            var url = serviceUrl + "/items/open";
            var json = new StringBuilder();
            json.Append("[");
            foreach (var model in models)
            {
                if (model.IsVirtual) continue;
                json.Append("{");
                json.AppendFormat("\"create_time\":\"{0}\",", model.CreateTime);
                json.AppendFormat("\"item_id\":\"{0}\",", model.ProductID);
                json.AppendFormat("\"modify_time\":\"{0}\",", DateTime.Now);
                json.AppendFormat("\"shop_nick\":\"{0}\",", shopName);
                json.AppendFormat("\"status\":{0},", status);
                json.AppendFormat("\"title\":\"{0}\",", model.ProductName);
                json.AppendFormat("\"barcode\":\"{0}\",", model.Barcode);
                json.AppendFormat("\"brand\":\"{0}\",", model.BrandID);
                json.AppendFormat("\"category_id\":\"{0}\",", model.CateID);
                json.AppendFormat("\"image_url\":\"{0}\",", model.Picture);
                json.AppendFormat("\"item_code\":\"{0}\",", model.ProductCode);
                json.AppendFormat("\"price\":{0},", model.ProductPrice);
                //json.AppendFormat("\"quantity\":{0},", model.ProductCount);
                json.AppendFormat("\"unit\":\"{0}\",", model.Unit);
                json.AppendFormat("\"weight\":{0},", model.Weight.HasValue ? model.Weight : 0);
                json.Append("\"skus\":[");
                if (model.HasSKU && skuModels.Count > 0)
                {
                    var skus = skuModels.Where(it => it.ProductID == model.ProductID);
                    foreach (var sku in skus)
                    {
                        var items = skuItems.Where(it => it.SkuID == sku.SkuID);
                        if (items.Count() > 0)
                        {
                            json.Append("{");
                            var attributes = new StringBuilder();
                            var image = "";
                            foreach (var item in items)
                            {
                                attributes.AppendFormat("{0}:{1};", item.AttributeName, item.ValueName);
                                image = !string.IsNullOrEmpty(item.Image)? item.Image: model.Picture;
                            }
                            attributes.Length--;
                            json.AppendFormat("\"attributes\":\"{0}\",", attributes.ToString());
                            json.AppendFormat("\"barcode\":\"{0}\",", sku.Barcode);
                            json.AppendFormat("\"create_time\":\"{0}\",", model.CreateTime);
                            json.AppendFormat("\"image_url\":\"{0}\",", image);
                            json.AppendFormat("\"item_id\":{0},", sku.ProductID);
                            json.AppendFormat("\"price\":{0},", sku.SalePrice);
                           // json.AppendFormat("\"quantity\":{0},", sku.Stock);
                            json.AppendFormat("\"sku_code\":\"{0}\",", sku.SKU);
                            json.AppendFormat("\"sku_id\":\"{0}\",", sku.SkuID);
                            json.AppendFormat("\"status\":{0},", status);
                            json.AppendFormat("\"weight\":{0}", sku.Weight.HasValue ? sku.Weight : 0);
                            json.Append("},");
                        }
                    }
                    json.Length--;
                }
                json.Append("]");
                json.Append("},");
            }
            json.Length--;
            json.Append("]");
            var paras = new Dictionary<string, string>();
            paras.Add("items", json.ToString().Replace("*", "-"));
            //Common.StaticLog.Debug(json.ToString());
            var data = SignParameters(paras);
            //Common.StaticLog.Debug(JsonSerializer.Serialize(data));
            var res = await httpService.PostFormAsync(url, data);
            Common.StaticLog.Debug(res);
            return JsonSerializer.Deserialize<Model.ApiModel.HupunResultModel>(res);
        }
        public async Task<HupunResultModel?> Product(Model.Shop_Product model, List<Model.Shop_SKUs> skus=null, List<Model.Shop_V_SKUItems> skuItems=null, int status = 1)
        {
            var url = serviceUrl + "/items/open";
            var settings = await settingsService.GetMasterSettings();
            if (!settings.IsProduction)
            {
                return null;
            }
            var json = new StringBuilder();
            json.Append("[{");
            json.AppendFormat("\"create_time\":\"{0}\",", model.CreateTime);
            json.AppendFormat("\"item_id\":\"{0}\",", model.ProductID);
            json.AppendFormat("\"modify_time\":\"{0}\",", DateTime.Now);
            json.AppendFormat("\"shop_nick\":\"{0}\",", shopName);
            json.AppendFormat("\"status\":{0},", status);
            json.AppendFormat("\"title\":\"{0}\",", model.ProductName);
            json.AppendFormat("\"barcode\":\"{0}\",", model.Barcode);
            json.AppendFormat("\"brand\":\"{0}\",", model.BrandID);
            json.AppendFormat("\"category_id\":\"{0}\",", model.CateID);
            json.AppendFormat("\"image_url\":\"{0}\",", model.Picture);
            json.AppendFormat("\"item_code\":\"{0}\",", model.ProductCode);
            json.AppendFormat("\"price\":{0},", model.ProductPrice);
            //json.AppendFormat("\"quantity\":{0},", model.ProductCount);
            json.AppendFormat("\"unit\":\"{0}\",", model.Unit);
            json.AppendFormat("\"weight\":{0},", model.Weight.HasValue ? model.Weight : 0);
            json.Append("\"skus\":[");
            if (model.HasSKU && skus.Count>0)
            {
                foreach (var sku in skus)
                {
                    var items = skuItems.Where(it=>it.SkuID==sku.SkuID);
                    if (items.Count() > 0)
                    {
                        json.Append("{");
                        var attributes = new StringBuilder();
                        var image = "";
                        foreach (var item in items)
                        {
                            attributes.AppendFormat("{0}:{1};", item.AttributeName, item.ValueName);
                            image = !string.IsNullOrEmpty(item.Image) ? item.Image : model.Picture;
                        }
                        attributes.Length--;
                        json.AppendFormat("\"attributes\":\"{0}\",", attributes.ToString());
                        json.AppendFormat("\"barcode\":\"{0}\",", sku.Barcode);
                        json.AppendFormat("\"create_time\":\"{0}\",", model.CreateTime);
                        json.AppendFormat("\"image_url\":\"{0}\",", image);
                        json.AppendFormat("\"price\":{0},", sku.SalePrice);
                        json.AppendFormat("\"item_id\":{0},", sku.ProductID);
                        //json.AppendFormat("\"quantity\":{0},", sku.Stock);
                        json.AppendFormat("\"sku_code\":\"{0}\",", sku.SKU);
                        json.AppendFormat("\"sku_id\":\"{0}\",", sku.SkuID);
                        json.AppendFormat("\"status\":{0},", status);
                        json.AppendFormat("\"weight\":{0}", sku.Weight.HasValue ? sku.Weight : 0);
                        json.Append("},");
                    }
                }
                json.Length--;
            }
            json.Append("]");
            json.Append("}]");
            var paras = new Dictionary<string, string>();
            paras.Add("items", json.ToString().Replace("*", "-"));
            var data = SignParameters(paras);
            var res = await httpService.PostFormAsync(url, data);
            Common.StaticLog.Debug(res);
            return JsonSerializer.Deserialize<Model.ApiModel.HupunResultModel>(res);
        }

        public async Task<HupunResultModel?> Order(Model.Shop_Order model, List<Model.Shop_V_OrderItems> items, int status = 2,int has_refund=0)
        {
            var url = serviceUrl + "/trades/open";
            var settings = await settingsService.GetMasterSettings();
            if (!settings.IsProduction)
            {
                return null;
            }
            var json = new StringBuilder();
            json.Append("[{");
            json.AppendFormat("\"create_time\":\"{0}\",", model.OrderDate);
            json.AppendFormat("\"trade_id\":\"{0}\",", model.OrderID);
            json.AppendFormat("\"modify_time\":\"{0}\",", DateTime.Now);
            json.AppendFormat("\"shop_nick\":\"{0}\",", shopName);
            json.AppendFormat("\"buyer\":\"{0}\",", model.FullName);
            if (model.FinishDate.HasValue && model.OrderStatus == (int)OrderStatus.Have)
            {
                json.AppendFormat("\"end_time\":\"{0}\",", model.FinishDate);
            }
            switch (model.OrderStatus)
            {
                case (int)OrderStatus.WaitPay:
                    status = 1;
                    break;
                case (int)OrderStatus.WaitShip:
                    status = 2;
                    break;
                case (int)OrderStatus.Have:
                    status = 3;
                    break;
                case (int)OrderStatus.Refund:
                    has_refund = 1;
                    status = 4;
                    break;
                case (int)OrderStatus.Cancel:
                    status = 4;
                    break;
                case (int)OrderStatus.WaitReceipt:
                    status = 5;
                    break;
                default:
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "该订单状态不能同步！" } };
            }
            json.AppendFormat("\"status\":{0},", status);
            json.AppendFormat("\"receiver_address\":\"{0}\",", model.Address);
            json.AppendFormat("\"receiver_name\":\"{0}\",", model.ShippName);
            json.AppendFormat("\"has_refund\":{0},", has_refund);
            json.AppendFormat("\"pay_no\":\"{0}\",", model.PayID);
            json.AppendFormat("\"pay_time\":\"{0}\",", model.PayDate);
            json.AppendFormat("\"pay_type\":\"{0}\",", model.PayType);
            json.AppendFormat("\"payment\":{0},", model.OrderTotal);
            json.AppendFormat("\"post_fee\":{0},", 0);
            json.AppendFormat("\"receiver_area\":\"{0}\",", model.District);
            json.AppendFormat("\"receiver_city\":\"{0}\",", model.City);
            json.AppendFormat("\"receiver_mobile\":\"{0}\",", model.ShippPhone);
            json.AppendFormat("\"receiver_province\":\"{0}\",", model.Province);
            json.AppendFormat("\"receiver_zip\":\"{0}\",", "");
            json.AppendFormat("\"seller_memo\":\"{0}\",", model.SellerRemark);
            json.AppendFormat("\"total_fee\":{0},", model.OrderTotal);
            json.AppendFormat("\"trade_type\":{0},", 1);
            json.Append("\"orders\":[");
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    json.Append("{");
                    json.AppendFormat("\"item_id\":\"{0}\",", item.ProductID);
                    json.AppendFormat("\"item_title\":\"{0}\",", item.ProductName);
                    json.AppendFormat("\"order_id\":\"{0}\",", item.ID);
                    json.AppendFormat("\"image_url\":\"{0}\",", item.Picture);
                    json.AppendFormat("\"size\":{0},", item.ProductCount);
                    json.AppendFormat("\"price\":{0},", item.ProductPrice);
                    json.AppendFormat("\"payment\":{0},", item.TotalMoney);
                    json.AppendFormat("\"item_code\":\"{0}\",", item.ProductCode);
                    if(item.HasSKU)//是否启动sku
                    {
                        json.AppendFormat("\"sku_code\":\"{0}\",", item.SKU);
                        json.AppendFormat("\"sku_id\":\"{0}\",", item.SkuID);
                        json.AppendFormat("\"sku_title\":\"{0}\",", item.SkuContent);
                    }
                    json.AppendFormat("\"trade_id\":\"{0}\",", item.OrderID);
                    json.AppendFormat("\"status\":{0}", status);
                    json.Append("},");
                }
                json.Length--;
            }
            json.Append("]");
            json.Append("}]");
            var paras = new Dictionary<string, string>();
            paras.Add("trades", json.ToString().Replace("*", "-"));
            var data = SignParameters(paras);
            var res = await httpService.PostFormAsync(url, data);
            Common.StaticLog.Debug(data);
            return JsonSerializer.Deserialize<Model.ApiModel.HupunResultModel>(res);
        }
        public async Task<HupunResultModel?> OrderReturn(Model.Shop_OrderReturn model, List<Model.Shop_V_OrderReturnItems> items)
        {
            var url = serviceUrl + "/refunds/open";
            var settings = await settingsService.GetMasterSettings();
            if (!settings.IsProduction)
            {
                return null;
            }
            var status = 0;
            var json = new StringBuilder();
            json.Append("{");
            json.AppendFormat("\"buyer\":\"{0}\",", "");
            json.AppendFormat("\"create\":\"{0}\",", model.CreateTime);
            json.AppendFormat("\"express_code\":\"{0}\",", model.ExpressNo);
            json.AppendFormat("\"logistics_name\":\"{0}\",", model.ExpressName);
            json.AppendFormat("\"modified\":\"{0}\",", DateTime.Now);
            json.AppendFormat("\"reason\":\"{0}\",", model.ReturnReason.Replace("*", "-"));
            json.Append("\"receiver\":{");
            json.AppendFormat("\"receiver_address\":\"{0}\",", model.ReceiverAddress);
            json.AppendFormat("\"receiver_area\":\"{0}\",", model.ReceiverDistrict);
            json.AppendFormat("\"receiver_city\":\"{0}\",", model.ReceiverCity);
            json.AppendFormat("\"receiver_mobile\":\"{0}\",", model.ReceiverMobile);
            json.AppendFormat("\"receiver_name\":\"{0}\",", model.ReceiverName);
            json.AppendFormat("\"receiver_province\":\"{0}\",", model.ReceiverProvince);
            json.Append("},");
            json.AppendFormat("\"refund_fee\":\"{0}\",", model.ReturnMoney);
            json.AppendFormat("\"refund_id\":\"{0}\",", model.ReturnID);
            json.AppendFormat("\"shop_nick\":\"{0}\",", shopName);
            json.AppendFormat("\"trade_id\":\"{0}\",", model.OrderID);
            json.AppendFormat("\"type\":{0},", model.ReturnType);
            if (model.ReturnDate.HasValue && (model.Status == (int)OrderReturnStatus.RefundFinish || model.Status == (int)OrderReturnStatus.ExchangeClose || model.Status == (int)OrderReturnStatus.Cancel || model.Status == (int)OrderReturnStatus.ExchangeSuccess))
            {
                json.AppendFormat("\"end_time\":\"{0}\",", model.ReturnDate);
            }
            switch (model.Status)
            {
                case (int)OrderReturnStatus.WaitReturn:
                    status = 2;
                    break;
                case (int)OrderReturnStatus.WaitReceipt:
                    status = 3;
                    break;
                case (int)OrderReturnStatus.WaitChange:
                    status = 4;
                    break;
                case (int)OrderReturnStatus.NoPass:
                case (int)OrderReturnStatus.Cancel:
                case (int)OrderReturnStatus.ExchangeClose:
                    status = 5;
                    break;
                case (int)OrderReturnStatus.RefundFinish:
                case (int)OrderReturnStatus.ExchangeSuccess:
                    status = 6;
                    break;
                case (int)OrderReturnStatus.WaitExchange:
                    status = 7;
                    break;
                default:
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "该退货状态不能同步！" } };
            }
            json.AppendFormat("\"status\":{0},", status);
            json.Append("\"items\":[");
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    json.Append("{");
                    json.AppendFormat("\"order_id\":\"{0}\",", item.ID);
                    if (model.ReturnType == 3)
                    {
                        json.AppendFormat("\"change_item_id\":\"{0}\",", item.ChangeProductID);
                        json.AppendFormat("\"change_num\":{0},", item.ChangeProductCount);
                        json.AppendFormat("\"change_sku_id\":\"{0}\",", item.ChangeSkuID);
                    }
                    json.AppendFormat("\"sku_code\":\"{0}\",", item.SKU);
                    json.AppendFormat("\"sku_num\":{0}", item.ProductCount);
                    json.Append("},");
                }
                json.Length--;
            }
            json.Append("]");
            json.Append("}");
            var paras = new Dictionary<string, string>();
            paras.Add("refund", json.ToString().Replace("*", "-"));
            var data = SignParameters(paras);
            var res = await httpService.PostFormAsync(url, data);
            Common.StaticLog.Debug(res);
            return JsonSerializer.Deserialize<Model.ApiModel.HupunResultModel>(res);
        }
        public string JavaURLEncode(string str)
        {
            string encodedStr = Uri.EscapeDataString(str)
                .Replace("!", "%21")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("~", "%7E")
                .Replace("%20", "+");
            return encodedStr;
        }

        public Dictionary<string, string> SignParameters(Dictionary<string, string> parameters)
        {
            if(parameters==null) parameters = new Dictionary<string, string>();
            var sortedParams = new SortedDictionary<string, string>(parameters)
        {
            { "_app", appKey },
            { "_t", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
        };

            var queryBuilder = new StringBuilder();
            foreach (var pair in sortedParams)
            {
                queryBuilder.Append(pair.Key)
                            .Append("=")
                            .Append(JavaURLEncode(pair.Value))
                            .Append("&");
            }

            queryBuilder.Length--; // Remove the last '&'

            var content = secret + queryBuilder + secret;
            var sign = Md5Hash(content);
            sortedParams.Add("_sign", sign);

            return new Dictionary<string, string>(sortedParams);
        }

        private string Md5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
