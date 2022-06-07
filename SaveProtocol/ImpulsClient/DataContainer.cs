using System;
using System.Collections.Generic;
using System.Text;

using SCADA.Common.LogicalParse;
using SCADA.Common.Enums;
using SCADA.Common.Log;

namespace SCADA.Common.ImpulsClient
{
	public class DataContainer
	{
		private readonly Dictionary<int, Station> _stations;
		
		public DataContainer()
		{
			_stations = new Dictionary<int, Station>();
		}

		public Dictionary<int, Station> Stations
		{
			get
			{
				return _stations;
			}
		}

        public Station GetStation(int stationCode)
        {
            return (_stations.ContainsKey(stationCode)) ? _stations[stationCode] : null;
        }

        public bool LoadStationsData(StationRecord[] inp_station_records, string tables_path)
		{
            foreach (StationRecord st_config in inp_station_records)
 			{
                Station st = new Station(st_config.Name, st_config.Code);
 				Impulse[] ts_impulses = null;
				Impulse[] tu_impulses = null;
				try
				{
                    TableLoader.GetStdImpulses(tables_path, st.Code, false, out ts_impulses);
					TableLoader.GetStdImpulses(tables_path, st.Code, true, out tu_impulses);
                }
				catch(SystemException ex)
				{
					System.Console.Error.WriteLine("TI for {0} not found. {1}", st.Code, ex.Message);
					continue;
				}
				if(ts_impulses == null)
				{
					System.Console.Error.WriteLine("TI for {0} not found.", st.Code);
					continue;
				}
				
				try
				{
					st.LoadData(ts_impulses, tu_impulses);
				}
				catch(Exception ex)
				{
					System.Console.Error.WriteLine("Can't load TI for {0}. {1}", st.Code, ex.Message);
					continue;
				}
 				try
 				{
					_stations.Add(st.Code, st);
				}
				catch(ArgumentException ex)
				{
					System.Console.Error.WriteLine(ex.Message);
					continue;
				}
			}
			if(_stations.Count == 0)
				return false;
			else
				return true;
		}

        public DataContainer Clone()
        {
            var cloneDataContainer = new DataContainer();
            foreach(var station in _stations)
                cloneDataContainer._stations.Add(station.Key, station.Value.Clone());
            //
            return cloneDataContainer;
        }

        /// <summary>
        /// получаем значение состояния
        /// </summary>
        /// <param name="stationDefault">номер станции</param>
        /// <param name="inNot"></param>
        /// <returns></returns>
        private InfixNotation.infix_states GetValueImpuls(int stationDefault, string formula)
        {
            try
            {
                InfixNotation inNot = new InfixNotation(formula);
                foreach (string impulsNameFull in inNot._impulsesNames)
                {
                    var nameImpuls = impulsNameFull;
                    var station = ParseStationNumber(ref nameImpuls, stationDefault);
                    if (Stations.ContainsKey(station))
                    {
                        switch (Stations[station].TS.GetState(nameImpuls))
                        {
                            case ImpulseState.ActiveState:
                                {
                                    inNot._impulsesValues[impulsNameFull] = InfixNotation.infix_states.ActiveState;
                                    break;
                                }
                            case ImpulseState.PassiveState:
                                {
                                    inNot._impulsesValues[impulsNameFull] = InfixNotation.infix_states.PassiveState;
                                    break;
                                }
                            default: inNot._impulsesValues[impulsNameFull] = InfixNotation.infix_states.UncontrolledState;
                                break;
                        }
                    }
                }
                return inNot.Compute();
            }
            catch
            {
                return InfixNotation.infix_states.UncontrolledState;
            }
        }

        public bool CheckFormula(int stationDefault, string formula)
        {
            var result = true;
            try
            {
                InfixNotation inNot = new InfixNotation(formula);
                foreach (string impulsNameFull in inNot._impulsesNames)
                {
                    var nameImpuls = impulsNameFull;
                    var station = ParseStationNumber(ref nameImpuls, stationDefault);
                    if (Stations.ContainsKey(station))
                    {
                        if (!Stations[station].TS.Contains(nameImpuls))
                        {
                            Logger.LogCommon.Error($"Импульса - '{nameImpuls}' нет на станции с код еср - {station}. Формула - '{formula}'");
                            result = false;
                        }
                    }
                    else
                    {
                        Logger.LogCommon.Error($"Станции с кодом еср - {station} не существует. Формула - '{formula}'");
                        result = false;
                    }

                }
            }
            catch
            {
                result =  false;
            }
            //
            return result;
        }

        private static int ParseStationNumber(ref string nameImpuls, int stationDefault)
        {
            var charsSplit = new char[] { '.', ':' };
            var cells = nameImpuls.Split(charsSplit, StringSplitOptions.RemoveEmptyEntries);
            if (cells.Length > 1)
            {
                int buffer;
                if (int.TryParse(cells[0], out buffer))
                {
                    var findIndex = nameImpuls.IndexOf(cells[0] + ".");
                    if ((findIndex == 0 && cells[0].Length >= 6) || findIndex != 0)
                    {
                        nameImpuls = nameImpuls.Substring(cells[0].Length + 1);
                        return buffer;
                    }
                }
            }
            //
            return stationDefault;
        }

        /// <summary>
        /// находим состояние элемента индикации
        /// </summary>
        /// <param name="stationnumber">номер станции контроля</param>
        /// <param name="impuls">формула импульсов управляющих</param>
        /// <returns></returns>
        public StatesControl GetStateControl(int stationnumber, string impuls)
        {
            switch (GetValueImpuls(stationnumber, impuls))
            {
                case InfixNotation.infix_states.ActiveState:
                    return StatesControl.activ;
                case InfixNotation.infix_states.PassiveState:
                    return StatesControl.pasiv;
                default:
                    return StatesControl.nocontrol;
            }
        }
	}
}
