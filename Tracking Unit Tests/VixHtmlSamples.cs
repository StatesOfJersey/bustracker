namespace Tracking_Unit_Tests
{
    public class VixHtmlSamples
    {
        //Returns Route: 1 = Due
        //Route: 2 = 64 mins
        //Route: 3 = 15:31
        //Stop 3895
        public static string WithScheduledEntriesTestHtml = @"
< !DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns = ""http://www.w3.org/1999/xhtml"">
<head><title>
	vixConnect
</title><link rel = ""shortcut icon"" href=""/Page_Content/Default/Images/favicon.ico? v = 1.7"" /><link rel = ""stylesheet"" type=""text/css"" href=""/Page_Content/Default/siteText.css? v = 1.7"" /></head>
<body>
    <form method = ""post"" action=""./WebDisplay.aspx? stopRef = 3895"" id=""form1"">
<div class=""aspNetHidden"">
<input type = ""hidden"" name=""__EVENTTARGET"" id=""__EVENTTARGET"" value="""" />
<input type = ""hidden"" name=""__EVENTARGUMENT"" id=""__EVENTARGUMENT"" value="""" />
<input type = ""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE"" value=""/wEPDwUJMzE0NDE3MTY5D2QWAgIDD2QWBgIDD2QWAmYPZBYCAgEPFgIeBFRleHQFpQE8YSBocmVmPSIuLi9EZWZhdWx0LmFzcHgiIHRpdGxlPSJWaWV3IHRoZSBtYXAgYmFzZWQgdmVyc2lvbiI+TWFwIFZlcnNpb248L2E+IDxhIGhyZWY9Ii4uL0Jhc2ljL0RlZmF1bHQuYXNweCIgdGl0bGU9IlZpZXcgdGhlIGJhc2ljIHZlcnNpb24iPkJhc2ljIFZlcnNpb248L2E+DQo8YnIgLz5kAgUPZBYCZg9kFg4CAw8WAh8ABYABPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+RGVwYXJ0dXJlPC9zcGFuPiBpbmZvcm1hdGlvbiBmb3IgPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+PC9zcGFuPiBhdCA8c3BhbiBjbGFzcz0iZW1waGFzaXNlIj4xMDo1ODwvc3Bhbj5kAgUPFgIfAAUtU3RvcCBSZWY6IDxzcGFuIGNsYXNzPSJlbXBoYXNpc2UiPjM4OTU8L3NwYW4+ZAIHDzwrABEDAA8WBB4LXyFEYXRhQm91bmRnHgtfIUl0ZW1Db3VudAIDZAEPFCsABjwrAAUBABYEHglEYXRhRmllbGQFGEpvdXJuZXlQdWJsaWNTZXJ2aWNlQ29kZR4KSGVhZGVyVGV4dAUHU2VydmljZRQrAAUWBB8DBQ5NYXJrZXRpbmdOYW1lcx8EBQxTZXJ2aWNlIE5hbWUWBh4PSG9yaXpvbnRhbEFsaWduCyopU3lzdGVtLldlYi5VSS5XZWJDb250cm9scy5Ib3Jpem9udGFsQWxpZ24BHghDc3NDbGFzcwULZ3JpZEJheUl0ZW0eBF8hU0ICgoAEFgQfBQsrBAIfBwKAgARkZDwrAAUBABYEHwMFC0Rlc3RpbmF0aW9uHwQFAlRvPCsABQEAFgQfAwUQU3RvcHBpbmdBdFN0cmluZx8EBQNWaWEUKwAFFgQfAwUEVGltZR8EBQRUaW1lFgQfBQsrBAMfBwKAgARkZGQ8KwAFAQAWBB8DBQxMb3dGbG9vclRleHQfBAUJTG93IEZsb29yFCsBBmZmZmZmZgwUKwAAFgJmD2QWCmYPZBYMZg8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCAQ8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCAg8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCAw8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCBA8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCBQ8PFgQfBgULaGVhZGVyLWNlbGwfBwICZGQCAQ9kFgxmDw8WBh8ABQE0HwYFCWJvZHktY2VsbB8HAgJkZAIBDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYGHwAFCVN0IEhlbGllch8GBQlib2R5LWNlbGwfBwICZGQCAw8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIEDw8WBh8ABQc2MyBNaW5zHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYEHwYFBm9kZHJvdx8HAgJkFgxmDw8WBh8ABQE0HwYFCWJvZHktY2VsbB8HAgJkZAIBDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYGHwAFCVN0IEhlbGllch8GBQlib2R5LWNlbGwfBwICZGQCAw8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIEDw8WBh8ABQUxNDowMR8GBQlib2R5LWNlbGwfBwICZGQCBQ8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIDD2QWDGYPDxYGHwAFATQfBgUJYm9keS1jZWxsHwcCAmRkAgEPDxYGHwAFBiZuYnNwOx8GBQlib2R5LWNlbGwfBwICZGQCAg8PFgYfAAUJU3QgSGVsaWVyHwYFCWJvZHktY2VsbB8HAgJkZAIDDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYGHwAFBTE1OjMxHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYCHgdWaXNpYmxlaGRkAhMPDxYCHgdFbmFibGVkaGRkAhcPDxYCHwhoZGQCGQ8PFgIfCGhkZAIfDxYEHwlnHghJbnRlcnZhbAKw6gFkAgcPZBYEAgIPZBYCAgEPDxYCHgtOYXZpZ2F0ZVVybAVIL1RleHQvUHJpdmFjeVBvbGljeS5hc3B4P2JhY2s9JTJmVGV4dCUyZldlYkRpc3BsYXkuYXNweCUzZnN0b3BSZWYlM2QzODk1ZGQCBA9kFgICAQ8WAh8ABRHCqSBWaXggVGVjaG5vbG9neWQYAQULR3JpZFZpZXdSVEkPPCsADAEIAgFkE6UOboN9tVnCwWE8mzWd06r489NK0muXjZ3gue5OsCU="" />
</div>

<script type = ""text/javascript"">
//<![CDATA[
var theForm = document.forms['form1'];
if (!theForm) {
    theForm = document.form1;
}
    function __doPostBack(eventTarget, eventArgument)
    {
        if (!theForm.onsubmit || (theForm.onsubmit() != false))
        {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    }
//]]>
</script>


<script src = "" / WebResource.axd ? d = mwBG1Ns - u0HwFmwTccdKiathe - Q4T0_fSTvXI7t9MKRlTNnBCRs9oUgc310PCcsxsXfW2IzZDDjKKzSrilfrDco - gKohV5K2IGpEewfCxao1 & amp; t=635802997220000000"" type=""text/javascript""></script>


<script src = "" / ScriptResource.axd ? d = zKAC - 7kV56ueOZlnYmfcc5XuES6E9ma8uz - rD - 0DVZVzEgmfkh23ZxO-8BxWNtncAqrqbrAulqaYKvXVu4woD-oQ2KhciEAzZuOJgNNhs8yiaANocfV8OM8WuNo6gTqZUaCEKS1MuC2EhxW2pHJ2kApjzlc_fecJDNfeZM-hA8U1&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = "" / ScriptResource.axd ? d = 5AOCe7sChY3ZsKLHO6_E8kGio5yXqV1zj2ER9w64B98ifYPC5vR - 681wtFZcCGRLRkZce6MuKuOsNX2FdHD9jvGk2r6AWyP9shsDXiUChKB5vEFXimNh9Z7d198NhBZIMovRZC-CuY-vfjcGs5xAT9LYnIDVE1k1Y0MijbO9lk00UkGbgAnN5tFx1IJpxjP-0&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = ""../ Scripts / page / audit.release.js"" type=""text/javascript""></script>
<script src = "" / ScriptResource.axd ? d = 1jQN_8R3yyhXi4to88zW4qjfzJX4Y07Cc6Je2W2n8P0W_vX2sF25LrQKn6dNtVVhzeYtP_Kscvy8GkQ6Wbvfb3iBEL0bzS13OeTYL4pCjzxc_gaL8haLoU5NVsv - X_smfCbBhwJ6lWHzAPHCvfeYgZOhcD2Qi6LXYKcY2CXwDxQ1 & amp; t=5f9d5645"" type=""text/javascript""></script>
<div class=""aspNetHidden"">

	<input type = ""hidden"" name=""__VIEWSTATEGENERATOR"" id=""__VIEWSTATEGENERATOR"" value=""B5743445"" />
	<input type = ""hidden"" name=""__EVENTVALIDATION"" id=""__EVENTVALIDATION"" value=""/wEdAAVyTuQM6uJKUWjTUqSIyKg6BSQ+BOPJgPaIfvOC6/j4WY2+K5Rdmk/7s2uffD0jtbFNLlNJhxgDG+LHuDcClHAvTabGGLaWs5Jxf8sotPOe6X2meIB2MSyJgEph3Wo+2D9hwivUHOAM4qde/wm3uRE0"" />
</div>
    <div>
        <script type = ""text/javascript"">
//<![CDATA[
Sys.WebForms.PageRequestManager._initialize('ScriptManager1', 'form1', ['tUpdatePanel1','UpdatePanel1'], [], [], 90, '');
//]]>
</script>


        <div id = ""header"">
            <div id = ""HeaderControl_HeaderContainer"">
	
    <a href = ""../ Default.aspx"" title=""View the map based version"">Map Version</a> <a href = ""../ Basic / Default.aspx"" title= ""View the basic version"">Basic Version</a>
<br />

</div>
        </div>
            
        <div id = ""UpdatePanel1"">
	
                <input type = ""hidden"" name= ""hidTimeOption"" id= ""hidTimeOption"" value= ""UseDepartureTimes"" />
                <p class=""titleLine"">
                    <span class=""emphasise"">Departure</span> information for <span class=""emphasise""></span> at<span class=""emphasise"">10:58</span>
                </p>

                <p>
                    Stop Ref: <span class=""emphasise"">3895</span>
                </p>

                <div>
		<table class=""webDisplayTable"" cellspacing=""0"" rules=""all"" border=""1"" id=""GridViewRTI"" style=""border-collapse:collapse;"">
			<tr>
				<th class=""header-cell"" scope=""col"">Service</th><th class=""header-cell"" align=""center"" scope=""col"">Service Name</th><th class=""header-cell"" scope=""col"">To</th><th class=""header-cell"" scope=""col"">Via</th><th class=""header-cell"" scope=""col"">Time</th><th class=""header-cell"" scope=""col"">Low Floor</th>
			</tr><tr>
				<td class=""body-cell"">1</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">St Helier</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">Due</td><td class=""body-cell"">&nbsp;</td>
			</tr><tr class=""oddrow"">
				<td class=""body-cell"">2</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">St John</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">64 Mins</td><td class=""body-cell"">&nbsp;</td>
			</tr><tr>
				<td class=""body-cell"">3</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Grouville</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">15:31</td><td class=""body-cell"">&nbsp;</td>
			</tr>
		</table>
	</div>

                

                <p>
                    
                </p>

                <p>
                    <a id = ""lbShowArrivals"" title=""Change between showing Arrivals and Departures"" class=""timeOption"" href=""javascript:__doPostBack(&#39;lbShowArrivals&#39;,&#39;&#39;)"">Show arrivals</a>
                    
                </p>

                <p>
                    
                </p>

                <p>
                    <a id = ""LinkButtonEarlyDeparture"" title= ""View earlier times"" class=""aspNetDisabled"">Earlier times</a>
                   </p>

                <p>
                    <a id = ""LinkButtonLateDeparture"" title= ""View later times"" href= ""javascript:__doPostBack(&#39;LinkButtonLateDeparture&#39;,&#39;&#39;)"">Later times</a>
                </p>

                

                

                <p>
                    <a id = ""LinkSearchAgain"" title= ""Perform another search"" href= ""Default.aspx"">Search again</a>
                </p>

                <input type = ""hidden"" name= ""CurrentOffset"" id= ""CurrentOffset"" value= ""0"" />
                <span id = ""RefreshTimer"" style= ""visibility:hidden; display:none;""></span>
            
</div>

        <div id = ""footer"">
            
    <hr id = ""FooterSeparatorLine"" />
<div id = ""FooterControl_PrivacyPolicyPanel"">
	
    <a id = ""FooterControl_PrivacyPolicyLink"" title=""View the sites privacy policy"" href=""/Text/PrivacyPolicy.aspx? back =% 2fText%2fWebDisplay.aspx%3fstopRef%3d3895"">Privacy Policy</a>
     
</div>

<div id = ""FooterControl_FooterContainer"">
	
    © Vix Technology

</div>
        </div>
    </div>
    

<script type = ""text/javascript"">
//<![CDATA[
if(!auditUsage)
    { auditUsage = new AuditUsage(auditSettings ={ }, false); }
    Sys.Application.add_init(function()
    {
    $create(Sys.UI._Timer, { ""enabled"":true,""interval"":30000,""uniqueID"":""RefreshTimer""}, null, null, $get(""RefreshTimer""));
    });
//]]>
</script>
</form>
</body>
</html>";
    }
}