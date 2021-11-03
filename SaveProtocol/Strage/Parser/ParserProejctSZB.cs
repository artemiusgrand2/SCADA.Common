using System;
using System.Collections.Generic;
using System.Text;
using SCADA.Common.HelpCommon;
using SCADA.Common.Constant;
using SCADA.Common.Strage.CostNames;
using SCADA.Common.Strage.SaveElement;
using SCADA.Common.Enums;
using SCADA.Common.Interface;
using SCADA.Common.SaveElement;

namespace SCADA.Common.Strage.Parser
{
    public class ParserProejctSZB
    {
        public static List<StrageAll> RunParser(string filename, LogForm logform,ref bool isHeightRotate)
        {
            List<StrageAll> answer_strage = new List<StrageAll>();
            string[] list_string = HelpFuctions.GetFile(filename);
            try
            {
                if (list_string != null && list_string.Length > 0)
                {
                    foreach (string current_str in list_string)
                    {
                        string[] result_str = current_str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (result_str.Length >= 4)
                        {
                            switch (result_str[0].Trim())
                            {
                                case CostName.StrageTitle:
                                    {
                                        string[] station_numbers = result_str[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (station_numbers.Length == 2)
                                        {
                                            int station = 0;
                                            if (int.TryParse(station_numbers[0], out station))
                                            {
                                                int stationleft = station;
                                                if (int.TryParse(station_numbers[1], out station))
                                                {
                                                    int stationright = station;
                                                    int index_strage = IsRepeatStrage(stationleft, stationright, answer_strage);
                                                    string[] path_names = result_str[3].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (path_names.Length > 0 && path_names[0].Trim().Length > 0)
                                                    {
                                                        string[] stations = result_str[2].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                                        if (stations.Length >= 2)
                                                        {
                                                            if (index_strage == -1)
                                                            {
                                                                StrageAll strageall = new StrageAll() { StationLeft = stationleft, StationRight = stationright };
                                                                foreach (string name_strage in path_names)
                                                                    AddNewStragePath(strageall, name_strage.Trim(), stationleft, stationright, stations[0].Trim(), stations[1].Trim());
                                                                //
                                                                answer_strage.Add(strageall);
                                                            }
                                                            else
                                                            {
                                                                foreach (string name_strage in path_names)
                                                                    AddNewStragePath(answer_strage[index_strage], name_strage.Trim(), stationleft, stationright, stations[0].Trim(), stations[1].Trim());
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case CostName.Location:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                        {
                                            if (answer_strage[answer_strage.Count - 1].StragePaths.ContainsKey(name_path))
                                            {
                                                double location = 0;
                                                switch (result_str[2].Trim())
                                                {
                                                    case CostName.DirectionLeft:
                                                        if (double.TryParse(ParserHeightRotate.GetFormatString(result_str[3]), out location))
                                                            answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.Start = location;
                                                        break;
                                                    case CostName.DirectionRight:
                                                        if (double.TryParse(ParserHeightRotate.GetFormatString(result_str[3]), out location))
                                                            answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.End = location;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case CostName.Lights:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (result_str.Length >= 7)
                                        {
                                            if (CheckStragePath(answer_strage, name_path))
                                            {
                                                switch (result_str[2].Trim())
                                                {
                                                    case CostName.LeftToRight:
                                                        CreateNewLight(answer_strage[answer_strage.Count - 1].StragePaths[name_path], result_str, LandmarksLights.bottom);
                                                        break;
                                                    case CostName.RightToLeft:
                                                        CreateNewLight(answer_strage[answer_strage.Count - 1].StragePaths[name_path], result_str, LandmarksLights.top);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case CostName.BlockNext:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                        {
                                            switch (result_str[2].Trim())
                                            {
                                                case CostName.DirectionLeft:
                                                    answer_strage[answer_strage.Count - 1].StragePaths[name_path].NameLeftBlock = result_str[3].Trim();
                                                    break;
                                                case CostName.DirectionRight:
                                                    answer_strage[answer_strage.Count - 1].StragePaths[name_path].NameRightBlock = result_str[3].Trim();
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case CostName.LightControl:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                            AddStateLight(result_str[3].Trim(), result_str[2].Trim(), result_str, answer_strage[answer_strage.Count - 1].StragePaths[name_path]);
                                    }
                                    break;
                                case CostName.BlockPush:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                        {
                                            int station = 0;
                                            if ((station = GetStationControl(answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.Stationnumberleft,
                                                answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.Stationnumberright, result_str[3].Trim())) != -1)
                                            {
                                                answer_strage[answer_strage.Count - 1].StragePaths[name_path].Trainumber.Station = station;
                                                answer_strage[answer_strage.Count - 1].StragePaths[name_path].Trainumber.Impulses = (result_str.Length > 4) ? result_str[4].Trim() : string.Empty;
                                            }
                                        }
                                    }
                                    break;
                                case CostName.BlockDirection:
                                    {
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                        {
                                            if (result_str.Length >= 6)
                                            {
                                                int station = 0;
                                                if ((station = GetStationControl(answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.Stationnumberleft,
                                                    answer_strage[answer_strage.Count - 1].StragePaths[name_path].Infostrage.Stationnumberright, result_str[4].Trim())) != -1)
                                                {
                                                    Direction direction = null;
                                                    if (result_str[3].Trim() == CostName.DirectionLeft)
                                                    {
                                                        answer_strage[answer_strage.Count - 1].StragePaths[name_path].Left.Station = station;
                                                        direction = answer_strage[answer_strage.Count - 1].StragePaths[name_path].Left;
                                                    }
                                                    else if (result_str[3].Trim() == CostName.DirectionRight)
                                                    {
                                                        answer_strage[answer_strage.Count - 1].StragePaths[name_path].Right.Station = station;
                                                        direction = answer_strage[answer_strage.Count - 1].StragePaths[name_path].Right;
                                                    }
                                                    //
                                                    if (direction != null)
                                                    {
                                                        var imp = result_str[5].Trim();
                                                        if (imp.ToLower() == CostName.BlockDirectionConst)
                                                            direction.ConstRotation = true;
                                                        else
                                                        {
                                                            switch (GetViewStateControl(result_str[2].Trim()))
                                                            {
                                                                case Viewmode.departure:
                                                                    direction.ImpulsesDeparture = imp;
                                                                    break;
                                                                case Viewmode.resolution_of_origin:
                                                                    direction.ImpulsesResolutionDeparture = imp;
                                                                    break;
                                                                case Viewmode.waiting_for_departure:
                                                                    direction.ImpulsesWaitDeparture = imp;
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case CostName.Move:
                                    AnalisStrageObject(result_str, CostName.Move, answer_strage, logform);
                                    break;
                                case CostName.KTCM:
                                    AnalisStrageObject(result_str, CostName.KTCM, answer_strage, logform);
                                    break;
                                case CostName.KGU:
                                    AnalisStrageObject(result_str, CostName.KGU, answer_strage, logform);
                                    break;
                                default:
                                    {
                                        double buffer;
                                        if (result_str[0].Trim().IndexOf(CostName.HelpChar) == -1 && !double.TryParse(ParserHeightRotate.GetFormatString(result_str[0]), out buffer))
                                            logform.AddNewMessage(string.Format("Идентификатор {0} не описан", result_str[0].Trim()));
                                    }
                                    break;
                            }
                        }
                        else if (result_str.Length >= 3)
                        {
                            switch (result_str[0].Trim())
                            {
                                case CostName.Heights:
                                    {
                                        isHeightRotate = true;
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                            ParserHeightRotate.RunAnalisSegment(result_str[2].Trim(), answer_strage[answer_strage.Count - 1].StragePaths[name_path], ViewSegment.height, logform);
                                    }
                                    break;
                                case CostName.Rotate:
                                    {
                                        isHeightRotate = true;
                                        string name_path = result_str[1].Trim();
                                        if (CheckStragePath(answer_strage, name_path))
                                            ParserHeightRotate.RunAnalisSegment(result_str[2].Trim(), answer_strage[answer_strage.Count - 1].StragePaths[name_path], ViewSegment.rotate, logform);
                                    }
                                    break;
                                default:
                                    if (result_str[0].Trim().IndexOf(CostName.HelpChar) == -1)
                                        logform.AddNewMessage(string.Format("Идентификатор {0} не описан", result_str[0].Trim()));
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception error) { System.Windows.Forms.MessageBox.Show(error.Message); }
            //
            return answer_strage;
        }

        private static void AnalisStrageObject(string[] currentstr, string currentobject, List<StrageAll> answer_strage, LogForm logform)
        {
            if (currentstr.Length >= 5)
            {
                double location;
                if (double.TryParse(ParserHeightRotate.GetFormatString(currentstr[2]), out location))
                {
                    string[] names_path = currentstr[4].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string namepath in names_path)
                    {
                        string name = namepath.Trim();
                        if (CheckStragePath(answer_strage, name))
                        {
                            int station = -1;
                            if ((station = GetStationControl(answer_strage[answer_strage.Count - 1].StragePaths[name].Infostrage.Stationnumberleft, answer_strage[answer_strage.Count - 1].StragePaths[name].Infostrage.Stationnumberright, currentstr[3].Trim())) != -1)
                            {
                                StrageObject newobject = new StrageObject() { Location = location, Name = currentstr[1].Trim(), Station = station };
                                switch (currentobject)
                                {
                                    case CostName.Move:
                                        {
                                            int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[name].MoveElements, currentstr[1].Trim());
                                            if (index == -1)
                                                answer_strage[answer_strage.Count - 1].StragePaths[name].MoveElements.Add(newobject);
                                        }
                                        break;
                                    case CostName.KTCM:
                                        {
                                            int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[name].KTCMelements, currentstr[1].Trim());
                                            if (index == -1)
                                                answer_strage[answer_strage.Count - 1].StragePaths[name].KTCMelements.Add(newobject);
                                        }
                                        break;
                                    case CostName.KGU:
                                        {
                                            int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[name].KGUelements, currentstr[1].Trim());
                                            if (index == -1)
                                                answer_strage[answer_strage.Count - 1].StragePaths[name].KGUelements.Add(newobject);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    logform.AddNewMessage(string.Format("У объекта {0} свойство координата имеет неверный формат ", currentstr[1].Trim()));
                }
            }
            else if (currentstr.Length >= 4)
            {
                if (answer_strage.Count > 0)
                {
                    foreach (KeyValuePair<string, StragePath> stragepath in answer_strage[answer_strage.Count - 1].StragePaths)
                    {
                        switch (currentobject)
                        {
                            case CostName.Move:
                                {
                                    int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[stragepath.Key].MoveElements, currentstr[1].Trim());
                                    if (index != -1)
                                    {
                                        string state = currentstr[2].Trim();
                                        if (state == CostName.Fault || state == CostName.Accident || state == CostName.ClosingButtonMove || state == CostName.ClosingMove)
                                        {
                                            stragepath.Value.MoveElements[index].Impulses.Add(new StateElement()
                                            {
                                                Impuls = currentstr[3].Trim(),
                                                Name = GetViewStateControl(state)
                                            });
                                        }
                                    }
                                }
                                break;
                            case CostName.KTCM:
                                {
                                    int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[stragepath.Key].KTCMelements, currentstr[1].Trim());
                                    if (index != -1)
                                    {
                                        string state = currentstr[2].Trim();
                                        if (state == CostName.Fault || state == CostName.PlayControlObject)
                                        {
                                            stragepath.Value.KTCMelements[index].Impulses.Add(new StateElement()
                                            {
                                                Impuls = currentstr[3].Trim(),
                                                Name = GetViewStateControl(state)
                                            });
                                        }
                                    }
                                }
                                break;
                            case CostName.KGU:
                                {
                                    int index = CheckNameStrageObject(answer_strage[answer_strage.Count - 1].StragePaths[stragepath.Key].KGUelements, currentstr[1].Trim());
                                    if (index != -1)
                                    {
                                        string state = currentstr[2].Trim();
                                        if (state == CostName.Fault || state == CostName.PlayControlObject)
                                        {
                                            stragepath.Value.KGUelements[index].Impulses.Add(new StateElement()
                                            {
                                                Impuls = currentstr[3].Trim(),
                                                Name = GetViewStateControl(state)
                                            });
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void AddStateLight(string namelight, string state, string[] strinfo, StragePath stragepath)
        {
            if (strinfo.Length >= 5)
            {
                if (state == CostName.CloseLight || state == CostName.OpenLight)
                {
                    int index = CheckNameLight(stragepath.Lightsmoves, namelight);
                    if (index != -1)
                        stragepath.Lightsmoves[index].Impulses.Add(new StateElement() { Name = GetViewStateControl(state), Impuls = strinfo[4].Trim() });
                }
            }
        }

        private static int GetStationControl(int stationleft, int stationright, string direction)
        {
            if (direction == CostName.DirectionLeft)
                return stationleft;
            else if (direction == CostName.DirectionRight)
                return stationright;
            //
            return -1;
        }

        private static ViewLights GetViewLight(string name)
        {
            switch (name)
            {
                case CostName.LightInput:
                    return ViewLights.input;
                case CostName.LightAnadromoust:
                    return ViewLights.anadromous;
                default:
                    return ViewLights.anadromous;
            }
        }

        private static bool CheckStragePath(List<StrageAll> strages, string namepath)
        {
            if (strages.Count > 0)
            {
                if (strages[strages.Count - 1].StragePaths.ContainsKey(namepath))
                {
                    return true;
                }
            }
            //
            return false;
        }

        private static Viewmode GetViewStateControl(string name)
        {
            switch (name)
            {
                case CostName.CloseLight:
                    return Viewmode.signal;
                case CostName.OpenLight:
                    return Viewmode.signal;
                case CostName.Occupation:
                    return Viewmode.occupation;
                case CostName.Departure:
                    return Viewmode.departure;
                case CostName.ResolutionOrigin:
                    return Viewmode.resolution_of_origin;
                case CostName.WaitingDeparture:
                    return Viewmode.waiting_for_departure;
                case CostName.Fault:
                    return Viewmode.fault;
                case CostName.Accident:
                    return Viewmode.accident;
                case CostName.ClosingMove:
                    return Viewmode.closing;
                case CostName.ClosingButtonMove:
                    return Viewmode.closing_button;
                case CostName.PlayControlObject:
                    return Viewmode.play_control_object;
                default:
                    return Viewmode.none; ;
            }
        }

        private static int CheckNameStrageObject(List<StrageObject> stragebjects, string name)
        {
            for (int i = 0; i < stragebjects.Count; i++)
            {
                if (stragebjects[i].Name == name)
                    return i;
            }
            //
            return -1;
        }

        private static int CheckNameLight(List<LightsProject> stragebjects, string name)
        {
            for (int i = 0; i < stragebjects.Count; i++)
            {
                if (stragebjects[i].Name == name)
                    return i;
            }
            //
            return -1;
        }


        private static void CreateNewLight(StragePath stragepath, string [] strlights, LandmarksLights direction)
        {
            string[] lights = new string[(strlights.Length == 7) ? 4 : 5];
             Array.Copy(strlights, 3, lights, 0, lights.Length);
            double location = 0;
            if (double.TryParse(ParserHeightRotate.GetFormatString(lights[1]), out location))
            {
                if (!CheckRepeat(stragepath.Lightsmoves, lights[0].Trim()))
                {
                    stragepath.Lightsmoves.Add(new LightsProject()
                    {
                        Name = lights[0].Trim(),
                        Landmarks = direction,
                        Location = location,
                        View = GetViewLight(lights[2].Trim()),
                        Station = GetStationControl(stragepath.Infostrage.Stationnumberleft, stragepath.Infostrage.Stationnumberright, lights[3].Trim()),
                        Visible = (lights.Length == 4) ? VisiblityLights.Yes : (((lights[4].Trim().ToUpper() == CostName.VisibleLight)) ? VisiblityLights.No : VisiblityLights.Yes)
                    });
                }
            }
        }

        private static bool CheckRepeat(IList<LightsProject> list, string name)
        {
            foreach (var row in list)
            {
                if (row.Name == name)
                    return true;
            }
            //
            return false;
        }

        private static void AddNewStragePath(StrageAll strageall, string namestrage, int stationleft, int stationright, string stationnameleft, string stationnameright)
        {
            if (!strageall.StragePaths.ContainsKey(namestrage))
            {
                StragePath new_strage_path = new StragePath();
                new_strage_path.Infostrage.Stationnumberleft = stationleft;
                new_strage_path.Infostrage.Stationnumberright = stationright;
                new_strage_path.Infostrage.Stationnameleft = stationnameleft;
                new_strage_path.Infostrage.Stationnameright = stationnameright;
                new_strage_path.Infostrage.NameMove = namestrage;
                //
                strageall.StragePaths.Add(new_strage_path.Infostrage.NameMove, new_strage_path);
            }
        }

        private static int IsRepeatStrage(int stationleft, int stationright, List<StrageAll> strages)
        {
            for (int i = 0; i < strages.Count; i++)
            {
                if (strages[i].StationLeft == stationleft && strages[i].StationRight == stationright)
                    return i;
            }
            //
            return -1;
        }
    }
}
