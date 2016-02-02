using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VisualKeyboard
{
    class Word : IComparable
    {
        private string word;
        private int frequency;

        public Word() { }
        public Word(string word0, int frequency0)
        {
            this.word = word0;
            this.frequency = frequency0;
        }

        public string get_word()
        {
            return word;
        }
        public int get_frequency()
        {
            return frequency;
        }

        public int CompareTo(object obj)
        {
            if (obj is Word)
            {
                Word tempWord = obj as Word;

                if (this.frequency.CompareTo(tempWord.frequency) >= 0)
                    return 0;
                else
                    return 1;
            }
            throw new NotImplementedException("obj is not a Word!");
        }

    }

    class FindWords
    {
        private const string strReadFilePath = "dict.txt";
        private string s_Origninal;
        private int[] s_Contained;
        private List<string> PendingWordsSet = new List<string>();
        private Dictionary<string, int> Dict = new Dictionary<string, int>();

        internal FindWords()
        {
            GenDict();
        }

        public List<string> Find(string s, int[] s0)
        {
            s_Origninal = s;
            s_Contained = s0;
            string s_start = null;
            PendingWordsSet = new List<string>();
            GenPendingWords(s_start, 0);
            return FindFinalWords();
        }

        public bool GenDict()
        {
            try
            {
                using (StreamReader srReadFile = new StreamReader(strReadFilePath))
                {
                    while (!srReadFile.EndOfStream)
                    {
                        string strReadLine = srReadFile.ReadLine(); //read by line
                        string[] split = strReadLine.Split(new char[] { ' ' });
                        string r1 = split[0];
                        string r2 = split[4];

                        int a = int.Parse(r2);
                        Dict.Add(r1, int.Parse(r2));//store the word as key and its frequency as value.
                    }
                    srReadFile.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                //                Console.WriteLine("The file could not be read:");
                //                Console.WriteLine(e.Message);
                return false;
            }
        }

        //Generate the pending words
        public void GenPendingWords(string s, int iLetter)
        {
            //The last letter
            if (iLetter == s_Origninal.Length - 1)
            {
                //The last letter must be contained, so no need to check the s_Contained[]
                StringBuilder s1 = new StringBuilder(s);//conver to string builder
                s1.Append(s_Origninal[iLetter]);        //add this letter
                string word = string.Copy(s1.ToString());//convert back to string and deep copy string

                //insert into set 
                PendingWordsSet.Add(word);
            }

            //Not the last letter
            else
            {
                //Must contain this letter
                if (s_Contained[iLetter] == 1)
                {
                    StringBuilder s1 = new StringBuilder(s);//conver to string builder
                    s1.Append(s_Origninal[iLetter]);        //add this letter
                    string word = string.Copy(s1.ToString());//convert back to string and deep copy string
                    GenPendingWords(word, iLetter + 1); //entering recursion
                }
                //two choices: whether contain this letter or not 
                else
                {
                    //Not contain this letter
                    GenPendingWords(s, iLetter + 1); //entering recursion

                    //contain this letter
                    StringBuilder s1 = new StringBuilder(s);//conver to string builder
                    s1.Append(s_Origninal[iLetter]);        //add this letter
                    string word = string.Copy(s1.ToString());//convert back to string and deep copy string
                    GenPendingWords(word, iLetter + 1); //entering recursion
                }
            }

        }


        public List<string> FindFinalWords()
        {
            List<Word> tempFinalWordsSet = new List<Word>();
            List<string> FinalWordsSet = new List<string>();
            //finging the words in dictionary

            foreach (string word in PendingWordsSet)
            {
                if (Dict.ContainsKey(word))
                {
                    tempFinalWordsSet.Add(new Word(word, Dict[word]));
                }
            }

            //sort the words by frequency
            tempFinalWordsSet.Sort();

            //Add words into FinalWordsSet
            foreach (Word word in tempFinalWordsSet)
            {
                FinalWordsSet.Add(word.get_word());
            }

            return FinalWordsSet;
        }


    }

    class L_Distance
    {
        //Levenshtein Distance
        public int LevenshteinDistance(string s, string t)
        {
            if (String.IsNullOrEmpty(s))
            {
                if (String.IsNullOrEmpty(t))
                    return 0;
                else
                    return t.Length;
            }

            if (String.IsNullOrEmpty(t))
                return s.Length;

            if (s.Length > t.Length)
            {
                var temp = t;
                t = s;
                s = temp;
            }

            var m = t.Length;
            var n = s.Length;
            var distance = new int[2, m + 1];

            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++)
                distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (t[j - 1] == s[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }
    }

}
