using Cosmos.System.FileSystem.Listing;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Linq;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using AbusaOS.Controls;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
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
            Label toolbar = new Label("Toolbar", 20, 20, font, Kernel.textColDark);
            controls.Add(toolbar);

            // Область навигации слева (например, дерево папок)
            Label folderLabel = new Label("Folders", 20, 60, font, Kernel.textColDark);
            controls.Add(folderLabel);
            int folderYPosition = 100;

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                ImageView dirView = new ImageView(folderImg, 20, folderYPosition);
                Label dirLabel = new Label(dir, 50, folderYPosition, font, Kernel.textColDark);
                controls.Add(dirView);
                controls.Add(dirLabel);
                folderYPosition += 40;
            }

            // Область содержимого справа (например, список файлов)
            Label fileLabel = new Label("Files", 250, 60, font, Kernel.textColDark);
            controls.Add(fileLabel);
            int fileYPosition = 100;

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                ImageView fileView = new ImageView(fileImg, 250, fileYPosition);
                Label fileLabelItem = new Label(file, 280, fileYPosition, font, Kernel.textColDark);
                controls.Add(fileView);
                controls.Add(fileLabelItem);
                fileYPosition += 40;
            }

            // Нижняя панель (например, статусная строка)
            Label statusBar = new Label("Status: Ready", 20, 250, font, Kernel.textColDark);
            controls.Add(statusBar);
        }

    }
}
