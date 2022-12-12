using PX.Data;
using PX.Objects.AP;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.EP
{
    public class EPReleaseProcessUCGExt : PXGraphExtension<EPReleaseProcess>
    {

        #region Override
        public delegate void ReleaseDocProcDelegate(EPExpenseClaim claim);
        [PXOverride]
        public virtual void ReleaseDocProc(EPExpenseClaim claim, ReleaseDocProcDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                baseMethod(claim);
                SetAPProject(claim);
                ts.Complete();
            }
        }
        #endregion

        #region Method
        protected virtual void SetAPProject(EPExpenseClaim claim)
        {
            //依據APRefNbr & APDocType 群組化
            var groups = GetDetails(claim.RefNbr)
                            .GroupBy(x =>
                                new
                                {
                                    x.GetItem<EPExpenseClaimDetails>().APRefNbr,
                                    x.GetItem<EPExpenseClaimDetails>().APDocType
                                }
                            );

            foreach (var group in groups)
            {
                List<PXResult<EPExpenseClaimDetails>> details = group.ToList();
                SetAPProject(group.Key.APRefNbr, group.Key.APDocType, details);
            }
        }

        protected virtual void SetAPProject(string apRefNbr, string apDocType, List<PXResult<EPExpenseClaimDetails>> details)
        {
            //是否有其他專案
            bool hasOther = false;
            int? tempProjectID = null;

            foreach (EPExpenseClaimDetails ds in details)
            {
                if (tempProjectID == null) tempProjectID = ds.ContractID;
                if (tempProjectID != ds.ContractID)
                {
                    hasOther = true;
                    break;
                }
            }

            //沒有其他專案(專案一致)
            if (!hasOther)
            {
                //APInvoice invoice = APInvoice.PK.Find(Base, apDocType, apRefNbr);
                PXUpdate<
                    Set<APInvoice.projectID, Required<APInvoice.projectID>>,
                    APInvoice,
                    Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                    And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                    .Update(Base, tempProjectID, apDocType, apRefNbr);
            }

        }
        #endregion

        #region BQL
        protected virtual PXResultset<EPExpenseClaimDetails> GetDetails(string refNbr)
        {
            return PXSelect<EPExpenseClaimDetails,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaimDetails.refNbr>>>>
                .Select(Base, refNbr);

        }
        #endregion

    }
}
