using Bredikhin.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace Bredikhin.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Window
    {
        readonly TradeEntities TradeEntities = new TradeEntities();
        readonly bool editMode;
        readonly Product curProduct;
        public AddEditPage(Product product, bool editMode = false)
        {
            InitializeComponent();
            curProduct = product;
            this.editMode = editMode;
            this.DataContext = product;
            StatusField.ItemsSource = new List<string>() { "TRUE", "FALSE" };
            if (editMode)
                ArticleField.IsEnabled = false;
            this.Closing += AddEditPage_Closing;
        }

        private void AddEditPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Product initialProduct = (from p in TradeEntities.Product where p.ProductArticleNumber == curProduct.ProductArticleNumber select p).First();
            curProduct.ProductManufacturer = initialProduct.ProductManufacturer;
            curProduct.ProductDescription = initialProduct.ProductDescription;
            curProduct.ProductCategory = initialProduct.ProductCategory;
            curProduct.ProductName = initialProduct.ProductName;
            curProduct.ProductPhoto = initialProduct.ProductPhoto;
            curProduct.ProductStatus = initialProduct.ProductStatus;
            curProduct.ProductCost = initialProduct.ProductCost;
            curProduct.ProductDiscountAmount = initialProduct.ProductDiscountAmount;
            curProduct.ProductQuantityInStock = initialProduct.ProductQuantityInStock;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editMode)
            {
                Product initialProduct = (from p in TradeEntities.Product where p.ProductArticleNumber == curProduct.ProductArticleNumber select p).First();
                initialProduct.ProductManufacturer = curProduct.ProductManufacturer;
                initialProduct.ProductDescription = curProduct.ProductDescription;
                initialProduct.ProductCategory = curProduct.ProductCategory;
                initialProduct.ProductName = curProduct.ProductName;
                initialProduct.ProductPhoto = curProduct.ProductPhoto;
                initialProduct.ProductStatus = curProduct.ProductStatus;
                initialProduct.ProductCost = curProduct.ProductCost;
                initialProduct.ProductDiscountAmount = curProduct.ProductDiscountAmount;
                initialProduct.ProductQuantityInStock = curProduct.ProductQuantityInStock;
            }
            else
            {
                TradeEntities.Product.Add(curProduct);
            }
            TradeEntities.SaveChanges();
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog()
            {
                Title = "Выберите фото",
                Filter = "Файлы фотографий|*.png;*.jpg",
                Multiselect = false,
            };
            fileDialog.ShowDialog();
            if (fileDialog.FileName == "")
                return;
            byte[] fileAsBytes = File.ReadAllBytes(fileDialog.FileName);
            curProduct.ProductPhoto = fileAsBytes;
            ProductImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
        }

        private void ArticleField_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            List<string> articles = (from p in TradeEntities.Product select p.ProductArticleNumber).ToList();
            if (articles.Contains(ArticleField.Text) && !editMode)
            {
                MessageBox.Show("Продукт с таким артикулом уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveButton.IsEnabled = false;
            }
            else if (!SaveButton.IsEnabled)
            {
                SaveButton.IsEnabled = true;
            }
        }
    }
}
