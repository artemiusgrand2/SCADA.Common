using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SCADA.Common.Interface;
using SCADA.Common.Strage.SaveElement;
using SCADA.Common.Strage.SaveElement.Segment;
using SCADA.Common.Enums;

namespace SCADA.Common.Strage.Parser
{
    public class ParserHeightRotate
    {
        /// <summary>
        /// открываем файл с информацией по высотам или горизоталем и запускаем анализ
        /// </summary>
        /// <param name="filename">путь к файлу</param>
        /// <param name="mov">текущий перегон</mov>
        public static void RunAnalisSegment(string file, StragePath mov, ViewSegment view, LogForm logform)
        {
            try
            {
                if (File.Exists(file))
                {
                    string[] massiv = File.ReadAllLines(file, Encoding.GetEncoding(1251));
                    if (massiv != null)
                    {
                        if (view == ViewSegment.height)
                            AnalizHeightFile(massiv, mov, logform, file);
                        else AnalizRotateFile(massiv, mov, logform, file);
                    }
                }
                else
                {
                    logform.AddNewMessage(String.Format("Файла '{0}' по адресу {1} не существует", (view == ViewSegment.height) ? "Высот" : "Поворотов", file));
                }

            }
            catch (Exception error)
            {
                logform.AddNewMessage(error.ToString());
            }
        }

        /// <summary>
        /// Анализируем данные таблицы высот
        /// </summary>
        /// <param name="dann">массив строк файла высот</param>
        private static void AnalizHeightFile(string[] dann, StragePath mov, LogForm logform, string file)
        {
            List<Segment> collection = ParserDataString(dann, ViewSegment.height, logform, file);
            List<Segmentheight> _collectionheight = new List<Segmentheight>();
            //делаем километровую сетку через каждые полкилометра
            _collectionheight.Add(new Segmentheight() { Location = mov.Infostrage.Start });
            CreateSetka(mov.Infostrage.Start, mov.Infostrage.End,  _collectionheight);
            _collectionheight.Add(new Segmentheight() { Location = mov.Infostrage.End });
            //вычисляем высоты для созданной километровой сетки
            foreach (Segmentheight height in _collectionheight)
            {
                Segmentheight find = FindLocation(height.Location, collection, mov);
                if (find != null)
                    height.Height = CalculateNewHeight(height.Location, find, mov);
                else logform.AddNewMessage(String.Format("Координата {0} не входит в диапозон километража из файла вертикальных профилей перегона", height.Location.ToString()));
            }
            mov.Infostrage.CollectionHeight.Clear();
            //заполняем таблицу высот новыми данными
            foreach (Segmentheight height in _collectionheight)
                mov.Infostrage.CollectionHeight.Add(height.Height);
        }

        /// <summary>
        /// Анализируем данные таблицы поворотов
        /// </summary>
        /// <param name="dann">массив строк файла высот</param>
        private static void AnalizRotateFile(string[] dann, StragePath mov, LogForm logform, string file)
        {
            List<Segment> collection = ParserDataString(dann, ViewSegment.rotate, logform, file);
            List<Segmentrotate> _collectionrotate = new List<Segmentrotate>();
            FullCollection(collection, _collectionrotate, mov);
            mov.Infostrage.CollectionRotate.Clear();
            ////заполняем таблицу поворотов новыми данными
            foreach (Segmentrotate rotate in _collectionrotate)
                mov.Infostrage.CollectionRotate.Add(rotate);
        }

        /// <summary>
        /// выщитываем высоту по километру
        /// </summary>
        private static double CalculateNewHeight(double kilometr, Segmentheight info, StragePath mov)
        {
            if (mov.Infostrage.End > mov.Infostrage.Start)
                return Math.Round(((kilometr - info.Location) * (info.Tg / 1000) + info.Height), 2);
            else return Math.Round(((info.Location - kilometr) * (info.Tg / 1000) + info.Height), 2);
        }

        private static void CreateSetka(double start, double end,  List<Segmentheight> collection)
        {
            if (start < end)
            {
                double startline = start + 100 - start % 100;
                double endline = end - end % 100;

                for (double i = startline; i <= endline; i += 100)
                {
                    if (i - start > 100 && end -    i > 100 && i % 500 == 0)
                        collection.Add(new Segmentheight() { Location = i });
                }
            }
            else
            {
                double startline = start - start % 100;
                double endline = end + 100 - end % 100;
                for (double i = startline; i >= endline; i -= 100)
                {
                    if (start - i > 100 && i - end > 100 && i % 500 == 0)
                        collection.Add(new Segmentheight() { Location = i });
                }
            }
        }

        /// <summary>
        /// Заполняем коллекцию поворотв 
        /// </summary>
        private static void FullCollection(List<Segment> collection, List<Segmentrotate> collectionrotate, StragePath mov)
        {
            if (mov.Infostrage.Start < mov.Infostrage.End)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if ((collection[i].Location + collection[i].Lenght) <= mov.Infostrage.End && collection[i].Location >= mov.Infostrage.Start)
                    {
                        collectionrotate.Add(collection[i] as Segmentrotate);
                        continue;
                    }
                    //конечный поворот
                    if ((collection[i].Location + collection[i].Lenght) > mov.Infostrage.End && collection[i].Location < mov.Infostrage.End)
                    {
                        collectionrotate.Add(CreateNewRotate(collection[i].Location, mov.Infostrage.End - collection[i].Location,
                       (collection[i] as Segmentrotate).Radius, (collection[i] as Segmentrotate).Direction));
                        continue;
                    }
                    //начальный поворот
                    if ((collection[i].Location + collection[i].Lenght) > mov.Infostrage.Start && collection[i].Location < mov.Infostrage.Start)
                    {
                        collectionrotate.Add(CreateNewRotate(mov.Infostrage.Start, collection[i].Location + collection[i].Lenght - mov.Infostrage.Start,
                       (collection[i] as Segmentrotate).Radius, (collection[i] as Segmentrotate).Direction));
                        continue;
                    }
                }
            }
            else
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if ((collection[i].Location + collection[i].Lenght) <= mov.Infostrage.Start && collection[i].Location >= mov.Infostrage.End)
                    {
                        collectionrotate.Add(collection[i] as Segmentrotate);
                        continue;
                    }
                    //конечный поворот
                    if ((collection[i].Location + collection[i].Lenght) > mov.Infostrage.End && collection[i].Location < mov.Infostrage.End)
                    {
                        collectionrotate.Add(CreateNewRotate(mov.Infostrage.End, collection[i].Lenght - (mov.Infostrage.End - collection[i].Location),
                       (collection[i] as Segmentrotate).Radius, (collection[i] as Segmentrotate).Direction));
                        continue;
                    }
                    //начальный поворот
                    if ((collection[i].Location + collection[i].Lenght) > mov.Infostrage.Start && collection[i].Location < mov.Infostrage.Start)
                    {
                        collectionrotate.Add(CreateNewRotate(collection[i].Location, mov.Infostrage.Start - collection[i].Location,
                       (collection[i] as Segmentrotate).Radius, (collection[i] as Segmentrotate).Direction));
                        continue;
                    }
                }
            }
        }

        private static Segmentrotate CreateNewRotate(double location, double lenghtrotate, double value, DirectionRotate direction)
        {
            return new Segmentrotate()
            {
                Lenght = lenghtrotate,
                Location = location,
                Radius = value,
                Direction = direction
            };
        }

        private static DirectionRotate GetRotateDirection(string direction)
        {
            if (direction == "Н")
                return DirectionRotate.left;
            else return DirectionRotate.right;
        }

        /// <summary>
        /// ищем подходящий километр вседи таблицы высот
        /// </summary>
        /// <param name="location">километр</param>
        /// <returns></returns>
        private static Segmentheight FindLocation(double location, List<Segment> collection, StragePath mov)
        {
            //if (mov.Infostrage.End > mov.Infostrage.Start)
            //{
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Location + collection[i].Lenght >= location && location >= collection[i].Location)
                        return collection[i] as Segmentheight;
                }
            //}
            //else
            //{
            //    for (int i = 0; i < collection.Count; i++)
            //    {
            //        if (collection[i].Location - collection[i].Lenght <= location && location <= collection[i].Location)
            //            return collection[i] as Segmentheight;
            //    }
            //}
            return null;
        }

        public static string GetFormatString(string data)
        {
            IList<char> filter = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' , '-'};
            string result = data;
            foreach (var bit in data.ToCharArray())
            {
                if (!filter.Contains(bit))
                    result = result.Replace(bit.ToString(), System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            //
            return result;
        }

        private static List<Segment> ParserDataString(string[] date_string, ViewSegment viewsegment, LogForm logform, string file)
        {
            List<Segment> collection = new List<Segment>();
            double buffer = 0;
            for (int i = 0; i < date_string.Length; i++)
            {
                string[] stroka_collection = date_string[i].Split(new char[] { ';' });
                if (stroka_collection.Length >= 4)
                {
                    if (viewsegment == ViewSegment.height)
                        collection.Add(new Segmentheight());
                    else
                        collection.Add(new Segmentrotate());
                    //
                    int index = 0;
                    while (index < stroka_collection.Length)
                    {
                        switch (index)
                        {
                            case 0:
                                if (double.TryParse(GetFormatString(stroka_collection[index]),  out buffer))
                                    collection[collection.Count - 1].Location = buffer;
                                else
                                {
                                    if (collection.Count > 1 && viewsegment == ViewSegment.height)
                                        goto Finish;
                                    //
                                    collection.RemoveAt(collection.Count - 1);
                                    index = stroka_collection.Length;
                                    logform.AddNewMessage(String.Format("Не правильный тип данных используется для задания расположения!!! Проверьте строку {0} в файле {1}", (i + 1).ToString(), file));
                                
                                }
                                break;
                            case 1:
                                if (double.TryParse(GetFormatString(stroka_collection[index]),  out buffer))
                                {
                                    collection[collection.Count - 1].Lenght = buffer;
                                    if (collection.Count > 1 && viewsegment == ViewSegment.height)
                                    {
                                        collection[collection.Count - 1].Location = collection[collection.Count - 2].Location + collection[collection.Count - 2].Lenght;
                                    }
                                }
                                else
                                {
                                    collection.RemoveAt(collection.Count - 1);
                                    index = stroka_collection.Length;
                                    logform.AddNewMessage(String.Format("Не правильный тип данных используется для задания длины участка!!! Проверьте строку {0} в файле {1}", (i + 1).ToString(), file));
                                }
                                break;
                            case 2:
                                if (double.TryParse(GetFormatString(stroka_collection[index]),  out buffer))
                                {
                                    if (collection[collection.Count - 1] is Segmentrotate)
                                        (collection[collection.Count - 1] as Segmentrotate).Radius = buffer;
                                    else
                                    {
                                        (collection[collection.Count - 1] as Segmentheight).Tg = buffer;
                                        if (collection.Count > 1)
                                        {
                                            (collection[collection.Count - 1] as Segmentheight).Height = (collection[collection.Count - 2]as Segmentheight).Height + (buffer / 1000) * collection[collection.Count - 2].Lenght;
                                        }
                                    }
                                }
                                else
                                {
                                    collection.RemoveAt(collection.Count - 1);
                                    index = stroka_collection.Length;
                                    logform.AddNewMessage(String.Format("Не правильный тип данных используется для задания типа поворота!!! Проверьте строку {0} в файле {1}", (i + 1).ToString(), file));
                                }
                                break;
                            case 3:
                                if (collection[collection.Count - 1] is Segmentrotate)
                                {
                                    if (stroka_collection[index].ToUpper() == "Н" || stroka_collection[index].ToUpper() == "В")
                                        (collection[collection.Count - 1] as Segmentrotate).Direction = GetRotateDirection(stroka_collection[index].ToUpper());
                                    else
                                    {
                                        collection.RemoveAt(collection.Count - 1);
                                        index = stroka_collection.Length;
                                        logform.AddNewMessage(String.Format("Не правильный тип данных используется для задания типа поворота!!! Проверьте строку {0} в файле {1}", (i + 1).ToString(), file));
                                    }
                                }
                                else
                                {
                                    if (double.TryParse(GetFormatString(stroka_collection[index]),  out buffer))
                                        (collection[collection.Count - 1] as Segmentheight).Height = buffer;
                                    else if (collection.Count == 1)
                                    {
                                        collection.RemoveAt(collection.Count - 1);
                                        index = stroka_collection.Length;
                                        logform.AddNewMessage(String.Format("Не правильный тип данных используется для задания высоты!!! Проверьте строку {0} в файле {1}", (i + 1).ToString(), file));
                                    }
                                }

                                break;
                        }
                        //
                    Finish:
                        index++;
                    }
                }
            }
            return collection;
        }
    }
}
