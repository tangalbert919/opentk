/* Licensed under the MIT/X11 license.
 * Copyright (c) 2009 Novell, Inc.
 * Copyright 2013 Xamarin Inc
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */
// Copyright 2011 Xamarin Inc. All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using OpenGLES;
using UIKit;
using ObjCRuntime;

using osuTK.Graphics;
using osuTK.Input;
using osuTK.Platform;

using All  = osuTK.Graphics.ES11.All;
using ES11 = osuTK.Graphics.ES11;
using ES20 = osuTK.Graphics.ES20;
using ES30 = osuTK.Graphics.ES30;

namespace osuTK.iOS
{
    internal sealed class GLCalls {
        public delegate void glBindFramebuffer(All target, int framebuffer);
        public delegate void glBindRenderbuffer(All target, int renderbuffer);
        public delegate void glDeleteFramebuffers(int n, ref int framebuffers);
        public delegate void glDeleteRenderbuffers(int n, ref int renderbuffers);
        public delegate void glFramebufferRenderbuffer(All target, All attachment, All renderbuffertarget, int renderbuffer);
        public delegate void glGenFramebuffers(int n, out int framebuffers);
        public delegate void glGenRenderbuffers(int n, out int renderbuffers);
        public delegate void glGetInteger(All name, out int value);
        public delegate void glScissor(int x, int y, int width, int height);
        public delegate void glViewport(int x, int y, int width, int height);
        public delegate void glGetRenderBufferParameter(All target, All pname, out int p);
        public delegate void glPixelStore(All pname, int param);
        public delegate void glReadPixels(int x, int y, int width, int height, All format, All type, byte[] pixels);

        public glBindFramebuffer BindFramebuffer;
        public glBindRenderbuffer BindRenderbuffer;
        public glDeleteFramebuffers DeleteFramebuffers;
        public glDeleteRenderbuffers DeleteRenderbuffers;
        public glFramebufferRenderbuffer FramebufferRenderbuffer;
        public glGenFramebuffers GenFramebuffers;
        public glGenRenderbuffers GenRenderbuffers;
        public glGetInteger GetInteger;
        public glScissor Scissor;
        public glViewport Viewport;
        public glGetRenderBufferParameter GetRenderbufferParameter;
        public glPixelStore PixelStore;
        public glReadPixels ReadPixels;

        public static GLCalls GetGLCalls(EAGLRenderingAPI api)
        {
            switch (api) {
                case EAGLRenderingAPI.OpenGLES1: return CreateES1();
                case EAGLRenderingAPI.OpenGLES2: return CreateES2();
                case EAGLRenderingAPI.OpenGLES3: return CreateES3();
            }
            throw new ArgumentException("api");
        }

        private static GLCalls CreateES1()
        {
            return new GLCalls() {
                BindFramebuffer          = (t, f)              => ES11.GL.Oes.BindFramebuffer(t, f),
                BindRenderbuffer         = (t, r)              => ES11.GL.Oes.BindRenderbuffer(t, r),
                DeleteFramebuffers       = (int n, ref int f)  => ES11.GL.Oes.DeleteFramebuffers(n, ref f),
                DeleteRenderbuffers      = (int n, ref int r)  => ES11.GL.Oes.DeleteRenderbuffers(n, ref r),
                FramebufferRenderbuffer  = (t, a, rt, rb)      => ES11.GL.Oes.FramebufferRenderbuffer(t, a, rt, rb),
                GenFramebuffers          = (int n, out int f)  => ES11.GL.Oes.GenFramebuffers(n, out f),
                GenRenderbuffers         = (int n, out int r)  => ES11.GL.Oes.GenRenderbuffers(n, out r),
                GetInteger               = (All n, out int v)  => ES11.GL.GetInteger(n, out v),
                Scissor                  = (x, y, w, h)        => ES11.GL.Scissor(x, y, w, h),
                Viewport                 = (x, y, w, h)        => ES11.GL.Viewport(x, y, w, h),
                GetRenderbufferParameter = (All t, All p, out int a) => ES11.GL.Oes.GetRenderbufferParameter (t, p, out a),
                PixelStore               = (n, p)                    => ES11.GL.PixelStore(n, p),
                ReadPixels               = (x, y, w, h, f, t, d)     => ES11.GL.ReadPixels(x, y, w, h, f, t, d),
            };
        }

        private static GLCalls CreateES2()
        {
            return new GLCalls() {
                BindFramebuffer          = (t, f)              => ES20.GL.BindFramebuffer((ES20.FramebufferTarget)t, f),
                BindRenderbuffer         = (t, r)              => ES20.GL.BindRenderbuffer((ES20.RenderbufferTarget)t, r),
                DeleteFramebuffers       = (int n, ref int f)  => ES20.GL.DeleteFramebuffers(n, ref f),
                DeleteRenderbuffers      = (int n, ref int r)  => ES20.GL.DeleteRenderbuffers(n, ref r),
                FramebufferRenderbuffer  = (t, a, rt, rb)      => ES20.GL.FramebufferRenderbuffer((ES20.FramebufferTarget)t, (ES20.FramebufferSlot)a, (ES20.RenderbufferTarget)rt, rb),
                GenFramebuffers          = (int n, out int f)  => ES20.GL.GenFramebuffers(n, out f),
                GenRenderbuffers         = (int n, out int r)  => ES20.GL.GenRenderbuffers(n, out r),
                GetInteger               = (All n, out int v)  => ES20.GL.GetInteger((ES20.GetPName)n, out v),
                Scissor                  = (x, y, w, h)        => ES20.GL.Scissor(x, y, w, h),
                Viewport                 = (x, y, w, h)        => ES20.GL.Viewport(x, y, w, h),
                GetRenderbufferParameter = (All t, All p, out int a) => ES20.GL.GetRenderbufferParameter ((ES20.RenderbufferTarget)t, (ES20.RenderbufferParameterName)p, out a),
                PixelStore               = (n, p)                    => ES20.GL.PixelStore((ES20.PixelStoreParameter)n, p),
                ReadPixels               = (x, y, w, h, f, t, d)     => ES20.GL.ReadPixels(x, y, w, h, (ES20.PixelFormat)f, (ES20.PixelType)t, d),
            };
        }

        private static GLCalls CreateES3()
        {
            return new GLCalls() {
                BindFramebuffer          = (t, f)              => ES30.GL.BindFramebuffer((ES30.FramebufferTarget)t, f),
                BindRenderbuffer         = (t, r)              => ES30.GL.BindRenderbuffer((ES30.RenderbufferTarget)t, r),
                DeleteFramebuffers       = (int n, ref int f)  => ES30.GL.DeleteFramebuffers(n, ref f),
                DeleteRenderbuffers      = (int n, ref int r)  => ES30.GL.DeleteRenderbuffers(n, ref r),
                FramebufferRenderbuffer  = (t, a, rt, rb)      => ES30.GL.FramebufferRenderbuffer((ES30.FramebufferTarget)t, (ES30.FramebufferAttachment)a, (ES30.RenderbufferTarget)rt, rb),
                GenFramebuffers          = (int n, out int f)  => ES30.GL.GenFramebuffers(n, out f),
                GenRenderbuffers         = (int n, out int r)  => ES30.GL.GenRenderbuffers(n, out r),
                GetInteger               = (All n, out int v)  => ES30.GL.GetInteger((ES30.GetPName)n, out v),
                Scissor                  = (x, y, w, h)        => ES30.GL.Scissor(x, y, w, h),
                Viewport                 = (x, y, w, h)        => ES30.GL.Viewport(x, y, w, h),
                GetRenderbufferParameter = (All t, All p, out int a) => ES30.GL.GetRenderbufferParameter ((ES30.RenderbufferTarget)t, (ES30.RenderbufferParameterName)p, out a),
                PixelStore               = (n, p)                    => ES30.GL.PixelStore((ES30.PixelStoreParameter)n, p),
                ReadPixels               = (x, y, w, h, f, t, d)     => ES30.GL.ReadPixels(x, y, w, h, (ES30.PixelFormat)f, (ES30.PixelType)t, d),
            };
        }
    }

    internal interface ITimeSource {
        void Suspend ();
        void Resume ();

        void Invalidate ();
    }

    [Register]
    internal class CADisplayLinkTimeSource : NSObject, ITimeSource {
        private const string selectorName = "runIteration:";
        private static Selector selRunIteration = new Selector (selectorName);

        private iOSGameView view;
        private CADisplayLink displayLink;

        public CADisplayLinkTimeSource (iOSGameView view, int frameInterval)
        {
            this.view = view;

            if (displayLink != null)
            {
                displayLink.Invalidate ();
            }

            displayLink = CADisplayLink.Create (this, selRunIteration);
            displayLink.FrameInterval = frameInterval;
            displayLink.Paused = true;
        }

        public void Suspend ()
        {
            displayLink.Paused = true;
        }

        public void Resume ()
        {
            displayLink.Paused = false;
            displayLink.AddToRunLoop (NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);
        }

        public void Invalidate ()
        {
            if (displayLink != null) {
                displayLink.Invalidate ();
                displayLink = null;
            }
        }

        [Export (selectorName)]
        [Preserve (Conditional = true)]
        private void RunIteration (NSObject parameter)
        {
            view.RunIteration (null);
        }
    }

    internal class NSTimerTimeSource : ITimeSource {
        private TimeSpan timeout;
        private NSTimer timer;

        private iOSGameView view;

        public NSTimerTimeSource (iOSGameView view, double updatesPerSecond)
        {
            this.view = view;

            // Can't use TimeSpan.FromSeconds() as that only has 1ms
            // resolution, and we need better (e.g. 60fps doesn't fit nicely
            // in 1ms resolution, but does in ticks).
            timeout = new TimeSpan ((long)(((1.0 * TimeSpan.TicksPerSecond) / updatesPerSecond) + 0.5));
        }

        public void Suspend ()
        {
            if (timer != null) {
                timer.Invalidate ();
                timer = null;
            }
        }

        public void Resume ()
        {
            if (timeout != new TimeSpan (-1))
            {
                timer = NSTimer.CreateRepeatingScheduledTimer (timeout, view.RunIteration);
            }
        }

        public void Invalidate ()
        {
            if (timer != null) {
                timer.Invalidate ();
                timer = null;
            }
        }
    }

    public class iOSGameView : UIView, IGameWindow
    {
        private bool suspended;
        private bool disposed;

        private int framebuffer, renderbuffer;

        private GLCalls gl;

        private ITimeSource timesource;
        private System.Diagnostics.Stopwatch stopwatch;
        private TimeSpan prevUpdateTime;
        private TimeSpan prevRenderTime;

        private IWindowInfo windowInfo = Utilities.CreateDummyWindowInfo();

        [Export("initWithCoder:")]
        public iOSGameView(NSCoder coder)
            : base(coder)
        {
            stopwatch = new System.Diagnostics.Stopwatch ();
        }

        [Export("initWithFrame:")]
        public iOSGameView(RectangleF frame)
            : base(frame)
        {
            stopwatch = new System.Diagnostics.Stopwatch ();
        }

        [Export ("layerClass")]
        public static Class GetLayerClass ()
        {
            return new Class (typeof(CAEAGLLayer));
        }

        private void AssertValid()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("");
            }
        }

        private void AssertContext()
        {
            if (GraphicsContext == null)
            {
                throw new InvalidOperationException("Operation requires a GraphicsContext, which hasn't been created yet.");
            }
        }

        private EAGLRenderingAPI api;
        public EAGLRenderingAPI ContextRenderingApi {
            get {
                AssertValid();
                return api;
            }
            set {
                AssertValid();
                if (GraphicsContext != null)
                {
                    throw new NotSupportedException("Can't change RenderingApi after GraphicsContext is constructed.");
                }
                this.api = value;
            }
        }

        public IGraphicsContext GraphicsContext {get; set; }
        public EAGLContext EAGLContext {
            get {
                AssertValid();
                if (GraphicsContext != null) {
                    iOSGraphicsContext c = GraphicsContext as iOSGraphicsContext;
                    if (c != null)
                    {
                        return c.EAGLContext;
                    }
                    var i = GraphicsContext as IGraphicsContextInternal;
                    IGraphicsContext ic = i == null ? null : i.Implementation;
                    c = ic as iOSGraphicsContext;
                    if (c != null)
                    {
                        return c.EAGLContext;
                    }
                }
                return null;
            }
        }

        private bool retainedBacking;
        public bool LayerRetainsBacking {
            get {
                AssertValid();
                return retainedBacking;
            }
            set {
                AssertValid();
                if (GraphicsContext != null)
                {
                    throw new NotSupportedException("Can't change LayerRetainsBacking after GraphicsContext is constructed.");
                }
                retainedBacking = value;
            }
        }

        private NSString colorFormat;
        public NSString LayerColorFormat {
            get {
                AssertValid();
                return colorFormat;
            }
            set {
                AssertValid();
                if (GraphicsContext != null)
                {
                    throw new NotSupportedException("Can't change LayerColorFormat after GraphicsContext is constructed.");
                }
                colorFormat = value;
            }
        }

        public int Framebuffer {
            get {return framebuffer; }
        }

        public int Renderbuffer {
            get {return renderbuffer; }
        }

        public bool AutoResize {get; set; }

        private UIViewController GetViewController()
        {
            UIResponder r = this;
            while (r != null) {
                var c = r as UIViewController;
                if (c != null)
                {
                    return c;
                }
                r = r.NextResponder;
            }
            return null;
        }

        public virtual string Title {
            get {
                AssertValid();
                var c = GetViewController();
                if (c != null)
                {
                    return c.Title;
                }
                throw new NotSupportedException();
            }
            set {
                AssertValid();
                var c = GetViewController();
                if (c != null) {
                    if (c.Title != value) {
                        c.Title = value;
                        OnTitleChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        protected virtual void OnTitleChanged(EventArgs e) => TitleChanged?.Invoke(this, EventArgs.Empty);

        bool INativeWindow.Focused {
            get {throw new NotImplementedException(); }
        }

        public virtual bool Visible {
            get {
                AssertValid();
                return !base.Hidden;
            }
            set {
                AssertValid();
                if (base.Hidden != !value) {
                    base.Hidden = !value;
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnVisibleChanged(EventArgs e) => VisibleChanged?.Invoke(this, EventArgs.Empty);

        bool INativeWindow.Exists {
            get {throw new NotImplementedException(); }
        }

        public virtual IWindowInfo WindowInfo {
            get {
                AssertValid();
                return windowInfo;
            }
        }

        public virtual WindowState WindowState {
            get {
                AssertValid();
                var c = GetViewController();
#if TVOS
                if (c != null && (c.EdgesForExtendedLayout == UIRectEdge.None))
#else
                if (c != null && c.WantsFullScreenLayout)
#endif
                {
                    return WindowState.Fullscreen;
                }
                return WindowState.Normal;
            }
            set {
                AssertValid();
                var c = GetViewController();
                if (c != null) {
                    var fullscreen = (value == WindowState.Fullscreen);
#if TVOS
                    if ((c.EdgesForExtendedLayout == UIRectEdge.None) == fullscreen) {
                        c.EdgesForExtendedLayout = fullscreen ? UIRectEdge.None : UIRectEdge.All;
                        OnWindowStateChanged(EventArgs.Empty);
                    }
#else
                    if (c.WantsFullScreenLayout == fullscreen) {
                        c.WantsFullScreenLayout = fullscreen;
                        OnWindowStateChanged(EventArgs.Empty);
                    }
#endif
                }
            }
        }

        protected virtual void OnWindowStateChanged(EventArgs e) => WindowStateChanged?.Invoke(this, EventArgs.Empty);

        public virtual WindowBorder WindowBorder {
            get {
                AssertValid();
                return WindowBorder.Hidden;
            }
            set {}
        }

        Rectangle INativeWindow.Bounds
        {
            get => new Rectangle(X, Y, Width, Height);
            set { }
        }


        public Point Location
        {
            get => new Point();
            set { }
        }


        private Size size;
        public Size Size
        {
            get => size;
            set
            {
                if (value.Width == size.Width && value.Height == size.Height)
                    return;
                size = value;
                OnResize(EventArgs.Empty);
            }
        }


        protected virtual void OnResize(EventArgs e) => Resize?.Invoke(this, e);

        public int X
        {
            get => 0;
            set { }
        }

        public int Y
        {
            get => 0;
            set { }
        }

        public int Width
        {
            get => size.Width;
            set { }
        }

        public int Height
        {
            get => size.Height;
            set { }
        }


        public Rectangle ClientRectangle
        {
            get => new Rectangle(0, 0, Width, Height);
            set { }
        }

        public Size ClientSize
        {
            get => new Size(Width, Height);
            set { }
        }

        protected virtual void CreateFrameBuffer()
        {
            AssertValid();
            if (LayerColorFormat == null)
            {
                throw new InvalidOperationException("Set the LayerColorFormat property to an EAGLColorFormat value before calling Run().");
            }

            CAEAGLLayer eaglLayer = (CAEAGLLayer)Layer;
            eaglLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys (
                    new NSObject [] {NSNumber.FromBoolean(LayerRetainsBacking), LayerColorFormat},
                    new NSObject [] {EAGLDrawableProperty.RetainedBacking,      EAGLDrawableProperty.ColorFormat}
            );
            ConfigureLayer(eaglLayer);

            int major = 0, minor = 0;
            switch (ContextRenderingApi)
            {
                case EAGLRenderingAPI.OpenGLES1: major = 1; minor = 1; break;
                case EAGLRenderingAPI.OpenGLES2: major = 2; minor = 0; break;
                case EAGLRenderingAPI.OpenGLES3: major = 3; minor = 0; break;
                default:
                    throw new ArgumentException("Unsupported EAGLRenderingAPI version: " + ContextRenderingApi);
            }
            GraphicsContext = new GraphicsContext(GraphicsMode.Default, WindowInfo, major, minor, GraphicsContextFlags.Embedded);
            GraphicsContext.MakeCurrent(WindowInfo);
            GraphicsContext.LoadAll();

            gl = GLCalls.GetGLCalls(ContextRenderingApi);

            int oldFramebuffer = 0;
            gl.GetInteger(All.FramebufferBindingOes, out oldFramebuffer);
            gl.GenFramebuffers(1, out framebuffer);

            CreateFrameBuffer(eaglLayer);

            frameBufferWindow = new WeakReference(Window);
            frameBufferLayer = new WeakReference(Layer);
        }

        protected virtual void ConfigureLayer(CAEAGLLayer eaglLayer)
        {
        }

        protected virtual void DestroyFrameBuffer()
        {
            AssertValid();
            AssertContext();
            EAGLContext oldContext = EAGLContext.CurrentContext;
            if (!GraphicsContext.IsCurrent)
            {
                MakeCurrent();
            }

            gl.DeleteFramebuffers (1, ref framebuffer);
            gl.DeleteRenderbuffers (1, ref renderbuffer);
            framebuffer = renderbuffer = 0;

            if (oldContext != EAGLContext)
            {
                EAGLContext.SetCurrentContext(oldContext);
            }
            else
            {
                EAGLContext.SetCurrentContext(null);
            }

            GraphicsContext.Dispose();
            GraphicsContext = null;
            gl = null;
        }

        /// <summary>
        /// Resizes the frame buffer.
        /// On IOS, when the device is rotated from portrait to landscape, if the framebuffer is not recreated
        /// then the EAGLContext will stretch the render buffer to fit the screen.
        /// If you care at all about aspect ratio, then this behavior is not desired, and in that case this method
        /// should be called when rotating the device.
        /// See here: http://stackoverflow.com/questions/20326947/opengl-what-need-to-reconfig-when-rotate-screen
        /// And also here: https://gamedev.stackexchange.com/questions/75965/how-do-i-reconfigure-my-gles-frame-buffer-after-a-rotation
        /// </summary>
        public void ResizeFrameBuffer()
        {
            MakeCurrent();

            gl.DeleteRenderbuffers(1, ref renderbuffer);

            CAEAGLLayer eaglLayer = (CAEAGLLayer)Layer;
            CreateFrameBuffer(eaglLayer);
        }

        private void CreateFrameBuffer(CAEAGLLayer eaglLayer)
        {
            int oldRenderbuffer = 1;
            gl.GetInteger(All.RenderbufferBindingOes, out oldRenderbuffer);

            gl.GenRenderbuffers(1, out renderbuffer);
            gl.BindRenderbuffer(All.RenderbufferOes, renderbuffer);

            if (!EAGLContext.RenderBufferStorage((uint)All.RenderbufferOes, eaglLayer))
            {
                gl.DeleteRenderbuffers(1, ref renderbuffer);
                renderbuffer = 0;
                gl.BindRenderbuffer(All.RenderbufferBindingOes, oldRenderbuffer);
                throw new InvalidOperationException("Error with EAGLContext.RenderBufferStorage!");
            }

            gl.BindFramebuffer(All.FramebufferOes, framebuffer);
            gl.FramebufferRenderbuffer(All.FramebufferOes, All.ColorAttachment0Oes, All.RenderbufferOes, renderbuffer);

            Size newSize = new Size(
                    (int)Math.Round(eaglLayer.Bounds.Size.Width * ContentScaleFactor),
                    (int)Math.Round(eaglLayer.Bounds.Size.Height * ContentScaleFactor));
            Size = newSize;

            gl.Viewport(0, 0, newSize.Width, newSize.Height);
            gl.Scissor(0, 0, newSize.Width, newSize.Height);
        }

        public virtual void Close()
        {
            AssertValid();
            OnClosed(EventArgs.Empty);
        }

        protected virtual void OnClosed(EventArgs e) => Closed?.Invoke(this, e);

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing) {
                if (timesource != null)
                {
                    timesource.Invalidate ();
                }
                timesource = null;
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                }
                stopwatch = null;
                DestroyFrameBuffer();
            }
            base.Dispose (disposing);
            disposed = true;
            if (disposing)
            {
                OnDisposed(EventArgs.Empty);
            }
        }

        protected virtual void OnDisposed(EventArgs e) => Disposed?.Invoke(this, e);

        void INativeWindow.ProcessEvents() => throw new NotSupportedException();

        public override void LayoutSubviews()
        {
            base.LayoutSubviews ();
            if (GraphicsContext == null)
            {
                return;
            }

            var bounds = Bounds;
            if (AutoResize && (Math.Round(bounds.Width) != Size.Width ||
                        Math.Round(bounds.Height) != Size.Height)) {
                DestroyFrameBuffer();
                CreateFrameBuffer();
            }
        }

        public virtual void MakeCurrent()
        {
            AssertValid();
            AssertContext();
            GraphicsContext.MakeCurrent(WindowInfo);
        }

        public virtual void SwapBuffers()
        {
            AssertValid();
            AssertContext();
            gl.BindRenderbuffer(All.RenderbufferOes, renderbuffer);
            GraphicsContext.SwapBuffers();
        }

        private WeakReference frameBufferWindow;
        private WeakReference frameBufferLayer;

        public void Run()
        {
            RunWithFrameInterval (1);
        }

        public void Run(double updatesPerSecond)
        {
            if (updatesPerSecond < 0.0)
            {
                throw new ArgumentException ("updatesPerSecond");
            }

            if (updatesPerSecond == 0.0) {
                RunWithFrameInterval (1);
                return;
            }

            if (timesource != null)
            {
                timesource.Invalidate ();
            }

            timesource = new NSTimerTimeSource (this, updatesPerSecond);

            CreateFrameBuffer ();
            OnLoad (EventArgs.Empty);
            Start ();
        }

        [Obsolete ("Use either Run (float updatesPerSecond) or RunWithFrameInterval (int frameInterval)")]
        public void Run(int frameInterval)
        {
            RunWithFrameInterval (frameInterval);
        }

        public void RunWithFrameInterval(int frameInterval)
        {
            AssertValid ();

            if (frameInterval < 1)
            {
                throw new ArgumentException ("frameInterval");
            }

            if (timesource != null)
            {
                timesource.Invalidate ();
            }

            timesource = new CADisplayLinkTimeSource (this, frameInterval);

            CreateFrameBuffer ();
            OnLoad (EventArgs.Empty);
            Start ();
        }

        private void Start ()
        {
            prevUpdateTime = TimeSpan.Zero;
            prevRenderTime = TimeSpan.Zero;
            Resume ();
        }

        public void Stop()
        {
            AssertValid();
            if (timesource != null) {
                timesource.Invalidate ();
                timesource = null;
            }
            suspended = false;
            OnUnload(EventArgs.Empty);
        }

        private void Suspend ()
        {
            if (timesource != null)
            {
                timesource.Suspend ();
            }
            stopwatch.Stop();
            suspended = true;
        }

        private void Resume ()
        {
            if (timesource != null)
            {
                timesource.Resume ();
            }
            stopwatch.Start();
            suspended = false;
        }

        public UIImage Capture ()
        {
            // This is from: https://developer.apple.com/library/ios/#qa/qa2010/qa1704.html

            int backingWidth, backingHeight;

            // Bind the color renderbuffer used to render the OpenGL ES view
            // If your application only creates a single color renderbuffer which is already bound at this point,
            // this call is redundant, but it is needed if you're dealing with multiple renderbuffers.
            // Note, replace "_colorRenderbuffer" with the actual name of the renderbuffer object defined in your class.
            gl.BindRenderbuffer (All.RenderbufferOes, Renderbuffer);

            // Get the size of the backing CAEAGLLayer
            gl.GetRenderbufferParameter (All.RenderbufferOes, All.RenderbufferWidthOes, out backingWidth);
            gl.GetRenderbufferParameter (All.RenderbufferOes, All.RenderbufferHeightOes, out backingHeight);

            int width = backingWidth, height = backingHeight;
            int dataLength = width * height * 4;
            byte[] data = new byte [dataLength];

            // Read pixel data from the framebuffer
            gl.PixelStore (All.PackAlignment, 4);
            gl.ReadPixels (0, 0, width, height, All.Rgba, All.UnsignedByte, data);

            // Create a CGImage with the pixel data
            // If your OpenGL ES content is opaque, use kCGImageAlphaNoneSkipLast to ignore the alpha channel
            // otherwise, use kCGImageAlphaPremultipliedLast
            using (var data_provider = new CGDataProvider (data, 0, data.Length)) {
                using (var colorspace = CGColorSpace.CreateDeviceRGB ()) {
                    using (var iref = new CGImage (width, height, 8, 32, width * 4, colorspace,
                                    (CGImageAlphaInfo)((int)CGBitmapFlags.ByteOrder32Big | (int)CGImageAlphaInfo.PremultipliedLast),
                                    data_provider, null, true, CGColorRenderingIntent.Default)) {

                        // OpenGL ES measures data in PIXELS
                        // Create a graphics context with the target size measured in POINTS
                        int widthInPoints, heightInPoints;
                        float scale = (float)ContentScaleFactor;
                        widthInPoints = (int)(width / scale);
                        heightInPoints = (int)(height / scale);
                        UIGraphics.BeginImageContextWithOptions (new System.Drawing.SizeF (widthInPoints, heightInPoints), false, scale);

                        try {
                            var cgcontext = UIGraphics.GetCurrentContext ();

                            // UIKit coordinate system is upside down to GL/Quartz coordinate system
                            // Flip the CGImage by rendering it to the flipped bitmap context
                            // The size of the destination area is measured in POINTS
                            cgcontext.SetBlendMode (CGBlendMode.Copy);
                            cgcontext.DrawImage (new System.Drawing.RectangleF (0, 0, widthInPoints, heightInPoints), iref);

                            // Retrieve the UIImage from the current context
                            var image = UIGraphics.GetImageFromCurrentImageContext ();

                            return image;
                        } finally {
                            UIGraphics.EndImageContext ();
                        }
                    }
                }
            }
        }

        public override void WillMoveToWindow(UIWindow window)
        {
            if (window == null && !suspended)
            {
                Suspend();
            }
            else if (window != null && suspended) {
                if (frameBufferLayer != null && ((CALayer)frameBufferLayer.Target) != Layer ||
                    frameBufferWindow != null && ((UIWindow)frameBufferWindow.Target) != window) {

                    DestroyFrameBuffer();
                    CreateFrameBuffer ();
                }

                Resume ();
            }
        }

        private FrameEventArgs updateEventArgs = new FrameEventArgs();
        private FrameEventArgs renderEventArgs = new FrameEventArgs();

        protected virtual bool ShouldCallOnUpdate => true;
        protected virtual bool ShouldCallOnRender => true;

        internal void RunIteration (NSTimer timer)
        {
            if (ShouldCallOnUpdate)
            {
                var curUpdateTime = stopwatch.Elapsed;
                if (prevUpdateTime.Ticks != 0)
                {
                    var t = (curUpdateTime - prevUpdateTime).TotalSeconds;
                    updateEventArgs.Time = t;
                }
                OnUpdateFrame(updateEventArgs);
                prevUpdateTime = curUpdateTime;
            }

            if (ShouldCallOnRender)
            {
                gl.BindFramebuffer(All.FramebufferOes, framebuffer);

                var curRenderTime = stopwatch.Elapsed;
                if (prevRenderTime.Ticks == 0)
                {
                    var t = (curRenderTime - prevRenderTime).TotalSeconds;
                    renderEventArgs.Time = t;
                }
                OnRenderFrame(renderEventArgs);
                prevRenderTime = curRenderTime;
            }
        }

        protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

        protected virtual void OnUnload(EventArgs e)
        {
            var h = Unload;
            DestroyFrameBuffer();
            h?.Invoke(this, e);
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e) => UpdateFrame?.Invoke(this, e);

        protected virtual void OnRenderFrame(FrameEventArgs e) => RenderFrame?.Invoke(this, e);

        public Point PointToClient(Point point) => point;
        public Point PointToScreen(Point point) => point;

        public event EventHandler<EventArgs> Move;
        public event EventHandler<EventArgs> Resize;
        public event EventHandler<CancelEventArgs> Closing;
        public event EventHandler<EventArgs> Closed;
        public event EventHandler<EventArgs> Disposed;
        public event EventHandler<EventArgs> IconChanged;
        public event EventHandler<EventArgs> TitleChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> FocusedChanged;
        public event EventHandler<EventArgs> WindowBorderChanged;
        public event EventHandler<EventArgs> WindowStateChanged;
        public event EventHandler<KeyboardKeyEventArgs> KeyDown;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<KeyboardKeyEventArgs> KeyUp;
        public event EventHandler<EventArgs> MouseLeave;
        public event EventHandler<EventArgs> MouseEnter;
        public event EventHandler<MouseButtonEventArgs> MouseDown;
        public event EventHandler<MouseButtonEventArgs> MouseUp;
        public event EventHandler<MouseMoveEventArgs> MouseMove;
        public event EventHandler<MouseWheelEventArgs> MouseWheel;
        public event EventHandler<FileDropEventArgs> FileDrop;

        public event EventHandler<EventArgs> Load;
        public event EventHandler<EventArgs> Unload;
        public event EventHandler<FrameEventArgs> UpdateFrame;
        public event EventHandler<FrameEventArgs> RenderFrame;

        public MouseCursor Cursor { get; set; }
        public bool CursorVisible { get; set; }

        public bool CursorGrabbed
        {
            get => true;
            set { }
        }

        public Icon Icon { get; set; }
    }
}

// vim: et ts=4 sw=4
