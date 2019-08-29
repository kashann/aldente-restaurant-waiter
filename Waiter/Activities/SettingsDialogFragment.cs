using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Waiter.Activities
{
    class SettingsDialogFragment : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SettingsDialogFragment, container, false);
            view.FindViewById<Button>(Resource.Id.settingsBack).Click += (sender, args) => Dismiss();
            var etWaiter = view.FindViewById<EditText>(Resource.Id.etWaiter);
            if (MainActivity.WaiterId > 0)
                etWaiter.Text = MainActivity.WaiterId.ToString();
            var btnSet = view.FindViewById<Button>(Resource.Id.settingsSet);
            btnSet.Click += (sender, args) =>
            {
                int w;
                var isWaiterNumeric = int.TryParse(etWaiter.Text, out w);
                if (!string.IsNullOrEmpty(etWaiter.Text) &&  w > 0 && etWaiter.Text.Length < 4 && isWaiterNumeric)
                {
                    MainActivity.WaiterId = w;
                    MainActivity.SaveWaiterId();
                    Dismiss();
                    Toast.MakeText(Activity, "Device setted to waiter id " + w, ToastLength.Long).Show();
                    Toast.MakeText(Activity, "Please REFRESH!", ToastLength.Long).Show();
                }
                else Toast.MakeText(Activity, "Wrong input!", ToastLength.Short).Show();
            };
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.DimAmount = 0.80f;
            Dialog.Window.AddFlags(WindowManagerFlags.DimBehind);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.DialogAnimation;
        }
    }
}