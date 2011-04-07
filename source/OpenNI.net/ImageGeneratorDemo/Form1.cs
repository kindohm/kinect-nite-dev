// This code is an adaptation from an OpenNI sample.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using xn;

namespace ImageGeneratorDemo
{
    public partial class Form1 : Form
    {
        readonly string SAMPLE_XML_FILE = @"SamplesConfig.xml";

        Context context;
        ImageGenerator image;
        Thread readerThread;
        bool shouldRun;
        Bitmap bitmap;

        public Form1()
        {
            InitializeComponent();

            this.context = new Context(SAMPLE_XML_FILE);
            this.image = context.FindExistingNode(NodeType.Image) as ImageGenerator;


            if (this.image == null)
            {
                throw new Exception("Viewer must have an image node!");
            }

            var mapMode = this.image.GetMapOutputMode();

            this.bitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.shouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (this)
            {
                e.Graphics.DrawImage(this.bitmap,
                    this.panelView.Location.X,
                    this.panelView.Location.Y,
                    this.panelView.Size.Width,
                    this.panelView.Size.Height);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Don't allow the background to paint
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.shouldRun = false;
            this.readerThread.Join();
            base.OnClosing(e);
        }

        /// <summary>
        /// this method uses image-processing code based off of the following example:
        /// http://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata.scan0.aspx
        /// </summary>
        unsafe void ReaderThread()
        {
            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.image);
                }
                catch (Exception)
                {
                }

                var imageBytes = this.GetImageBytes();

                lock (this)
                {
                    // Lock the bitmap's bits.  
                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    System.Drawing.Imaging.BitmapData bmpData =
                        bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        bitmap.PixelFormat);

                    // Get the address of the first line.
                    IntPtr ptr = bmpData.Scan0;

                    // Declare an array to hold the bytes of the bitmap.
                    int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;

                    // Copy the RGB values back to the bitmap
                    Marshal.Copy(imageBytes, 0, ptr, bytes);

                    // Unlock the bits.
                    bitmap.UnlockBits(bmpData);

                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// this method sourced from:
        /// http://groups.google.com/group/openni-dev/browse_thread/thread/f3d82e8f2de0a59f/19cb38853f9d2672?lnk=gst&q=draw+rgb+image#19cb38853f9d2672
        /// </summary>
        unsafe byte[] GetImageBytes()
        {
            byte[] bmpBytes = null;
            var imageMD = new ImageMetaData();
            this.image.GetMetaData(imageMD);
            lock (this)
            {
                int byteCount = imageMD.XRes * imageMD.YRes * 3;    // 3-byte color representation - RGB
                if (bmpBytes == null || bmpBytes.Length != byteCount)
                    bmpBytes = new byte[byteCount];
                fixed (byte* texturePointer = &bmpBytes[0])
                {
                    RGB24Pixel* pImage = (RGB24Pixel*)this.image.GetImageMapPtr().ToPointer();
                    int pointerWalker = 0;
                    for (int y = 0; y < imageMD.YRes; ++y)
                    {
                        for (int x = 0; x < imageMD.XRes; ++x, ++pImage, pointerWalker += 3)
                        {
                            texturePointer[pointerWalker] = pImage->nRed;
                            texturePointer[pointerWalker + 1] = pImage->nGreen;
                            texturePointer[pointerWalker + 2] = pImage->nBlue;
                        }
                    }
                }
            }
            return bmpBytes;
        }

    }
}
