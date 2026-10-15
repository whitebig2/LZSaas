using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Users.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class UsersControllerBase : ApiControllerBase
    {
    }
}
