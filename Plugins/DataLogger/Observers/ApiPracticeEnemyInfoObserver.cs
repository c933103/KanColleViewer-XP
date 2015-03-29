using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using LynLogger.Models.Battling;
using LynLogger.Utilities;
using Codeplex.Data;

namespace LynLogger.Observers
{
    class ApiPracticeEnemyInfoObserver : IObserver<Session>
    {
        private Action<PracticeEnemyInfo> _onPracticeEnemyInfo;
        public event Action<PracticeEnemyInfo> OnPracticeEnemyInfo
        {
            add { _onPracticeEnemyInfo += value.MakeWeak(x => _onPracticeEnemyInfo -= x); }
            remove { }
        }

        public void OnNext(Session value)
        {
            if(_onPracticeEnemyInfo == null) return;

            try {
                var response = value.ResponseBody;
                string json = Encoding.ASCII.GetString(response, 7, response.Length-7);
                dynamic res = DynamicJson.Parse(json);
                if(!res.api_result() || res.api_result != 1) return;

                var data = res.api_data;
                List<PracticeEnemyInfo.EnemyShipInfo> enemyShips = new List<PracticeEnemyInfo.EnemyShipInfo>();

                var ships = data.api_deck.api_ships;
                for(int i = 0; ships.IsDefined(i); i++) {
                    if(ships[i].api_id < 0) break;
                    enemyShips.Add(new PracticeEnemyInfo.EnemyShipInfo() {
                        ZwId = (int)ships[i].api_id,
                        ZwLevel = (int)ships[i].api_level,
                        ZwStar = (int)ships[i].api_star,
                        ZwShipType = Helpers.LookupShipNameInfo((int)ships[i].api_ship_id)
                    });
                }

                PracticeEnemyInfo result = new PracticeEnemyInfo() {
                    ZwEnemyShips = enemyShips.ToArray(),
                    ZwRawData = json
                };
                _onPracticeEnemyInfo(result);
            } catch (Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }

        public void OnCompleted() { return; }
        public void OnError(Exception error) { return; }
    }
}
