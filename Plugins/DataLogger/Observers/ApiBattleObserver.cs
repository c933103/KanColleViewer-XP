using Codeplex.Data;
using Grabacr07.KanColleWrapper;
using LynLogger.Models.Battling;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LynLogger.Observers
{
    class ApiBattleObserver : IObserver<Fiddler.Session>
    {
        private Action<BattleStatus> _onBattle;
        public event Action<BattleStatus> OnBattle
        {
            add { _onBattle += value.MakeWeak(x => _onBattle -= x); }
            remove { }
        }

        public void OnNext(Fiddler.Session value)
        {
            if(_onBattle == null) return;

            try {
                var response = value.ResponseBody;
                string json = Encoding.ASCII.GetString(response, 7, response.Length-7);
                dynamic res = DynamicJson.Parse(json);
                if(!res.api_result() || res.api_result != 1) return;

                var data = res.api_data;

                BattleStatus result = new BattleStatus();
                result.ZwRawData = json;

                int fleetId;
                if(data.api_dock_id()) {
                    fleetId = Convert.ToInt32(data.api_dock_id);
                } else {
                    fleetId = Convert.ToInt32(data.api_deck_id);
                }
                result.ZwOurShips = ConvertOurFleet(fleetId, data.api_nowhps, data.api_maxhps, data.api_fParam);
                result.ZwEnemyShips = ConvertEnemyFleet(data.api_ship_ke, data.api_ship_lv, data.api_nowhps, data.api_maxhps, data.api_eParam, data.api_eKyouka, data.api_eSlot);

                if(data.api_formation()) {
                    result.ZwOurFormation = (BattleStatus.Formation)(int)data.api_formation[0];
                    result.ZwEnemyFormation = (BattleStatus.Formation)(int)data.api_formation[1];
                    result.ZwEncounter = (BattleStatus.EncounterForm)(int)data.api_formation[2];
                }
                if(data.api_hourai_flag()) { //昼战
                    result.ZwAirWarfare = ConvertAirWarfare(result, data.api_kouku);
                    result.ZwOurReconn = (BattleStatus.ReconnResult)(int)data.api_search[0];
                    result.ZwEnemyReconn = (BattleStatus.ReconnResult)(int)data.api_search[1];

                    if(data.api_opening_flag == 1) {
                        result.ZwOpeningTorpedoAttack = ConvertTorpedoInfo(result, data.api_opening_atack);
                    }
                    if(data.api_hourai_flag[3] == 1) {
                        result.ZwClosingTorpedoAttack = ConvertTorpedoInfo(result, data.api_raigeki);
                    }
                    if(data.api_hourai_flag[1] == 1) {
                        result.ZwBombards = new BattleStatus.BombardInfo[][] {
                            ConvertBombards(result, data.api_hougeki1),
                            ConvertBombards(result, data.api_hougeki2)
                        };
                    } else if(data.api_hourai_flag[0] == 1) {
                        result.ZwBombards = new BattleStatus.BombardInfo[][] {
                            ConvertBombards(result, data.api_hougeki1)
                        };
                    }
                } else if(data.api_flare_pos()) { //夜战
                    result.ZwAirWarfare = CreateNightAirWarfare(result, data.api_touch_plane);
                    result.ZwBombards = new BattleStatus.BombardInfo[][] {
                        ConvertBombards(result, data.api_hougeki)
                    };
                }

                _onBattle(result);
            } catch(Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }

        private BattleStatus.ShipInfo[] ConvertOurFleet(int fleetId, dynamic nowHps, dynamic maxHps, dynamic param)
        {
            return KanColleClient.Current.Homeport.Organization.Fleets[fleetId].Ships.Select((x, i) => {
                var localShip = Models.DataStore.Instance.Ships[x.Id];
                return new BattleStatus.ShipInfo() {
                    ZwShipTypeName = x.Info.ShipType.Name,
                    ZwShipName = x.Info.Name,
                    ZwShipId = x.Info.Id,
                    ZwId = x.Id,
                    ZwLv = x.Level,
                    ZwCurrentHp = (int)nowHps[i+1],
                    ZwMaxHp = (int)maxHps[i+1],
                    ZwParameter = new BattleStatus.ShipInfo.ParameterInfo() {
                        ZwPower = (int)param[i][0],
                        ZwTorpedo = (int)param[i][1],
                        ZwAntiAir = (int)param[i][2],
                        ZwDefense = (int)param[i][3]
                    },
                    ZwEnhancement = new BattleStatus.ShipInfo.ParameterInfo() {
                        ZwAntiAir = localShip.EnhancedAntiAir,
                        ZwDefense = localShip.EnhancedDefense,
                        ZwPower = localShip.EnhancedPower,
                        ZwTorpedo = localShip.EnhancedTorpedo
                    },
                    ZwEquipts = x.EquippedSlots.Select(si => new Models.EquiptInfo(si)).ToArray()
                };
            }).ToArray();
        }

        private BattleStatus.ShipInfo[] ConvertEnemyFleet(dynamic types, dynamic levels, dynamic nowHps, dynamic maxHps, dynamic param, dynamic enhance, dynamic slots)
        {
            List<BattleStatus.ShipInfo> r = new List<BattleStatus.ShipInfo>(6);
            for(int i = 0; i < 6; i++) {
                if(types[i+1] <= 0) break;
                int shipId = (int)types[i+1];
                var ship = KanColleClient.Current.Master.Ships[shipId];
                var equipts = new List<Models.EquiptInfo>(5);
                for(int j = 0; j < 5; j++) {
                    if(slots[i][j] <= 0) break;
                    int equiptId = (int)slots[i][j];
                    equipts.Add(new Models.EquiptInfo(KanColleClient.Current.Master.SlotItems[equiptId], ship.Slots[j]));
                }

                r.Add(new BattleStatus.ShipInfo() {
                    ZwCurrentHp = (int)nowHps[i+7],
                    ZwMaxHp = (int)maxHps[i+7],
                    ZwEnhancement = new BattleStatus.ShipInfo.ParameterInfo() {
                        ZwPower = (int)enhance[i][0],
                        ZwTorpedo = (int)enhance[i][1],
                        ZwAntiAir = (int)enhance[i][2],
                        ZwDefense = (int)enhance[i][3]
                    },
                    ZwParameter = new BattleStatus.ShipInfo.ParameterInfo() {
                        ZwPower = (int)param[i][0],
                        ZwTorpedo = (int)param[i][1],
                        ZwAntiAir = (int)param[i][2],
                        ZwDefense = (int)param[i][3]
                    },
                    ZwId = i+1,
                    ZwLv = (int)levels[i+1],
                    ZwShipId = shipId,
                    ZwShipName = ship.Name,
                    ZwShipTypeName = ship.ShipType.Name,
                    ZwEquipts = equipts.ToArray()
                });
            }
            return r.ToArray();
        }

        private BattleStatus.AirWarfareInfo ConvertAirWarfare(BattleStatus holder, dynamic data)
        {
            List<int> planeFrom = new List<int>(12);
            if(data.api_plane_from != null) {
                for(int i = 0; i < 6; i++) {
                    if(data.api_plane_from[0].IsDefined(i) && data.api_plane_from[0][i] > 0) {
                        planeFrom.Add((int)data.api_plane_from[0][i]);
                    }
                    if(data.api_plane_from[1].IsDefined(i) && data.api_plane_from[1][i] > 0) {
                        planeFrom.Add((int)data.api_plane_from[1][i]);
                    }
                }
            }
            List<bool> ourBombed = new List<bool>(6);
            List<bool> ourTorpedoed = new List<bool>(6);
            List<double> ourDamage = new List<double>(6);
            List<bool> enemyBombed = new List<bool>(6);
            List<bool> enemyTorpedoed = new List<bool>(6);
            List<double> enemyDamage = new List<double>(6);
            if(data.api_stage3 != null) {
                for(int i = 1; i < 7; i++) {
                    if(data.api_stage3.api_fdam.IsDefined(i) && data.api_stage3.api_fdam[i] >= 0) {
                        ourBombed.Add(data.api_stage3.api_fbak_flag[i] != 0);
                        ourTorpedoed.Add(data.api_stage3.api_frai_flag[i] != 0);
                        ourDamage.Add(data.api_stage3.api_fdam[i]);
                    }
                    if(data.api_stage3.api_edam.IsDefined(i) && data.api_stage3.api_edam[i] >= 0) {
                        enemyBombed.Add(data.api_stage3.api_ebak_flag[i] != 0);
                        enemyTorpedoed.Add(data.api_stage3.api_erai_flag[i] != 0);
                        enemyDamage.Add(data.api_stage3.api_edam[i]);
                    }
                }
            }
            BattleStatus.AirWarfareInfo.AirspaceControl ac = BattleStatus.AirWarfareInfo.AirspaceControl.None;
            switch((int?)(data.api_stage1?.api_disp_seiku)) {
                case null:
                    ac = BattleStatus.AirWarfareInfo.AirspaceControl.None;
                    break;
                case 0:
                    ac = BattleStatus.AirWarfareInfo.AirspaceControl.Parity;
                    break;
                case 3:
                case 4:
                    ac = (BattleStatus.AirWarfareInfo.AirspaceControl)(int)(data.api_stage1.api_disp_seiku + 1);
                    break;
                case 1:
                case 2:
                    ac = (BattleStatus.AirWarfareInfo.AirspaceControl)(int)data.api_stage1.api_disp_seiku;
                    break;
                default:
                    ac = (BattleStatus.AirWarfareInfo.AirspaceControl)(int)-Math.Abs(data.api_stage1.api_disp_seiku);
                    break;
            }
            var r = new BattleStatus.AirWarfareInfo(holder) {
                ZwEnemyCarrierShip = planeFrom.Where(x => x > 6).Select(x => x-7).ToArray(),
                ZwEnemyReconnInTouch = (int)(data.api_stage1?.api_touch_plane[1] ?? -1),
                ZwEnemyStage1Engaged = (int)(data.api_stage1?.api_e_count        ?? 0),
                ZwEnemyStage1Lost    = (int)(data.api_stage1?.api_e_lostcount    ?? 0),
                ZwEnemyStage2Engaged = (int)(data.api_stage2?.api_e_count        ?? 0),
                ZwEnemyStage2Lost    = (int)(data.api_stage2?.api_e_lostcount    ?? 0),

                ZwOurCarrierShip     = planeFrom.Where(x => x < 7).Select(x => x-1).ToArray(),
                ZwOurReconnInTouch   = (int)(data.api_stage1?.api_touch_plane[0] ?? -1),
                ZwOurStage1Engaged   = (int)(data.api_stage1?.api_f_count        ?? 0),
                ZwOurStage1Lost      = (int)(data.api_stage1?.api_f_lostcount    ?? 0),
                ZwOurStage2Engaged   = (int)(data.api_stage2?.api_f_count        ?? 0),
                ZwOurStage2Lost      = (int)(data.api_stage2?.api_f_lostcount    ?? 0),
                ZwOurAirspaceControl = ac,
                ZwEnemyShipBombed = enemyBombed.ToArray(),
                ZwEnemyShipDamages = enemyDamage.ToArray(),
                ZwEnemyShipTorpedoed = enemyTorpedoed.ToArray(),
                ZwOurShipBombed = ourBombed.ToArray(),
                ZwOurShipDamages = ourDamage.ToArray(),
                ZwOurShipTorpedoed = ourTorpedoed.ToArray()
            };
            if(data.api_stage2 != null && data.api_stage2.api_air_fire() && data.api_stage2.api_air_fire != null) {//对空CI
                r.ZwCutInShipNo = (int)data.api_stage2.api_air_fire.api_idx;
                r.ZwCutInType = (BattleStatus.AirWarfareInfo.AaCutInType)(int)data.api_stage2.api_air_fire.api_kind;
                List<int> ciEquipts = new List<int>();
                for(int i = 0; data.api_stage2.api_air_fire.api_use_items.IsDefined(i); i++) {
                    ciEquipts.Add((int)data.api_stage2.api_air_fire.api_use_items[i]);
                }
                r.ZwCutInEquipts = ciEquipts.Select(x => new Models.EquiptInfo(KanColleClient.Current.Master.SlotItems[x], 0)).ToArray();
            }
            if(r.ZwOurReconnInTouch < 0) {
                r.ZwOurReconnInTouchName = "没有舰载机";
            } else {
                r.ZwOurReconnInTouchName = Helpers.GetEquiptNameWithFallback(r.ZwOurReconnInTouch, "{0} 号侦察机");
            }
            if(r.ZwEnemyReconnInTouch < 0) {
                r.ZwEnemyReconnInTouchName = "没有舰载机";
            } else {
                r.ZwEnemyReconnInTouchName = Helpers.GetEquiptNameWithFallback(r.ZwEnemyReconnInTouch, "{0} 号侦察机");
            }
            return r;
        }

        private BattleStatus.TorpedoInfo[] ConvertTorpedoInfo(BattleStatus holder, dynamic data)
        {
            var r = new List<BattleStatus.TorpedoInfo>(12);
            for(int i = 1; i < 7; i++) {
                if(data.api_frai.IsDefined(i) && (data.api_frai[i] > 0)) {
                    r.Add(new BattleStatus.TorpedoInfo(holder) {
                        ZwFrom = i,
                        ZwTo = (int)data.api_frai[i] + 6,
                        ZwDamage = data.api_fydam[i]
                    });
                }
                if(data.api_erai.IsDefined(i) && (data.api_erai[i] > 0)) {
                    r.Add(new BattleStatus.TorpedoInfo(holder) {
                        ZwFrom = i+6,
                        ZwTo = (int)data.api_erai[i],
                        ZwDamage = data.api_eydam[i]
                    });
                }
            }
            return r.ToArray();
        }

        private BattleStatus.AirWarfareInfo CreateNightAirWarfare(BattleStatus holder, dynamic touch_plane)
        {
            var r = new BattleStatus.AirWarfareInfo(holder) {
                ZwEnemyCarrierShip = new int[0],
                ZwEnemyReconnInTouch = (int)touch_plane[1],
                ZwEnemyStage1Engaged = 0,
                ZwEnemyStage1Lost = 0,
                ZwEnemyStage2Engaged = 0,
                ZwEnemyStage2Lost = 0,
                ZwOurCarrierShip = new int[0],
                ZwOurReconnInTouch = (int)touch_plane[0],
                ZwOurStage1Engaged = 0,
                ZwOurStage1Lost = 0,
                ZwOurStage2Engaged = 0,
                ZwOurStage2Lost = 0,
                ZwOurAirspaceControl = BattleStatus.AirWarfareInfo.AirspaceControl.None,
                ZwEnemyShipBombed = new bool[6],
                ZwEnemyShipDamages = new double[6],
                ZwEnemyShipTorpedoed = new bool[6],
                ZwOurShipBombed = new bool[6],
                ZwOurShipDamages = new double[6],
                ZwOurShipTorpedoed = new bool[6]
            };
            if(r.ZwOurReconnInTouch < 0) {
                r.ZwOurReconnInTouchName = "没有舰载机";
            } else {
                r.ZwOurReconnInTouchName = Helpers.GetEquiptNameWithFallback(r.ZwOurReconnInTouch, "{0} 号侦察机");
            }
            if(r.ZwEnemyReconnInTouch < 0) {
                r.ZwEnemyReconnInTouchName = "没有舰载机";
            } else {
                r.ZwEnemyReconnInTouchName = Helpers.GetEquiptNameWithFallback(r.ZwEnemyReconnInTouch, "{0} 号侦察机");
            }
            return r;
        }

        private BattleStatus.BombardInfo[] ConvertBombards(BattleStatus holder, dynamic data)
        {
            List<BattleStatus.BombardInfo> r = new List<BattleStatus.BombardInfo>(12);
            for(int i = 1; data.api_at_list.IsDefined(i); i++) {
                List<double> dmgs = new List<double>(2);
                List<int> tgts = new List<int>(2);
                List<int> sis = new List<int>(3);
                int attackType;

                for(int j = 0; data.api_damage[i].IsDefined(j); j++ ) {
                    if(data.api_df_list[i][j] > 0) {
                        dmgs.Add(data.api_damage[i][j]);
                        tgts.Add((int)data.api_df_list[i][j]);
                    }
                }
                for(int j = 0; data.api_si_list[i].IsDefined(j); j++) {
                    if(data.api_si_list[i][j] > 0) {
                        sis.Add((int)data.api_si_list[i][j]);
                    }
                }

                if(data.api_at_type()) {
                    attackType = (int)data.api_at_type[i];
                } else {
                    attackType = (int)-data.api_sp_list[i];
                }

                r.Add(new BattleStatus.BombardInfo(holder) {
                    ZwFrom = (int)data.api_at_list[i],
                    ZwTo = tgts.ToArray(),
                    ZwDamage = dmgs.ToArray(),
                    ZwEquipts = sis.Select(x => new Models.EquiptInfo(KanColleClient.Current.Master.SlotItems[x], 0)).ToArray(),
                    ZwType = (BattleStatus.BombardInfo.AttackType)attackType
                });
            }
            return r.ToArray();
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
    }
}
