using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Product.Service.Services
{
    public class CommentService : ServicesBase<Model.Shop_Comment>, ICommentService, IScopedDependency
    {
        public async Task<List<Shop_Comment>> List(Expression<Func<Shop_Comment, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Comment>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Comment>().Where(exp).OrderByDescending(it => it.CreateTime).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Shop_Comment model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Shop_Comment> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Shop_Comment, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Comment>().AnyAsync(exp);
        }

        public override async Task<bool> UpdateAsync(Shop_Comment model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Comment> Get(int id)
        {
            return await base.Context.Queryable<Shop_Comment>().FirstAsync(it => it.ID == id);
        }

        public async Task<Shop_Comment> Get(Expression<Func<Shop_Comment, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Comment>().FirstAsync(exp);
        }

        public async Task<int> Count(Expression<Func<Shop_Comment, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Comment>().CountAsync(exp);
        }

        public async Task<bool> Like(int id)
        {
            var model = await Get(id);
            if (model == null)
            {
                return false;
            }
            var likeCount = (model.LikeCount ?? 0) + 1;
            return await base.Context.Updateable<Shop_Comment>()
                .SetColumns(it => it.LikeCount == likeCount)
                .Where(it => it.ID == id)
                .ExecuteCommandAsync() > 0;
        }

        public async Task<CommentSummary> Summary(int productId)
        {
            var items = await base.Context.Queryable<Shop_Comment>()
                .Where(it => it.ProductID == productId)
                .Select(it => new { it.Score, it.Pictures })
                .ToListAsync();
            var summary = new CommentSummary();
            summary.Count = items.Count;
            if (summary.Count == 0)
            {
                return summary;
            }
            summary.PictureCount = items.Count(it => !string.IsNullOrEmpty(it.Pictures));
            summary.Score1 = items.Count(it => it.Score == 1);
            summary.Score2 = items.Count(it => it.Score == 2);
            summary.Score3 = items.Count(it => it.Score == 3);
            summary.Score4 = items.Count(it => it.Score == 4);
            summary.Score5 = items.Count(it => it.Score == 5);
            summary.GoodCount = summary.Score4 + summary.Score5;
            var scoreSum = items.Sum(it => it.Score ?? 0);
            summary.AvgScore = Math.Round((decimal)scoreSum / summary.Count, 1);
            return summary;
        }

        public async Task<bool> Delete(int id, int userId = 0)
        {
            var expable = Expressionable.Create<Shop_Comment>();
            expable.And(it => it.ID == id);
            expable.AndIF(userId > 0, it => it.UserID == userId);
            var has = await base.Context.Deleteable<Shop_Comment>().Where(expable.ToExpression()).ExecuteCommandAsync() > 0;
            if (!has)
            {
                throw new ErrorCodeException(-1, "删除失败，评价不存在或无权删除！");
            }
            return has;
        }
    }
}
