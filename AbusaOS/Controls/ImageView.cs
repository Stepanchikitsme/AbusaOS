using Cosmos.System;
using Cosmos.System.Graphics;
using System;

namespace AbusaOS.Controls
{
    public class ImageView : Control
    {
        public Bitmap img;
        VBECanvas canv;

        // Добавляем событие клика мыши
        public event EventHandler MouseClick;

        public ImageView(Bitmap img, int x, int y)
        {
            this.img = img;
            this.x = x;
            this.y = y;
            canv = Kernel.canv;
        }

        public override void Update(int pX, int pY)
        {
            if (!Visible) return;
            canv.DrawImageAlpha(img, x + pX, y + pY);

            // Проверка клика мыши
            if (MouseManager.X >= x + pX && MouseManager.X <= x + pX + img.Width &&
                MouseManager.Y >= y + pY && MouseManager.Y <= y + pY + img.Height &&
                MouseManager.MouseState == MouseState.Left)
            {
                OnMouseClick(EventArgs.Empty);
            }
        }

        protected virtual void OnMouseClick(EventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }
    }
}
