using GalaSoft.MvvmLight.Command;
using sample.wpf.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace sample.wpf
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private string _logThis;
        private ILogger _logger;

        public MainViewModel(ILogger logger)
        {
            _logger = logger;
            LogThisCommand = new RelayCommand(Log);
        }

        public string LogThis
        {
            set
            {
                _logThis = value;
            }
        }

        public ICommand LogThisCommand { get; }

        private void Log()
        {
            _logger.Log(_logThis);
        }
    }
}
