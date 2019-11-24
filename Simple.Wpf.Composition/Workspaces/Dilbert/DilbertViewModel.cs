﻿using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    public sealed class DilbertViewModel : BaseViewModel
    {
        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => SetPropertyAndNotify(ref _filePath, value);
        }
    }
}