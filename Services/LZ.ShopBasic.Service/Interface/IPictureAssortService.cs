using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IPictureAssortService
    {
        Task<List<Model.Shop_PictureAssort>> List(Expression<Func<Model.Shop_PictureAssort, bool>> exp);

        Task<int> Add(Model.Shop_PictureAssort model);

        Task<bool> Update(Model.Shop_PictureAssort model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_PictureAssort, bool>> exp);
        Task<Model.Shop_PictureAssort> Get(int id);
    }
}
