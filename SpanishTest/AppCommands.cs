using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpanishTest
{

    class AppCommands
    {
        private static RoutedUICommand cmdVerbFinder;
        private static RoutedUICommand cmdPhrases;
        private static RoutedUICommand cmdTranslations;
        private static RoutedUICommand cmdFlashCards;
        private static RoutedUICommand cmdQuit;

        public static ICommand VerbFinder
        {
            get { return cmdVerbFinder ?? (cmdVerbFinder = new RoutedUICommand("Verb Finder operation", "Verb FInder", typeof(AppCommands))); }
        }

        public static ICommand Phrases
        {
            get { return cmdPhrases ?? (cmdPhrases = new RoutedUICommand("Phrases operation", "Phrases", typeof(AppCommands))); }
        }
        public static ICommand Translations
        {
            get { return cmdTranslations ?? (cmdTranslations = new RoutedUICommand("Translations operation", "Translations", typeof(AppCommands))); }
        }

        public static ICommand FlashCards
        {
            get { return cmdFlashCards ?? (cmdFlashCards = new RoutedUICommand("Flash Cards operation", "FlashCards", typeof(AppCommands))); }
        }

        public static ICommand Quit
        {
            get { return cmdQuit ?? (cmdQuit = new RoutedUICommand("Close App", "Close", typeof(AppCommands))); }
        }
    }
}
