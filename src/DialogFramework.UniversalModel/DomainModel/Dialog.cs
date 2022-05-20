using System.Collections.Generic;
using System.Linq;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel
{
    public partial record Dialog
    {
        public IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                              IEnumerable<IDialogPartResult> newDialogPartResults)
        {
            // Decision: By default, only the results from the requested part are replaced.
            // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
            var dialogPartIds = newDialogPartResults.GroupBy(x => x.DialogPartId).Select(x => x.Key).ToArray();
            return existingDialogPartResults.Where(x => !dialogPartIds.Contains(x.DialogPartId)).Concat(newDialogPartResults);
        }

        public IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                                          IDialogPart currentPart)
        {
            // Decision: By default, only remove the results from the requested part.
            // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
            return existingDialogPartResults.Where(x => x.DialogPartId != currentPart.Id);
        }

        public bool CanNavigateTo(IDialogPart currentPart,
                                  IDialogPart navigateToPart,
                                  IEnumerable<IDialogPartResult> existingDialogPartResults)
        {
            // Decision: By default, you can navigate to either the current part, or any part you have already visited.
            // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
            return currentPart.Id == navigateToPart.Id || existingDialogPartResults.Any(x => x.DialogPartId == navigateToPart.Id);
        }
    }
}
