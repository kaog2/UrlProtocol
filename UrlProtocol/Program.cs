using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Win32;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Net;

/// <summary>
///   The protocol is designed to open text documents as well as office, pdf or txt and to send Emails with attached documents. 
///   The URL is composed as 'protocol://domain:port/path?Query=parameters'
///   This program is executed as 'keor://<viewfile><sendfile>/email[Sendto{e1;e2...}/SendCC{e1;e2;..}/SendBcc{e1;e2;..}/Subject/Body]?attachedFiles=Pathfiles[file;file;file]'
///   To be able to execute the application:
///   First you must load the registry keor_command_register.reg found in the reg folder of the project or the programm can self it install, but if don't work, you have to install manually them.
/// </summary>
namespace UrlProtocol
{
	class Program
	{
		public static string subKey = "appstarter";
		/*if we want to use win-forms in console (1)*/
		/* (1) [STAThread]*/
		[STAThread]
		static void Main(string[] args)
		{
			try
			{

				//install
				if (args[0].Equals("-i"))
				{
					try
					{
						//set the directories to start the program
						Configuration(args[1]);
						DialogResult dialog = MessageBox.Show("App wurde erfolgreich installiert");
					}
					catch (Exception ex)
					{
						DialogResult dialog = MessageBox.Show(String.Format("App wurde nicht installiert \n {0}", ex.Message));
					}

				}
				if (args[0].Equals("-si"))
				{
					try
					{
						//set the directories to start the program
						Configuration(args[1]);
					}
					catch (Exception ex)
					{
						throw;
					}
				}
				else if (args[0].Equals("-u"))
				{
					try
					{
						//set the directories to start the program
						Uninstall(args[1]);
						DialogResult dialog = MessageBox.Show("App wurde erfolgreich deinstalliert");
					}
					catch (Exception ex)
					{
						DialogResult dialog = MessageBox.Show("App wurde nicht deinstalliert");
					}
				}
				else if (args[0].Equals("-su"))
				{
					try
					{
						//set the directories to start the program
						Uninstall(args[1]);
					}
					catch (Exception ex)
					{
						throw;
					}
				}
				else if (args[0].StartsWith("appstarter://"))
				{

					Regex urlExpresion = new Regex(@"^(?<protocol>[a-z]+)(?::\/\/)(?<domain>[a-z]+(?:[a-z]|(?:\.))+[a-z]{2,})(?:(?::)(?<port>[0-9]+))?(?<path>(?:\/).*?)?(?:(?:\?)|$)(?:\?)?(?<parameters>.*)?");
					//Regex exp = new Regex(@"^(?<protocol>[a-z]+)(?::\/\/)(?<domain>[a-z]+(?:[a-z]|(?:\.))+[a-z]{2,})(?:(?::)(?<port>[0-9]+))?(?<path>(?:\/).*?)(?:(?:\?)|$)(?:\?)?(?<parameters>.*)?"); Normal
					//Regex exp = new Regex(@"^(?<protocol>[a-z]+)(?::\/\/)(?<domain>[a-z]+(?:[a-z]|(?:\.))+[a-z]{2,})(?:(?::)(?<port>[0-9]+))?(?<path>(?:\\).*?)(?:(?:\?)|$)(?:\?)?(?<parameters>.*)?");

					try
					{   //if match
						 //string parameter = "textFile=W0FsbGdlbWVpbl0KUHJvZmlsYW56YWhsPTUKUHJvZmlsbmFtZT1BQlNfUHJvZmlsTmFtZQpTdGFuZGFyZHByb2ZpbD1BYnNlbmRlcmluZm9ybWF0aW9uXzUKQUJTX2VNYWlsPUUtTWFpbApBQlNfRnVua3Rpb249RnVua3Rpb24KQUJTX1RlbGVmb249Rk9OCkFCU19IYW5keT1Nb2JpbApBQlNfRmF4PUZBWApBQlNfQmVyZWljaDE9QmVyZWljaCAxCkFCU19CZXJlaWNoMj1CZXJlaWNoIDIKQUJTX0dydcOfZm9ybWVsPUdydcOfZm9ybWVsCkFCU19VbnRlcnNjaHJpZnQxPVVudGVyc2NocmlmdCAxCkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjE9VW50ZXJzY2hyaWZ0IEZ1bmt0aW9uIDEKQUJTX1VudGVyc2NocmlmdFdlcnRpZ2tlaXQxPVVudGVyc2NocmlmdCBXZXJ0aWdrZWl0IDEKQUJTX1VudGVyc2NocmlmdDI9VW50ZXJzY2hyaWZ0IDIKQUJTX1VudGVyc2NocmlmdEZ1bmt0aW9uMj1VbnRlcnNjaHJpZnQgRnVua3Rpb24gMgpBQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDI9VW50ZXJzY2hyaWZ0IFdlcnRpZ2tlaXQgMgpTVE9fQmV6ZWljaG51bmcxPVN0YW5kb3J0YmV6ZWljaG51bmcgMQpTVE9fQmV6ZWljaG51bmcyPVN0YW5kb3J0YmV6ZWljaG51bmcgMgpTVE9fT3J0SGF1cz1BYnNlbmRlcm9ydApTVE9fU3RyYXNzZT1TdHJhw59lClNUT19UZWxlZm9uPUZPTiB2b20gU3RhbmRvcnQKU1RPX0ZheD1GQVggdm9tIFN0YW5kb3J0ClNUT19lTWFpbD1FLU1haWwgdm9tIFN0YW5kb3J0CkFCU19UaXRlbFZvcm5hbWVOYWNobmFtZT1OYW1lClNUT19QTFpIYXVzT3J0SGF1cz1QTFogLyBPcnQgZGVyIEhhdXNhbnNjaHJpZnQKU1RPX1BMWlBvc3RPcnRQb3N0PVBMWiAvIE9ydCBkZXIgUG9zdGFuc2NocmlmdApTVE9fYVN0cmFzc2U9U3RyYcOfZSAoYWJ3ZWljaGVuZCkKU1RPX2FQTFpIYXVzPVBMWiAoYWJ3ZWljaGVuZCkKU1RPX2FPcnRIYXVzPU9ydCAoYWJ3ZWljaGVuZCkKU1RPX2FQTFpIYXVzT3J0SGF1cz1QTFogLyBPcnQgKGFid2VpY2hlbmQpClNUT19hUExaUG9zdD1QTFogIFBvc3RhbnNjaHJpZnQgKGFidy4pClNUT19hT3J0UG9zdD1PcnQgIFBvc3RhbnNjaHJpZnQgKGFidy4pClNUT19hUExaUG9zdE9ydFBvc3Q9UExaIC8gT3J0IChhYncuIFBvc3RhbnNjaHJpZnQpClNUT19hQmV6ZWljaG51bmc9U3RhbmRvcnQgKGFid2VpY2hlbmQpCkFCU19Jbml0aWFsZW49S8O8cnplbCBWZXJmYXNzZXIgLyBBYnNlbmRlcgpTVE9fV2ViU2l0ZT1NRFItV2Vic2VpdGUKQUJTX01pdGFyYmVpdGVyU3RhdHVzPU1pdGFyYmVpdGVyLVN0YXR1cwpPUkRFUl9BQlNfZU1haWw9NwpPUkRFUl9BQlNfRnVua3Rpb249MApPUkRFUl9BQlNfVGVsZWZvbj00Ck9SREVSX0FCU19IYW5keT02Ck9SREVSX0FCU19GYXg9NQpPUkRFUl9BQlNfQmVyZWljaDE9OApPUkRFUl9BQlNfQmVyZWljaDI9OQpPUkRFUl9BQlNfR3J1w59mb3JtZWw9MTAKT1JERVJfQUJTX1VudGVyc2NocmlmdDE9MTEKT1JERVJfQUJTX1VudGVyc2NocmlmdEZ1bmt0aW9uMT0xMgpPUkRFUl9BQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDE9MTMKT1JERVJfQUJTX1VudGVyc2NocmlmdDI9MTQKT1JERVJfQUJTX1VudGVyc2NocmlmdEZ1bmt0aW9uMj0xNQpPUkRFUl9BQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDI9MTYKT1JERVJfU1RPX0JlemVpY2hudW5nMT0xNwpPUkRFUl9TVE9fQmV6ZWljaG51bmcyPTE4Ck9SREVSX1NUT19PcnRIYXVzPTI2Ck9SREVSX1NUT19TdHJhc3NlPTE5Ck9SREVSX1NUT19UZWxlZm9uPTIyCk9SREVSX1NUT19GYXg9MjMKT1JERVJfU1RPX2VNYWlsPTI0Ck9SREVSX0FCU19UaXRlbFZvcm5hbWVOYWNobmFtZT0xCk9SREVSX1NUT19QTFpIYXVzT3J0SGF1cz0yMApPUkRFUl9TVE9fUExaUG9zdE9ydFBvc3Q9MzAKT1JERVJfU1RPX2FTdHJhc3NlPTI4Ck9SREVSX1NUT19hUExaSGF1cz0yOQpPUkRFUl9TVE9fYU9ydEhhdXM9MzAKT1JERVJfU1RPX2FQTFpIYXVzT3J0SGF1cz0zMQpPUkRFUl9TVE9fYVBMWlBvc3Q9MzIKT1JERVJfU1RPX2FPcnRQb3N0PTMzCk9SREVSX1NUT19hUExaUG9zdE9ydFBvc3Q9MzQKT1JERVJfU1RPX2FCZXplaWNobnVuZz0yNwpPUkRFUl9BQlNfSW5pdGlhbGVuPTMKT1JERVJfU1RPX1dlYlNpdGU9MjUKT1JERVJfQUJTX01pdGFyYmVpdGVyU3RhdHVzPTIKCltBYnNlbmRlcmluZm9ybWF0aW9uXzFdCkFCU19Mb2dpbj1vcnRlZ2FrCkFCU19Qcm9maWxOYW1lPUV4dGVybmUgVGVzdApBQlNfQW5yZWRlPUhlcnIKQUJTX1RpdGVsPURyLgpBQlNfVm9ybmFtZT1PcnRlZ2EKQUJTX05hY2huYW1lPUtldmluCkFCU19UaXRlbFZvcm5hbWVOYWNobmFtZT1Eci4gT3J0ZWdhIEtldmluCkFCU19UZWxlZm9uPSs0OSAzNDEgMzU1OTI3MzkKQUJTX0ZheD0rNDkgMzQxIDM1NTkyNzM5CkFCU19IYW5keT0rNDkgMzQxIDM1NTkyNzM5CkFCU19lTWFpbD1rZXZpbi5vcnRlZ2FAa2JzLWxlaXB6aWcuZGUKQUJTX0Z1bmt0aW9uPXN0ZWxsdi4gSMO2cmZ1bmtkaXJla3RvcgpBQlNfQmVyZWljaDE9TGVpcHppZwpBQlNfQmVyZWljaDI9SW5mb3JtYXRpbGsKQUJTX0dydcOfZm9ybWVsPU1pdCBmcmV1bmRsaWNoZW4gR3LDvMOfZW4KQUJTX0luaXRpYWxlbj0KQUJTX1VudGVyc2NocmlmdDE9RHIuIERldGxlZiBSZW50c2NoCkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjE9VGVzdCAxCkFCU19VbnRlcnNjaHJpZnRXZXJ0aWdrZWl0MT1wcGEuCkFCU19VbnRlcnNjaHJpZnQyPQpBQlNfVW50ZXJzY2hyaWZ0RnVua3Rpb24yPVJlY2h0cwpBQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDI9aS4gQS4KU1RPX0JlemVpY2hudW5nMT1CRVRSSUVCU0RJUkVLVElPTgpTVE9fQmV6ZWljaG51bmcyPQpTVE9fU3RyYXNzZT1LYW50c3RyYcOfZSA3MS03MwpTVE9fUExaSGF1cz0wNDI3NQpTVE9fT3J0SGF1cz1FcmZ1cnQKU1RPX1BMWkhhdXNPcnRIYXVzPTA0Mjc1IExlaXB6aWcKU1RPX1BMWlBvc3Q9MDQzNjAKU1RPX09ydFBvc3Q9TGVpcHppZwpTVE9fUExaUG9zdE9ydFBvc3Q9MDQzNjAgTGVpcHppZwpTVE9fVGVsZWZvbj0oMDM0MSkgMyAwMCAwClNUT19GYXg9ClNUT19lTWFpbD0KU1RPX1dlYlNpdGU9d3d3Lm1kci5kZQpTVE9fYVN0cmFzc2U9R290aGFlciBTdHJhw59lIDM2ClNUT19hUExaSGF1cz05OTA5NApTVE9fYU9ydEhhdXM9RXJmdXJ0ClNUT19hUExaSGF1c09ydEhhdXM9OTkwOTQgRXJmdXJ0ClNUT19hQmV6ZWljaG51bmc9ClNUT19hUExaUG9zdD0KU1RPX2FPcnRQb3N0PQpTVE9fYVBMWlBvc3RPcnRQb3N0PQpTREZfSUQ9ClNERl9CZXplaWNobnVuZz0KU0RGX0Jlc2NocmVpYnVuZzE9ClNERl9CZXNjaHJlaWJ1bmcyPQpEU1JfU3RhbmRhcmRhZHJlc3NlPVRydWUKQUJTX01pdGFyYmVpdGVyU3RhdHVzPUZyZWllciBNaXRhcmJlaXRlcgoKW0Fic2VuZGVyaW5mb3JtYXRpb25fMl0KQUJTX0xvZ2luPW9ydGVnYWsKQUJTX1Byb2ZpbE5hbWU9UmVudHNjaApBQlNfQW5yZWRlPUhlcnIKQUJTX1RpdGVsPURpcGwuCkFCU19Wb3JuYW1lPU9ydGVnYQpBQlNfTmFjaG5hbWU9S2V2aW4KQUJTX1RpdGVsVm9ybmFtZU5hY2huYW1lPURpcGwuIE9ydGVnYSBLZXZpbgpBQlNfVGVsZWZvbj0rNDkgMzQxIDM1NTkyNzM5CkFCU19GYXg9KzQ5IDM0MSAzNTU5MjczOQpBQlNfSGFuZHk9KzQ5IDM0MSAzNTU5MjczOQpBQlNfZU1haWw9a2V2aW4ub3J0ZWdhQGticy1sZWlwemlnLmRlCkFCU19GdW5rdGlvbj1zdGVsbHYuIEjDtnJmdW5rZGlyZWt0b3IKQUJTX0JlcmVpY2gxPUxlaXB6aWcKQUJTX0JlcmVpY2gyPUluZm9ybWF0aWxrCkFCU19HcnXDn2Zvcm1lbD1NaXQgZnJldW5kbGljaGVuIEdyw7zDn2VuCkFCU19Jbml0aWFsZW49TXVzdGVyCkFCU19VbnRlcnNjaHJpZnQxPURyLiBEZXRsZWYgUmVudHNjaApBQlNfVW50ZXJzY2hyaWZ0RnVua3Rpb24xPVRlc3QgMQpBQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDE9aS4gQS4KQUJTX1VudGVyc2NocmlmdDI9CkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjI9UmVjaHRzCkFCU19VbnRlcnNjaHJpZnRXZXJ0aWdrZWl0Mj1pLiBBLgpTVE9fQmV6ZWljaG51bmcxPUpVUklTVElTQ0hFIERJUkVLVElPTgpTVE9fQmV6ZWljaG51bmcyPQpTVE9fU3RyYXNzZT1LYW50c3RyYcOfZSA3MS03MwpTVE9fUExaSGF1cz0wNDI3NQpTVE9fT3J0SGF1cz1MZWlwemlnClNUT19QTFpIYXVzT3J0SGF1cz0wNDI3NSBMZWlwemlnClNUT19QTFpQb3N0PTA0MzYwClNUT19PcnRQb3N0PUxlaXB6aWcKU1RPX1BMWlBvc3RPcnRQb3N0PTA0MzYwIExlaXB6aWcKU1RPX1RlbGVmb249KDAzNDEpIDMgMDAgMApTVE9fRmF4PQpTVE9fZU1haWw9ClNUT19XZWJTaXRlPXd3dy5tZHIuZGUKU1RPX2FTdHJhc3NlPVJpY2h0ZXJzdHJhc3NlIDcKU1RPX2FQTFpIYXVzPTA0MTA1ClNUT19hT3J0SGF1cz1MZWlwemlnClNUT19hUExaSGF1c09ydEhhdXM9MDQxMDUgTGVpcHppZwpTVE9fYUJlemVpY2hudW5nPQpTVE9fYVBMWlBvc3Q9MDQzNjAKU1RPX2FPcnRQb3N0PUxlaXB6aWcKU1RPX2FQTFpQb3N0T3J0UG9zdD0wNDM2MCBMZWlwemlnClNERl9JRD0KU0RGX0JlemVpY2hudW5nPQpTREZfQmVzY2hyZWlidW5nMT0KU0RGX0Jlc2NocmVpYnVuZzI9CkRTUl9TdGFuZGFyZGFkcmVzc2U9VHJ1ZQpBQlNfTWl0YXJiZWl0ZXJTdGF0dXM9CgpbQWJzZW5kZXJpbmZvcm1hdGlvbl8zXQpBQlNfTG9naW49b3J0ZWdhawpBQlNfUHJvZmlsTmFtZT11ZmZmCkFCU19BbnJlZGU9SGVycgpBQlNfVGl0ZWw9RHIuCkFCU19Wb3JuYW1lPU9ydGVnYQpBQlNfTmFjaG5hbWU9S2V2aW4KQUJTX1RpdGVsVm9ybmFtZU5hY2huYW1lPURyLiBPcnRlZ2EgS2V2aW4KQUJTX1RlbGVmb249KzQ5IDM0MSAzNTU5MjczOQpBQlNfRmF4PSs0OSAzNDEgMzU1OTI3MzkKQUJTX0hhbmR5PSs0OSAzNDEgMzU1OTI3MzkKQUJTX2VNYWlsPWtldmluLm9ydGVnYUBrYnMtbGVpcHppZy5kZQpBQlNfRnVua3Rpb249c3RlbGx2LiBIw7ZyZnVua2RpcmVrdG9yCkFCU19CZXJlaWNoMT1MZWlwemlnCkFCU19CZXJlaWNoMj1JbmZvcm1hdGlsawpBQlNfR3J1w59mb3JtZWw9TWl0IGZyZXVuZGxpY2hlbiBHcsO8w59lbgpBQlNfSW5pdGlhbGVuPQpBQlNfVW50ZXJzY2hyaWZ0MT1Eci4gRGV0bGVmIFJlbnRzY2gKQUJTX1VudGVyc2NocmlmdEZ1bmt0aW9uMT1UZXN0IDEKQUJTX1VudGVyc2NocmlmdFdlcnRpZ2tlaXQxPXBwYS4KQUJTX1VudGVyc2NocmlmdDI9CkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjI9UmVjaHRzCkFCU19VbnRlcnNjaHJpZnRXZXJ0aWdrZWl0Mj1pLiBBLgpTVE9fQmV6ZWljaG51bmcxPUJFVFJJRUJTRElSRUtUSU9OClNUT19CZXplaWNobnVuZzI9ClNUT19TdHJhc3NlPUthbnRzdHJhw59lIDcxLTczClNUT19QTFpIYXVzPTA0Mjc1ClNUT19PcnRIYXVzPUVyZnVydApTVE9fUExaSGF1c09ydEhhdXM9MDQyNzUgTGVpcHppZwpTVE9fUExaUG9zdD0wNDM2MApTVE9fT3J0UG9zdD1MZWlwemlnClNUT19QTFpQb3N0T3J0UG9zdD0wNDM2MCBMZWlwemlnClNUT19UZWxlZm9uPSgwMzQxKSAzIDAwIDAKU1RPX0ZheD0KU1RPX2VNYWlsPQpTVE9fV2ViU2l0ZT13d3cubWRyLmRlClNUT19hU3RyYXNzZT1Hb3RoYWVyIFN0cmHDn2UgMzYKU1RPX2FQTFpIYXVzPTk5MDk0ClNUT19hT3J0SGF1cz1FcmZ1cnQKU1RPX2FQTFpIYXVzT3J0SGF1cz05OTA5NCBFcmZ1cnQKU1RPX2FCZXplaWNobnVuZz0KU1RPX2FQTFpQb3N0PQpTVE9fYU9ydFBvc3Q9ClNUT19hUExaUG9zdE9ydFBvc3Q9ClNERl9JRD0KU0RGX0JlemVpY2hudW5nPQpTREZfQmVzY2hyZWlidW5nMT0KU0RGX0Jlc2NocmVpYnVuZzI9CkRTUl9TdGFuZGFyZGFkcmVzc2U9VHJ1ZQpBQlNfTWl0YXJiZWl0ZXJTdGF0dXM9CgpbQWJzZW5kZXJpbmZvcm1hdGlvbl80XQpBQlNfTG9naW49b3J0ZWdhawpBQlNfUHJvZmlsTmFtZT1WYXJpYW50ZSAxMQpBQlNfQW5yZWRlPUhlcnIKQUJTX1RpdGVsPURyLgpBQlNfVm9ybmFtZT1PcnRlZ2EKQUJTX05hY2huYW1lPUtldmluCkFCU19UaXRlbFZvcm5hbWVOYWNobmFtZT1Eci4gT3J0ZWdhIEtldmluCkFCU19UZWxlZm9uPSs0OSAzNDEgMzU1OTI3MzkKQUJTX0ZheD0rNDkgMzQxIDM1NTkyNzM5CkFCU19IYW5keT0rNDkgMzQxIDM1NTkyNzM5CkFCU19lTWFpbD1rZXZpbi5vcnRlZ2FAa2JzLWxlaXB6aWcuZGUKQUJTX0Z1bmt0aW9uPXN0ZWxsdi4gSMO2cmZ1bmtkaXJla3RvcgpBQlNfQmVyZWljaDE9TGVpcHppZwpBQlNfQmVyZWljaDI9SW5mb3JtYXRpbGsKQUJTX0dydcOfZm9ybWVsPU1pdCBmcmV1bmRsaWNoZW4gR3LDvMOfZW4KQUJTX0luaXRpYWxlbj0KQUJTX1VudGVyc2NocmlmdDE9RHIuIERldGxlZiBSZW50c2NoCkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjE9VGVzdCAxCkFCU19VbnRlcnNjaHJpZnRXZXJ0aWdrZWl0MT1wcGEuCkFCU19VbnRlcnNjaHJpZnQyPQpBQlNfVW50ZXJzY2hyaWZ0RnVua3Rpb24yPVJlY2h0cwpBQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDI9aS4gQS4KU1RPX0JlemVpY2hudW5nMT1CRVRSSUVCU0RJUkVLVElPTgpTVE9fQmV6ZWljaG51bmcyPQpTVE9fU3RyYXNzZT1LYW50c3RyYcOfZSA3MS03MwpTVE9fUExaSGF1cz0wNDI3NQpTVE9fT3J0SGF1cz1NYWdkZWJ1cmcKU1RPX1BMWkhhdXNPcnRIYXVzPTA0Mjc1IExlaXB6aWcKU1RPX1BMWlBvc3Q9MDQzNjAKU1RPX09ydFBvc3Q9TGVpcHppZwpTVE9fUExaUG9zdE9ydFBvc3Q9MDQzNjAgTGVpcHppZwpTVE9fVGVsZWZvbj0oMDM0MSkgMyAwMCAwClNUT19GYXg9ClNUT19lTWFpbD0KU1RPX1dlYlNpdGU9d3d3Lm1kci5kZQpTVE9fYVN0cmFzc2U9U3RhZHRwYXJrc3RyYcOfZSA4ClNUT19hUExaSGF1cz0zOTExNApTVE9fYU9ydEhhdXM9TWFnZGVidXJnClNUT19hUExaSGF1c09ydEhhdXM9MzkxMTQgTWFnZGVidXJnClNUT19hQmV6ZWljaG51bmc9ClNUT19hUExaUG9zdD0KU1RPX2FPcnRQb3N0PQpTVE9fYVBMWlBvc3RPcnRQb3N0PQpTREZfSUQ9ClNERl9CZXplaWNobnVuZz0KU0RGX0Jlc2NocmVpYnVuZzE9ClNERl9CZXNjaHJlaWJ1bmcyPQpEU1JfU3RhbmRhcmRhZHJlc3NlPVRydWUKQUJTX01pdGFyYmVpdGVyU3RhdHVzPQoKW0Fic2VuZGVyaW5mb3JtYXRpb25fNV0KQUJTX0xvZ2luPW9ydGVnYWsKQUJTX1Byb2ZpbE5hbWU9VmFyaWFudGUgNQpBQlNfQW5yZWRlPUhlcnIKQUJTX1RpdGVsPURyLgpBQlNfVm9ybmFtZT1PcnRlZ2EKQUJTX05hY2huYW1lPUtldmluCkFCU19UaXRlbFZvcm5hbWVOYWNobmFtZT1Eci4gT3J0ZWdhIEtldmluCkFCU19UZWxlZm9uPSs0OSAzNDEgMzU1OTI3MzkKQUJTX0ZheD0rNDkgMzQxIDM1NTkyNzM5CkFCU19IYW5keT0rNDkgMzQxIDM1NTkyNzM5CkFCU19lTWFpbD1rZXZpbi5vcnRlZ2FAa2JzLWxlaXB6aWcuZGUKQUJTX0Z1bmt0aW9uPXN0ZWxsdi4gSMO2cmZ1bmtkaXJla3RvcgpBQlNfQmVyZWljaDE9TGVpcHppZwpBQlNfQmVyZWljaDI9SW5mb3JtYXRpbGsKQUJTX0dydcOfZm9ybWVsPU1pdCBmcmV1bmRsaWNoZW4gR3LDvMOfZW4KQUJTX0luaXRpYWxlbj0KQUJTX1VudGVyc2NocmlmdDE9RHIuIERldGxlZiBSZW50c2NoCkFCU19VbnRlcnNjaHJpZnRGdW5rdGlvbjE9VGVzdCAxCkFCU19VbnRlcnNjaHJpZnRXZXJ0aWdrZWl0MT1wcGEuCkFCU19VbnRlcnNjaHJpZnQyPQpBQlNfVW50ZXJzY2hyaWZ0RnVua3Rpb24yPVJlY2h0cwpBQlNfVW50ZXJzY2hyaWZ0V2VydGlna2VpdDI9aS4gQS4KU1RPX0JlemVpY2hudW5nMT1CRVRSSUVCU0RJUkVLVElPTgpTVE9fQmV6ZWljaG51bmcyPQpTVE9fU3RyYXNzZT1LYW50c3RyYcOfZSA3MS03MwpTVE9fUExaSGF1cz0wNDI3NQpTVE9fT3J0SGF1cz1FcmZ1cnQKU1RPX1BMWkhhdXNPcnRIYXVzPTA0Mjc1IExlaXB6aWcKU1RPX1BMWlBvc3Q9MDQzNjAKU1RPX09ydFBvc3Q9TGVpcHppZwpTVE9fUExaUG9zdE9ydFBvc3Q9MDQzNjAgTGVpcHppZwpTVE9fVGVsZWZvbj0oMDM0MSkgMyAwMCAwClNUT19GYXg9ClNUT19lTWFpbD0KU1RPX1dlYlNpdGU9d3d3Lm1kci5kZQpTVE9fYVN0cmFzc2U9R290aGFlciBTdHJhw59lIDM2ClNUT19hUExaSGF1cz05OTA5NApTVE9fYU9ydEhhdXM9RXJmdXJ0ClNUT19hUExaSGF1c09ydEhhdXM9OTkwOTQgRXJmdXJ0ClNUT19hQmV6ZWljaG51bmc9ClNUT19hUExaUG9zdD0KU1RPX2FPcnRQb3N0PQpTVE9fYVBMWlBvc3RPcnRQb3N0PQpTREZfSUQ9ClNERl9CZXplaWNobnVuZz0KU0RGX0Jlc2NocmVpYnVuZzE9ClNERl9CZXNjaHJlaWJ1bmcyPQpEU1JfU3RhbmRhcmRhZHJlc3NlPVRydWUKQUJTX01pdGFyYmVpdGVyU3RhdHVzPQoKCg==";
						if (urlExpresion.IsMatch(args[0]))
						{
							var match = urlExpresion.Match(args[0]);
							var url = new Url(match.Groups[0].ToString(),//url complet
												match.Groups[1].ToString(),//protocol
												match.Groups[2].ToString(),//domain
												match.Groups[3].ToString(),//port
												match.Groups[4].ToString(),//path would be the email description
																					//parameter);//parameters would be the files to open or send
												match.Groups[5].ToString());//parameters would be the files to open or send

							//analisys of the domain
							var action = url.Domain.Split('.');

							if (action.First() == "viewfile") ViewFile(url);
							//else if (action.First() == "sendfile") SendFile(url);
							else if (action.First() == "sendfile") CreateNewEmail(url);
							else if (action.First() == "createfile") CreateTextFile(url);

						}
						else
						{
							DialogResult dialog = MessageBox.Show("Die URL ist fehlerhaft");
							//Console.WriteLine("Die URL ist fehlerhaft");
						}
					}
					catch (Exception ex)
					{
						//Console.WriteLine("Fehler: {0}", ex);
						//Console.WriteLine("Passen Sie auf!. Müssen Sie eine URL eingeben. Z.b: UrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>");
						DialogResult dialog = MessageBox.Show("Passen Sie auf!. Müssen Sie eine URL eingeben. Z.b: UrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>" + ex.Message);
					}

				}
				else
				{
					DialogResult dialog = MessageBox.Show("Bitte wählen Sie entweder Option [-i] zum Instalieren oder [-u] zum Löschen der App oder Das URL Protocol:\nUrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles> aqui");
					/*Console.WriteLine("Bitte wählen Sie entweder Option [-i] zum Instalieren oder [-u] zum Löschen der App oder Das URL Protocol: \n UrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>");
					Console.WriteLine("Das Fenster wird in 5 Sekunden automatisch geschlossen werden");
					Thread.Sleep(5000);*/
					//Console.WriteLine("Drücken Sie Enter, um das Fenster zu schließen");
				}
			}
			catch (Exception ex)
			{
				DialogResult dialog = MessageBox.Show("Bitte wählen Sie entweder Option [-i] zum Installieren oder [-u] zum Löschen der App oder Das URL Protocol:\nUrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>" + ex.Message);
				//Console.WriteLine("Bitte wählen Sie entweder Option [-i] zum Installieren oder [-u] zum Löschen der App oder Das URL Protocol: \n UrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>");
				//Console.WriteLine("Das Fenster wird in 5 Sekunden automatisch geschlossen werden");
				//Thread.Sleep(5000);
			}

		}

		public static void Uninstall(string pathApp)
		{
			// If directory does not exist, don't even try   
			if (Directory.Exists(pathApp))
			{
				Directory.Delete(pathApp, true);
			}

			if (Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true).GetSubKeyNames().Contains(subKey))
			{
				Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree(subKey, true);
			}

			/*if (Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).GetSubKeyNames().Contains(subKey))
		{
			 Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree(subKey, true);
		}*/

		}

		/// <summary>
		/// send File with the current user function
		/// </summary>
		/// <param name="url"></param>
		public static void CreateNewEmail(Url url)
		{
			// Create the Outlook application.
			// in-line initialization
			Outlook.Application Application = new Outlook.Application();

			try
			{
				List<string> attachments = new List<string>();
				List<string> adressesTo = new List<string>();
				List<string> adressesCC = new List<string>();
				List<string> adressesBCC = new List<string>();

				//foreach file in the Url
				foreach (var att in url.QueryDecodedPath)
				{
					attachments.Add(att);
				}

				//get only the emails that are encoded (To, CC and BCC)
				var partsOfEmail = url.Path.Split('/');

				if (!string.IsNullOrEmpty(partsOfEmail[0]))
				{
					foreach (var adresse in partsOfEmail[0].Split(';').ToList())
					{
						adressesTo.Add(adresse);
					}
				}

				if (!string.IsNullOrEmpty(partsOfEmail[1]))
				{
					foreach (var adresse in partsOfEmail[1].Split(';').ToList())
					{
						adressesCC.Add(adresse);
					}
				}

				if (!string.IsNullOrEmpty(partsOfEmail[2]))
				{
					foreach (var adresse in partsOfEmail[2].Split(';').ToList())
					{
						adressesBCC.Add(adresse);
					}
				}

				var subject = partsOfEmail[3];
				var body = partsOfEmail[4];
				var signature = ReadSignature();

				var email = new Email(adressesTo, adressesCC, adressesBCC, subject, body, attachments, signature);

				SendEmailWithAttachments(Application, email);
				Application.Quit();

			}//Error handler.
			catch (Exception e)
			{
				Application.Quit();
				Console.WriteLine("{0} ", e);
				//Console.WriteLine("Error Handler");

			}
		}

		/// <summary>
		/// send File without the current user function
		/// </summary>
		/// <param name="url"></param>

		/*public static void SendFile(Url url)
		{
			 try
			 {
				  // Create the Outlook application.
				  // in-line initialization
				  Outlook.Application Application = new Outlook.Application();
				  List<string> attachments = new List<string>();
				  List<string> adresses = new List<string>();

				  //foreach file path in the Url
				  foreach (var att in url.QueryDecodedPath)
				  {
						attachments.Add(att);
				  }
				  //get only the emails that are encoded 
				  var partsOfEmail = url.Path.Split('/');
				  foreach(var adresse in partsOfEmail[0].Split(';').ToList())
				  {
						adresses.Add(adresse);
				  }

				  var subject = partsOfEmail[1];
				  var body = partsOfEmail[2];
				  //List<string> ass = new List<string>();
				  //ass.Add("/o=ExchangeLabs/ou=Exchange Administrative Group (FYDIBOHF23SPDLT)/cn=Recipients/cn=0ed05cd207c542b1a7bd0a9d97b9a752-Kevin Orteg");
				  //var email = new Email(ass, subject, body, attachments);
				  var email = new Email(adresses, subject, body, attachments);
				  //SendEmailWithAttachments(Application, "Test: Wenn das geht, sag mir bescheid bitte!", "Wenn das geht, sag mir bescheid bitte!", recipients, "kevin.ortega@keor-leipzig.de", attachments);
				  //SendEmailWithAttachments(Application, email.Subject, email.Body, email.EmailAdresse, "kevin.ortega@keor-leipzig.de", email.Attachments);
				  SendEmailWithAttachments(Application, email.Subject, email.Body, email.EmailAdresseDecoded, null, email.Attachments);
				  Application.Quit();
			 }//Error handler.
			 catch (Exception e)
			 {
				  Console.WriteLine("{0} Exception caught: ", e);
			 }
		}*/

		/// <summary>
		/// send File with the current user function
		/// </summary>
		/// <param name="url"></param>
		private static void SendEmailWithAttachments(Outlook.Application application, Email email)
		{

			// Create a new MailItem and set the To, Subject, and Body properties.
			var newMail = application.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;

			Outlook.Recipient recipTo;
			Outlook.Recipient recipCc;
			Outlook.Recipient recipBcc;

			// Set up all the recipients To.
			foreach (var to in email.EmailAdresseToDecoded)
			{
				recipTo = newMail.Recipients.Add(to);
				recipTo.Type = (int)Outlook.OlMailRecipientType.olTo;
			}

			foreach (var cc in email.EmailAdresseCcDecoded)
			{
				recipCc = newMail.Recipients.Add(cc);
				recipCc.Type = (int)Outlook.OlMailRecipientType.olCC;
			}

			foreach (var bcc in email.EmailAdresseBccDecoded)
			{
				recipBcc = newMail.Recipients.Add(bcc);
				recipBcc.Type = (int)Outlook.OlMailRecipientType.olBCC;
			}

			// Set up all the recipients.
			/*foreach (var recipient in recipients)
			{
				 newMail.Recipients.Add(recipient);
			}*/

			if (newMail.Recipients.ResolveAll())
			{
				newMail.Subject = email.Subject;
				newMail.Body = email.Body;
				foreach (string attachment in email.Attachments)
				{
					newMail.Attachments.Add(attachment, Outlook.OlAttachmentType.olByValue);
				}
				//then read the signature of a the e-mail
				string signature = email.Signature;
				newMail.HTMLBody = signature;
			}

			// Retrieve the account that has the specific SMTP address.
			var currentUser = application.Session.CurrentUser;
			var smtpAddress = currentUser.AddressEntry.GetExchangeUser().PrimarySmtpAddress;
			Outlook.Account account = GetAccountForEmailAddress(application, smtpAddress);


			// Use this account to send the e-mail.
			newMail.SendUsingAccount = account;
			newMail.Send();
		}

		/// <summary>
		/// This function read the signature of a Outlook account
		/// </summary>
		/// <returns></returns>
		private static string ReadSignature()
		{
			string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Signatures";
			string signature = string.Empty;
			DirectoryInfo diInfo = new DirectoryInfo(appDataDir);
			if (diInfo.Exists)
			{
				FileInfo[] fiSignature = diInfo.GetFiles("*.htm");

				if (fiSignature.Length > 0)
				{
					StreamReader sr = new StreamReader(fiSignature[0].FullName, Encoding.Default);
					signature = sr.ReadToEnd();
					if (!string.IsNullOrEmpty(signature))
					{
						string fileName = fiSignature[0].Name.Replace(fiSignature[0].Extension, string.Empty);
						signature = signature.Replace(fileName + "_files/", appDataDir + "/" + fileName + "_files/");
					}
				}
			}
			return signature;
		}


		/// <summary>
		/// SendEmailWithAttachments for the last version
		/// </summary>
		/// <param name="application"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="recipients"></param>
		/// <param name="smtpAddress"></param>
		/// <param name="attachments"></param>
		/*private static void SendEmailWithAttachments(Outlook.Application application, Email email ,string smtpAddress)
		{

			 // Create a new MailItem and set the To, Subject, and Body properties.
			 var newMail = application.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;

			 Outlook.Recipient recipTo;
			 Outlook.Recipient recipCc;
			 Outlook.Recipient recipBcc;

			 // Set up all the recipients To.
			 foreach (var to in email.EmailAdresseToDecoded)
			 {
				  recipTo = newMail.Recipients.Add(to);
				  recipTo.Type = (int)Outlook.OlMailRecipientType.olTo;
			 }

			 foreach (var cc in email.EmailAdresseCcDecoded)
			 {
				  recipCc = newMail.Recipients.Add(cc);
				  recipCc.Type = (int)Outlook.OlMailRecipientType.olCC;
			 }

			 foreach (var bcc in email.EmailAdresseBccDecoded)
			 {
				  recipBcc = newMail.Recipients.Add(bcc);
				  recipBcc.Type = (int)Outlook.OlMailRecipientType.olBCC;
			 }


			 if (newMail.Recipients.ResolveAll())
			 {
				  newMail.Subject = email.Subject;
				  newMail.Body = email.Body;
				  foreach (string attachment in email.Attachments)
				  {
						newMail.Attachments.Add(attachment, Outlook.OlAttachmentType.olByValue);
				  }
			 }

			 // Retrieve the account that has the specific SMTP address.
			 Outlook.Account account = GetAccountForEmailAddress(application, smtpAddress);
			 //Outlook.Account account = application.Session.CurrentUser.
			 // Use this account to send the e-mail.
			 newMail.SendUsingAccount = account;
			 newMail.Send();
		}*/

		private static Outlook.Account GetAccountForEmailAddress(Outlook.Application application, string smtpAddress)
		{

			// Loop over the Accounts collection of the current Outlook session.
			Outlook.Accounts accounts = application.Session.Accounts;
			foreach (Outlook.Account account in accounts)
			{
				//When the email address matches, then the result is 0 => account.SmtpAddress == smtpAddress
				var result = String.Compare(account.SmtpAddress, smtpAddress, StringComparison.OrdinalIgnoreCase);
				// When the email address matches, return the account.
				if (result.Equals(0))
				{
					return account;
				}
				//return account;
			}
			// If you get here, no matching account was found.
			throw new System.Exception(string.Format("No Account with SmtpAddress: {0} exists!",
				 smtpAddress));
		}

		public static void ViewFile(Url url)
		{
			foreach (var queryDecodedPath in url.QueryDecodedPath)
			{
				//if file exist and the extension is permited
				//if (IsExtensionPermit(queryDecodedPath) && FileExist(queryDecodedPath))
				Console.WriteLine("QueryDecodedPath   " + url.QueryDecodedPath[0]);
				Console.WriteLine("QueryEncodedPath   " + url.QueryEncodedPath[0]);
				if (FileExist(queryDecodedPath))
				{
					Console.WriteLine("Startvorgang");
					if (Path.GetExtension(queryDecodedPath) != ".txt")
					{
						//for a default app, run this line
						Process.Start(queryDecodedPath);
					}
					else
					{
						Process.Start(queryDecodedPath);
						//for a specific app, we need to give the app that we want to run, run this line
						/*var psi = new ProcessStartInfo(queryDecodedPath)
			 {
				  Arguments = Path.GetFileName(queryDecodedPath),
				  UseShellExecute = false,//with visual environment we need to set up in false
				  WorkingDirectory = Path.GetDirectoryName(queryDecodedPath),
				  FileName = @"C:\Program Files (x86)\Notepad++\notepad++.exe",
				  Verb = "OPEN"
			 };
			 Process.Start(psi);*/
					}
				}
			}

		}

		/// <summary>
		/// set the directories where the program have to run into a machine
		/// if all the process run gut then return true, if not return false
		/// </summary>
		public static bool Configuration(string directory)
		{
			try
			{
				//Gets the current location where the file is downloaded
				var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
				string argumentRegister = String.Format(" %1");
				string register = Path.Combine(directory, loc.Split('\\').Last()) + argumentRegister;

				if (!Directory.Exists(directory))
				{
					System.IO.Directory.CreateDirectory(directory);
					Console.WriteLine("Verzeichnis wurde im '{0}' erstellt", directory);
				}
				//Creates the file in the specified folder
				if (!File.Exists(Path.Combine(directory, loc.Split('\\').Last())))
				{

					//File.Copy(loc, directory + loc.Split('\\').Last());
					File.Copy(loc, Path.Combine(directory, loc.Split('\\').Last()));
					var path = Path.GetDirectoryName(loc);
					Console.WriteLine("Programm wurde im '{0}'\'{1}' erstellt", directory, loc.Split('\\').Last());

					//create the register for the app, if this not exist
					if (!IsRegistered(register))
					{
						Console.WriteLine("Programm wurde erfolgreich registriert");
						return false;
					}

					//return false and close if the program was recently installed
					//return false;
				}//if the file was there, then msg that it rigth to use
				else
				{
					Console.WriteLine("Programm ist bereit vorhanden");
					Console.WriteLine("Müssen Sie eine URL eingeben.Z.b: UrlProtocol://<viewfile><sendfile>.keor/<email>?attachFiles=<pathFiles>");
				}

				if (!IsRegistered(register))
				{
					Console.WriteLine("Programm wurde erfolgreich registriert");
					return false;
				}
				//return true to continue with the program
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Fehler in Funktion Configuration(): {0}", ex);
				throw ex;
			}

		}

		/// <summary>
		/// if the program was registered then return true, if not it register the program and return false
		/// </summary>
		/// <returns></returns>
		public static bool IsRegistered(string register)
		{
            //string register = @"H:\Dokumente\keor\keor.exe %1";
            //string subKey = "keor";
            //var subKey = ConfigurationManager.AppSettings["SubKey"] ?? "not found";
            //var register = ConfigurationManager.AppSettings["RegisterApp"] ?? "not found";
            //var regkeyValue = ConfigurationManager.AppSettings["regkeySetValue"] ?? "not found";
            bool flag = true;

			if (!Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true).GetSubKeyNames().Contains(subKey))
			{
				//the keys for a current user must be in software/classes because then the root can take this key and recognize them
				var key = Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true);
				RegistryKey regkey = key.CreateSubKey(subKey);
				regkey.CreateSubKey(@"DefaultIcon").SetValue("", register);
				//regkey.SetValue("URL Protocol", regkeyValue); 
				regkey.SetValue("URL Protocol", "URL:AppStarter Protocol");
				regkey.CreateSubKey(@"shell\open\command").SetValue("", register);

                flag = false;
			}

            /*
            if (!Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Internet Explorer", true).OpenSubKey("ProtocolExecute", true).GetSubKeyNames().Contains("AppStarter"))
            {
                //the keys for a current user must be in software/classes because then the root can take this key and recognize them
                var key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Internet Explorer", true).OpenSubKey("ProtocolExecute", true);


                RegistryKey regkey = key.CreateSubKey("AppStarter");
                //regkey.CreateSubKey(@"DefaultIcon").SetValue("", register);
                //regkey.SetValue("URL Protocol", regkeyValue); 
                regkey.SetValue("WarnOnOpen", "dword:00000000");
                //regkey.CreateSubKey(@"shell\open\command").SetValue("", register);

                flag = false;
            }

            if (!Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Internet Explorer", true).OpenSubKey("ProtocolExecute", true).GetSubKeyNames().Contains("mailto"))
            {
                //the keys for a current user must be in software/classes because then the root can take this key and recognize them
                var key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Internet Explorer", true).OpenSubKey("ProtocolExecute", true);


                RegistryKey regkey = key.CreateSubKey("mailto");
                //regkey.CreateSubKey(@"DefaultIcon").SetValue("", register);
                //regkey.SetValue("URL Protocol", regkeyValue); 
                regkey.SetValue("WarnOnOpen", "dword:00000000");
                //regkey.CreateSubKey(@"shell\open\command").SetValue("", register);

                flag = false;
            }
            */
            //string regAppStarter = "AppStarter.reg";
            //string regMailTo = "mailto.reg";
            string directory = @"E:\Projects\Desktop\bin\registerEdge.bat";
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.FileName = "regedit.exe";
            //startInfo.Arguments = "/s " + directory;
            //startInfo.Arguments =  directory;
            Process proc = Process.Start("E:\\Projects\\Desktop\\bin\\registerEdge.bat");
            //Process.Start(directory);
            //string directory = @"E:\Projects\Desktop\bin\AppStarter.reg";
            //string directory = System.Reflection.Assembly.GetExecutingAssembly().Location+ @"\AppStarter.reg"; 
            //Process.Start("regedit.exe", "/s E:\\Projects\\Desktop\\bin\\AppStarter.reg");
            //var executable = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\').Last();
            //var path
            return flag;
		}


		public static bool FileExist(string path)
		{
			if (File.Exists(path))
			{
				return true;
			}
			else if (IsWeb(path))
			{
				return true;
			}
			else
			{
				DialogResult dialog = MessageBox.Show("Datei wurde nicht gefunden");
				Console.WriteLine("Datei wurde nicht gefunden");
				return false;
			}
		}

		public static bool IsWeb(string path)
		{
			var pagePermit = "http,htm,html,asp,aspx,php";
			var listPagePermit = pagePermit.Split(',');
			foreach (var web in listPagePermit)
			{
				if (path.Contains(web))
				{
					return true;
				}
			}
			//if (listFilePermit.Contains(path)) return true;
			//else { return false; }
			return false;
		}

		public static bool IsExtensionPermit(string path)
		{
			try
			{
				//var filePermit = ConfigurationManager.AppSettings["FilesAllowed"] ?? "not found";
				var filePermit = ".docx,.xlsx,.pptx,.pdf,.txt,.htm,.html,.xsn,.oft,.dot,.asp,.xltm,.xlsm,.dotm,.xsn";
				var listFilePermit = filePermit.Split(',');
				if (listFilePermit.Contains(Path.GetExtension(path))) return true;
				else
				{
					Console.WriteLine("Die Dateiendung ist nicht erlaubt");
					Console.WriteLine("Bitte, versuchen Sie mit Datei als:");
					//var extension = filePermit.Split(',');
					//foreach (var f in extension)
					foreach (var f in listFilePermit)
					{
						Console.WriteLine(f);
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("IsExtensionPermit Fehler: {0}", ex);
				return false;
			}

		}

		public static bool CreateTextFile(Url url)
		{
			try
			{
				
				var directory = @"H:\Config\Vorlagen\Office\profile.ini";
				var directoryAux = @"H:\Config\Vorlagen\Office\profileAux.ini";
				
				FileInfo fi = new FileInfo(directory);
				FileInfo fiAux = new FileInfo(directoryAux);

				if (!Directory.Exists(Path.GetDirectoryName(directory)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(directory));

				}
				// Check if file already exists. If yes, delete it.

				if (fi.Exists) { fi.Delete();	}
				if (fiAux.Exists) { fiAux.Delete(); }

				//Create Request
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.QueryDecodedPath[0]);

				//Create Client
				WebClient client = new WebClient();

				//Assign Credentials
				client.UseDefaultCredentials = true;
				client.Encoding = System.Text.Encoding.Default;
                //Grab Data
                try
                {
                    string htmlCode = client.DownloadString(url.QueryDecodedPath[0]);
                    var text = htmlCode.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

                    using (TextWriter tw = new StreamWriter(directoryAux, true, Encoding.GetEncoding(1252)))
                    {
                        tw.Write(text);
                    }

                    StreamReader fileStream = new StreamReader(directoryAux);
                    string fileContent = fileStream.ReadToEnd();
                    fileStream.Close();

                    StreamWriter ansiWriter = new StreamWriter(directory, true, Encoding.GetEncoding(1252));
                    ansiWriter.Write(fileContent);
                    ansiWriter.Close();


                    return true;
                }
                catch (Exception ex)
                {

                    DialogResult dialog = MessageBox.Show("Datei in Server nicht gefunden");
                    return false;
                }
				
				//Encoding.Default

				//for PC format windows
				
			}
			catch (Exception ex)
			{
				DialogResult dialog = MessageBox.Show(ex.Message);
				Console.Write(ex);
				return false;
			}
		}
	}
}
