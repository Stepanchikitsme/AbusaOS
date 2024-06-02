using AbusaOS.Controls;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

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

        public string path = @"0:\";

        private int folderScrollOffset = 0;
        private int fileScrollOffset = 0;
        private const int maxItemsToShow = 8;

        private string[] cachedDirs;
        private string[] cachedFiles;

        private Button folderScrollUp;
        private Button folderScrollDown;
        private Button fileScrollUp;
        private Button fileScrollDown;
        private Button backButton;

        public Explorer() : base(100, 100, 600, 500, "Explorer", Kernel.defFont)
        {
            try
            {
                folderImg = new Bitmap(folderIcon);
                fileImg = new Bitmap(fileIcon);

                UpdateFolderContent();
            }
            catch (Exception ex)
            {
                Kernel.ShowMessage($"Initialization error: {ex.Message}", "Explorer", MsgType.Error);
            }
        }

        private void UpdateFolderContent()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Kernel.ShowMessage($"Path not found: {path}", "Explorer", MsgType.Error);
                    return;
                }

                cachedDirs = Directory.GetDirectories(path);
                cachedFiles = Directory.GetFiles(path);

                UpdateDisplayedItems();
            }
            catch (Exception ex)
            {
                Kernel.ShowMessage($"Error updating folder content: {ex.Message}", "Explorer", MsgType.Error);
            }
        }

        private void UpdateDisplayedItems()
        {
            try
            {
                controls.RemoveAll(control => control is Button);

                if (path != @"0:\")
                {
                    backButton = new Button("..", 20, 100, Color.Black, font, 12);
                    controls.Add(backButton);
                }

                int folderYPosition = path != @"0:\" ? 155 : 100; // Добавлено расстояние между .. и папками
                int fileYPosition = 100;

                // Очищаем предыдущий статус и создаем новый с правильным отображением пути
                controls.RemoveAll(control => control is Label && ((Label)control).Text.StartsWith("Status:"));
                string displayedPath = $"Status: Browsing {path}";
                if (displayedPath.Length > 100) // Ограничиваем длину строки
                {
                    displayedPath = displayedPath.Substring(0, 97) + "...";
                }
                Label statusBar = new Label(displayedPath, 20, 450, font, Kernel.textColDark);
                controls.Add(statusBar);

                for (int i = folderScrollOffset; i < Math.Min(cachedDirs.Length, folderScrollOffset + maxItemsToShow); i++)
                {
                    Button dirItem = new Button(Path.GetFileName(cachedDirs[i]), 20, folderYPosition, Color.Black, font, 12, folderImg)
                    {
                        Tag = cachedDirs[i]
                    };
                    controls.Add(dirItem);
                    folderYPosition += 45; // Добавлено расстояние между элементами
                }

                for (int i = fileScrollOffset; i < Math.Min(cachedFiles.Length, fileScrollOffset + maxItemsToShow); i++)
                {
                    Button fileItem = new Button(Path.GetFileName(cachedFiles[i]), 200, fileYPosition, Color.Black, font, 12, fileImg);
                    controls.Add(fileItem);
                    fileYPosition += 45; // Добавлено расстояние между элементами
                }

                if (cachedDirs.Length > maxItemsToShow)
                {
                    folderScrollUp = new Button("Up", 150, 100, Color.Black, font, 12);
                    controls.Add(folderScrollUp);

                    folderScrollDown = new Button("Down", 150, 400, Color.Black, font, 12);
                    controls.Add(folderScrollDown);
                }

                if (cachedFiles.Length > maxItemsToShow)
                {
                    fileScrollUp = new Button("Up", 500, 100, Color.Black, font, 12);
                    controls.Add(fileScrollUp);

                    fileScrollDown = new Button("Down", 500, 400, Color.Black, font, 12);
                    controls.Add(fileScrollDown);
                }
            }
            catch (Exception ex)
            {
                Kernel.ShowMessage($"Error updating displayed items: {ex.Message}", "Explorer", MsgType.Error);
            }
        }
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            try
            {
                base.Update(canv, mX, mY, mD, dmX, dmY);

                bool updated = false;

                if (folderScrollUp != null && folderScrollUp.clickedOnce)
                {
                    folderScrollOffset = Math.Max(0, folderScrollOffset - 1);
                    updated = true;
                }
                if (folderScrollDown != null && folderScrollDown.clickedOnce)
                {
                    folderScrollOffset = Math.Min(cachedDirs.Length - maxItemsToShow, folderScrollOffset + 1);
                    updated = true;
                }
                if (fileScrollUp != null && fileScrollUp.clickedOnce)
                {
                    fileScrollOffset = Math.Max(0, fileScrollOffset - 1);
                    updated = true;
                }
                if (fileScrollDown != null && fileScrollDown.clickedOnce)
                {
                    fileScrollOffset = Math.Min(cachedFiles.Length - maxItemsToShow, fileScrollOffset + 1);
                    updated = true;
                }
                if (backButton != null && backButton.clickedOnce)
                {
                    // Переходим к предыдущему каталогу
                    if (path != @"0:\")
                    {
                        path = Directory.GetParent(path)?.FullName ?? @"0:\";
                        UpdateFolderContent();
                    }
                }

                foreach (var control in controls)
                {
                    if (control is Button button && button.clickedOnce && button.Tag is string newPath)
                    {
                        path = Path.Combine(path, newPath); // Обновляем путь
                        UpdateFolderContent();
                        return;
                    }
                }

                if (updated)
                {
                    UpdateDisplayedItems();
                }
            }
            catch (Exception ex)
            {
                Kernel.ShowMessage($"Update error: {ex.Message}", "Explorer", MsgType.Error);
            }
        }
    }
}
