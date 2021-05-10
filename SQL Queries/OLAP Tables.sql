
DROP TABLE DimDeviceTable
DROP TABLE DimTimeTable
DROP TABLE DimLocationTable
DROP TABLE DimMeasurments


CREATE TABLE DimTimeTable
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

CREATE TABLE DimLocationTable (
	Id bigint not null identity(1,1) PRIMARY KEY,
	Longitude float not null,
	Latitude float not null
)
GO

CREATE TABLE DimDeviceTable (
	Id nvarchar(17) not null PRIMARY KEY,
	SensorType nvarchar(50) not null,
	Vendor nvarchar(50) not null,
	DeviceModel nvarchar(50) not null,
)
GO

CREATE TABLE DimMeasurments (
	Id bigint not null identity(1,1) PRIMARY KEY,
	otherId bigint not null,
	Temp float,
	Humidity float
)

CREATE TABLE FactMeasurmentTable (
	Id bigint not null identity(1,1) PRIMARY KEY,
	TimeId bigint not null references DimTimeTable([TimeStamp]),
	DeviceId nvarchar(17) not null references DimDeviceTable(Id),
	LocationId bigint not null references DimLocationTable(Id),
	Measurments bigint not null references DimMeasurments(Id)
)




