using AbusaOS.Controls;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System.Collections.Generic;
using System.Drawing;

namespace AbusaOS.Windows
{
    public class Window
    {
        public int x, y;
        public int w, h;
        public string title;
        public Font font;
        public bool dragging;
        public bool resizing;
        protected const int window_titlebarsize = 30;
        bool lmD;
        public List<Control> controls = new();
        public bool resizable = false;
        public bool windowed = true; // New property to control the presence of the maximize button
        public Button closeButton;
        public Button maximizeButton;
        protected int myIndex = -1;
        private int dragOffsetX, dragOffsetY;
        private bool maximized = false;
        private Rectangle previousBounds;

        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.Applogos.gear.bmp")]
        static byte[] gearBytes;

        public Bitmap logo;

        public Window(int x, int y, int w, int h, string title, Font font, bool resizable = false, bool windowed = true)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.title = title;
            this.font = font;
            this.resizable = resizable;
            this.windowed = windowed;
            closeButton = new Button("X", w - 40, 2, Color.Red, font);

            if (windowed)
            {
                maximizeButton = new Button("[]", w - 80, 2, Color.FromArgb(0, 166, 9), font);
            }

            logo = new Bitmap(gearBytes);

            FixBounds();
        }

        public void Close()
        {
            if (Kernel.activeIndex == myIndex)
                Kernel.activeIndex = -1;
            Kernel.windows.Remove(this);
        }

        void FixBounds()
        {
            if (w <= font.Width * title.Length + 80)
            {
                w = font.Width * title.Length + 80;
            }
            int maxparam = int.MinValue;

            Control MostBottomControl = null;

            int margin = 80;

            foreach (Control obj in controls)
            {
                if (obj.y + margin > maxparam)
                {
                    maxparam = obj.y + margin;
                    MostBottomControl = obj;
                }
            }

            if (h <= MostBottomControl.y + margin)
            {
                h = MostBottomControl.y + margin;
            }
        }

        public virtual void Start(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {

        }

        public virtual void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            if (myIndex == -1)
                myIndex = Kernel.windows.FindIndex(x => x == this);

            if (Clicked(mX, mY, mD && !lmD))
            {
                if (Kernel.activeIndex != myIndex)
                    Kernel.activeIndex = myIndex;
                dragging = true;
                dragOffsetX = mX - x;
                dragOffsetY = mY - y;
            }
            if (ClickedResize(mX, mY, mD && !lmD) && resizable)
            {
                resizing = true;
            }

            closeButton.x = w - 40;
            if (windowed)
            {
                maximizeButton.x = w - 80;
            }

            if (closeButton.clickedOnce)
            {
                Close();
            }

            if (windowed && maximizeButton.clickedOnce)
            {
                if (maximized)
                {
                    // Restore previous size and position
                    x = previousBounds.X;
                    y = previousBounds.Y;
                    w = previousBounds.Width;
                    h = previousBounds.Height;
                    maximized = false;
                }
                else
                {
                    // Save current size and position
                    previousBounds = new Rectangle(x, y, w, h);
                    // Maximize the window
                    x = 0;
                    y = 0;
                    w = (int)canv.Mode.Width;
                    h = (int)canv.Mode.Height - window_titlebarsize;
                    maximized = true;
                }
            }

            if (resizing)
            {
                w += dmX;
                h += dmY;

                FixBounds();

                if (!mD)
                {
                    resizing = false;
                }
            }

            canv.DrawFilledRectangle(Kernel.bgCol, x, y + window_titlebarsize, w, h);

            if (Kernel.activeIndex == Kernel.windows.FindIndex(x => x == this) && Kernel.activeIndex != -1)
            {
                canv.DrawFilledRectangle(Kernel.highlightCol, x, y, w, window_titlebarsize);
                canv.DrawRectangle(Kernel.highlightCol, x, y + window_titlebarsize, w, h);
                canv.DrawString(title, font, Kernel.textColLight, x + (int)logo.Width + 20, y + 10);
            }
            else
            {
                canv.DrawFilledRectangle(Kernel.bgCol, x, y, w, window_titlebarsize);
                canv.DrawRectangle(Kernel.textColLight, x, y + window_titlebarsize, w, h);
                canv.DrawRectangle(Kernel.highlightCol, x, y, w, window_titlebarsize);
                canv.DrawString(title, font, Kernel.highlightCol, x + (int)logo.Width + 20, y + 10);
            }

            canv.DrawRectangle(Color.Black, x, y, w, h + window_titlebarsize);

            canv.DrawImageAlpha(logo, x + 10, y + 5);

            if (dragging && Kernel.activeIndex == myIndex)
            {
                x = mX - dragOffsetX;
                y = mY - dragOffsetY;
                if (y <= 0)
                {
                    y = 0;
                }
                if (x <= 0)
                {
                    x = 0;
                }
                if (x >= canv.Mode.Width - w)
                {
                    x = (int)canv.Mode.Width - w;
                }
                if (y >= canv.Mode.Height - h - window_titlebarsize)
                {
                    y = (int)canv.Mode.Height - h - window_titlebarsize;
                }

                if (!mD)
                {
                    dragging = false;
                }
            }

            if (resizable && !maximized)
                canv.DrawFilledRectangle(Kernel.textColLight, x + w - 20, y + h - 20 + window_titlebarsize, 20, 20);

            foreach (Control c in controls)
            {
                c.Update(x, y + window_titlebarsize);
            }
            closeButton.Update(x, y);
            if (windowed)
            {
                maximizeButton.Update(x, y);
            }

            lmD = mD;
        }

        public bool Clicked(int mX, int mY, bool mD)
        {
            if (mX >= x && mX <= x + w &&
                mY >= y && mY <= y + window_titlebarsize)
            {
                if (mD)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ClickedResize(int mX, int mY, bool mD)
        {
            if (mX >= x + w - 20 && mX <= x + w &&
                mY >= y + h - 20 && mY <= y + h + window_titlebarsize)
            {
                if (mD)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
