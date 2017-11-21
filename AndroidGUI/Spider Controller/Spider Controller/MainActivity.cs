using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System.Collections.Generic;

namespace Spider_Controller
{
    [Activity(Label = "Spider Controller", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);

            // ToDo: create BluetoothAdapter and connect to spider

            List<ImageButton> directionalPad = new List<ImageButton>();
            directionalPad.Add(FindViewById<ImageButton>(Resource.Id.button_forward));
            directionalPad.Add(FindViewById<ImageButton>(Resource.Id.button_backward));
            directionalPad.Add(FindViewById<ImageButton>(Resource.Id.button_left));
            directionalPad.Add(FindViewById<ImageButton>(Resource.Id.button_right));

            // Add custom touch listener to register when button is held down
            ButtonListener buttonListener = new ButtonListener();
            // ToDo: set buttonListener's btAdapter
            foreach (var button in directionalPad) {
                button.SetOnTouchListener(buttonListener);
            }
        }
    }
}

