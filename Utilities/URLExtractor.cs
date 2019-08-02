using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace JIRASupport
{

    internal class URLExtractor
    {
        static Dictionary<string, string> Browsers= new Dictionary<string, string>()
            {
                {"firefox", "MozillaWindowClass"},
                {"chrome", "Chrome_WidgetWin_1"}
            };


        internal static string GetJIRANumberFromURL()
        {
            foreach( KeyValuePair<string,string> browser in Browsers )
            {
                Process[] procesChrome = Process.GetProcessesByName(browser.Key);

                foreach (Process proc in procesChrome)
                {
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }

                    AutomationElement elem = AutomationElement.FromHandle(proc.MainWindowHandle);

                    AutomationProperty[] properties = elem.GetSupportedProperties();

                    try
                    {
                        // for firefox the NameProperty is not reliable, not like Chrome the name remains the same
                        var elem1 = elem.FindFirst(TreeScope.Children | TreeScope.Element, new PropertyCondition(AutomationElement.ClassNameProperty, browser.Value));
                        if (elem1 == null)
                        {
                            continue;
                        }

                        string URLName = elem1.Current.Name;

                        string patten = @"\[(\D{2})-(\d{1,4})\]";
                        Regex reg = new Regex(patten);
                        Match m = reg.Match(URLName);

                        if (m.Success)
                        {
                            return m.Value.Substring(1, m.Length - 2);
                            //return m.Value; // return [ES-XXX]
                        }
                        else
                            continue;

                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return null; 
        }
    }
}
