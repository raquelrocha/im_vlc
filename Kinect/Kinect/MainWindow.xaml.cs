using System;
using System.Collections.Generic;
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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Runtime.InteropServices;

namespace Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Request rst;
        private KinectSensorChooser sensorChooser;
        public MainWindow()
        {
            InitializeComponent();
            rst = new Request();
            rst.play();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }
        
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException inEX) { error = true; }
            }

            if (args.NewSensor != null)
            {
                switch (Convert.ToString(args.NewSensor.Status))
                {
                    case "Undefined": KinectStatus.Content = "Undefined"; break;
                    case "Disconnected": KinectStatus.Content = "Disconnected"; break;
                    case "Connected": KinectStatus.Content = "Connected"; break;
                    case "Initializing": KinectStatus.Content = "Initializing"; break;
                    case "Error": KinectStatus.Content = "Error"; break;
                    case "NotPowered": KinectStatus.Content = "NotPowered"; break;
                    case "NotReady": KinectStatus.Content = "NotReady"; break;
                    case "DeviceNotGenuine": KinectStatus.Content = "DeviceNotGenuine"; break;
                    case "DeviceNotSupported": KinectStatus.Content = "DeviceNotSupported"; break;
                    case "InsufficientBandwidth": KinectStatus.Content = "InsufficientBandwidth"; break;
                    default: KinectStatus.Content = "Undefined"; break;
                }
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();
                   /* try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException inEX)
                    {*/
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                       // error = true;
                    //}
                }
                catch (InvalidOperationException inEX) { error = true; }
            }
            if (!error) {
                kinectRegion.KinectSensor = args.NewSensor; 
            }
        }



        public void OnClickPause(object sender, RoutedEventArgs args)
        {
            rst.pause();

        }

        public void OnClickPlay(object sender, RoutedEventArgs args)
        {
            rst.play();
        }

        public void OnClickPrevious(object sender, RoutedEventArgs args)
        {
            rst.previousFile();
        }

        public void OnClickNext(object sender, RoutedEventArgs args)
        {
            rst.nextFile();
        }

        public void OnClickPlus(object sender,RoutedEventArgs args)
        {
            rst.changeVolume(2);
        }

        public void OnClickMinus(object sender, RoutedEventArgs args)
        {
            rst.changeVolume(-2);
        }
    }
}
