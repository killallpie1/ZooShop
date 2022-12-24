using Bredikhin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bredikhin.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        readonly TradeEntities _tradeEntities = new TradeEntities();
        readonly IQueryable<Product> _products = new TradeEntities().Product;
        readonly List<string> manufacturers;
        public MainPage(int roleId)
        {
            InitializeComponent();
            if (roleId == 1)
            {
                AdminPanel.Visibility = System.Windows.Visibility.Visible;
            }
            ItemsList.ItemsSource = _products.ToList();
            manufacturers = (from p in _products select p.ProductManufacturer).Distinct().ToList();
            manufacturers.Add("Все производители");
            ManufacturerFilter.ItemsSource = manufacturers;
            ManufacturerFilter.SelectedIndex = manufacturers.IndexOf("Все производители");
        }

        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchField.Text = "";
            ManufacturerFilter.SelectedIndex = manufacturers.IndexOf("Все производители");
            SortAscending.IsChecked = false;
            SortDescending.IsChecked = false;
            Update();
        }

        private void Update()
        {
            List<Product> products = _products.ToList();
            if (SearchField.Text != "")
            {
                products = products.FindAll((e) =>
                {
                    return e.ProductName.ToLower().Contains(SearchField.Text.ToLower()) ||
                           e.ProductManufacturer.ToLower().Contains(SearchField.Text.ToLower()) ||
                           e.ProductCategory.ToLower().Contains(SearchField.Text.ToLower()) ||
                           e.ProductDescription.ToLower().Contains(SearchField.Text.ToLower());
                });
            }
            if ((string)ManufacturerFilter.SelectedItem != "Все производители")
            {
                products = products.FindAll(e => { return e.ProductManufacturer == (string)ManufacturerFilter.SelectedItem; });
            }
            if (SortAscending.IsChecked != false)
            {
                products.Sort((p1, p2) => { if (p1.ProductCost > p2.ProductCost) return -1; if (p1.ProductCost < p2.ProductCost) return 1; return 0; });
            }
            if (SortDescending.IsChecked != false)
            {
                products.Sort((p1, p2) => { if (p1.ProductCost > p2.ProductCost) return 1; if (p1.ProductCost < p2.ProductCost) return -1; return 0; });
            }
            ShownAmountText.Text = $"Показано: {products.Count} из {_products.Count()}";
            ItemsList.ItemsSource = products;
        }

        private void SearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void ManufacturerFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void SortDescending_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Update();
        }

        private void SortAscending_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult toDelete = MessageBox.Show($"Удалить {((Product)ItemsList.SelectedItem).ProductDescription}?", "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (toDelete == MessageBoxResult.Yes)
            {
                try
                {
                    _tradeEntities.Product.Remove((from p in _products where p.ProductArticleNumber == ((Product)ItemsList.SelectedItem).ProductArticleNumber select p).First());
                    _tradeEntities.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Невозможно удалить товар.\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return;
        }

        private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddEditPage addEditPage = new AddEditPage((Product)ItemsList.SelectedItem, true);
            addEditPage.Show();
            addEditPage.Closed += (s, ea) => { Update(); };
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddEditPage addEditPage = new AddEditPage(new Product());
            addEditPage.Show();
            addEditPage.Closed += (s, ea) => { Update(); };
        }
    }
}
