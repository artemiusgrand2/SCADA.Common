using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using SCADA.Common.Enums;

namespace SCADA.Common.ImpulsClient
{
	
	public static class TableLoader
	{
		/// <summary>
		/// Чтение таблицы импульсов (TI-файл)
		/// </summary>
		/// <param name="fileName">Имя TI-файла</param>
		/// <param name="ts">true - если надо загрузить импульсы ТС, false - если надо загрузить импульсы ТС.</param>
		/// <param name="records">Результирующий массив импульсов. Все поля, кроме Name, нулевые</param>
		/// <exception cref="ArgumentException">Если fileName содержит пустую строку</exception>
		/// <exception cref="ArgumentNullException">Если fileName равен null</exception>
		/// <exception cref="FileNotFoundException">Если fileName содержит неправильный путь к файлу или такого файла нет.</exception>
		/// <exception cref="IOException">Если произошла ошибка при чтении файла.</exception>
		/// <remarks>Если в файле нет записей, то records будет равен null.</remarks>
		public static void ReadImpulseTable(string fileName, bool isTu, out Impulse[] records)
		{
			if(fileName == null)
				throw new ArgumentNullException("fileName");

			if(!File.Exists(fileName))
				throw new FileNotFoundException(string.Format("File {0} not found", fileName));

			records = null;

			System.IO.StreamReader sr;

			//открываю файл
			try
			{
				sr = new StreamReader(fileName, Encoding.GetEncoding(866)); 
			}
			catch(IOException e)
			{
				throw new FileNotFoundException(string.Format("File {0} not found", fileName), e);
			}

			if(sr.BaseStream.CanRead == false) 
			{
				sr.Close();
				throw new IOException(string.Format("File {0} not found", fileName)); 
			}

			string pattern, patternToolTip;

            if (isTu)
            {
                pattern = @"^@U\s*'(.+)'\s*";
                patternToolTip = @"^@U\s*'(.+)'\s*;\s*(.+)\s*";
            }
            else
            {
                pattern = @"^@N\s*'(.+)'\s*";
                patternToolTip = @"^@N\s*'(.+)'\s*;\s*(.+)\s*";
            }
// 			switch(tableType)
// 			{
// 			case ImpulsesTableType.TS:
// 				pattern = @"^@N\s*'.+'.*$";
// 				break;
// 			case ImpulsesTableType.TU:
// 				pattern = @"^@U\s*'.+'.*$";
// 				break;
// 			case ImpulsesTableType.Blocks:
// 				goto case ImpulsesTableType.TS;
// 			case ImpulsesTableType.Channels:
// 				goto case ImpulsesTableType.TS;
// 			default:
// 				return;
// 			}
			try
			{
				string input;
				int count = 0;
				//определяю количество импульсов в файле
				while((input = sr.ReadLine()) != null)
					if(Regex.IsMatch(input, pattern)) 
						count++;

				if (count == 0)
				{
					sr.Close();
					return;
				}
				//задоет позициб в текущем потоке
				sr.BaseStream.Seek(0, SeekOrigin.Begin);
				records = new Impulse[count];
				count = 0;
				while ((input = sr.ReadLine()) != null)
					if (Regex.IsMatch(input, pattern))
					{
						int nameStartPos = input.IndexOf('\'', 0) + 1;
                        records[count] = new Impulse(input.Substring(nameStartPos, input.IndexOf('\'', nameStartPos) - nameStartPos), (isTu) ? TypeImpuls.tu : TypeImpuls.ts);
                        //ищем описание
                        var parserToolTip = Regex.Match(input, patternToolTip);
                        if (parserToolTip.Success)
                            records[count].ToolTip = parserToolTip.Groups[2].Value;
                        //
                        count++;
					}

				sr.Close();
			}
			catch(SystemException e) 
			{
				sr.Close();
				throw new IOException(string.Format("File {0} not found", fileName), e); 
			}
		}

		/// <summary>
		/// Заполнение таблицы импульсов значениями номеров контактов/разъемов (IO-файл)
		/// </summary>
		/// <param name="fileName">Имя IO-файла</param>
		/// <param name="ts">Признак телесигнализации, иначе ТУ</param>
		/// <param name="matrix">Номер матрицы.</param>
		/// <param name="records">Массив импульсов.</param>
		/// <exception cref="ArgumentException">Если fileName содержит пустую строку</exception>
		/// <exception cref="ArgumentNullException">Если fileName равен null</exception>
		/// <exception cref="FileNotFoundException">Если fileName содержит неправильный путь к файлу или такого файла нет.</exception>
		/// <exception cref="IOException">Если произошла ошибка при чтении файла.</exception>
		public static void ReadImpulsesIO(string fileName, /*ImpulsesTableType tableType,*/ int matrix, Impulse[] records)
		{
			if(records == null)
				throw new ArgumentNullException("records");

			if(!File.Exists(fileName))
				throw new FileNotFoundException(string.Format("File {0} not found", fileName));

			System.IO.StreamReader sr;
			//открываю файл
			try
			{
				sr = new StreamReader(fileName, Encoding.GetEncoding(866));
			}
			catch(IOException e)
			{
				throw new FileNotFoundException(string.Format("File {0} not found", fileName), e);
			}

			if(sr.BaseStream.CanRead == false)
			{
				sr.Close();
				throw new IOException(string.Format("File {0} not found", fileName));
			}

			string pattern;
			string firstChars;
			string pattern2 = string.Empty;
			string firstChars2 = string.Empty;
			
			pattern = @"^@DS\s+([0-9]+),\s*([0-9]+),\s*'(.+)'.*$";
			firstChars = "@DS";
			pattern2 = @"^@MS\s+([0-9]+),\s*([0-9]+),\s*'(.+)',\s*'(.+)'.*$";
			firstChars2 = "@MS";

// 			switch(tableType)
// 			{
// 			case ImpulsesTableType.TS:
// 				pattern = @"^@DS\s+([0-9]+),\s*([0-9]+),\s*'(.+)'.*$";
// 				firstChars = "@DS";
// 				pattern2 = @"^@MS\s+([0-9]+),\s*([0-9]+),\s*'(.+)',\s*'(.+)'.*$";
// 				firstChars2 = "@MS";
// 				break;
// 			case ImpulsesTableType.TU:
// 				pattern = @"^@UR\s+([0-9]+),\s*([0-9]+),\s*'(.+)'.*$";
// 				firstChars = "@UR";
// 				break;
// 			case ImpulsesTableType.Blocks:
// 				pattern = @"^@BS\s+([0-9]+),\s*([0-9]+),\s*([0-9]+),\s*'(.+)'.*$";
// 				firstChars = "@BS";
// 				break;
// 			case ImpulsesTableType.Channels:
// 				pattern = @"^@CS\s+([0-9]+),\s*([0-9]+),\s*'(.+)'.*$";
// 				firstChars = "@CS";
// 				break;
// 			default:
// 				return;
// 			}

			int count = 0;

			try
			{
				string input;
				while((input = sr.ReadLine()) != null)
				{
					if(!input.StartsWith(firstChars))
						if(!input.StartsWith(firstChars2))
							continue;
					Match m = Regex.Match(input, pattern);
					if(!m.Success)
					{
						if(pattern2 != string.Empty)
						{
							m = Regex.Match(input, pattern2);
							if(!m.Success)
								continue;
						}
						else
							continue;
					}
					GroupCollection gc = m.Groups;
					if(gc.Count < 4)
						continue;
					string name2 = string.Empty;

					if(gc.Count < 4)
						continue;
                    var tmp = new Impulse(gc[3].Value);
                    int buffer;
                    if (!int.TryParse(gc[1].Value, out buffer))
                        continue;
                    else
                        tmp.Box = buffer;
                    if (!int.TryParse(gc[2].Value, out buffer))
                        continue;
                    else
                        tmp.Contact = buffer;
                    //
                    if (gc.Count > 4)
						name2 = gc[4].Value;
					tmp.Matrix = matrix;
// 					switch(tableType)
// 					{
// 					case ImpulsesTableType.TS:
// 						if(gc.Count < 4)
// 							continue;
// 						if(!int.TryParse(gc[1].Value, out tmp.Box))
// 							continue;
// 						if(!int.TryParse(gc[2].Value, out tmp.Contact))
// 							continue;
// 						tmp.Name = gc[3].Value;
// 						if(gc.Count > 4)
// 							name2 = gc[4].Value;
// 						tmp.Matrix = matrix;
// 						break;
// 					case ImpulsesTableType.TU:
// 						goto case ImpulsesTableType.TS;
// 					case ImpulsesTableType.Channels:
// 						goto case ImpulsesTableType.TS;
// 					case ImpulsesTableType.Blocks:
// 						if(gc.Count < 5)
// 							continue;
// 						if(!int.TryParse(gc[1].Value, out tmp.Matrix))
// 							continue;
// 						if(!int.TryParse(gc[2].Value, out tmp.Box))
// 							continue;
// 						if(!int.TryParse(gc[3].Value, out tmp.Contact))
// 							continue;
// 						tmp.Name = gc[4].Value;
// 						break;
// 					}

					for(int i = 0; i < records.Length; i++)
					{
						if(records[i].Name == tmp.Name)
						{
							records[i].Box = tmp.Box;
							records[i].Contact = tmp.Contact;
							records[i].Matrix = tmp.Matrix;
							count++;
							break;
						}
						if(records[i].Name == name2)
						{
							records[i].Box = tmp.Box;
							records[i].Contact = tmp.Contact;
							records[i].Matrix = tmp.Matrix;
							count++;
							break;
						}
					}
				}
				sr.Close();
			}
			catch 
			{
				throw new IOException(string.Format("File {0} not found", fileName)
																						  );
			}
		}

		/// <summary>
		/// Загрузить таблицу импульсов с описанием контактов
		/// </summary>
		/// <param name="path">Путь к папке в которой находятся папки с таблицами импульсов и контактов.</param>
		/// <param name="station">Код станции, для которой нужно загрузить импульсы</param>
		/// <param name="loadTs">Если true, то загружать таблицу ТС, иначе таблицу ТУ</param>
		/// <param name="records">Массив с описаниями импульсов</param>
 		/// <exception cref="ArgumentException">Если fileName содержит пустую строку</exception>
		/// <exception cref="ArgumentNullException">Если fileName равен null</exception>
		/// <exception cref="FileNotFoundException">Если нет файла соответствующего станции, либо path содержит неправильный путь.</exception>
		/// <exception cref="IOException">Если произошла ошибка при чтении файла.</exception>
		/// <remarks>Если в файле с импульсами нет записей, то records будет равен null.</remarks>
		public static void GetStdImpulses(string path, int station, bool is_tu, out Impulse[] records)
		{
			records = null;
			string fileName = "";
			if(path == null)
				throw new ArgumentNullException("path");
			try
			{
				DirectoryInfo info = null;
				FileInfo[] files = null;
                string i = Path.Combine(path, String.Format("{0:D6}", station));
				info = new DirectoryInfo(Path.Combine(path, String.Format("{0:D6}", station)));
				files = info.GetFiles("*", SearchOption.TopDirectoryOnly);
				
				string pattern = String.Format("^TI{0:D6}.ASM$", station);
					
				foreach(FileInfo file in files)
				{
					if(Regex.IsMatch(file.Name, pattern, RegexOptions.IgnoreCase))
					{
						if(fileName != "")
						{
							System.Console.Error.WriteLine("Warning !!! Not one file for {0} was" + 
							                                               " found: {1} and {2}\nUsing {1}", 
							                                               station, fileName, file.FullName);
							continue;
						}
						fileName = file.FullName;
					}
				}
			}
			catch(ArgumentException e)
			{
				throw new FileNotFoundException(string.Format("File {0} not found", path), e);
			}

			if(fileName == "")
				throw new FileNotFoundException(string.Format("File {0} not found", path));

			
			ReadImpulseTable(fileName, is_tu, out records);

			/*//надо найти все файлы IO*.asm, которые могут быть в этой директории
			System.IO.DirectoryInfo dir = new DirectoryInfo(string.Format(@"{0}/{1:D6}", path, station));
			FileInfo[] files = dir.GetFiles("IO*.ASM");
			//сортирую по имени по возрастанию, чтобы определить номер платы
			for(int i = 0; i < files.Length; i++)
			{
				for(int j = i + 1; j < files.Length; j++)
				{
					if(string.Compare(files[i].Name, files[j].Name) > 0)
					{
						FileInfo tmp = files[i];
						files[i] = files[j];
						files[j] = tmp;
					}
				}
			}

			for(int i = 0; i < files.Length; i++ )
			{
				ReadImpulsesIO(files[i].FullName,
				               //tableType,
				               i + 1, records);
			}*/
		}
	}
}
