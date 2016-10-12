using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Siri_Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracking_Common;

namespace Tracking_Unit_Tests
{
    [TestClass]
    public class BusStopIntegrationTests
    {
        [TestMethod]
        public void TestParsingBusTimingFromVixXHTML()
        {
            #region sample xml
            string standardXhtml = @"


<!DOCTYPE html PUBLIC "" -//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
< html xmlns = ""http://www.w3.org/1999/xhtml"" >
 < head >< title >
     vixConnect
 </ title >< link rel = ""shortcut icon"" href = ""/Page_Content/Default/Images/favicon.ico?v=1.7"" />< link rel = ""stylesheet"" type = ""text/css"" href = ""/Page_Content/Default/siteText.css?v=1.7"" /></ head >
             < body >
                 < form method = ""post"" action = ""./WebDisplay.aspx?stopRef=2537&amp;stopName=Rue+des+Arbres"" id = ""form1"" >
                  < div class=""aspNetHidden"">
<input type = ""hidden"" name=""__EVENTTARGET"" id=""__EVENTTARGET"" value="""" />
<input type = ""hidden"" name=""__EVENTARGUMENT"" id=""__EVENTARGUMENT"" value="""" />
<input type = ""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE"" value=""/wEPDwUJMzE0NDE3MTY5D2QWAgIDD2QWBgIDD2QWAmYPZBYCAgEPFgIeBFRleHQFpQE8YSBocmVmPSIuLi9EZWZhdWx0LmFzcHgiIHRpdGxlPSJWaWV3IHRoZSBtYXAgYmFzZWQgdmVyc2lvbiI+TWFwIFZlcnNpb248L2E+IDxhIGhyZWY9Ii4uL0Jhc2ljL0RlZmF1bHQuYXNweCIgdGl0bGU9IlZpZXcgdGhlIGJhc2ljIHZlcnNpb24iPkJhc2ljIFZlcnNpb248L2E+DQo8YnIgLz5kAgUPZBYCZg9kFg4CAw8WAh8ABY4BPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+RGVwYXJ0dXJlPC9zcGFuPiBpbmZvcm1hdGlvbiBmb3IgPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+UnVlIGRlcyBBcmJyZXM8L3NwYW4+IGF0IDxzcGFuIGNsYXNzPSJlbXBoYXNpc2UiPjA4OjEzPC9zcGFuPmQCBQ8WAh8ABS1TdG9wIFJlZjogPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+MjUzNzwvc3Bhbj5kAgcPPCsAEQMADxYEHgtfIURhdGFCb3VuZGceC18hSXRlbUNvdW50AgNkAQ8UKwAGPCsABQEAFgQeCURhdGFGaWVsZAUYSm91cm5leVB1YmxpY1NlcnZpY2VDb2RlHgpIZWFkZXJUZXh0BQdTZXJ2aWNlFCsABRYEHwMFDk1hcmtldGluZ05hbWVzHwQFDFNlcnZpY2UgTmFtZRYGHg9Ib3Jpem9udGFsQWxpZ24LKilTeXN0ZW0uV2ViLlVJLldlYkNvbnRyb2xzLkhvcml6b250YWxBbGlnbgEeCENzc0NsYXNzBQtncmlkQmF5SXRlbR4EXyFTQgKCgAQWBB8FCysEAh8HAoCABGRkPCsABQEAFgQfAwULRGVzdGluYXRpb24fBAUCVG88KwAFAQAWBB8DBRBTdG9wcGluZ0F0U3RyaW5nHwQFA1ZpYRQrAAUWBB8DBQRUaW1lHwQFBFRpbWUWBB8FCysEAx8HAoCABGRkZDwrAAUBABYEHwMFDExvd0Zsb29yVGV4dB8EBQlMb3cgRmxvb3IUKwEGZmZmZmZmDBQrAAAWAmYPZBYKZg9kFgxmDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIBDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAICDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIDDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIEDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIFDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIBD2QWDGYPDxYGHwAFATUfBgUJYm9keS1jZWxsHwcCAmRkAgEPDxYGHwAFBiZuYnNwOx8GBQlib2R5LWNlbGwfBwICZGQCAg8PFgYfAAUSTGliZXJhdGlvbiBTdGF0aW9uHwYFCWJvZHktY2VsbB8HAgJkZAIDDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYGHwAFBTA5OjIxHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYEHwYFBm9kZHJvdx8HAgJkFgxmDw8WBh8ABQE1HwYFCWJvZHktY2VsbB8HAgJkZAIBDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYGHwAFEkxpYmVyYXRpb24gU3RhdGlvbh8GBQlib2R5LWNlbGwfBwICZGQCAw8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIEDw8WBh8ABQUxMDozMR8GBQlib2R5LWNlbGwfBwICZGQCBQ8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIDD2QWDGYPDxYGHwAFATUfBgUJYm9keS1jZWxsHwcCAmRkAgEPDxYGHwAFBiZuYnNwOx8GBQlib2R5LWNlbGwfBwICZGQCAg8PFgYfAAUSTGliZXJhdGlvbiBTdGF0aW9uHwYFCWJvZHktY2VsbB8HAgJkZAIDDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYGHwAFBTExOjMxHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYCHgdWaXNpYmxlaGRkAhMPDxYCHgdFbmFibGVkaGRkAhcPDxYCHwhoZGQCGQ8PFgIfCGhkZAIfDxYEHwlnHghJbnRlcnZhbAKw6gFkAgcPZBYEAgIPZBYCAgEPDxYCHgtOYXZpZ2F0ZVVybAVoL1RleHQvUHJpdmFjeVBvbGljeS5hc3B4P2JhY2s9JTJmVGV4dCUyZldlYkRpc3BsYXkuYXNweCUzZnN0b3BSZWYlM2QyNTM3JTI2c3RvcE5hbWUlM2RSdWUlMmJkZXMlMmJBcmJyZXNkZAIED2QWAgIBDxYCHwAFEcKpIFZpeCBUZWNobm9sb2d5ZBgBBQtHcmlkVmlld1JUSQ88KwAMAQgCAWR97vlpuXhUd545rwGiYtGXMgSH1DXJFOTlo5NqnPtQjA=="" />
</div>

<script type = ""text/javascript"" >
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


<script src = ""/WebResource.axd?d=mwBG1Ns-u0HwFmwTccdKiathe-Q4T0_fSTvXI7t9MKRlTNnBCRs9oUgc310PCcsxsXfW2IzZDDjKKzSrilfrDco-gKohV5K2IGpEewfCxao1&amp;t=635802997220000000"" type=""text/javascript""></script>


<script src = ""/ScriptResource.axd?d=zKAC-7kV56ueOZlnYmfcc5XuES6E9ma8uz-rD-0DVZVzEgmfkh23ZxO-8BxWNtncAqrqbrAulqaYKvXVu4woD-oQ2KhciEAzZuOJgNNhs8yiaANocfV8OM8WuNo6gTqZUaCEKS1MuC2EhxW2pHJ2kApjzlc_fecJDNfeZM-hA8U1&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = ""/ScriptResource.axd?d=5AOCe7sChY3ZsKLHO6_E8kGio5yXqV1zj2ER9w64B98ifYPC5vR-681wtFZcCGRLRkZce6MuKuOsNX2FdHD9jvGk2r6AWyP9shsDXiUChKB5vEFXimNh9Z7d198NhBZIMovRZC-CuY-vfjcGs5xAT9LYnIDVE1k1Y0MijbO9lk00UkGbgAnN5tFx1IJpxjP-0&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = ""../Scripts/page/audit.release.js"" type=""text/javascript""></script>
<script src = ""/ScriptResource.axd?d=1jQN_8R3yyhXi4to88zW4qjfzJX4Y07Cc6Je2W2n8P0W_vX2sF25LrQKn6dNtVVhzeYtP_Kscvy8GkQ6Wbvfb3iBEL0bzS13OeTYL4pCjzxc_gaL8haLoU5NVsv-X_smfCbBhwJ6lWHzAPHCvfeYgZOhcD2Qi6LXYKcY2CXwDxQ1&amp;t=5f9d5645"" type=""text/javascript""></script>
<div class=""aspNetHidden"">

	<input type = ""hidden"" name=""__VIEWSTATEGENERATOR"" id=""__VIEWSTATEGENERATOR"" value=""B5743445"" />
	<input type = ""hidden"" name=""__EVENTVALIDATION"" id=""__EVENTVALIDATION"" value=""/wEdAAXYFNAdQe95LDdpXeWhgGmABSQ+BOPJgPaIfvOC6/j4WY2+K5Rdmk/7s2uffD0jtbFNLlNJhxgDG+LHuDcClHAvTabGGLaWs5Jxf8sotPOe6Q9rcK6zEMzsdI4aSttpI5KEMelxozuYGOhm+ICO15Hm"" />
</div>
    <div>
        <script type = ""text/javascript"" >
//<![CDATA[
Sys.WebForms.PageRequestManager._initialize('ScriptManager1', 'form1', ['tUpdatePanel1', 'UpdatePanel1'], [], [], 90, '');
//]]>
</script>


        <div id = ""header"" >
            < div id=""HeaderControl_HeaderContainer"">
	
    <a href = ""../Default.aspx"" title=""View the map based version"">Map Version</a> <a href = ""../Basic/Default.aspx"" title= ""View the basic version"" > Basic Version</a>
  <br />

</div>
        </div>
            
        <div id = ""UpdatePanel1"" >
  
                  < input type= ""hidden"" name= ""hidTimeOption"" id= ""hidTimeOption"" value= ""UseDepartureTimes"" />
                  < p class=""titleLine"">
                    <span class=""emphasise"">Departure</span> information for <span class=""emphasise"">Rue des Arbres</span> at<span class=""emphasise"">08:13</span>
                </p>

                <p>
                    Stop Ref: <span class=""emphasise"">2537</span>
                </p>

                <div>
		<table class=""webDisplayTable"" cellspacing=""0"" rules=""all"" border=""1"" id=""GridViewRTI"" style=""border-collapse:collapse;"">
			<tr>
				<th class=""header-cell"" scope=""col"">Service</th><th class=""header-cell"" align=""center"" scope=""col"">Service Name</th><th class=""header-cell"" scope=""col"">To</th><th class=""header-cell"" scope=""col"">Via</th><th class=""header-cell"" scope=""col"">Time</th><th class=""header-cell"" scope=""col"">Low Floor</th>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">Due</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">15</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">19:21</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr class=""oddrow"">
				<td class=""body-cell"">5</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Greve de Lecq</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">17 Mins</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">8 Mins</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Old Update</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">09:21</td><td class=""body-cell"">&nbsp;</td>
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
                    <a id = ""LinkButtonLateDeparture"" title= ""View later times"" href= ""javascript:__doPostBack(&#39;LinkButtonLateDeparture&#39;,&#39;&#39;)"" > Later times</a>
                   </p>

                

                

                <p>
                    <a id = ""LinkSearchAgain"" title= ""Perform another search"" href= ""Default.aspx"" > Search again</a>
                   </p>

                <input type = ""hidden"" name= ""CurrentOffset"" id= ""CurrentOffset"" value= ""0"" />
                   < span id= ""RefreshTimer"" style= ""visibility:hidden;display:none;"" ></ span >
   
   </ div >
   
           < div id= ""footer"" >
   
       < hr id= ""FooterSeparatorLine"" />
   < div id= ""FooterControl_PrivacyPolicyPanel"" >
   
       < a id= ""FooterControl_PrivacyPolicyLink"" title= ""View the sites privacy policy"" href= ""/Text/PrivacyPolicy.aspx?back=%2fText%2fWebDisplay.aspx%3fstopRef%3d2537%26stopName%3dRue%2bdes%2bArbres"" > Privacy Policy</a>
   
</div>

<div id = ""FooterControl_FooterContainer"" >
   	
    © Vix Technology

</div>
        </div>
    </div>
    

<script type = ""text/javascript"" >
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
</html>
";
            #endregion

            var times = BusETA.ConvertVixXhtmlToBusETAs(standardXhtml, DateTime.Parse("2015/12/08 12:00"), 1234);
            Assert.AreEqual(4, times.Count);
            Assert.AreEqual("Liberation Station", times[0].Destination);
            Assert.AreEqual("Liberation Station", times[1].Destination);
            Assert.AreEqual("Greve de Lecq", times[2].Destination);
            Assert.AreEqual("Liberation Station", times[3].Destination);

            Assert.AreEqual("7", times[0].ServiceNumber);
            Assert.AreEqual("15", times[1].ServiceNumber);
            Assert.AreEqual("5", times[2].ServiceNumber); 
            Assert.AreEqual("7", times[3].ServiceNumber);

            Assert.AreEqual("2015/12/08 12:00:00", times[0].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual("2015/12/08 19:21:00", times[1].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual("2015/12/08 12:17:00", times[2].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual("2015/12/08 12:08:00", times[3].ETA.ToString("yyyy/MM/dd HH:mm:ss"));

            Assert.AreEqual(1234, times[0].StopNumber);
            Assert.AreEqual(1234, times[1].StopNumber);
            Assert.AreEqual(1234, times[2].StopNumber);
            Assert.AreEqual(1234, times[3].StopNumber);

        }

        [TestMethod]
        public void TestParsingBusTimingFromVixXHTML_Bug_BST_Case()
        {
            #region sample xml
            string standardXhtml = @"


<!DOCTYPE html PUBLIC "" -//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
< html xmlns = ""http://www.w3.org/1999/xhtml"" >
 < head >< title >
     vixConnect
 </ title >< link rel = ""shortcut icon"" href = ""/Page_Content/Default/Images/favicon.ico?v=1.7"" />< link rel = ""stylesheet"" type = ""text/css"" href = ""/Page_Content/Default/siteText.css?v=1.7"" /></ head >
             < body >
                 < form method = ""post"" action = ""./WebDisplay.aspx?stopRef=2537&amp;stopName=Rue+des+Arbres"" id = ""form1"" >
                  < div class=""aspNetHidden"">
<input type = ""hidden"" name=""__EVENTTARGET"" id=""__EVENTTARGET"" value="""" />
<input type = ""hidden"" name=""__EVENTARGUMENT"" id=""__EVENTARGUMENT"" value="""" />
<input type = ""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE"" value=""/wEPDwUJMzE0NDE3MTY5D2QWAgIDD2QWBgIDD2QWAmYPZBYCAgEPFgIeBFRleHQFpQE8YSBocmVmPSIuLi9EZWZhdWx0LmFzcHgiIHRpdGxlPSJWaWV3IHRoZSBtYXAgYmFzZWQgdmVyc2lvbiI+TWFwIFZlcnNpb248L2E+IDxhIGhyZWY9Ii4uL0Jhc2ljL0RlZmF1bHQuYXNweCIgdGl0bGU9IlZpZXcgdGhlIGJhc2ljIHZlcnNpb24iPkJhc2ljIFZlcnNpb248L2E+DQo8YnIgLz5kAgUPZBYCZg9kFg4CAw8WAh8ABY4BPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+RGVwYXJ0dXJlPC9zcGFuPiBpbmZvcm1hdGlvbiBmb3IgPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+UnVlIGRlcyBBcmJyZXM8L3NwYW4+IGF0IDxzcGFuIGNsYXNzPSJlbXBoYXNpc2UiPjA4OjEzPC9zcGFuPmQCBQ8WAh8ABS1TdG9wIFJlZjogPHNwYW4gY2xhc3M9ImVtcGhhc2lzZSI+MjUzNzwvc3Bhbj5kAgcPPCsAEQMADxYEHgtfIURhdGFCb3VuZGceC18hSXRlbUNvdW50AgNkAQ8UKwAGPCsABQEAFgQeCURhdGFGaWVsZAUYSm91cm5leVB1YmxpY1NlcnZpY2VDb2RlHgpIZWFkZXJUZXh0BQdTZXJ2aWNlFCsABRYEHwMFDk1hcmtldGluZ05hbWVzHwQFDFNlcnZpY2UgTmFtZRYGHg9Ib3Jpem9udGFsQWxpZ24LKilTeXN0ZW0uV2ViLlVJLldlYkNvbnRyb2xzLkhvcml6b250YWxBbGlnbgEeCENzc0NsYXNzBQtncmlkQmF5SXRlbR4EXyFTQgKCgAQWBB8FCysEAh8HAoCABGRkPCsABQEAFgQfAwULRGVzdGluYXRpb24fBAUCVG88KwAFAQAWBB8DBRBTdG9wcGluZ0F0U3RyaW5nHwQFA1ZpYRQrAAUWBB8DBQRUaW1lHwQFBFRpbWUWBB8FCysEAx8HAoCABGRkZDwrAAUBABYEHwMFDExvd0Zsb29yVGV4dB8EBQlMb3cgRmxvb3IUKwEGZmZmZmZmDBQrAAAWAmYPZBYKZg9kFgxmDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIBDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAICDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIDDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIEDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIFDw8WBB8GBQtoZWFkZXItY2VsbB8HAgJkZAIBD2QWDGYPDxYGHwAFATUfBgUJYm9keS1jZWxsHwcCAmRkAgEPDxYGHwAFBiZuYnNwOx8GBQlib2R5LWNlbGwfBwICZGQCAg8PFgYfAAUSTGliZXJhdGlvbiBTdGF0aW9uHwYFCWJvZHktY2VsbB8HAgJkZAIDDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYGHwAFBTA5OjIxHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYEHwYFBm9kZHJvdx8HAgJkFgxmDw8WBh8ABQE1HwYFCWJvZHktY2VsbB8HAgJkZAIBDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgIPDxYGHwAFEkxpYmVyYXRpb24gU3RhdGlvbh8GBQlib2R5LWNlbGwfBwICZGQCAw8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIEDw8WBh8ABQUxMDozMR8GBQlib2R5LWNlbGwfBwICZGQCBQ8PFgYfAAUGJm5ic3A7HwYFCWJvZHktY2VsbB8HAgJkZAIDD2QWDGYPDxYGHwAFATUfBgUJYm9keS1jZWxsHwcCAmRkAgEPDxYGHwAFBiZuYnNwOx8GBQlib2R5LWNlbGwfBwICZGQCAg8PFgYfAAUSTGliZXJhdGlvbiBTdGF0aW9uHwYFCWJvZHktY2VsbB8HAgJkZAIDDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYGHwAFBTExOjMxHwYFCWJvZHktY2VsbB8HAgJkZAIFDw8WBh8ABQYmbmJzcDsfBgUJYm9keS1jZWxsHwcCAmRkAgQPDxYCHgdWaXNpYmxlaGRkAhMPDxYCHgdFbmFibGVkaGRkAhcPDxYCHwhoZGQCGQ8PFgIfCGhkZAIfDxYEHwlnHghJbnRlcnZhbAKw6gFkAgcPZBYEAgIPZBYCAgEPDxYCHgtOYXZpZ2F0ZVVybAVoL1RleHQvUHJpdmFjeVBvbGljeS5hc3B4P2JhY2s9JTJmVGV4dCUyZldlYkRpc3BsYXkuYXNweCUzZnN0b3BSZWYlM2QyNTM3JTI2c3RvcE5hbWUlM2RSdWUlMmJkZXMlMmJBcmJyZXNkZAIED2QWAgIBDxYCHwAFEcKpIFZpeCBUZWNobm9sb2d5ZBgBBQtHcmlkVmlld1JUSQ88KwAMAQgCAWR97vlpuXhUd545rwGiYtGXMgSH1DXJFOTlo5NqnPtQjA=="" />
</div>

<script type = ""text/javascript"" >
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


<script src = ""/WebResource.axd?d=mwBG1Ns-u0HwFmwTccdKiathe-Q4T0_fSTvXI7t9MKRlTNnBCRs9oUgc310PCcsxsXfW2IzZDDjKKzSrilfrDco-gKohV5K2IGpEewfCxao1&amp;t=635802997220000000"" type=""text/javascript""></script>


<script src = ""/ScriptResource.axd?d=zKAC-7kV56ueOZlnYmfcc5XuES6E9ma8uz-rD-0DVZVzEgmfkh23ZxO-8BxWNtncAqrqbrAulqaYKvXVu4woD-oQ2KhciEAzZuOJgNNhs8yiaANocfV8OM8WuNo6gTqZUaCEKS1MuC2EhxW2pHJ2kApjzlc_fecJDNfeZM-hA8U1&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = ""/ScriptResource.axd?d=5AOCe7sChY3ZsKLHO6_E8kGio5yXqV1zj2ER9w64B98ifYPC5vR-681wtFZcCGRLRkZce6MuKuOsNX2FdHD9jvGk2r6AWyP9shsDXiUChKB5vEFXimNh9Z7d198NhBZIMovRZC-CuY-vfjcGs5xAT9LYnIDVE1k1Y0MijbO9lk00UkGbgAnN5tFx1IJpxjP-0&amp;t=5f9d5645"" type=""text/javascript""></script>
<script src = ""../Scripts/page/audit.release.js"" type=""text/javascript""></script>
<script src = ""/ScriptResource.axd?d=1jQN_8R3yyhXi4to88zW4qjfzJX4Y07Cc6Je2W2n8P0W_vX2sF25LrQKn6dNtVVhzeYtP_Kscvy8GkQ6Wbvfb3iBEL0bzS13OeTYL4pCjzxc_gaL8haLoU5NVsv-X_smfCbBhwJ6lWHzAPHCvfeYgZOhcD2Qi6LXYKcY2CXwDxQ1&amp;t=5f9d5645"" type=""text/javascript""></script>
<div class=""aspNetHidden"">

	<input type = ""hidden"" name=""__VIEWSTATEGENERATOR"" id=""__VIEWSTATEGENERATOR"" value=""B5743445"" />
	<input type = ""hidden"" name=""__EVENTVALIDATION"" id=""__EVENTVALIDATION"" value=""/wEdAAXYFNAdQe95LDdpXeWhgGmABSQ+BOPJgPaIfvOC6/j4WY2+K5Rdmk/7s2uffD0jtbFNLlNJhxgDG+LHuDcClHAvTabGGLaWs5Jxf8sotPOe6Q9rcK6zEMzsdI4aSttpI5KEMelxozuYGOhm+ICO15Hm"" />
</div>
    <div>
        <script type = ""text/javascript"" >
//<![CDATA[
Sys.WebForms.PageRequestManager._initialize('ScriptManager1', 'form1', ['tUpdatePanel1', 'UpdatePanel1'], [], [], 90, '');
//]]>
</script>


        <div id = ""header"" >
            < div id=""HeaderControl_HeaderContainer"">
	
    <a href = ""../Default.aspx"" title=""View the map based version"">Map Version</a> <a href = ""../Basic/Default.aspx"" title= ""View the basic version"" > Basic Version</a>
  <br />

</div>
        </div>
            
        <div id = ""UpdatePanel1"" >
  
                  < input type= ""hidden"" name= ""hidTimeOption"" id= ""hidTimeOption"" value= ""UseDepartureTimes"" />
                  < p class=""titleLine"">
                    <span class=""emphasise"">Departure</span> information for <span class=""emphasise"">Rue des Arbres</span> at<span class=""emphasise"">08:13</span>
                </p>

                <p>
                    Stop Ref: <span class=""emphasise"">2537</span>
                </p>

                <div>
		<table class=""webDisplayTable"" cellspacing=""0"" rules=""all"" border=""1"" id=""GridViewRTI"" style=""border-collapse:collapse;"">
			<tr>
				<th class=""header-cell"" scope=""col"">Service</th><th class=""header-cell"" align=""center"" scope=""col"">Service Name</th><th class=""header-cell"" scope=""col"">To</th><th class=""header-cell"" scope=""col"">Via</th><th class=""header-cell"" scope=""col"">Time</th><th class=""header-cell"" scope=""col"">Low Floor</th>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">Due</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">15</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">19:21</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr class=""oddrow"">
				<td class=""body-cell"">5</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Greve de Lecq</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">17 Mins</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Liberation Station</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">8 Mins</td><td class=""body-cell"">&nbsp;</td>
			</tr>

            <tr>
				<td class=""body-cell"">7</td><td class=""body-cell"" align=""left"">&nbsp;</td><td class=""body-cell"">Old Update</td><td class=""body-cell"">&nbsp;</td><td class=""body-cell"" align=""right"">09:21</td><td class=""body-cell"">&nbsp;</td>
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
                    <a id = ""LinkButtonLateDeparture"" title= ""View later times"" href= ""javascript:__doPostBack(&#39;LinkButtonLateDeparture&#39;,&#39;&#39;)"" > Later times</a>
                   </p>

                

                

                <p>
                    <a id = ""LinkSearchAgain"" title= ""Perform another search"" href= ""Default.aspx"" > Search again</a>
                   </p>

                <input type = ""hidden"" name= ""CurrentOffset"" id= ""CurrentOffset"" value= ""0"" />
                   < span id= ""RefreshTimer"" style= ""visibility:hidden;display:none;"" ></ span >
   
   </ div >
   
           < div id= ""footer"" >
   
       < hr id= ""FooterSeparatorLine"" />
   < div id= ""FooterControl_PrivacyPolicyPanel"" >
   
       < a id= ""FooterControl_PrivacyPolicyLink"" title= ""View the sites privacy policy"" href= ""/Text/PrivacyPolicy.aspx?back=%2fText%2fWebDisplay.aspx%3fstopRef%3d2537%26stopName%3dRue%2bdes%2bArbres"" > Privacy Policy</a>
   
</div>

<div id = ""FooterControl_FooterContainer"" >
   	
    © Vix Technology

</div>
        </div>
    </div>
    

<script type = ""text/javascript"" >
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
</html>
";
            #endregion

            //A date which is in the summer
            var summerTimes = BusETA.ConvertVixXhtmlToBusETAs(standardXhtml, DateTime.Parse("2015/08/01 12:00"), 1234);
            Assert.AreEqual("2015/08/01 12:00:00", summerTimes[0].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual("2015/08/01 19:21:00", summerTimes[1].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual(DateTimeKind.Local, summerTimes[1].ETA.Kind);

            //A date which is in the winter
            var winterTimes = BusETA.ConvertVixXhtmlToBusETAs(standardXhtml, DateTime.Parse("2015/12/01 12:00"), 1234);
            Assert.AreEqual("2015/12/01 12:00:00", winterTimes[0].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual("2015/12/01 19:21:00", winterTimes[1].ETA.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual(DateTimeKind.Unspecified, winterTimes[1].ETA.Kind);
            
            //these two dates are different and they are sent as json in the correct format later on
            Assert.AreNotEqual(summerTimes[1].ETA, winterTimes[1].ETA);
        }

        [TestMethod]
        public void ParseHtml_WithScheduledEntries_ReturnsCorrectIsTrackedResults()
        {
            //Arrange
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016,07,21,11,15,00);

            //Act
            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, 3895);

            //Assert
            Assert.AreEqual(result[0].IsTracked,true);
            Assert.AreEqual(result[1].IsTracked, true);
            Assert.AreEqual(result[2].IsTracked, false);
        }

        [TestMethod]
        public void ParseHtml_ReturnsCorrectETAs_Succeeds()
        {
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016, 07, 21, 11, 15, 00);

            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, 3895);

            Assert.AreEqual(result[0].ETA, time);
            Assert.AreEqual(result[1].ETA, time.AddMinutes(64));
            Assert.AreEqual(result[2].ETA,(new DateTime(2016, 07, 21, 15, 31, 0)));
        }

        [TestMethod]
        public void ParseHtml_ReturnsCorrectStopNumber_Succeeds()
        {
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016, 07, 21, 11, 15, 00);
            var expected = 3895;

            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, expected).First();

            Assert.AreEqual(result.StopNumber, expected);
        }

        [TestMethod]
        public void ParseHtml_ReturnsCorrectServiceNumbers_Succeeds()
        {
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016, 07, 21, 11, 15, 00);

            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, 3895);

            Assert.AreEqual(result[0].ServiceNumber, "1");
            Assert.AreEqual(result[1].ServiceNumber, "2");
            Assert.AreEqual(result[2].ServiceNumber, "3");
        }

        [TestMethod]
        public void ParseHtml_ReturnsCorrectDestinations_Succeeds()
        {
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016, 07, 21, 11, 15, 00);

            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, 3895);

            Assert.AreEqual(result[0].Destination, "St Helier");
            Assert.AreEqual(result[1].Destination, "St John");
            Assert.AreEqual(result[2].Destination, "Grouville");
        }

        [TestMethod]
        public void ParseHtml_ReturnsCorrectCount_Succeeds()
        {
            var testHtml = VixHtmlSamples.WithScheduledEntriesTestHtml;
            var time = new DateTime(2016, 07, 21, 11, 15, 00);

            var result = BusETA.ConvertVixXhtmlToBusETAs(testHtml, time, 3895);

            Assert.AreEqual(result.Count, 3);
        }
    }
}