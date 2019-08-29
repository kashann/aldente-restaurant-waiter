using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Waiter.Adapters;
using Waiter.Classes;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Waiter.Activities
{
    [Activity(Theme = "@style/MyTheme")]
    public class TableActivity : AppCompatActivity
    {
        private static int _pos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Table);
            ConfigureToolbar();
            _pos = Intent.GetIntExtra("table_nr", -1);
            Title = "TABLE " + MainActivity.Tables[_pos].TableNumber;
            var status = FindViewById<Spinner>(Resource.Id.statusSpinner);
            var total = FindViewById<TextView>(Resource.Id.tvTotal);
            var lv = FindViewById<ListView>(Resource.Id.lvOrders);
            OrderListViewAdapter listAdapter = new OrderListViewAdapter(this, MainActivity.Tables[_pos].Order);
            lv.Adapter = listAdapter;
            total.Text = "Total: " + MainActivity.Tables[_pos].Total + " RON (" + MainActivity.Tables[_pos].Tip + ") - " +
                         MainActivity.Tables[_pos].Payment;
            var spinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.status, Resource.Layout.SpinnerItemLayout);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            status.Adapter = spinnerAdapter;
            switch(MainActivity.Tables[_pos].Status)
            {
                case EStatus.Thinking:
                    status.SetSelection(0);
                    total.Visibility = ViewStates.Invisible;
                    break;
                case EStatus.Ordered:
                    status.SetSelection(1);
                    total.Visibility = ViewStates.Invisible;
                    break;
                case EStatus.Waiting:
                    status.SetSelection(2);
                    total.Visibility = ViewStates.Invisible;
                    break;
                case EStatus.Served:
                    status.SetSelection(3);
                    total.Visibility = ViewStates.Invisible;
                    break;
                case EStatus.Bill:
                    status.SetSelection(4);
                    total.Visibility = ViewStates.Visible;
                    break;
                case EStatus.Paid:
                    status.SetSelection(5);
                    total.Visibility = ViewStates.Visible;
                    break;
            }
            status.ItemSelected += (sender, args) =>
            {
                Spinner s = (Spinner) sender;
                MainActivity.Tables[_pos].Status = (EStatus)Enum.Parse(typeof(EStatus), s.GetItemAtPosition(args.Position).ToString(), true);
                if(MainActivity.Tables[_pos].Status == EStatus.Bill || MainActivity.Tables[_pos].Status == EStatus.Paid)
                    total.Visibility = ViewStates.Visible;
                else total.Visibility = ViewStates.Invisible;
            };
        }

        private void ConfigureToolbar()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbarTable);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        #region Server
        private async void UpdateTable()
        {
            var endpoint = new Uri(MainActivity.WebApi + "tables/" + MainActivity.Tables[_pos].TableNumber);
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    status = MainActivity.Tables[_pos].Status.ToString()
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await MainActivity.Client.PutAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                if (responseMessage == null)
                {
                    responseMessage = new HttpResponseMessage();
                }
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.ReasonPhrase = $"RestHttpClient.SendRequest failed: {ex}";
                Toast.MakeText(Application.Context, "Server error!", ToastLength.Short).Show();
            }
        }

        public static async void UpdateOrderStatus(int id, bool value)
        {
            var endpoint = new Uri(MainActivity.WebApi + "tables/" + MainActivity.Tables[_pos].TableNumber + "/orders/" + id);
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    served = value.ToString().ToLower()
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await MainActivity.Client.PutAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                if (responseMessage == null)
                {
                    responseMessage = new HttpResponseMessage();
                }
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.ReasonPhrase = $"RestHttpClient.SendRequest failed: {ex}";
                Toast.MakeText(Application.Context, "Server error!", ToastLength.Short).Show();
            }
        }

        private async void DeleteTable()
        {
            await MainActivity.Client.DeleteAsync(MainActivity.WebApi + "tables/" + MainActivity.Tables[_pos].TableNumber);
        }
        #endregion

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            Toast.MakeText(this, "Status not updated!", ToastLength.Short).Show();
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbarTable, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_save:
                    UpdateTable();
                    OnBackPressed();
                    Toast.MakeText(this, "Table " + MainActivity.Tables[_pos].TableNumber + " updated!", ToastLength.Short).Show();
                    break;
                case Resource.Id.menu_delete:
                    DeleteTable();
                    OnBackPressed();
                    Toast.MakeText(this, "Table " + MainActivity.Tables[_pos].TableNumber + " deleted!", ToastLength.Short).Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}