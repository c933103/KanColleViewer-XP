using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
    public enum NightBattleAttackType
    {
        [Description("×")]
        None,

        [Description("纯雷击CI")]
        TorpedoCutIn,

        [Description("纯炮击CI - 三主炮")]
        TriArtilleryCutIn,

        [Description("纯炮击CI - 双主炮+副炮")]
        DualArtilleryWithCannonCutIn,

        [Description("炮雷混合CI - 双主炮+鱼雷")]
        DualArtilleryWithTorpedoCutIn,

        [Description("炮雷混合CI - 主炮+鱼雷")]
        ArtilleryWithTorpedoCutIn,

        [Description("二连 - 双主炮")]
        DualArtillery,

        [Description("二连 - 主副炮")]
        ArtilleryWithCannon,

        [Description("二连 - 双副炮")]
        DualCannon,
    }

    [Flags]
    public enum SpecialAttackType
    {
        [Description("×")]
        None = 0,

        [Description("弹着观测 - 双主炮")]
        DualArtilleryWithCorrection = 1,

        [Description("弹着观测 - 主炮+彻甲弹")]
        ArtilleryWithApShellWithCorrection = 2,

        [Description("弹着观测 - 主炮+电探")]
        ArtilleryWithRadarCorrection = 4,

        [Description("弹着观测 - 主炮+副炮")]
        ArtilleryWithCanonWithCorrection = 8,

        [Description("二连 - 双主炮")]
        DualArtillery = 16,
    }

    public enum AaCutInType
    {
        [Description("×")]
        None,

        [Description("秋月 / 双高角炮 / 有电探")]
        AkizukiDualNavalGunWithRadar,

        [Description("秋月 / 高角炮 / 有电探")]
        AkizukiNavalGunWithRadar,

        [Description("秋月 / 双高角炮 / 无电探")]
        AkizukiDualNavalGunNoRadar,

        [Description("大口径主炮 / 三式弹 / 高射装置 / 有电探")]
        ArtilleryAaT3ShellWithRadar,

        [Description("双高角炮+高射装置 / 有电探")]
        DualNavalAndAaGunWithRadar,

        [Description("大口径主炮 / 三式弹 / 高射装置 / 无电探")]
        ArtilleryAaT3ShellNoRadar,

        [Description("高角炮 / 高射装置 / 有电探")]
        NavalGunWithAaGunWithRadar,

        [Description("高角炮+高射装置 / 有电探")]
        NavalAndAaGunWithRadar,

        [Description("高角炮 / 高射装置 / 无电探")]
        NavalGunWithAaGunNoRadar,
    }
}
