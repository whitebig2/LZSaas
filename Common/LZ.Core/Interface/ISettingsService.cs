using LZ.Model.Enums;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface ISettingsService
    {

        Task Save(Model.SiteSettings settings);

        Task<SiteSettings> GetMasterSettings();
    }
}
