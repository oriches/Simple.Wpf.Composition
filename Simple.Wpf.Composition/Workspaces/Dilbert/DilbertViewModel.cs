namespace Simple.Wpf.Composition.Workspaces.Dilbert
{
    public sealed class DilbertViewModel : BaseViewModel
    {
        private string _filePath;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                SetPropertyAndNotify(ref _filePath, value);
            }
        }
    }
}