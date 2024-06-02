namespace AbusaOS.Controls
{
    public abstract class Control
    {
        public int x, y;
        public bool Visible = true;

        // Добавляем свойство Tag
        public object Tag { get; set; }

        public abstract void Update(int pX, int pY);
    }
}

