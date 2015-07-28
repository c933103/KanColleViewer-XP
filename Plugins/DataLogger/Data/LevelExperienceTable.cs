using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Data
{
    class LevelExperienceTable
    {
        private static readonly LevelExperienceTable _instance = new LevelExperienceTable();
        private static readonly LevelExperienceTable _accumulated = new AccumulatedLevelExperienceTable();

        public static LevelExperienceTable Instance => _instance;
        public static LevelExperienceTable Accumulated => _accumulated;

        private LevelExperienceTable() { }

        public virtual int this[int lv]
        {
            get
            {
                if(lv < 1) return int.MaxValue;
                if(lv <= 51) { return  50*lv*lv-   50*lv;        }
                if(lv <= 61) { return 100*lv*lv- 5100*lv+127500; }
                if(lv <= 71) { return 150*lv*lv-11150*lv+310500; }
                if(lv <= 81) { return 200*lv*lv-18200*lv+559000; }
                if(lv <= 91) { return 250*lv*lv-26250*lv+883000; }
                switch(lv) {
                    case 92: return 584500;
                    case 93: return 606500;
                    case 94: return 631500;
                    case 95: return 661500;
                    case 96: return 701500;
                    case 97: return 761500;
                    case 98: return 851500;
                    case 99: return 1000000;
                    case 100: return 0;
                }
                if(lv <= 111) { return  500*lv*lv- 100500*lv+ 5060000; }
                if(lv <= 116) { return 1000*lv*lv- 211000*lv+11165000; }
                if(lv <= 121) { return 1500*lv*lv- 326500*lv+17835000; }
                if(lv <= 131) { return 2000*lv*lv- 447000*lv+25095000; }
                if(lv <= 140) { return 2500*lv*lv- 577500*lv+33610000; }
                if(lv <= 145) { return 3500*lv*lv- 856500*lv+53070000; }
                if(lv <= 150) { return 4000*lv*lv-1001000*lv+63510000; }
                return int.MaxValue;
            }
        }

        private class AccumulatedLevelExperienceTable : LevelExperienceTable
        {
            public override int this[int lv]
            {
                get
                {
                    if(lv > 99) return _instance[lv] + _instance[99];
                    return _instance[lv];
                }
            }
        }
    }
}
