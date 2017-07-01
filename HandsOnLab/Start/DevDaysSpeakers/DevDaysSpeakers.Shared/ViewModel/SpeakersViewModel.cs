using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using DevDaysSpeakers.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using DevDaysSpeakers.Services;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged

    {
        public ObservableCollection<Speaker> Speakers { get; set; }

        public Command GetSpeakersCommand { get; set; }

        private bool isBusy;
        public event PropertyChangedEventHandler PropertyChanged;

        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();

            GetSpeakersCommand = new Command(
                async () => await GetSpeakers(),
                () => !IsBusy
                );
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged();
                // Update the can execute
                GetSpeakersCommand.ChangeCanExecute();
            }
        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
            {
                return;
            }

            Exception error = null;
            try
            {
                IsBusy = true;

                var service = DependencyService.Get<AzureService>();
                var items = await service.GetSpeakers();

                Speakers.Clear();
                foreach (var item in items)
                {
                    Speakers.Add(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e);
                error = e;
            }
            finally
            {
                IsBusy = false;
            }

            if (error != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
