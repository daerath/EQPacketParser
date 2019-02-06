# EQPacketParser
Tool for parsing packet captures from Live and Emulator servers. Targets .NET Core 2.1

The tool expects captures to be tab separated with specific column headers as produced by Microsoft Message Analyzer. Column order does not matter, but the column header and names must be present. MMA will output a number of default columns as well as any you select to be visible in the output.

The only required columns are: Source, Destination, and Payload.

Example capture:

MessageNumber	DiagnosisTypes	Timestamp	TimeElapsed	Source	Destination	Module	Summary	PayloadLength	Payload
434	None	2019-01-26T15:44:06.4803768	0.0000013	192.168.1.3	10.0.0.2	UDP	SrcPort: 7000, DstPort: 59812, Length: 15	7	00,15,A5,00,24,23,08	
