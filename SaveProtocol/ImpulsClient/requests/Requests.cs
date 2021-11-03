
namespace SCADA.Common.ImpulsClient.requests
{
	public enum RequestType
	{
        Unknown = 0x0000,
        StationTables = 0x0101,
        Command = 0x0102,
        ListOfTables = 0x0103,
        Time = 0x0104
	}

	enum AnswerType
	{
        StationTables = 1,
        Command = 2,
        Time = 3,
        Error = -1
	}

	public enum RequestError
	{
		UnexpectedError = -1,
		Successful = 0,
		UnknownRequest = 1,
		AccessDenied = 2,
		UnknownStation = 3,
		UnknownCommand = 4,
		IOError = 5,
        WrongRequest = 6
	}

	enum Broadcast
	{
        Unknown = 0,
        ImpulsesTable = 1,
        Command = 3,
        CommandAnswer = 4
	}
}
