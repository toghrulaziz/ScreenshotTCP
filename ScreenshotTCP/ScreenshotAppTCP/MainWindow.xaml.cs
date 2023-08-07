using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenshotAppTCP
{
    
    public partial class MainWindow : Window
    {
        public BitmapImage PhotoSource
        {
            get { return (BitmapImage)GetValue(PhotoSourceProperty); }
            set { SetValue(PhotoSourceProperty, value); }
        }


        public static readonly DependencyProperty PhotoSourceProperty =
            DependencyProperty.Register("PhotoSource", typeof(BitmapImage), typeof(MainWindow));


        public TcpClient Client { get; set; }
        public int Port { get; set; }
        public IPAddress IpAddress { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Port = 27001;
            IpAddress = IPAddress.Parse("192.168.0.104");
        }


        public RelayCommand StartCommand
        {
            get => new RelayCommand(() =>
            {

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Client = new();
                        Client.Connect(IpAddress, Port);
                        while (true)
                        {
                            if (Client.Connected)
                            {
                                try
                                {
                                    using (NetworkStream networkStream = Client.GetStream())
                                    {

                                        byte[] imageData = new byte[4096];
                                        int bytesRead;
                                        using (MemoryStream memoryStream = new MemoryStream())
                                        {

                                            while ((bytesRead = networkStream.Read(imageData, 0, imageData.Length)) > 0)
                                            {
                                                memoryStream.Write(imageData, 0, bytesRead);
                                            }

                                            memoryStream.Seek(0, SeekOrigin.Begin);
                                            BitmapImage bitmapImage = new BitmapImage();
                                            bitmapImage.BeginInit();
                                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                            bitmapImage.StreamSource = memoryStream;
                                            bitmapImage.EndInit();
                                            bitmapImage.Freeze();

                                            Dispatcher.Invoke(() => PhotoSource = bitmapImage);

                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error loading the image: " + ex.Message);
                                }

                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }, TaskCreationOptions.LongRunning);

            });
        }



    }
}
