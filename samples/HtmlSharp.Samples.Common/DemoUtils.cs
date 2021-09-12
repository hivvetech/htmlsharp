using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlSharp.Core.Entities;

namespace HtmlSharp.Samples.Common
{
    public class DemoUtils
    {
        private const int Iterations = 20;

        /// <summary>
        /// The HTML text used in sample form for HtmlLabel.
        /// </summary>
        public static string SampleHtmlLabelText
        {
            get
            {
                return "This is an <b>HtmlLabel</b> on transparent background with <span style=\"color: red\">colors</span> and links: " +
                       "<a href=\"http://htmlrenderer.codeplex.com/\">HTML Renderer</a>";
            }
        }

        /// <summary>
        /// The HTML text used in sample form for HtmlPanel.
        /// </summary>
        public static String SampleHtmlPanelText =>
            "This is an <b>HtmlPanel</b> with <span style=\"color: red\">colors</span> and links: <a href=\"http://htmlrenderer.codeplex.com/\">HTML Renderer</a>" +
            "<div style=\"font-size: 1.2em; padding-top: 10px;\" >If there is more text than the size of the control scrollbars will appear.</div>" +
            "<br/>Click me to change my <code>Text</code> property.";

        /// <summary>
        /// Handle stylesheet resolve.
        /// </summary>
        public static void OnStylesheetLoad(object sender, HtmlStylesheetLoadEventArgs e)
        {
            var stylesheet = GetStylesheet(e.Src);
            if (stylesheet != null)
                e.SetStyleSheet = stylesheet;
        }

        /// <summary>
        /// Get stylesheet by given key.
        /// </summary>
        public static string GetStylesheet(string src)
        {
            if (src == "StyleSheet")
            {
                return @"h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin-bottom: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    a:link { text-decoration: none; }
                    a:hover { text-decoration: underline; }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:10px; padding:15px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; }";
            }
            return null;
        }

        /// <summary>
        /// Get image by resource key.
        /// </summary>
        public static Stream GetImageStream(string src)
        {
            return src.ToLower() switch
            {
                "htmlicon" => Resources.Html32,
                "staricon" => Resources.Favorites32,
                "fonticon" => Resources.Font32,
                "commenticon" => Resources.Comment16,
                "imageicon" => Resources.Image32,
                "methodicon" => Resources.Method16,
                "propertyicon" => Resources.Property16,
                "eventicon" => Resources.Event16,
                _ => null
            };
        }

        public static string RunSamplesPerformanceTest(Action<String> setHtmlDelegate)
        {
            GC.Collect();

            var baseStopwatch = RunTest(setHtmlDelegate, false, out var baseMemory);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var runStopwatch = RunTest(setHtmlDelegate, true, out var runMemory);

            var memory = runMemory - baseMemory;
            var elapsedMilliseconds = runStopwatch.ElapsedMilliseconds - baseStopwatch.ElapsedMilliseconds;

            float htmlSize = SamplesLoader.ShowcaseSamples.Aggregate<HtmlSample, float>(0, (current, sample) => current + sample.Html.Length * 2);
            htmlSize = htmlSize / 1024f;

            var sampleCount = SamplesLoader.ShowcaseSamples.Count;
            var msg = string.Format("{0} HTMLs ({1:N0} KB)\r\n{2} Iterations", sampleCount, htmlSize, Iterations);
            msg += "\r\n\r\n";
            msg += string.Format("CPU:\r\nTotal: {0} msec\r\nIterationAvg: {1:N2} msec\r\nSingleAvg: {2:N2} msec",
                elapsedMilliseconds, elapsedMilliseconds / (double)Iterations, elapsedMilliseconds / (double)Iterations / sampleCount);

            if (Environment.Version.Major >= 4)
            {
                msg += "\r\n\r\n";
                msg += string.Format("Memory:\r\nTotal: {0:N0} KB\r\nIterationAvg: {1:N0} KB\r\nSingleAvg: {2:N0} KB\r\nOverhead: {3:N0}%",
                    memory, memory / Iterations, memory / Iterations / sampleCount, 100 * (memory / Iterations) / htmlSize);
            }

            msg += "\r\n\r\n\r\n";
            msg += string.Format("Full CPU:\r\nTotal: {0} msec\r\nIterationAvg: {1:N2} msec\r\nSingleAvg: {2:N2} msec",
                runStopwatch.ElapsedMilliseconds, runStopwatch.ElapsedMilliseconds / (double)Iterations, runStopwatch.ElapsedMilliseconds / (double)Iterations / sampleCount);

            if (Environment.Version.Major >= 4)
            {
                msg += "\r\n\r\n";
                msg += string.Format("Full Memory:\r\nTotal: {0:N0} KB\r\nIterationAvg: {1:N0} KB\r\nSingleAvg: {2:N0} KB\r\nOverhead: {3:N0}%",
                    runMemory, runMemory / Iterations, runMemory / Iterations / sampleCount, 100 * (runMemory / Iterations) / htmlSize);
            }

            return msg;
        }

        private static Stopwatch RunTest(Action<String> setHtmlDelegate, bool real, out double totalMem)
        {
            totalMem = 0;
            long startMemory = 0;
            if (Environment.Version.Major >= 4)
            {
                typeof(AppDomain).GetProperty("MonitoringIsEnabled").SetValue(null, true, null);
                startMemory = (long)AppDomain.CurrentDomain.GetType().GetProperty("MonitoringTotalAllocatedMemorySize").GetValue(AppDomain.CurrentDomain, null);
            }

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < Iterations; i++)
            {
                foreach (var sample in SamplesLoader.ShowcaseSamples)
                {
                    setHtmlDelegate(real ? sample.Html : string.Empty);
                }
            }

            sw.Stop();

            if (Environment.Version.Major >= 4)
            {
                var endMemory = (long)AppDomain.CurrentDomain.GetType().GetProperty("MonitoringTotalAllocatedMemorySize").GetValue(AppDomain.CurrentDomain, null);
                totalMem = (endMemory - startMemory) / 1024f;
            }

            return sw;
        }
    }
}