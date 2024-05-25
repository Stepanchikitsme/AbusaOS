using AbusaOS.Controls;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace AbusaOS.Windows
{
    internal class About : Window
    {
        [ManifestResourceStream(ResourceName = "AbusaOS.Resource.logotext.bmp")]
        static byte[] logotext;
        Bitmap logoImg;

        Label creds, creds1, creds2, creds3;
        ImageView logoView;
        public About() : base(300, 300, 500, 170, "System info", Kernel.defFont)
        {
            logo = Kernel.logo;
            logoImg = new Bitmap(logotext);
            creds = new("Created by", 20, 60, font, Kernel.textColDark);
            creds1 = new("Abusa Development Group LLC", 40, 80, font, Kernel.textColDark);
            creds2 = new("OS creator: CHERN STEPANOV", 20, 100, font, Kernel.textColDark);
            creds2 = new("Credits: Iceik _Kot (Design)", 20, 100, font, Kernel.textColDark);
            creds3 = new($"Version {Kernel.version}", 20, 130, font, Kernel.textColDark);
            logoView = new(logoImg, 20, 10);
            controls.Add(creds);
            controls.Add(creds1);
            controls.Add(creds2);
            controls.Add(creds3);
            controls.Add(logoView);
        }
    }
}
