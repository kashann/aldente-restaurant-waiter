using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Waiter.Activities
{
    [Activity(Label = "@string/guide", Theme = "@style/MyTheme")]
    public class TableSettingActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TableSetting);
            ConfigureToolbar();
        }

        private void ConfigureToolbar()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbarTable);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbarTableSetting, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.basic:
                    FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.basic);
                    break;
                case Resource.Id.formal:
                    FindViewById<ImageView>(Resource.Id.imageView).SetImageResource(Resource.Drawable.formal);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}