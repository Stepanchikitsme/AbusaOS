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
        private const int maxItemsToShow = 4;

        private string[] cachedDirs;
        private string[] cachedFiles;

        private Button folderScrollUp;
        private Button folderScrollDown;
        private Button fileScrollUp;
        private Button fileScrollDown;

        public Explorer() : base(100, 100, 600, 500, "Explorer", Kernel.defFont)
        {
            try
            {
                folderImg = new Bitmap(folderIcon);
                fileImg = new Bitmap(fileIcon);

                Label toolbar = new Label("Toolbar", 20, 20, font, Kernel.textColDark);
                controls.Add(toolbar);

                Label folderLabel = new Label("Folders", 20, 60, font, Kernel.textColDark);
                controls.Add(folderLabel);

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

                int folderYPosition = 100;
                int fileYPosition = 100;

                Label statusBar = new Label($"Status: Browsing {path}", 20, 450, font, Kernel.textColDark);
                controls.Add(statusBar);

                for (int i = folderScrollOffset; i < Math.Min(cachedDirs.Length, folderScrollOffset + maxItemsToShow); i++)
                {
                    Button dirItem = new Button(Path.GetFileName(cachedDirs[i]), 20, folderYPosition, Color.Black, font, 12, folderImg)
                    {
                        Tag = cachedDirs[i]
                    };
                    controls.Add(dirItem);
                    folderYPosition += 40;
                }

                for (int i = fileScrollOffset; i < Math.Min(cachedFiles.Length, fileScrollOffset + maxItemsToShow); i++)
                {
                    Button fileItem = new Button(Path.GetFileName(cachedFiles[i]), 200, fileYPosition, Color.Black, font, 12, fileImg);
                    controls.Add(fileItem);
                    fileYPosition += 40;
                }

                if (cachedDirs.Length > maxItemsToShow)
                {
                    folderScrollUp = new Button("Up", 150, 100, Color.Black, font, 12);
                    controls.Add(folderScrollUp);

                    folderScrollDown = new Button("Down", 150, 200, Color.Black, font, 12);
                    controls.Add(folderScrollDown);
                }

                if (cachedFiles.Length > maxItemsToShow)
                {
                    fileScrollUp = new Button("Up", 550, 100, Color.Black, font, 12);
                    controls.Add(fileScrollUp);

                    fileScrollDown = new Button("Down", 550, 200, Color.Black, font, 12);
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

                foreach (var control in controls)
                {
                    if (control is Button button && button.clickedOnce && button.Tag is string newPath)
                    {
                        path = Path.Combine(path, newPath); // Update path
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
