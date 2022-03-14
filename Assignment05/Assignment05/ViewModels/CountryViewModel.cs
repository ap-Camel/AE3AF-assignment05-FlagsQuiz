using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Essentials;

using Assignment05.Models;
using Assignment05.Services;


namespace Assignment05.ViewModels
{
    class CountryViewModel : BaseViewModel
    {


        // this is not a property, won't be used in the View
        // this is the list of all countries obtained from the service 
        private List<Country> countries;

        // this is not a property
        // it represents how many points you get for correct answer (example: 10)
        // half points are deducted for wrong answer (example: 10/2 = 5)
        private int points = 10;

        // this full property represents the current correct answer
        private Country currentCountry;
        public Country CurrentCountry
        {
            get => currentCountry;
            set { currentCountry = value; OnPropertyChanged(); }
        }

        // this full property represents the current question (flag url)
        private string currentFlagUrl;
        public string CurrentFlagUrl
        {
            get => currentFlagUrl;
            set { currentFlagUrl = value; OnPropertyChanged(); }
        }

        // this full property tells the user if the choice was correct or wrong
        private string answerResult;
        public string AnswerResult
        {
            get => answerResult;
            set { answerResult = value; OnPropertyChanged(); }
        }

        // this full property represents the user's score
        //private int score;
        public int Score
        {
            get => Preferences.Get("score", 0);
            set { Preferences.Set("score", value); OnPropertyChanged(); }
        }

        // this property represents the question options (4 random countries)
        // an ObservableCollection doesn't require a full private + public + get + set definition
        // it is read-only
        // ObservableCollections are useful when you want to display list of elements
        public ObservableCollection<Country> CountryOptions { get; }

        // this full property represents the user's selected country
        private Country selectedCountry;

        public Country SelectedCountry
        {
            get => selectedCountry;
            set { selectedCountry = value; OnPropertyChanged(); }
        }

        // this is not a property
        // this object is used to get random numbers
        // we want to choose 4 random countries in each question
        private readonly Random randomGenerator;

        // this is not a property
        // the number of options for a question
        private int numberOfOptions = 4;

        // this command loads the data from Internet and generates a new question
        public ICommand LoadDataCommand { private set; get; }

        // this command validates the user's choice to see if it is correct or not
        public ICommand EvaluateChoiceCommand { private set; get; }

        // this method is private, it will be executed by the command
        // it calls the service to get information of all countries
        // and it also creates a new question
        private async Task LoadData()
        {
            countries = await FlagApiService.GetCountries();
            await CreateQuestion();
        }

        // this method picks 4 random countries from the list
        private async Task CreateQuestion()
        {
            // the IsBusy property (inherited) will be used to tell the user to wait 
            // true at the beginning means the code is "doing something" so please wait
            IsBusy = true;

            int numberOfCountries = countries.Count;

            if (numberOfCountries > 0)
            {
                // 3000 milisecond delay so the user has to wait
                await Task.Delay(10000);

                // rightAnswerIndex tells us the index of the right answer
                // we will generate 4 options
                int rightAnswerIndex = randomGenerator.Next(0, numberOfOptions);

                // options is a list of countries
                // we want to generate options for a question
                List<Country> questionOptions = new List<Country>();

                // the for loop picks the 4 options (random countries)
                for (int i = 0; i < numberOfOptions; i++)
                {
                    // let's get a random value between 0 and the number of countries
                    int index = randomGenerator.Next(0, numberOfCountries);

                    // let's access that country
                    Country country = countries[index];

                    // let´s check that we haven't picked a duplicated country in the options
                    // it uses the Any Linq method
                    if (!questionOptions.Any(x => x.GeonameId == country.GeonameId))
                    {
                        // if it is not duplicated, it is added to the options
                        questionOptions.Add(country);

                        // the right answer index
                        if (i == rightAnswerIndex)
                        {
                            CurrentCountry = country;

                            // let's convert the countrycode to lowercase
                            string countryCode = CurrentCountry.CountryCode.ToLower();
                            // we use a GitHub repository that contains all flags
                            // example: Czech Republic (cz): https://raw.githubusercontent.com/hjnilsson/country-flags/master/png250px/cz.png
                            // the country code must be lowercase
                            // then we get the flag
                            CurrentFlagUrl = $"https://raw.githubusercontent.com/hjnilsson/country-flags/master/png250px/{countryCode}.png";
                        }
                    }
                    else
                    {
                        // if we get a duplicated country, let's get another one instead
                        i--;
                    }
                }

                // remove the previous options
                CountryOptions.Clear();

                // add each new option
                foreach (Country option in questionOptions)
                    CountryOptions.Add(option);

                // delete the previous user's response
                AnswerResult = string.Empty;
            }

            // false at the end means the code has finished, no need to wait anymore
            IsBusy = false;
        }

        // this method checks if the user selected the right answer or not
        private async Task EvaluateChoice()
        {
            // if the user has selected an option
            if (!IsBusy && SelectedCountry != null)
            {
                // check the id's of the right answer and the user's choice
                if (CurrentCountry.GeonameId == SelectedCountry.GeonameId)
                {
                    // if correct, add points and update message
                    Score += points;
                    AnswerResult = $"correct {CurrentCountry.capital}";
                }
                else
                {
                    // if wrong, substract points and update message
                    Score -= (points / 2);
                    AnswerResult = "Wrong! =(";
                }

                // create a new question
                await CreateQuestion();
            }
        }

        // ViewModel constructor
        public CountryViewModel()
        {
            // initialize the generator
            randomGenerator = new Random();

            // Set the initial current country to empty
            CurrentCountry = new Country() { CountryCode = string.Empty };

            // initialize the options
            CountryOptions = new ObservableCollection<Country>();

            // Associate the commands to the methods
            LoadDataCommand = new Command(async () => await LoadData());
            EvaluateChoiceCommand = new Command(async () => await EvaluateChoice());
        }



    }
}
