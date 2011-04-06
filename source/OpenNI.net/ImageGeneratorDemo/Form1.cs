using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using xn;

namespace ImageGeneratorDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.context = new Context(SAMPLE_XML_FILE);
            this.image = context.FindExistingNode(NodeType.Image) as ImageGenerator;


            if (this.image == null)
            {
                throw new Exception("Viewer must have an image node!");
            }



            //this.histogram = new int[this.image.GetDeviceMaxDepth()];

            //this.histogram = this.image.GetDataSize();

            var mapMode = this.image.GetMapOutputMode();

            //MapOutputMode mapMode = this.depth.GetMapOutputMode();

            this.bitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes/*, System.Drawing.Imaging.PixelFormat.Format24bppRgb*/);
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

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                Close();
            }
            switch (e.KeyChar)
            {
                case (char)27:
                    break;
                case 'b':
                    this.shouldDrawBackground = !this.shouldDrawBackground;
                    break;
                case 'x':
                    this.shouldDrawPixels = !this.shouldDrawPixels;
                    break;
                case 's':
                    this.shouldDrawSkeleton = !this.shouldDrawSkeleton;
                    break;
                case 'i':
                    this.shouldPrintID = !this.shouldPrintID;
                    break;
                case 'l':
                    this.shouldPrintState = !this.shouldPrintState;
                    break;

            }
            base.OnKeyPress(e);
        }

        private unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        this.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }

        private Color[] colors = { Color.Red, Color.Blue, Color.ForestGreen, Color.Yellow, Color.Orange, Color.Purple, Color.White };
        private Color[] anticolors = { Color.Green, Color.Orange, Color.Red, Color.Purple, Color.Blue, Color.Yellow, Color.Black };
        private int ncolors = 6;




        private unsafe void ReaderThread()
        {
            //DepthMetaData depthMD = new DepthMetaData();


            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.image);
                }
                catch (Exception)
                {
                }

                //this.depth.GetMetaData(depthMD);

                //CalcHist(depthMD);

                lock (this)
                {
                    Rectangle rect = new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height);
                    BitmapData data = this.bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    var imageMD = new ImageMetaData();
                    byte[] bmpBytes = null;
                    this.image.GetMetaData(imageMD);

                    int byteCount = imageMD.XRes * imageMD.YRes * 4;    // 4-byte color representation - RGBA 
                    if (bmpBytes == null || bmpBytes.Length != byteCount)
                        bmpBytes = new byte[byteCount];

                    //fixed (byte* texturePointer = &bmpBytes[0])
                    //{
                    RGB24Pixel* pImage = (RGB24Pixel*)this.image.GetImageMapPtr().ToPointer();
                    int pointerWalker = 0;
                    for (int y = 0; y < imageMD.YRes; ++y)
                    {
                        byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                        for (int x = 0; x < imageMD.XRes; ++x, ++pImage, pointerWalker += 3)
                        {
                            pDest[pointerWalker] = pImage->nRed;

                            pDest[pointerWalker + 1] = pImage->nGreen;

                            pDest[pointerWalker + 2] = pImage->nBlue;

                            //texturePointer[pointerWalker + 3] = 255;    //Set alpha to 1 
                        }
                    }

                    //}


                    //if (this.shouldDrawPixels)
                    //{
                    //ushort* pDepth = (ushort*)this.depth.GetDepthMapPtr().ToPointer();
                    //ushort* pLabels = (ushort*)this.userGenerator.GetUserPixels(0).SceneMapPtr.ToPointer();

                    //ushort* pImage = (ushort*)this.image.GetImageMapPtr().ToPointer();

                    //// set pixels
                    //var imageMD = new ImageMetaData();

                    //for (int y = 0; y < imageMD.YRes; ++y)
                    //{
                    //    byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                    //    for (int x = 0; x < imageMD.XRes; ++x, ++pImage, pDest += 3)
                    //    {
                    //        pDest[0] = pDest[1] = pDest[2] = 0;

                    //        if (this.shouldDrawBackground)
                    //        {
                    //            //byte pixel = (byte)this.histogram[*pImage];

                    //            //pDest[0] = (byte)(pixel * (255 / 256.0));
                    //            //pDest[1] = (byte)(pixel * (0 / 256.0));
                    //            //pDest[2] = (byte)(pixel * (0 / 256.0));

                    //            pDest[0] = pImage->nRed;
                    //        }
                    //    }
                    //}


                    this.bitmap.UnlockBits(data);

                    Graphics g = Graphics.FromImage(this.bitmap);
                    g.Dispose();
                }

                this.Invalidate();
            }
        }

        //public unsafe byte[] GetImageBytes()
        //{

        //    return bmpBytes;

        //    //if (imageTexture == null)
        //    //{
        //    //    imageTexture = new Texture2D(game.GraphicsDevice, imageMD.XRes, imageMD.YRes);
        //    //}
        //    //imageTexture.SetData(bmpBytes);
        //    //return imageTexture;
        //}


        private readonly string SAMPLE_XML_FILE = @"SamplesConfig.xml";

        private Context context;
        private ImageGenerator image;
        private Thread readerThread;
        private bool shouldRun;
        private Bitmap bitmap;
        private int[] histogram;

        private bool shouldDrawPixels = true;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldPrintState = true;
        private bool shouldDrawSkeleton = true;
    }
}
