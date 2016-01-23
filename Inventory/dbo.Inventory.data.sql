select vendor, count(*) 
from dbo.Inventory (nolock)
group by vendor

select *
 from dbo.Inventory (nolock)
Where title ='49-285'

vendor ='hainan'
group by condition

select top 10 *
 
update dbo.Inventory 
 set [Body (HTML)] =  [Body (HTML)]  + '<li>Lead time is 5-7 days days based on available inventory</li>' --replace([Body (HTML)], '<li>Comes with No </li>', '') 
 --where [Body (HTML)] like '%<b>Condition:</b> NS%'
 
 select * from dbo.Inventory
 where condition ='NE'

select * from dbo.inventory
 where [Body (HTML)] like '%<b>Condition:</b> NS%'

-- <b>Description: </b>MARKER<br><b>Part Number: </b>00033031-7<br><b> 
-- Note:</b><br><li><b>Condition:</b> NS</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li>
-- <li>Sale price is effective for available inventory only </li>
-- <li><b>FOB China</b> </li>

-- <b>Description: </b>OVERINFLATION PLUG<br><b>Part Number: </b>49-285<br><b> Note:</b><br><li><b>Condition:</b> NS</li><li>Comes with No </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>FOB China</b> </li>
update dbo.Inventory
set [Option3 Value] = '$150', Moq = '$150'

option3Condition = 'NS', 
--[Body (HTML)] = replace([Body (HTML)], '<b>Condition:</b> NE', '<b>Condition:</b> NS')
--Where vendor ='hainan' 
--and title not in ('65-53793-38', 'PL88-982LH00')
-- 12425

where [Body (HTML)] like '%<li><b>3-5 days</b> </li>%'
and vendor='hainan'

update dbo.Inventory 
set [Option2 Value] = '3-5'

--set [Body (HTML)] = replace([Body (HTML)], '<li><b>3-5 days</b> </li>', '<li><b>FOB Washington, USA </b></li>')
--where [Body (HTML)] like '%<li><b>3-5 days</b> </li>%'

--<b>Description: </b><br><b>Part Number: </b>15945<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Minimum Order Amount:$200</li><li>Sale price is effective for available inventory only </li><li><b>FOB China</b> </li>
--<b>Description: </b><br><b>Part Number: </b>15945<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>FOB China</b> </li>
------truncate table dbo.Inventory (nolock)
----update dbo.Inventory
----set ModifiedDate = CreatedDate

--select * from dbo.products_export
--where title ='bacc2a6f00754fg'


--delete from dbo.inventory --(nolock)
--where vendor='hainan'


--select * from dbo.products_export
--where title like '31J2498-01%'

<b>Description: </b>RECEPTACLE<br><b>Part Number: </b>ABS0368-01<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>BOLT<br><b>Part Number: </b>22201BC080021L<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>WASHER SOLID<br><b>Part Number: </b>ABS0370-01<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>SPHERICAL BEARING<br><b>Part Number: </b>11-6488P<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>PIN,SHEAR<br><b>Part Number: </b>MS20392-2C13<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>BOLT<br><b>Part Number: </b>22201CM050003E<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>CONTROL BOX ASSY<br><b>Part Number: </b>L2-117-93<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>QUICK-RELEASE-PIN<br><b>Part Number: </b>MS17990C305<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>POTENTIOMETER<br><b>Part Number: </b>MP21R50K/50K<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>
<b>Description: </b>AHRS CONN.HARDENED COVER<br><b>Part Number: </b>732-8052-00<br><b> Note:</b><br><li><b>Condition:</b> NE</li><li>Comes with FAA 8130 form 3 </li><li>All parts are subject to prior sale </li><li>Sale price is effective for available inventory only </li><li><b>3-5 days</b> </li>


select title, [Original Price], [Variant Price], cast(round([Variant Price], 2) as numeric(36,2))
from dbo.inventory
where vendor = 'hainan'
and [Original Price] >=100


update dbo.inventory
set [Variant Price] = cast(round([Variant Price], 2) as numeric(36,2))
where vendor = 'hainan'
and [Original Price] >=100

update dbo.inventory
set [Original Price] =8.19,
[Variant Price] = cast(round(8.19*1.75, 2) as numeric(36,2))
where title = '80-724'
and vendor = 'hainan'


select [Original Price], [Variant Price]-- = 544.05,
--[Variant Price] = cast(round(544.05 *1.5, 2) as numeric(36,2))
from dbo.inventory
where title = '49-285'
and vendor = 'hainan'


select * from dbo.Inventory
where Published = 0


update dbo.Inventory
set [Variant Price] =  [Original Price]* 1.75
--select [Original Price] ,  [Original Price]* 1.75
--from dbo.Inventory
where Vendor = 'hainan'
and [Original Price] >=100

update dbo.Inventory
set  [Original Price]  = 2000,--cast(round(37604.00/6.0, 2) as numeric(36,2)), 
[Variant Price] = 5000--cast(round(37604.00/6.0, 2) as numeric(36,2))*1.5
where title = '179536-1'


select  title, [Original Price] , [Variant Price], ModifiedDate--, cast(round(327.40 /6.0, 2) as numeric(36,2))
from dbo.Inventory
where Vendor = 'hainan'
and [Original Price] >=100
and title = 'CL-4-BLPB-1.25S'

select distinct vendor from dbo.inventory
insert into [dbo].[Vendor]
(Vendor)
select distinct vendor 
from dbo.inventory

select * from dbo.vendor

update dbo.inventory
set  [Variant Price] = case when [Original Price]<50 then [Original Price] * 2 
							when [Original Price] between 50 and 100 then [Original Price] +30
							when [Original Price] between 100 and 1000 then [Original Price] * 1.25 
							when [Original Price] between 1000 and 3000 then [Original Price] * 1.2
							when [Original Price] between 3000 and 10000 then [Original Price] * 1.15
							when [Original Price] >= 10000 then [Original Price] * 1.11 end
	from dbo.inventory
	where vendorshort = 'ab'

                                    --if (originalPrice < 0.01)
                                    --    finalPrice = 0;
                                    --else if (originalPrice < 50)
                                    --    finalPrice = originalPrice * 2;
                                    --else if (originalPrice < 100)
                                    --    finalPrice = originalPrice + 30;
                                    --else if (originalPrice < 1000)
                                    --    finalPrice = originalPrice * 1.25;
                                    --else if (originalPrice < 3000)
                                    --    finalPrice = originalPrice * 1.2;
                                    --else if (originalPrice < 10000)
                                    --    finalPrice = originalPrice * 1.15;
                                    --else
                                    --    finalPrice = originalPrice * 1.11;


Update dbo.inventory
set vendor ='AirbusSpare'
where vendor = 'Airbus Spare'

select count(*) 
from dbo.Inventory
where vendor='Airbus Spare' and VendorShort = 'ab1'

select * from dbo.inventory
where title ='D5331030220100'


select count(*)
from dbo.inventory
where vendor='Zodiac2'

update dbo.inventory
set vendor ='Zodiac'
where vendor ='Zodiac2'


select count(*)
from dbo.inventory
where vendorshort='zg' and title ='115760-11'--'117601-113'

update dbo.Inventory
set [Body (HTML)] = REPLACE([Body (HTML)], 'weeks based on available', 'week(s) based on available')

--delete from dbo.Inventory
where vendorshort='zg' 


<b>Description: </b>FILTER<br><b>Part Number: </b>117601-113<br><b> Note:</b><br><li><b>Condition:</b> FN</li> <li>Comes with EASA Form1 </li> <li>All parts are subject to prior sale </li> <li>Sale price is effective for available inventory only </li> <li><b> FOB Europe </b></li> <li>Lead time is 1 weeks based on available inventory </li> 


select top 10 * from dbo.inventory