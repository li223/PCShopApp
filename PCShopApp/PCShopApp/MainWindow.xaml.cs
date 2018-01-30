using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PCShopApp.Objects;
using System.Text.RegularExpressions;
using Npgsql;

namespace PCShopApp
{
    public partial class MainWindow : Window
    {
        public Staff User { get; set; }
        public DatabaseInteractivity DbInter { get; set; }
        public NpgsqlConnection Connection { get; set; }
        private IEnumerable<Order> _orders { get; set; }
        private IEnumerable<Item> _items { get; set; }
        private List<Item> _orderedItems = new List<Item>();
        private List<string> _listBoxItems = new List<string>();

        public MainWindow() => InitializeComponent();
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _orders = await DbInter.SelectAsync<Order>("orders", Connection);
            _items = await DbInter.SelectAsync<Item>("items", Connection);
        }

        private void Button_Click(object sender, RoutedEventArgs e) => tbctrl.SelectedIndex = 3;

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await ResetCreateWindow();
            tbctrl.SelectedIndex = 0;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tbctrl.SelectedIndex = 2;
            ExistingOrders_List.ItemsSource = _orders.Where(a => a.PaymentType == "null").Select(x => $"Order ID: {x.Id} | Customer ID: {x.CustomerId}");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) => Environment.Exit(0);

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            await ResetCreateWindow();
            tbctrl.SelectedIndex = 4;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustID.Text) || string.IsNullOrWhiteSpace(StaffID.Text))
            {
                MessageBox.Show("Must Enter IDs", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            decimal total = 0;
            foreach (var item in _orderedItems) total += item.Price;
            var entries = new string[] { "0", CustID.Text, StaffID.Text, "null", string.Join(",", _orderedItems.Select(x => x.Name)), total.ToString() };
            var rows = await DbInter.InsertAsync(Connection, "orders", entries);
            _orders = await DbInter.SelectAsync<Order>("orders", Connection);
            MessageBox.Show("Order Added", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            await ResetCreateWindow();
            tbctrl.SelectedIndex = 4;
        }

        private void PSUADD_Click(object sender, RoutedEventArgs e)
        {
            var item = PSUCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            PSUADD.IsEnabled = false;
            PSUCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void RAMADD_Click(object sender, RoutedEventArgs e)
        {
            var item = RAMCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            RAMADD.IsEnabled = false;
            RAMCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void CPUADD_Click(object sender, RoutedEventArgs e)
        {
            var item = CPUCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            CPUADD.IsEnabled = false;
            CPUCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void MBADD_Click(object sender, RoutedEventArgs e)
        {
            var item = MBCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            MBADD.IsEnabled = false;
            MBCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void CaseADD_Click(object sender, RoutedEventArgs e)
        {
            var item = CaseCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            CaseADD.IsEnabled = false;
            CaseCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void HDDADD_Click(object sender, RoutedEventArgs e)
        {
            var item = HDDCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void Payment_Button_Click(object sender, RoutedEventArgs e)
        {
            var data = ExistingOrders_List?.SelectedItem?.ToString();
            if (data == null) return;
            var id = int.Parse(new Regex(@"(Order ID: [0-9]+)").Match(data).Value.Replace("Order ID: ", ""));
            var order = _orders.FirstOrDefault(x => x.Id == id);
            CompleteOrderListBox.ItemsSource = order.Parts.Split(',').Select(x => x);
            SubTotalLabel.Content = $"Subtotal: £{Math.Round(order.Total, 2, MidpointRounding.AwayFromZero)}";
            tbctrl.SelectedIndex = 1;
        }

        private async void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            var data = ExistingOrders_List?.SelectedItem?.ToString();
            if (data == null) return;
            var id = int.Parse(new Regex(@"(Order ID: [0-9]+)").Match(data).Value.Replace("Order ID: ", ""));
            await DbInter.DeleteAsync(Connection, "orders", new DBEntityObject() { Column = "OrderId", Value = id.ToString() });
            _orders = await DbInter.SelectAsync<Order>("orders", Connection);
            ExistingOrders_List.ItemsSource = _orders.Where(a => a.PaymentType == "null").Select(x => $"Order ID: {x.Id} | Customer ID: {x.CustomerId}");
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e) => tbctrl.SelectedIndex = 4;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var method = PaymentMethod_Combo.SelectedValue.ToString();
            switch(method)
            {
                case "Pay in full by cash":
                    {
                        TotalCostLabel_Copy.Content = $"{SubTotalLabel.Content.ToString().Replace("Subtotal", "Total")}";
                        InterestLabel.Content = $"Interest: N/A";
                        DepositLabel.Content = $"Deposit: N/A";
                        break;
                    }
                case "Pay in full by card":
                    {
                        TotalCostLabel_Copy.Content = $"{SubTotalLabel.Content.ToString().Replace("Subtotal", "Total")}";
                        InterestLabel.Content = $"Interest: N/A";
                        DepositLabel.Content = $"Deposit: N/A";
                        break;
                    }
                case "6 months interest free":
                    {
                        var deposit = Math.Round((decimal.Parse(SubTotalLabel.Content.ToString().Replace("Subtotal: £", "")) / 10), 2, MidpointRounding.AwayFromZero);
                        var total = Math.Round((decimal.Parse(SubTotalLabel.Content.ToString().Replace("Subtotal: £", "")) - deposit), 2, MidpointRounding.AwayFromZero);
                        total = Math.Round((total / 6), 2, MidpointRounding.AwayFromZero);
                        DepositLabel.Content = $"Deposit: £{deposit}";
                        InterestLabel.Content = $"Interest: N/A";
                        TotalCostLabel_Copy.Content = $"Total: £{total} per month";
                        break;
                    }
                case "12 months 13% interest":
                    {
                        var deposit = Math.Round((decimal.Parse(SubTotalLabel.Content.ToString().Replace("Subtotal: £", "")) / 10), 2, MidpointRounding.AwayFromZero);
                        var total = Math.Round((decimal.Parse(SubTotalLabel.Content.ToString().Replace("Subtotal: £", "")) - deposit), 2, MidpointRounding.AwayFromZero);
                        var interest = Math.Round((total * (decimal)1.3), 2, MidpointRounding.AwayFromZero);
                        total += interest;
                        total = Math.Round((total / 12), 2, MidpointRounding.AwayFromZero);
                        InterestLabel.Content = $"Interest: £{interest}";
                        DepositLabel.Content = $"Deposit: £{deposit}";
                        TotalCostLabel_Copy.Content = $"Total: £{total} per month";
                        break;
                    }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) => tbctrl.SelectedIndex = 4;

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            await DbInter.UpdateAsync(Connection, "orders", new DBEntityObject() { Column = "PaymentType", Value = PaymentMethod_Combo.SelectedValue.ToString() });
            _orders = await DbInter.SelectAsync<Order>("orders", Connection);
            ExistingOrders_List.ItemsSource = _orders.Where(a => a.PaymentType == "null").Select(x => $"Order ID: {x.Id} | Customer ID: {x.CustomerId}");
            tbctrl.SelectedIndex = 2;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            FNText.Clear();
            SNText.Clear();
            TelText.Clear();
            AddressText.Clear();
            tbctrl.SelectedIndex = 4;
        }

        private async void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            var arr = new string[] { FNText.Text, SNText.Text, TelText.Text, AddressText.Text };
            if (arr.Any(x => x == null
            || new Regex(@"(?i)((?:'\);)|(?:DROP (?:DATABASE|TABLE|COLUMN)?)(?: (?:public\.)?[\w]+))").IsMatch(x))
            || TelText.Text.Length != 11)
            {
                MessageBox.Show("There was an error when validating the information");
                return;
            }
            await DbInter.InsertAsync(Connection, "customers", new[] { null, FNText.Text, SNText.Text, TelText.Text, AddressText.Text });
            MessageBox.Show("Customer Added", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            tbctrl.SelectedIndex = 4;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Use the Combo box to select a part type. Then click the add button next to it, to add the part to the list. Once you have added the parts, click the 'Add Order' button", "Help", MessageBoxButton.OK, MessageBoxImage.Information);

        private void GPUADD_Click(object sender, RoutedEventArgs e)
        {
            var item = GPUCombo?.SelectedValue?.ToString();
            if (item == null) return;
            _listBoxItems.Add(item);
            _orderedItems.Add(_items.First(x => x.Name == item));
            GPUADD.IsEnabled = false;
            GPUCombo.IsEnabled = false;
            OrderListBox.ItemsSource = _listBoxItems.Select(x => x);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tbctrl.Height = this.Height;
            tbctrl.Width = this.Width;
        }

        private Task ResetCreateWindow()
        {
            PSUADD.IsEnabled = true;
            CPUADD.IsEnabled = true;
            CaseADD.IsEnabled = true;
            RAMADD.IsEnabled = true;
            MBADD.IsEnabled = true;
            GPUADD.IsEnabled = true;

            PSUCombo.IsEnabled = true;
            CPUCombo.IsEnabled = true;
            CaseCombo.IsEnabled = true;
            RAMCombo.IsEnabled = true;
            MBCombo.IsEnabled = true;
            GPUCombo.IsEnabled = true;

            PSUCombo.SelectedValue = null;
            CPUCombo.SelectedValue = null;
            CaseCombo.SelectedValue = null;
            RAMCombo.SelectedValue = null;
            MBCombo.SelectedValue = null;
            GPUCombo.SelectedValue = null;
            HDDCombo.SelectedValue = null;

            CustID.Clear();
            StaffID.Clear();
            _orderedItems.Clear();
            _listBoxItems.Clear();
            OrderListBox.ItemsSource = _listBoxItems;
            return Task.CompletedTask;
        }
    }
}
