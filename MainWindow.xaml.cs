using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace ingif
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isRecording;
        private Recorder recorder;
        public MainWindow()
        {
            InitializeComponent();
            isRecording= false; 
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && isRecording == false)
            {
                this.DragMove();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecording == false)
            {
                isRecording = true;
                RecordImage.Source = new BitmapImage(new Uri("resources/record_done.png", UriKind.Relative));

                int X = Convert.ToInt32(Application.Current.MainWindow.Left);
                int Y = Convert.ToInt32(Application.Current.MainWindow.Top);
                int Width = Convert.ToInt32(Application.Current.MainWindow.Width);
                int Height = Convert.ToInt32(Application.Current.MainWindow.Height);
                Trace.WriteLine(String.Format("Size: {0}, {1}", Width, Height));
                Trace.WriteLine(String.Format("Position: {0}, {1}", X, Y));

                this.recorder = new Recorder();
                recorder.Start(X, Y, Width, Height);
            } 
            else
            {
                recorder.Stop();
                isRecording = false;
                RecordImage.Source = new BitmapImage(new Uri("resources/record_ready.png", UriKind.Relative));

                // send to editor
            }
        }
    }
}
