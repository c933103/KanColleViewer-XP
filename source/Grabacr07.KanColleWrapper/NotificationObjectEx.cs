using Livet;
using System.Runtime.CompilerServices;

namespace Grabacr07.KanColleWrapper
{
    public class NotificationObjectEx : NotificationObject
    {
        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
        }
    }
}
