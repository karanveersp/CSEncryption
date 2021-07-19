using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CypherWinUI.ViewModels
{
    class PastebinViewModel : ViewModelBase
    {
        private bool isEncryptMode;

        private string key;
        private string textContent;
       

        public PastebinViewModel()
        {
            isEncryptMode = true;
        }

        public bool IsEncryptMode
        {
            get { return isEncryptMode; }
            set
            {
                if (value != this.isEncryptMode)
                {
                    this.isEncryptMode = value;
                    RaisePropertyChanged();
                    // Also trigger events for properties dependent on
                    // the current mode.
                    RaisePropertyChanged(nameof(TextInputHeader));
                    RaisePropertyChanged(nameof(ButtonContent));
                }
            }
        }

        public string TextContent
        {
            get { return textContent; }
            set { 
                if (value != textContent)
                {
                    textContent = value;
                    RaisePropertyChanged();
                } 
            }
        }

        public string Key
        {
            get { return key; }
            set
            {
                if (value != key)
                {
                    key = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TextInputHeader
        {
            get { return IsEncryptMode ? "Enter Plaintext" : "Enter Cyphertext"; }
        }

        public string ButtonContent
        {
            get { return IsEncryptMode ? "Encrypt" : "Decrypt"; }
        }
    }
}
