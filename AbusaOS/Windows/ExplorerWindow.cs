using AbusaOS.Controls;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.IO;

namespace AbusaOS.Windows
{
    internal class Explorer : Window
    {
        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.folder.bmp")]
        static byte[] folderIcon;
        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.file.bmp")]
        static byte[] fileIcon;

        Bitmap folderImg;
        Bitmap fileImg;

        Label folderLabel, fileLabel;
        ImageView folderView;

        public Explorer() : base(300, 300, 500, 300, "Explorer", Kernel.defFont)
        {
            string path = @"0:\";
            // Загружаем иконки
            folderImg = new Bitmap(folderIcon);
            fileImg = new Bitmap(fileIcon);

            // Верхняя панель (например, панель инструментов)
            Label toolbar = new("Toolbar", 20, 20, font, Kernel.textColDark);
            controls.Add(toolbar);

            // Область навигации слева (например, дерево папок)
            Label folderLabel = new("Folders", 20, 60, font, Kernel.textColDark);
            controls.Add(folderLabel);
            int folderYPosition = 100;

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                ImageView dirView = new(folderImg, 20, folderYPosition);
                Label dirLabel = new(dir, 50, folderYPosition, font, Kernel.textColDark);
                controls.Add(dirView);
                controls.Add(dirLabel);
                folderYPosition += 40;
            }

            // Область содержимого справа (например, список файлов)
            Label fileLabel = new("Files", 250, 60, font, Kernel.textColDark);
            controls.Add(fileLabel);
            int fileYPosition = 100;

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                ImageView fileView = new(fileImg, 250, fileYPosition);
                Label fileLabelItem = new(file, 280, fileYPosition, font, Kernel.textColDark);
                controls.Add(fileView);
                controls.Add(fileLabelItem);
                fileYPosition += 40;
            }

            // Нижняя панель (например, статусная строка)
            Label statusBar = new("Status: Ready", 20, 250, font, Kernel.textColDark);
            controls.Add(statusBar);
        }

    }
}
