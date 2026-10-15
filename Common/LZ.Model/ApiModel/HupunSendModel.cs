using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LZ.Model.ApiModel
{
    public class HupunSendModel
    {
        public HupunSendDataModel data { get; set; }
    }
    public class HupunSendDataModel
    {
        public string trade_id { get; set; }
        public string delivery_code { get; set; }
        public string delivery_name { get; set; }
        public string waybill { get; set; }
        public string shop_nick { get; set; }
        [JsonConverter(typeof(HupunJsonStringOrListConverter<HupunSendPackageModel>))]
        public List<HupunSendPackageModel> packages { get; set; }
    }

    public class HupunSendPackageModel
    {
        public string deliveryName { get; set; }
        public string deliveryCode { get; set; }
        public string wayBill { get; set; }
        [JsonConverter(typeof(HupunJsonStringOrListConverter<HupunSendOrderItemModel>))]
        public List<HupunSendOrderItemModel> orderInfos { get; set; }
    }

    public class HupunSendOrderItemModel
    {
        public object orderID { get; set; }
        public int num { get; set; }

        public int? GetOrderItemId()
        {
            if (orderID == null)
            {
                return null;
            }
            if (int.TryParse(orderID.ToString(), out var id) && id > 0)
            {
                return id;
            }
            return null;
        }
    }

    public class ErpPackageSendResult
    {
        public bool Success { get; set; }
        public bool IsNewSend { get; set; }
        public LZ.Model.Shop_Order Order { get; set; }
        public string ExpressNo { get; set; }
        public string ExpressType { get; set; }
        public string ExpressName { get; set; }
        public List<ErpExpressSubscribeItem> SubscribeItems { get; set; } = new List<ErpExpressSubscribeItem>();
    }

    public class ErpExpressSubscribeItem
    {
        public string ExpressNo { get; set; }
        public string ExpressType { get; set; }
    }

    public class HupunJsonStringOrListConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                var json = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            var token = JToken.Load(reader);
            return token.ToObject<List<T>>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
