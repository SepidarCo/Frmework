select distinct [Character], unicode([Character]) as Value
from
(
	select substring(ibm.[Text], v.number + 1, 1) as [Character]
	from 
	(
		select *
		from InputBufferMessages 
	) ibm
	join master..spt_values v on v.number < len(ibm.[Text])
	where v.type = 'P'
) temp
order by [Character]