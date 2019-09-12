using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodosShared;


namespace TodosUITranslator
{
    public class UITranslator: IUITranslator
    {

        public Action<string> OnNotFoundWord;

        public Action OnUILanguageChanged;


        public int ComboUILanguagesSelectedIndex { get; set; } = 0;

        public TSUILanguage CurrUILanguage { get; set; } = null;

        public List<TSUILanguage> TSUILanguagesList = new List<TSUILanguage>();


        public  List<TSUIWordNative> TSUIWordNativesList = new List<TSUIWordNative>();
        private Dictionary<string, string> UIWordsDictionary { get; set; } = new Dictionary<string, string>();

        private Dictionary<Guid, string> TSUIWordForeignsDictionary { get; set; } = new Dictionary<Guid, string>();

        public List<TSUIWordsPair> TSUIWordsPairsList = new List<TSUIWordsPair>();


        public void PrepareDict(List<TSUIWordForeign> _TSUIWordForeignsList)
        {
            TSUIWordsPairsList = new List<TSUIWordsPair>();


            if (TSUIWordNativesList.Any() && _TSUIWordForeignsList.Any())
            {
                TSUIWordForeignsDictionary = _TSUIWordForeignsList.ToDictionary(x => x.UIWordNativeID, x => x.Word);


                UIWordsDictionary = TSUIWordNativesList.Select(g => new TSUIWordsPair { Native = g.Word, Foreign = GetForeignWord(g.ID) }).ToDictionary(x => x.Native, x => x.Foreign);

                TSUIWordForeignsDictionary = new Dictionary<Guid, string>();

            }

            
        }



        public void PrepareDictFromIndexedDB(List<TSUIWordsPair> dictInIndexedDB)
        {
            UIWordsDictionary = dictInIndexedDB.ToDictionary(x => x.Native, x => x.Foreign);
        }


        private string GetForeignWord(Guid nativeWordID)
        {
            string result = string.Empty;

            TSUIWordForeignsDictionary.TryGetValue(nativeWordID, out result);

            return result;
        }


        public string Translate(string word)
        {

            string result = string.Empty;


            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }


            if (CurrUILanguage is null)
            {
                return word;
            }

            if (string.IsNullOrEmpty(CurrUILanguage.Code))
            {
                return word;
            }


            if (CurrUILanguage.Code.Equals("en", StringComparison.InvariantCultureIgnoreCase))
            {
                return word;
            }


            if (!TSUILanguagesList.Any())
            {
                return word;
            }


            if (!UIWordsDictionary.Any())
            {
                return word;
            }



            if (UIWordsDictionary.TryGetValue(word.Trim().ToLower(), out result))
            {
                return result;
            }
            else
            {

                OnNotFoundWord?.Invoke(word);

                return word;
            }

        }


        //public void AddWord()
        //{

        //}

        //public void AddLanguage()
        //{

        //}

    }
}
