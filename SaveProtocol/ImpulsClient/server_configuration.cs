using System;
using System.Xml;
using System.Xml.Serialization;

namespace SCADA.Common.ImpulsClient
{
	[Serializable]
	[XmlRoot(ElementName = "Configuration")]
	public class ServerConfiguration
	{
		private StationRecord[] m_stations;

		public ServerConfiguration()
		{
			m_stations = new StationRecord[0];
		}

		[XmlArrayItem(ElementName = "Station")]
		[XmlArray()]
		public StationRecord[] Stations
		{
			get { return m_stations; }
			set { m_stations = value; }
		}

		public static ServerConfiguration FromFile(string configFile)
		{
			XmlSerializer ser = null;
			ser = new XmlSerializer(typeof(ServerConfiguration));

			XmlReader reader = null;

            try
            {
                reader = XmlReader.Create(configFile);
            }
            catch { }
			ServerConfiguration settings = new ServerConfiguration();
			try
			{
				settings = (ServerConfiguration)ser.Deserialize(reader);
			}
			catch(SystemException)
			{
                if(reader!=null)
                    reader.Close();
			}

			return settings;
		}
	}

    [Serializable]
    public class StationRecord
    {
        private string m_name;
        private int m_code;
        private TableRecord[] m_tables;

        public StationRecord()
        {
            m_name = string.Empty;
            m_code = -1;
            m_tables = null;
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public int Code
        {
            get { return m_code; }
            set { m_code = value; }
        }

        [XmlArrayItem(ElementName = "Table")]
        public TableRecord[] Tables
        {
            get { return m_tables; }
            set { m_tables = value; }
        }
    }
	
	[Serializable]
	public class TableRecord
	{
		private int m_type;
		private string m_source;
		private int m_id;

		public TableRecord()
		{
			m_type = 0;
			m_source = string.Empty;
			m_id = 0;
		}

		public int Type
		{
			get { return m_type; }
			set { m_type = value; }
		}
		public string Source
		{
			get { return m_source; }
			set { m_source = value; }
		}
		public int ID
		{
			get { return m_id; }
			set { m_id = value; }
		}
	}
}

