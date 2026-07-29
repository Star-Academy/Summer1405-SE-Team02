using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchHistoryApp
{
    public class SearchHistoryManager
    {
        private readonly List<string> _history = new List<string>();
        private readonly Stack<int> _backIndex = new Stack<int>();
        private readonly Stack<int> _frontIndex = new Stack<int>();
        private int _currentIndex = -1;

        public void Search(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine(Messages.EnterWord);
                return;
            }

            if (_currentIndex >= 0)
            {
                _backIndex.Push(_currentIndex);
            }

            _history.Add(argument);
            _currentIndex = _history.Count - 1;
            _frontIndex.Clear();

            PrintCurrent();
        }

        public void PrintCurrent()
        {
            if (_currentIndex >= 0 && _currentIndex < _history.Count)
            {
                Console.WriteLine(string.Format(Messages.CurrentFormat, _history[_currentIndex]));
            }
            else
            {
                Console.WriteLine(Messages.HistoryEmpty);
            }
        }

        public void GoBack()
        {
            if (_backIndex.Any())
            {
                _frontIndex.Push(_currentIndex);
                _currentIndex = _backIndex.Pop();
                PrintCurrent();
            }
            else
            {
                Console.WriteLine(Messages.NoBack);
            }
        }

        public void GoForward()
        {
            if (_frontIndex.Any())
            {
                _backIndex.Push(_currentIndex);
                _currentIndex = _frontIndex.Pop();
                PrintCurrent();
            }
            else
            {
                Console.WriteLine(Messages.NoForward);
            }
        }

        public void PrintStats()
        {
            if (!_history.Any())
            {
                Console.WriteLine(Messages.HistoryEmpty);
                return;
            }

            var topSearches = _history
                .GroupBy(word => word)
                .Select(group => new { Word = group.Key, Count = group.Count() })
                .OrderByDescending(item => item.Count)
                .Take(3);

            foreach (var item in topSearches)
            {
                Console.WriteLine(string.Format(Messages.StatFormat, item.Word, item.Count));
            }
        }

        public void PrintUnique()
        {
            var uniqueCount = _history.Distinct().Count();
            Console.WriteLine(uniqueCount);
        }
    }
}