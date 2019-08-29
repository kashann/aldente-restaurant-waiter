using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Waiter.Classes;

namespace Waiter.Adapters
{
    class CustomListViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly List<Table> _tables;

        public CustomListViewAdapter(Context context, List<Table> tables)
        {
            _context = context;
            _tables = tables;
        }

        public override int Count => _tables.Count;

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
                view = inflater.Inflate(Resource.Layout.ListViewLayout, null);
                TextView table = view.FindViewById<TextView>(Resource.Id.listTable);
                TextView status = view.FindViewById<TextView>(Resource.Id.listStatus);
                table.Text = "Table " + _tables[position].TableNumber;
                status.Text = _tables[position].Status.ToString().ToUpper();
                switch (_tables[position].Status)
                {
                    case EStatus.Thinking:
                        status.SetTextColor(Color.Black);
                        break;
                    case EStatus.Ordered:
                        status.SetTextColor(Color.OrangeRed);
                        break;
                    case EStatus.Waiting:
                        status.SetTextColor(Color.Red);
                        break;
                    case EStatus.Served:
                        status.SetTextColor(Color.Black);
                        break;
                    case EStatus.Bill:
                        status.SetTextColor(Color.Green);
                        break;
                    case EStatus.Paid:
                        status.SetTextColor(Color.ForestGreen);
                        break;
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