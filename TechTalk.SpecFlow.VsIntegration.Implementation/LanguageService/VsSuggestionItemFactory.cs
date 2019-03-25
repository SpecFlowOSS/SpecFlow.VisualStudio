using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.VsIntegration.StepSuggestions;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    public class VsSuggestionItemFactory : INativeSuggestionItemFactory<Completion>
    {
        static public readonly VsSuggestionItemFactory Instance = new VsSuggestionItemFactory();

        public Completion Create(string displayText, string insertionText, int level, string iconDescriptor, object parentObject)
        {
            var result = new CompletionWithImage(new string(' ', level*2) + displayText, insertionText, null, null, null) {IconDescriptor = iconDescriptor};
            if (parentObject != null)
                result.Properties.AddProperty("parentObject", parentObject);

            result.Properties.AddProperty("level", level);
            return result;
        }

        public Completion CloneTo(Completion nativeSuggestionItem, object parentObject)
        {
            return Create(nativeSuggestionItem.DisplayText.TrimStart(), nativeSuggestionItem.InsertionText,
                GetLevel(nativeSuggestionItem), ((CompletionWithImage) nativeSuggestionItem).IconDescriptor,
                parentObject);
        }

        public string GetInsertionText(Completion nativeSuggestionItem)
        {
            return nativeSuggestionItem.InsertionText;
        }

        public int GetLevel(Completion nativeSuggestionItem)
        {
            return nativeSuggestionItem.Properties.GetProperty<int>("level");
        }
    }
}