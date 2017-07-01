using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using Plugin.TextToSpeech;

using DevDaysSpeakers.ViewModel;

namespace DevDaysSpeakers.View
{
    public partial class DetailsPage : ContentPage
    {
        Speaker speaker;
        public DetailsPage(Speaker speaker)
        {
            InitializeComponent();
            
            //Set local instance of speaker and set BindingContext
            this.speaker = speaker;
            BindingContext = this.speaker;

            ButtonSpeak.Clicked += ButtonSpeak_Clicked;
            ButtonWebsite.Clicked += ButtonWebsite_Clicked;
            ButtonAnalyze.Clicked += ButtonAnalysed_ClickedAsync;
        }

        private async void ButtonAnalysed_ClickedAsync(object sender, EventArgs e)
        {
            // todo: EmotionService could be outdated
            // take a look at: https://docs.microsoft.com/en-gb/azure/cognitive-services/emotion/quickstarts/csharp//
            var level = await EmotionService.GetAverageHappinessScoreAsync(this.speaker.Avatar);
            await DisplayAlert("Happiness Level", EmotionService.GetHappinessMessage(level), "OK");
        }

        private void ButtonSpeak_Clicked(object sender, EventArgs e)
        {
            CrossTextToSpeech.Current.Speak(this.speaker.Description);
        }

        private void ButtonWebsite_Clicked(object sender, EventArgs e)
        {
            if (speaker.Website.StartsWith("http"))
                Device.OpenUri(new Uri(speaker.Website));
        }

    }
}
