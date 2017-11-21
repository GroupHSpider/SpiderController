using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;

namespace Spider_Controller
{
    [Activity(Label = "Spider Controller", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ImageButton buttonForward, buttonBackward, buttonLeft, buttonRight;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);

            buttonForward = FindViewById<ImageButton>(Resource.Id.button_forward);
            buttonBackward = FindViewById<ImageButton>(Resource.Id.button_backward);
            buttonLeft = FindViewById<ImageButton>(Resource.Id.button_left);
            buttonRight = FindViewById<ImageButton>(Resource.Id.button_right);

            // Add custom touch listener to register when button is held down
            ButtonListener buttonListener = new ButtonListener();
            buttonForward.SetOnTouchListener(buttonListener);
            buttonBackward.SetOnTouchListener(buttonListener);
            buttonLeft.SetOnTouchListener(buttonListener);
            buttonRight.SetOnTouchListener(buttonListener);

        }
    }
}

