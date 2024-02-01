select 
json_each(convert_from(o.PayloadBinary, 'UTF8')::json) jx
,grainidhash
,grainidn0
,grainidn1
,graintypehash
,graintypestring
,grainidextensionstring
,serviceid
,payloadbinary
,modifiedon
,version
from public.orleansstorage o
LIMIT 1000
