﻿using System.Windows;

namespace OpalOPC.WPF.GuiUtil
{
    public interface IMessageBoxUtil
    {
        void Show(string message);
    }
    public class MessageBoxUtil : IMessageBoxUtil
    {
        public void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}