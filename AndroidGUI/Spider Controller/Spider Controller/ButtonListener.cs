using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Spider_Controller
{
    class ButtonListener : Java.Lang.Object, View.IOnTouchListener
    {
        public bool OnTouch(View v, MotionEvent e)
        { 
            switch (v.Id)
            {
                case Resource.Id.button_forward:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            Toast.MakeText(Application.Context, "forward", ToastLength.Short).Show();
                            break;
                        case MotionEventActions.Up:
                            Toast.MakeText(Application.Context, "stop", ToastLength.Short).Show();
                            break;
                    }
                    break;
                case Resource.Id.button_backward:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            Toast.MakeText(Application.Context, "back", ToastLength.Short).Show();
                            break;
                        case MotionEventActions.Up:
                            Toast.MakeText(Application.Context, "stop", ToastLength.Short).Show();
                            break;
                    }
                    break;
        }

            
            return true;
        }
    }
}