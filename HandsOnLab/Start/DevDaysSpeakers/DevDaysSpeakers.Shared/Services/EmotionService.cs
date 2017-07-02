
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace DevDaysSpeakers.Services
{
    public class EmotionService
    {
        private static async Task<Emotion[]> GetHappinessAsync(string url)
        {
            var emotionClient = new EmotionServiceClient("29cb5416ff1a499fa754da3cfe7ad529");

            var emotionResults = await emotionClient.RecognizeAsync(url);

            if (emotionResults == null || !emotionResults.Any())
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScoreAsync(string url)
        {
            var emotionResults = await GetHappinessAsync(url);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Length;
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}
