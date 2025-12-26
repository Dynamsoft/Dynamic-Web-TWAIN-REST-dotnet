using Dynamsoft.DocumentViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        private MainWindow _mainWindow;
        public SaveWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            try
            {
                lbSave.Background = new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + @"normal\save_now.png", UriKind.RelativeOrAbsolute)));
            }
            catch { }
            lbSave.IsEnabled = true;
            this.txtFileName.Text = "Output";
            this.cbFileType.Text = "PDF";
            this._mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        private void cbFileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFileType.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                string? fileType = selectedItem.Content.ToString();
                if (!string.IsNullOrEmpty(fileType))
                {
                    UpdateOptions(fileType);
                }
            }
        }

        private void UpdateOptions(string fileType)
        {
            optionsPanel.Children.Clear();

            switch (fileType)
            {
                case "JPEG":
                case "PNG":
                    optionsPanel.Children.Add(new CheckBox { Content = "Save Annotations", Name = "chkSaveAnnotations" });
                    break;

                case "TIFF":
                case "PDF":
                    // Save Page Options
                    optionsPanel.Children.Add(new Label { Content = "Save Pages:" });
                    optionsPanel.Children.Add(new RadioButton { Content = "Save Current Page", Name = "rbSaveCurrentPage", GroupName = "SavePages"});
                    optionsPanel.Children.Add(new RadioButton { Content = "Save Selected Pages", Name = "rbSaveSelectedPages", GroupName = "SavePages" });
                    optionsPanel.Children.Add(new RadioButton { Content = "Save All Pages", Name = "rbSaveAllPages", GroupName = "SavePages", IsChecked = true });
                                        
                    // Additional Options
                    if (fileType == "TIFF")
                    {
                        optionsPanel.Children.Add(new CheckBox { Content = "Save Annotations", Name = "chkSaveAnnotations", IsChecked = true });
                    }
                    else if (fileType == "PDF")
                    {
                        // Page Type
                        optionsPanel.Children.Add(new Label { Content = "Page Type:" });
                        var cbPageType = new ComboBox { Name = "cbPageType" };
                        cbPageType.Items.Add("page/default");
                        cbPageType.Items.Add("page/a4");
                        cbPageType.Items.Add("page/a3");
                        cbPageType.Items.Add("page/letter");
                        cbPageType.Items.Add("page/legal");
                        cbPageType.SelectedIndex = 0;
                        optionsPanel.Children.Add(cbPageType);

                        // Annotation
                        optionsPanel.Children.Add(new Label { Content = "Annotation:" });
                        var cbAnnotation = new ComboBox { Name = "cbAnnotation" };
                        cbAnnotation.Items.Add("none");
                        cbAnnotation.Items.Add("image");
                        cbAnnotation.Items.Add("annotation");
                        cbAnnotation.Items.Add("flatten");
                        cbAnnotation.SelectedIndex = 2;
                        optionsPanel.Children.Add(cbAnnotation);

                        // Password
                        optionsPanel.Children.Add(new Label { Content = "Password:" });
                        optionsPanel.Children.Add(new PasswordBox { Name = "txtPassword" });
                    }
                    break;
            }
        }


        private async void lbSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var count = await _mainWindow.JSInterop.GetPageCount();
            if (count == 0)
            {
                MessageBox.Show("There is no images in the buffer.");
                return;
            }

            string key = "active/" + "save_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbSave.Background = MainWindow.icons[key];
            }
            catch { }

            // Retrieve File Name
            string fileName = txtFileName.Text.Trim();
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter a file name.");
                return;
            }

            // Updated line to handle potential null value safely by using null-coalescing operator.
            string fileType = (cbFileType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            // Retrieve Options
            bool saveAnnotations = false;
            PageOption pageOption = PageOption.Current; // Default to "Save Current Page"
            PdfPageType pdfPageType = PdfPageType.PageDefault;
            SaveAnnotationMode annotationMode = SaveAnnotationMode.None;
            string password = "";

            foreach (var child in optionsPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.Name == "chkSaveAnnotations")
                {
                    saveAnnotations = checkBox.IsChecked == true;
                }
                else if (child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    if (radioButton.Name == "rbSaveCurrentPage") pageOption = PageOption.Current;
                    else if (radioButton.Name == "rbSaveSelectedPages") pageOption = PageOption.Selected;
                    else if (radioButton.Name == "rbSaveAllPages") pageOption = PageOption.All;
                }
                else if (child is ComboBox comboBox)
                {
                    if (comboBox.Name == "cbPageType") pdfPageType = PdfPageTypeHelper.FromStringValue(comboBox.SelectedItem?.ToString());
                    else if (comboBox.Name == "cbAnnotation") annotationMode = SaveAnnotationModeHelper.FromStringValue(comboBox.SelectedItem?.ToString());
                }
                else if (child is PasswordBox passwordBox)
                {
                    password = passwordBox.Password;
                }
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            if (VerifyFileName(fileName))
            {
                try
                {
                    saveFileDialog.FileName = this.txtFileName.Text;
                    byte[]? result = null; // Updated to use nullable byte array

                    if (fileType == "JPEG")
                    {
                        saveFileDialog.Filter = "JPEG|*.JPG;*.JPEG;*.JPE;*.JFIF";
                        saveFileDialog.DefaultExt = "jpg";
                        if ((bool)saveFileDialog.ShowDialog().GetValueOrDefault() == true)
                        {
                            result = await _mainWindow.JSInterop.SaveAsJpeg(saveAnnotations);
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        saveFileDialog.Filter = "PNG|*.PNG";
                        saveFileDialog.DefaultExt = "png";
                        if ((bool)saveFileDialog.ShowDialog().GetValueOrDefault() == true)
                        {
                            result = await _mainWindow.JSInterop.SaveAsPng(saveAnnotations);
                        }
                    }
                    else if (fileType == "TIFF")
                    {
                        saveFileDialog.Filter = "TIFF|*.TIF;*.TIFF";
                        saveFileDialog.DefaultExt = "tiff";
                        if ((bool)saveFileDialog.ShowDialog().GetValueOrDefault() == true)
                        {
                            result = await _mainWindow.JSInterop.SaveAsTiff(pageOption, saveAnnotations);
                        }
                    }
                    else if (fileType == "PDF")
                    {
                        saveFileDialog.Filter = "PDF|*.PDF";
                        saveFileDialog.DefaultExt = "pdf";
                        if ((bool)saveFileDialog.ShowDialog().GetValueOrDefault() == true)
                        {
                            result = await _mainWindow.JSInterop.SaveAsPdf(pageOption, pdfPageType, annotationMode, password);
                        }
                    }
                    if (result != null) // Check for null before using the result
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, result);
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
            }
            else
            {
                this.txtFileName.Focus();
            }
        }

        private void lbSave_MouseEnter(object sender, MouseEventArgs e)
        {
            string key = "hover/" + "save_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbSave.Background = MainWindow.icons[key];
            }
            catch { }
        }

        private void lbSave_MouseLeave(object sender, MouseEventArgs e)
        {
            string key = "normal/" + "save_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbSave.Background = MainWindow.icons[key];
            }
            catch { }
        }

        /// <summary>
        /// Verified the file name. If the file name is ok, return true, else return false.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        private bool VerifyFileName(string fileName)
        {
            try
            {
                if (fileName.LastIndexOfAny(System.IO.Path.GetInvalidFileNameChars()) == -1)
                    return true;
            }
            catch
            {
                // Exception handling logic can be added here if needed
            }
            MessageBox.Show("The file name contains invalid chars!", "Save Image To File", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
    }
}
