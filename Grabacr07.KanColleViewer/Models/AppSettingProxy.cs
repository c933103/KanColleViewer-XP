using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
    static class AppSettingProxy
    {
        public static Uri KanColleUrl => Properties.Settings.Default.KanColleUrl;
    }
}
