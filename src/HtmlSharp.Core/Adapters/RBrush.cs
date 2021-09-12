using System;

namespace HtmlSharp.Core.Adapters
{
    /// <summary>
    /// Adapter for platform specific brush objects - used to fill graphics (rectangles, polygons and paths).<br/>
    /// The brush can be solid color, gradient or image.
    /// </summary>
    public abstract class RBrush : IDisposable
    {
        public abstract void Dispose();
    }
}