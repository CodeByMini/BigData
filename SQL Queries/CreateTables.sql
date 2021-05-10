DROP TABLE MeasurmentTable
DROP TABLE DeviceTable
DROP TABLE TimeTable
DROP TABLE LocationTable
DROP TABLE DeviceTypeTable
DROP TABLE VendorTable
DROP TABLE DeviceModelTable


CREATE TABLE TimeTable
(
	[TimeStamp]		BIGINT PRIMARY KEY,
	[UtcDateTime]	AS DATEADD(S, [TimeStamp], '1970-01-01 00:00:00'), 
	[Date]			AS CONVERT(DATE, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')), 
	[Year]			AS DATEPART(YEAR, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),  
	[Quarter]		AS DATEPART(QUARTER, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[QuarterName]	AS CONVERT(CHAR(2), CASE DATEPART(QUARTER, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')) WHEN 1 THEN 'Q1' WHEN 2 THEN 'Q2' WHEN 3 THEN 'Q3' WHEN 4 THEN 'Q4' END),  
	[Month]			AS DATEPART(MONTH, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),  
	[MonthName]		AS DATENAME(MONTH, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[WeekdayOfMonth]AS DATEPART(DAY, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[WeekdayName]	AS DATENAME(WEEKDAY, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[DayOfWeek]		AS DATEPART(WEEKDAY, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[DayOfYear]		AS DATEPART(DAYOFYEAR, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[Hour]			AS DATEPART(HOUR, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[Minute]		AS DATEPART(MINUTE, DATEADD(S, [TimeStamp], '1970-01-01 00:00:00')),
	[Second]		AS DATEPART(SECOND,	DATEADD(S, [TimeStamp], '1970-01-01 00:00:00'))
)
GO

CREATE TABLE LocationTable (
	Id bigint not null identity(1,1) PRIMARY KEY,
	Longitude float not null,
	Latitude float not null
)
GO

CREATE TABLE SensorTypeTable (
	Id bigint not null identity(1,1) PRIMARY KEY,
	SensorType nvarchar(50) not null unique
)
GO

CREATE TABLE VendorTable(
	Id bigint not null identity(1,1) PRIMARY KEY,
	Vendor nvarchar(50) not null unique
)
GO

CREATE TABLE DeviceModelTable(
	Id bigint not null identity(1,1) PRIMARY KEY,
	DeviceModel nvarchar(50) not null unique
)
GO

CREATE TABLE DeviceTable (
	Id nvarchar(17) not null PRIMARY KEY,
	SensorTypeId bigint not null references SensorTypeTable(Id),
	VendorId bigint not null references VendorTable(Id),
	DeviceModelId bigint not null references DeviceModelTable(Id),

)
GO
CREATE TABLE MeasurmentTable (
	Id bigint not null identity(1,1) PRIMARY KEY,
	TimeId bigint not null references TimeTable([TimeStamp]),
	DeviceId nvarchar(17) not null references DeviceTable(Id),
	LocationId bigint not null references LocationTable(Id),
	Temp float,
	Humidity float

)




