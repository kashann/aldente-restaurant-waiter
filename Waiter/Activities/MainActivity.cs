using System;
using System.Collections.Generic;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Org.Json;
using Waiter.Adapters;
using Waiter.Classes;
using Exception = System.Exception;
using ListView = Android.Widget.ListView;
using Object = Java.Lang.Object;
using Socket = SocketIO.Client.Socket;

namespace Waiter.Activities
{
    [Activity(Theme = "@style/MyTheme", Icon = "@drawable/aldente", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        #region Attributes
        public static List<Table> Tables = new List<Table>();
        private const string SharedPreferencesString = "WAITER";
        public static int WaiterId;
        public const string WebApi = "https://webtech-kashann.c9users.io/";
        public static HttpClient Client = new HttpClient();
        public static Socket Socket = new SocketHelper().GetSocket();
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetFormat(Format.Rgba8888);
            SetContentView(Resource.Layout.Main);
            ConfigureToolbar();
            ConfigureListView();
            GetWaiterId();
            GetTables();
            #region Socket
            try
            {
                Socket.On("waiter", delegate (Object[] objects)
                {
                    JSONObject msg = (JSONObject)objects[0];
                    int id = -1;
                    int table = -1;
                    string status = "";
                    try
                    {
                        id = msg.GetInt("id");
                        status = msg.GetString("status");
                        table = msg.GetInt("table");
                    }
                    catch (JSONException e)
                    {
                        Console.WriteLine(e);
                    }
                    if (id == WaiterId)
                    {
                        RunOnUiThread(() =>
                        {
                            GetTables();
                            Notification(table, status);
                        });
                    }
                });
                Socket.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            #endregion
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Socket.Disconnect();
        }

        public void ConfigureListView()
        {
            CustomListViewAdapter adapter = new CustomListViewAdapter(this, Tables);
            var lv = FindViewById<ListView>(Resource.Id.listView);
            lv.Adapter = adapter;
            lv.ItemClick += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(TableActivity));
                intent.PutExtra("table_nr", e.Position);
                StartActivity(intent);
            };
        }

        private async void GetTables()
        {
            try
            {
                var endpoint = new Uri(WebApi + "tables/waiter/" + WaiterId);
                var content = "";
                HttpResponseMessage response = await Client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                }
                Tables = JsonConvert.DeserializeObject<List<Table>>(content);
                FindViewById<ListView>(Resource.Id.listView).Dispose();
                ConfigureListView();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Toast.MakeText(this, "Server error!", ToastLength.Short).Show();
            }
        }

        #region Notify
        private void Vibrate()
        {
            Vibrator vibrator = (Vibrator)GetSystemService(VibratorService);
            long[] pattern = { 0, 400, 600, 800 };
            vibrator.Vibrate(pattern, -1);
        }

        private void Notification(int table, string status)
        {
            string text = "";
            switch (status)
            {
                case "Thinking":
                    return;
                case "Ordered":
                    text = "The table has submitted a new order!";
                    break;
                case "Waiting":
                    text = "The table needs your service!";
                    break;
                case "Served":
                    return;
                case "Bill":
                    text = "The table has requested the check!";
                    break;
                case "Paid":
                    return;
                case "Ready":
                    text = "One or more orders are ready to be served!";
                    break;
            }
            NotificationCompat.Builder builder = (NotificationCompat.Builder) new NotificationCompat.Builder(this)
                .SetAutoCancel(true)
                .SetContentTitle($"Table {table}")
                .SetSmallIcon(Resource.Drawable.aldente_white)
                .SetContentText(text);

            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(1, builder.Build());
            Vibrate();
        }
        #endregion

        #region SharedPreferences
        public void GetWaiterId()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            var nr = pref.GetString("waiter_id", null);
            if (nr == null)
            {
                WaiterId = -1;
                Toast.MakeText(this, "Please set the waiter ID!", ToastLength.Long).Show();
            }
            else
                WaiterId = int.Parse(nr);
        }

        public static void SaveWaiterId()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("waiter_id", WaiterId.ToString()).Apply();
        }
        #endregion

        #region Toolbar
        private void ConfigureToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetTitle(Resource.String.app_name);
            SupportActionBar.SetHomeButtonEnabled(false);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Mipmap.restaurant_white_48dp);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbarMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_guide:
                    Intent intent = new Intent(this, typeof(TableSettingActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_refresh:
                    Toast.MakeText(this, "Refreshing...", ToastLength.Short).Show();
                    GetTables();
                    break;
                case Resource.Id.menu_settings:
                    FragmentTransaction transactionSettings = FragmentManager.BeginTransaction();
                    SettingsDialogFragment dialogSettings = new SettingsDialogFragment();
                    dialogSettings.SetStyle(DialogFragmentStyle.NoFrame, 0);
                    dialogSettings.Show(transactionSettings, "dialog_settings");
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion
    }
}