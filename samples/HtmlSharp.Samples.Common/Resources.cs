

using System.IO;

namespace HtmlSharp.Samples.Common
{
    /// <summary>
    /// Get font, image and text resources for HtmlRenderer demo.
    /// </summary>
    public static class Resources
    {
        public static byte[] CustomFont
        {
            get
            {
                var stream = GetManifestResourceStream("CustomFont.ttf");

                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }

        public static Stream Comment16
        {
            get { return GetManifestResourceStream("comment16.gif"); }
        }

        public static Stream Event16
        {
            get { return GetManifestResourceStream("Event16.png"); }
        }

        public static Stream Favorites32
        {
            get { return GetManifestResourceStream("favorites32.png"); }
        }

        public static Stream Font32
        {
            get { return GetManifestResourceStream("font32.png"); }
        }

        public static Stream Html32
        {
            get { return GetManifestResourceStream("html32.png"); }
        }

        public static Stream Image32
        {
            get { return GetManifestResourceStream("image32.png"); }
        }

        public static Stream Method16
        {
            get { return GetManifestResourceStream("method16.gif"); }
        }

        public static Stream Property16
        {
            get { return GetManifestResourceStream("property16.gif"); }
        }

        public static Stream WebPallete
        {
            get { return GetManifestResourceStream("web_pallete.gif"); }
        }

        public static string Tooltip
        {
            get
            {
                using (var reader = new StreamReader(GetManifestResourceStream("Tooltip.html")))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static Stream GetManifestResourceStream(string name)
        {
            return typeof(Resources).Assembly.GetManifestResourceStream("TheArtOfDev.HtmlRenderer.Demo.Common.Resources." + name);
        }
    }
}