using LZ.Model;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IPromotionTaskService
    {
        Task UpdateHaveMoneyOfPromotionTask();

        Task<List<Shop_PromotionTask>> List(PromotionPageTaskModel query);

        Task<int> Add(Shop_PromotionTask model);

        Task<int> Update(Shop_PromotionTask model);

        Task<bool> Delete(int id);
    }
}
