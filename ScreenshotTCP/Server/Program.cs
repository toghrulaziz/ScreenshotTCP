using System.Drawing;
using System.Net;
using System.Net.Sockets;

var ipAdress = IPAddress.Parse("192.168.0.104");
var port = 27001;

TcpListener listener = new(ipAdress, port);

listener.Start();

Console.WriteLine($"Listening at {listener.LocalEndpoint}...");


while (true)
{
    var client = listener.AcceptTcpClient();

    Console.WriteLine($"{client.Client.RemoteEndPoint} connected...");

    Task.Run(() =>
    {
        while (true)
        {
            var stream = client.GetStream();
            var image = ScreenShot();

            ImageConverter imageConverter = new();

            var bytes = (byte[])imageConverter.ConvertTo(image, typeof(byte[]))!;
            stream.Write(bytes);
            Console.WriteLine("Screenshoted!");

            Console.WriteLine($"{bytes.Length} Sent");

            stream.Close();

            Task.Delay(5000).Wait();
        }

    });


}


Bitmap ScreenShot()
{
    Bitmap memoryImage;
    memoryImage = new Bitmap(1920, 1080);
    Size s = new Size(memoryImage.Width, memoryImage.Height);

    Graphics memoryGraphics = Graphics.FromImage(memoryImage);

    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

    return memoryImage;
}

