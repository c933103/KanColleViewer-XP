using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
    public enum FlashOverrideMode
    {
        [Description("通过COM接口修改Flash参数，修改渲染模式时可能白屏")]
        Dispatch,

        [Description("删除并重建Flash标记，会刷新Flash，可能卡黑船")]
        Reload,

        [System.Xml.Serialization.XmlEnum("Intercept")]
        [Description("拦截网络通信并注入脚本，不能实时修改")]
        Shim
    }

    public enum FlashQuality
    {
        [Description("使用舰C的默认设置")]
        Default,

        [Description("平滑贴图关闭，抗锯齿关闭")]
        Low,

        [Description("平滑贴图关闭，抗锯齿按需开启")]
        AutoLow,

        [Description("平滑贴图关闭，抗锯齿按需关闭")]
        AutoHigh,

        [Description("平滑贴图关闭，抗锯齿开启")]
        Medium,

        [Description("静态平滑贴图开启，抗锯齿开启（2015年5月 舰C的默认设置）")]
        High,

        [Description("平滑贴图完全开启，抗锯齿开启")]
        Best
    }

    public enum FlashRenderMode
    {
        [Description("使用舰C的默认设置")]
        Default,

        [Description("独立HWND句柄，硬件加速不可用")]
        Window,

        [Description("透明混合启用，硬件加速不可用")]
        Transparent,

        [Description("透明混合关闭，硬件加速关闭（2015年5月 舰C的默认设置）")]
        Opaque,

        [Description("硬件加速开启")]
        Direct,

        [Description("完全硬件加速，兼容性较差")]
        GPU
    }
}
