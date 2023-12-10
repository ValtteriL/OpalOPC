﻿using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using OpalOPC.WPF.ViewModels;

namespace OpalOPC.WPF.Views;

/// <summary>
/// Interaction logic for ScanView.xaml
/// </summary>
public partial class ScanView : UserControl
{

    private readonly OpenFileDialog _openFileDialog;
    private readonly ScanViewModel _viewModel;

    public ScanView()
    {
        DataContext = new ScanViewModel();
        _viewModel = (ScanViewModel)DataContext;
        InitializeComponent();

        _openFileDialog = new()
        {
            Filter = "All files (*.*)|*.*",
            CheckFileExists = false
        };
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
        _viewModel.AddTargetsFromFile(path);
    }

    private void DragAndDropTargetsFileButton_Click(object sender, RoutedEventArgs e)
    {
        string path = GetFilePathFromUser();
        _viewModel.AddTargetsFromFile(path);
    }

    private void BrowseOutputReportFileButton_Click(object sender, RoutedEventArgs e)
    {
        string path = GetFilePathFromUser();
        _viewModel.SetOutputFileLocation(path);
    }


    private void TargetListViewItemDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Button? btn = sender as Button;

        // Handle target deletion
        _viewModel.DeleteTarget((Uri)btn!.DataContext);
    }

    private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // populate target textbox with selected target
        TextBlock? block = sender as TextBlock;

        // Handle target deletion
        _viewModel.SetTargetToAdd((Uri)block!.DataContext);
    }

    private string GetFilePathFromUser()
    {
        string filename = _openFileDialog.FileName;
        _openFileDialog.FileName = string.Empty;

        if (_openFileDialog.ShowDialog() == true)
        {
            return System.IO.Path.GetFullPath(filename);
        }

        return string.Empty;
    }
}