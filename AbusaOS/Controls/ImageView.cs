using Cosmos.System.Graphics;

namespace AbusaOS.Controls
{
    public class ImageView : Control
    {
        public Bitmap img;
        VBECanvas canv;

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
        }
    }
}
