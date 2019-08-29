using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Waiter.Activities;
using Waiter.Classes;

namespace Waiter.Adapters
{
    class OrderListViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly List<OrderedItem> _order;

        public OrderListViewAdapter(Context context, List<OrderedItem> order)
        {
            _context = context;
            _order = order;
        }

        public override int Count => _order.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.OrderListViewLayout, null);
                TextView name = view.FindViewById<TextView>(Resource.Id.orderName);
                TextView obs = view.FindViewById<TextView>(Resource.Id.orderObs);
                CheckBox cb = view.FindViewById<CheckBox>(Resource.Id.orderCb);
                string nume = _order[position].Name;
                if (nume.Length > 30)
                {
                    nume = nume.Substring(0, 30) + "...";
                }
                name.Text = _order[position].Quantity + "X " + nume;
                obs.Text = _order[position].Observation;
                cb.Checked = _order[position].Served;
                cb.CheckedChange += (s, e) =>
                {
                    _order[position].Served = e.IsChecked;
                    TableActivity.UpdateOrderStatus(_order[position].Id, e.IsChecked);
                };
                if (_order[position].Ready)
                {
                    name.SetTextColor(Color.ForestGreen);
                }
            }
            else
            {
                view = convertView;
            }
            return view;
        }
    }
}