using PX.Data;
using UCG_Customization.Descriptor;

namespace UCG_Customization.Utils
{
    public class SqlFunction
    {
        public static decimal P_UsedExpense(AmountType type, int? companyID, int? projectID, int? taskID = null, int? inventoryID = null, int? accountGroupID = null)
        {
            string amountType = "";
            switch (type)
            {
                case AmountType.BUDGET_EXPENSE:
                    amountType = "BUDGET";
                    break;
                case AmountType.USED_EXPENSE:
                    amountType = "USED";
                    break;
            }

            decimal returnValue = 0m;
            var obj = PXDatabase.Execute("P_UsedExpenseBudget",
                    new PXSPInParameter("AmountType", amountType),
                    new PXSPInParameter("CompanyID", companyID),
                    new PXSPInParameter("ProjectID", projectID),
                    new PXSPInParameter("TaskID", taskID),
                    new PXSPInParameter("InventoryID", inventoryID),
                    new PXSPInParameter("AccountGroupID", accountGroupID),
                    new PXSPOutParameter("ReturnValue", returnValue)
                );
            if (obj != null && obj.Length > 0) returnValue = (decimal)obj[0];
            return returnValue;
        }
    }
}
