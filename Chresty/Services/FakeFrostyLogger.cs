using Frosty.Core.Attributes;
using FrostySdk.Interfaces;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Chresty.Services
{
    public class FakeFrostyLogger : ILogger, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Log(string text, params object[] vars)
        {
        }

        public void LogWarning(string text, params object[] vars)
        {
        }

        public void LogError(string text, params object[] vars)
        {
        }

        public void AddBinding(UIElement elementToBind, DependencyProperty propertyToBind)
        {
        }

        protected void RaisePropertyChanged(string name)
        {
        }
    }
}
