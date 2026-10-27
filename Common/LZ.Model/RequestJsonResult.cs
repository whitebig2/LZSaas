using Microsoft.AspNetCore.Mvc;

namespace LZ.Model
{

    public class RequestJsonResult : JsonResult
    {
        public RequestJsonResult(object value) : base(value)
        {
        }
    }
       
}
