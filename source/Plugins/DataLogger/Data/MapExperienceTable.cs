﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Data
{
    class MapExperienceTable
    {
        private static readonly Dictionary<string, int> table = new Dictionary<string, int>() {
            ["1-1"] = 30,
            ["1-2"] = 50,
            ["1-3"] = 80,
            ["1-4"] = 100,
            ["1-5"] = 150,
            ["2-1"] = 120,
            ["2-2"] = 150,
            ["2-3"] = 200,
            ["2-4"] = 300,
            ["2-5"] = 250,
            ["3-1"] = 310,
            ["3-2"] = 320,
            ["3-3"] = 330,
            ["3-4"] = 350,
            ["3-5"] = 400,
            ["4-1"] = 310,
            ["4-2"] = 320,
            ["4-3"] = 330,
            ["4-4"] = 340,
            ["5-1"] = 360,
            ["5-2"] = 380,
            ["5-3"] = 400,
            ["5-4"] = 420,
            ["5-5"] = 450,
            ["6-1"] = 380,
            ["6-2"] = 420,
        };
        
        public int this[string k] => (k != null && table.ContainsKey(k)) ? table[k] : 2;
        public IEnumerable<string> Keys => table.Keys;

        public static MapExperienceTable Instance => _instance;
        private static readonly MapExperienceTable _instance = new MapExperienceTable();

        private MapExperienceTable() { }
    }
}
