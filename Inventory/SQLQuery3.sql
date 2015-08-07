select symbol, count(*) 
from dbo.nasdaq
group by symbol
having count(*) >1

--delete from dbo.finance 
--where symbol = 'ORCL'

select * from dbo.nasdaq 
where symbol = 'symbol'

-- File Creation Time: 0721201521:32
select count(*) from dbo.nasdaq

--truncate table dbo.finance

select count(*) from dbo.finance with (nolock)
select * from dbo.finance where symbol ='AAIT'

select
  tt1.day,
  tt1.symbol,
  tt1.[close] day1, 
  tt2.[close] day2,
  (tt1.[close]-tt2.[close]) as delta,
  CONVERT(VARCHAR(50),cast(round((tt1.[close]-tt2.[close])/tt1.[close]*100, 2) as numeric(36,2))) +' %' AS [%]
from dbo.finance tt1
  left outer JOIN dbo.finance tt2 on tt1.symbol = tt2.symbol
    and (DATEDIFF(dd, tt1.day, tt2.day) + 1)
  -(DATEDIFF(wk, tt2.day, tt1.day) * 2)
  -(CASE WHEN DATENAME(dw, tt2.day) = 'Sunday' THEN 1 ELSE 0 END)
  -(CASE WHEN DATENAME(dw, tt2.day) = 'Saturday' THEN 1 ELSE 0 END) = 2
where tt1.symbol ='sbux'
order by tt1.day

select * from dbo.finance
where symbol ='AAIT'
order by [day]


cast(round(37604.00/6.0, 2) as numeric(36,2))

SET @StartDate ='2015-01-09'
SET @EndDate = '2015-01-12'
SELECT
   (DATEDIFF(dd, '2015-01-08', '2015-01-09'))
  -(DATEDIFF(wk, '2015-01-08', '2015-01-09') * 2)
  -(CASE WHEN DATENAME(dw, '2015-01-09') = 'Sunday' THEN 1 ELSE 0 END)
  -(CASE WHEN DATENAME(dw, '2015-01-09') = 'Saturday' THEN 1 ELSE 0 END)

SELECT
   DATEDIFF(dd, '2015-01-09', '2015-01-08') 

  select * from dbo.finance
  where symbol ='sbux'

insert into [dbo].[dailyDelta]
select
  tt2.day,
  tt1.symbol,
  tt1.[close] previousday, 
  tt2.[close] currentday,
  (tt1.[close]-tt2.[close]) as delta,
  cast(round((tt1.[close]-tt2.[close])/tt1.[close]*100, 2) as numeric(36,2))
from dbo.finance tt1
  left outer JOIN dbo.finance tt2 on tt1.symbol = tt2.symbol
    and tt2.id-tt1.id = 1
where tt1.symbol ='sbux'
order by tt1.day

--truncate table [dbo].[dailyDelta]

DECLARE @name VARCHAR(50) 
DECLARE db_cursor CURSOR FOR  
SELECT count(symbol )
FROM dbo.nasdaq 
where id > 2699


open db_cursor
fetch next from db_cursor into @name

WHILE @@FETCH_STATUS = 0   
BEGIN   
		print @name
		insert into [dbo].[dailyDelta] (day, symbol, pdayclose, todayclose, delta, delta2pdayPrecentage)
		select
		  tt2.day,
		  tt1.symbol,
		  tt1.[close] previousday, 
		  tt2.[close] currentday,
		  (tt1.[close]-tt2.[close]) as delta,
		  cast(round((tt1.[close]-tt2.[close])/tt1.[close]*100, 2) as numeric(36,2))
		from dbo.finance tt1
		  left outer JOIN dbo.finance tt2 on tt1.symbol = tt2.symbol
			and tt2.id-tt1.id = 1
		where tt1.symbol =@name
		order by tt1.day 

       FETCH NEXT FROM db_cursor INTO @name   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor 