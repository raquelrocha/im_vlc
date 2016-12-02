using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Paelife.KinectFramework;
using Paelife.KinectFramework.Gestures;

namespace VLC_KinectControl
{
    public partial class Form1 : Form
    {
        Request rqt;
        public Form1()
        {
            InitializeComponent();
            rqt = new Request();
        }

        //SimpleKinectManager já adiciona o swipeLeft e swipeRight
        //E retorna mais opções em PropertyChange
        //Paelife.KinectFramework.SimpleKinectManager skm;
        
        KinectManager km;

        private void Form1_Load(object sender, EventArgs e)
        {

            //Inicializar Kinect Manager
            km = new KinectManager();

            //Add Property change to Manager
            km.PropertyChanged += km_PropertyChanged;


            //Add Swipe Left or Right with the Right Hand
            GestureDetector RightHandSwipeGestureDetector =
                new SwipeGestureDetector(Microsoft.Kinect.JointType.HandRight);
            RightHandSwipeGestureDetector.OnGestureDetected += new Action<string>(SwipeGestureDetector_OnGestureDetected); ;
            km.GestureDetectors.Add(RightHandSwipeGestureDetector);

            //Add Swipe Left or Right with the Left Hand
            GestureDetector LeftHandSwipeGestureDetector =
                new SwipeGestureDetector(Microsoft.Kinect.JointType.HandLeft);
            LeftHandSwipeGestureDetector.OnGestureDetected += new Action<string>(SwipeGestureDetector_OnGestureDetected); ;
            km.GestureDetectors.Add(LeftHandSwipeGestureDetector);


            //Add UP and Down Gesture to Right Hand
            GestureDetector RightHandUpDownGestureDetector =
                new UpDownGestures(Microsoft.Kinect.JointType.HandRight);
            RightHandUpDownGestureDetector.OnGestureDetected += new Action<string>(SwipeGestureDetector_OnGestureDetected); ;
            km.GestureDetectors.Add(RightHandUpDownGestureDetector);

        }

        private void SwipeGestureDetector_OnGestureDetected(string gesture)
        {
            //Print message on Gesture
            swipeText.Text = gesture + ":: time: " + DateTime.Now.Ticks;
            textBox1.Text += gesture + ":: time: " + DateTime.Now.Ticks + System.Environment.NewLine;

            switch (gesture)
            {
                case "SwipeDown":
                    rqt.changeVolume(-2);
                    break;
                case "SwipeUp":
                    rqt.changeVolume(2);
                    break;
            }
        }


        void km_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //print message on property change
            textBox1.Text += e.PropertyName + " -- ";
            if (e.PropertyName.Equals("NumberOfDetectUsers"))
                textBox1.Text += km.NumberOfDetectUsers;

            textBox1.Text += System.Environment.NewLine;
        }


    }
}
