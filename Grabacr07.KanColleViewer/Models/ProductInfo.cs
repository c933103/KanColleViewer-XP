using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Models
{
    public class ProductInfo
    {
        public string Title => AppProductInfo.Title;
        public string Description => AppProductInfo.Description;
        public string Company => AppProductInfo.Company;
        public string Product => AppProductInfo.Product;
        public string Copyright => AppProductInfo.Copyright;
        public string Trademark => AppProductInfo.Trademark;
        public Version Version => AppProductInfo.Version;
        public string VersionString => AppProductInfo.VersionString;
        public string ModRelease => AppProductInfo.ModRelease;
        public bool IsBetaRelease => AppProductInfo.IsBetaRelease;
        public bool IsDebug => AppProductInfo.IsDebug;
        public ICollection<Library> Libraries => AppProductInfo.Libraries;
    }
}
