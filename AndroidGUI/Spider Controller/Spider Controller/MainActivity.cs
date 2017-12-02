using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content;
using Android.Util;
using Java.Lang;
using System;
using Java.Util.Logging;
using System.Threading;

namespace Spider_Controller
{
    [Activity(Label = "Spider Controller", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Message types sent from the BluetoothService Handler
        public const int MESSAGE_STATE_CHANGE = 1;
        public const int MESSAGE_READ = 2;
        public const int MESSAGE_WRITE = 3;
        public const int MESSAGE_DEVICE_NAME = 4;
        public const int MESSAGE_TOAST = 5;

        // Key names received from the BluetoothChatService Handler
        public const string DEVICE_NAME = "device_name";
        public const string TOAST = "toast";

        private const int REQUEST_CONNECT_DEVICE = 1;
        const int RequestEnableBt = 2;

        private BluetoothAdapter adaptor = null;
        private BluetoothService service = null;
        // String buffer for outgoing messages
        private StringBuffer outStringBuffer;
        // Name of the connected device
        protected string connectedDeviceName = null;
        // Array adapter for the conversation thread
        protected ArrayAdapter<string> conversationArrayAdapter;

        public Timer timer;

        // string to get from the buttonListener and control robot commands
        private string movementState = "stop";

        public string MovementState { get => movementState; set => movementState = value; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);

            //Getting bluetooth connection
            adaptor = BluetoothAdapter.DefaultAdapter;

            if (adaptor == null)
            {
                Toast.MakeText(Application.Context, "Bluetooth is not available", ToastLength.Short).Show();
                Finish();
                return;
            }
            
            // ToDo: create BluetoothAdapter and connect to spider
            List<ImageButton> directionalPad = new List<ImageButton>
            {
                FindViewById<ImageButton>(Resource.Id.button_forward),
                FindViewById<ImageButton>(Resource.Id.button_backward),
                FindViewById<ImageButton>(Resource.Id.button_left),
                FindViewById<ImageButton>(Resource.Id.button_right)
            };

            // Add custom touch listener to register when button is held down
            ButtonListener buttonListener = new ButtonListener(this);
            foreach (var button in directionalPad) {
                button.SetOnTouchListener(buttonListener);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (!adaptor.IsEnabled)
            {
                Intent enablebt = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enablebt, RequestEnableBt);
            } else
            {
                if (service == null)
                    SetupService();
            }

            var serverIntent = new Intent(this, typeof(DeviceListActivity));
            StartActivityForResult(serverIntent, REQUEST_CONNECT_DEVICE);


        }

        private void SendMessageFromState()
        {
            var message = "";
            switch(movementState)
            {
                case "stop":
                    message = "ATST\r";
                    break;
                case "forward":
                    message = "ATFW\r";
                    break;
                case "backward":
                    message = "ATBW\r";
                    break;
                case "left":
                    message = "ATTL\r";
                    break;
                case "right":
                    message = "ATTR\r";
                    break;
            }
            SendMessage(new Java.Lang.String(message));
        }

        private void SetupService()
        {
            service = new BluetoothService(this, new MyHandler(this));
            outStringBuffer = new StringBuffer("");

        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name='message'>
        /// A string of text to send.
        /// </param>
        private void SendMessage(Java.Lang.String message)
        {
            // Check that we're actually connected before trying anything
            if (service.GetState() != 3)
            {
                return;
            }

            // Check that there's actually something to send
            if (message.Length() > 0)
            {
                // Get the message bytes and tell the BluetoothChatService to write
                byte[] send = message.GetBytes();
                service.Write(send);

                // Reset out string buffer to zero and clear the edit text field
                outStringBuffer.SetLength(0);
            }
        }

        // The Handler that gets information back from the BluetoothChatService
        private class MyHandler : Android.OS.Handler
        {
            MainActivity activity;

            public MyHandler(MainActivity main)
            {
                activity = main;
            }

            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case MESSAGE_STATE_CHANGE:
                        switch (msg.Arg1)
                        {
                            case 3:
                                //activity.title.SetText(Resource.String.title_connected_to);
                                //activity.title.Append(activity.connectedDeviceName);
                                //activity.conversationArrayAdapter.Clear();
                                break;
                            case 2:
                                //activity.title.SetText(Resource.String.title_connecting);
                                break;
                            case 1:
                            case 0:
                                //activity.title.SetText(Resource.String.title_not_connected);
                                break;
                        }
                        break;
                    case MESSAGE_WRITE:
                        byte[] writeBuf = (byte[])msg.Obj;
                        // construct a string from the buffer
                        var writeMessage = new Java.Lang.String(writeBuf);
                        activity.conversationArrayAdapter.Add("Me: " + writeMessage);
                        break;
                    case MESSAGE_READ:
                        byte[] readBuf = (byte[])msg.Obj;
                        // construct a string from the valid bytes in the buffer
                        var readMessage = new Java.Lang.String(readBuf, 0, msg.Arg1);
                        activity.conversationArrayAdapter.Add(activity.connectedDeviceName + ":  " + readMessage);
                        break;
                    case MESSAGE_DEVICE_NAME:
                        // save the connected device's name
                        activity.connectedDeviceName = msg.Data.GetString(DEVICE_NAME);
                        Toast.MakeText(Application.Context, "Connected to " + activity.connectedDeviceName, ToastLength.Short).Show();
                        break;
                    case MESSAGE_TOAST:
                        Toast.MakeText(Application.Context, msg.Data.GetString(TOAST), ToastLength.Short).Show();
                        break;
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            switch (requestCode)
            {
                case REQUEST_CONNECT_DEVICE:
                    // When DeviceListActivity returns with a device to connect
                    if (resultCode == Result.Ok)
                    {
                        // Get the device MAC address
                        var address = data.Extras.GetString(DeviceListActivity.EXTRA_DEVICE_ADDRESS);
                        // Get the BLuetoothDevice object
                        BluetoothDevice device = adaptor.GetRemoteDevice(address);

                        // Attempt to connect to the device
                        service.Connect(device);


                        // Call movement command every second
                        var startTimeSpan = TimeSpan.Zero;
                        var periodTimeSpan = TimeSpan.FromSeconds(1);
                        timer = new Timer((e) =>
                        {
                            SendMessageFromState();
                        }, null, startTimeSpan, periodTimeSpan);
                    }
                    break;
                case RequestEnableBt:
                    // When the request to enable Bluetooth returns
                    if (resultCode == Result.Ok)
                    {
                        // Bluetooth is now enabled, so set up a chat session
                        SetupService();
                       
                    }
                    else
                    {
                        // User did not enable Bluetooth or an error occured
                        Toast.MakeText(this, "Bluetooth not enabled", ToastLength.Short).Show();
                        Finish();
                    }
                    break;
            }
        }
    }

}

