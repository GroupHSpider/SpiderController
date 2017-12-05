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
using Android.Bluetooth;

namespace Spider_Controller
{
    class ButtonListener : Java.Lang.Object, View.IOnTouchListener
    {
        private MainActivity mainActivity;

        public ButtonListener(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public bool OnTouch(View v, MotionEvent e)
        { 
            switch (v.Id)
            {
                case Resource.Id.button_forward:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            mainActivity.MovementState = "forward";
                            break;
                        case MotionEventActions.Up:
                            mainActivity.MovementState = "stop";
                            break;
                    }
                    break;
                case Resource.Id.button_backward:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            mainActivity.MovementState = "backward";
                            break;
                        case MotionEventActions.Up:
                            mainActivity.MovementState = "stop";
                            break;
                    }
                    break;
                case Resource.Id.button_left:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            mainActivity.MovementState = "left";
                            break;
                        case MotionEventActions.Up:
                            mainActivity.MovementState = "stop";
                            break;
                    }
                    break;
                case Resource.Id.button_right:
                    switch (e.Action)
                    {
                        case MotionEventActions.Down:
                            mainActivity.MovementState = "right";
                            break;
                        case MotionEventActions.Up:
                            mainActivity.MovementState = "stop";
                            break;
                    }
                    break;
            }
            return true;
        }
        
    }
}