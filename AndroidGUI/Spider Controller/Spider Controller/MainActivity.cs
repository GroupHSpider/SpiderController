using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content;
using Android.Util;

namespace Spider_Controller
{
    [Activity(Label = "Spider Controller", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const int REQUEST_CONNECT_DEVICE = 1;
        const int RequestEnableBt = 2;
        private BluetoothAdapter adaptor = null;

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
            ButtonListener buttonListener = new ButtonListener();
            // ToDo: set buttonListener's btAdapter
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
            }

            var serverIntent = new Intent(this, typeof(DeviceListActivity));
            StartActivityForResult(serverIntent, REQUEST_CONNECT_DEVICE);
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
                        // chatService.Connect(device);

                    }
                    break;
                case RequestEnableBt:
                    // When the request to enable Bluetooth returns
                    if (resultCode == Result.Ok)
                    {
                        // Bluetooth is now enabled, so set up a chat session
                        //SetupChat();
                        //todo: set up a session for sending signal to the sipder
                       
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

