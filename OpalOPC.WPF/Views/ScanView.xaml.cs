using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using OpalOPC.WPF.ViewModels;

namespace OpalOPC.WPF.Views;

/// <summary>
/// Interaction logic for ScanView.xaml
/// </summary>
public partial class ScanView : UserControl
{
    public ScanView()
    {
        DataContext = new ScanViewModel();
        InitializeComponent();
    }

    private void DragAndDropTargetsFileButton_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Note that you can have more than one file.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            HandleFileOpen(files[0]);
        }
    }

    private void HandleFileOpen(string path)
    {
        // Handle target file
        ScanViewModel viewModel = (ScanViewModel)DataContext;
        viewModel.AddTargetsFromFile(path);
    }

    private void DragAndDropTargetsFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "All files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string fileName = openFileDialog.FileName;

            // Handle target file
            ScanViewModel viewModel = (ScanViewModel)DataContext;
            viewModel.AddTargetsFromFile(System.IO.Path.GetFullPath(fileName));
        }
    }

    private void BrowseOutputReportFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "All files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            CheckFileExists = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string path = System.IO.Path.GetFullPath(openFileDialog.FileName);

            // Handle output location selection
            ScanViewModel viewModel = (ScanViewModel)DataContext;
            viewModel.SetOutputFileLocation(path);
        }
    }


    private void TargetListViewItemDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Button? btn = sender as Button;

        // Handle target deletion
        ScanViewModel viewModel = (ScanViewModel)DataContext;
        viewModel.DeleteTarget((string)btn!.DataContext);
    }

    private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // populate target textbox with selected target
        TextBlock? block = sender as TextBlock;

        // Handle target deletion
        ScanViewModel viewModel = (ScanViewModel)DataContext;
        viewModel.SetTargetToAdd((string)block!.DataContext);
    }
}
